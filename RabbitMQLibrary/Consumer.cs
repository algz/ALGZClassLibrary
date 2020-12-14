using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RabbitMQLibrary
{
    public class Consumer
    {
        /// <summary>
        /// 设置服务器地址
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// 设置端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 设置虚拟主机
        /// </summary>
        public string VirtualHost { get; set; }

        /// <summary>
        /// 设置队列名称
        /// </summary>
        public string Queue { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        public int _AutoReconnectWaitSeconds = 10000;
        /// <summary>
        /// 自动重连等待毫秒数(默认10秒）
        /// </summary>
        public int AutoReconnectWaitSeconds { get { return _AutoReconnectWaitSeconds; } set { _AutoReconnectWaitSeconds = value; } }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        private ConnectionFactory factory = new ConnectionFactory();

        private EventingBasicConsumer consumer = null;

        private IConnection connection;

        private IModel channel;

        private Action<string> processMethod;

        private int prefetchCount = 0;
        private bool autoAck;

        public Consumer()
        {

        }

        public Consumer(string HostName, int Port, string VirtualHost, string UserName, string Password)
        {
            this.HostName = HostName;
            this.Port = Port;
            this.VirtualHost = VirtualHost;
            this.UserName = UserName;
            this.Password = Password;
        }

        /// <summary>
        /// 消息分配后，新增的consumer只能处理新增的请求，原有的消息分配后不能变动。
        /// prefetchCount 如果为0，则按服务端分配均衡方式，接收所有分配的消息。如果设计指定数，如2，则只接收2条，处理完2条数据后，由服务器在分配2条消息。
        /// 先把消息按prefetchCount数量接收到，然后一条一条执行。
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="processMethod"></param>
        /// <param name="prefetchCount">接收消息的最大数量，0无限制.如果不设置(即0),则服务端把消息都均衡分配给现有consumer</param>
        /// <param name="autoAck">是否自动确认</param>
        public string Receive(string queue, Action<string> processMethod, int prefetchCount = 0, bool autoAck = false)
        {
            this.Queue = queue;
            this.processMethod = processMethod;
            this.prefetchCount = prefetchCount;
            this.autoAck = autoAck;

            //2、设置服务器地址
            factory.HostName = this.HostName ?? "127.0.0.1";
            //3、设置端口
            factory.Port = this.Port;// 5672;
            //4、设置虚拟主机、用户名、密码
            factory.VirtualHost = this.VirtualHost ?? "/";
            factory.UserName = this.UserName ?? "guest";
            factory.Password = this.Password ?? "guest";

            //这个效果只作用于，服务器没有挂掉，只是中间有一些网络问题时才可以进行重连
            //但有一种情况是没有处理到的，我们已经在客户端对RabbitMQ某个队列进行监控，但服务器突然挂掉，然后几分钟后重新启动了，这时，客户端可以重新建立连接，但却不会自动对队列产生监控，无法拿到消息
            factory.AutomaticRecoveryEnabled = true;//设置端口后自动恢复连接属性.

            factory.RequestedHeartbeat = 30;//心跳包
            factory.AutomaticRecoveryEnabled = true;//自动重连
            factory.TopologyRecoveryEnabled = true;//拓扑重连
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);

            try
            {

                //5、通过连接工厂获取连接
                //using (IConnection connection = factory.CreateConnection())
                //{
                connection = factory.CreateConnection();

                Console.WriteLine("创建连接成功！");

                //断开连接时，调用方法自动重连
                connection.ConnectionShutdown += (sender, e) =>
                {
                    //LogHandler.WriteLog("):  RabbitMQ已经断开连接，正在尝试重新连接至RabbitMQ服务器");

                    Reconnect();
                };

                //using (IModel channel = connection.CreateModel())
                //{
                //创建通道
                channel = connection.CreateModel();

                //prefetchSize为预取的长度，一般设置为0即可，表示长度不限；prefetchCount表示预取的条数，即发送的最大消息条数；global表示是否在Connection中全局设置，true表示Connetion下的所有channel都设置为这个配置。
                channel.BasicQos(prefetchSize: 0, prefetchCount: Convert.ToUInt16(prefetchCount), global: false);

                //1.通道增加队列
                channel.QueueDeclare(queue, true, false, false, null);
                //2.创建通道的消息者
                consumer = new EventingBasicConsumer(channel);
                //3.通道关联队列、是否自动应答、消费者。
                channel.BasicConsume(queue, autoAck, consumer);//autoAck=设置手动应签 false,自动应答 true
                //4.异步接收,不阻塞程序，自动向后执行。当队列中有新消息时，自动触发，没有则函数不执行。
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        //model: RabbitMQ.Client.Events.EventingBasicConsumer
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);

                        processMethod(message);

                        //string outInfo = string.Format(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " 已接收：{0}", message);
                        //Console.WriteLine(outInfo);

                        //5.是否手动确认
                        if (!autoAck)
                        {

                            EventingBasicConsumer bc = model as EventingBasicConsumer;
                            //deliveryTag（唯一标识 ID）：当一个消费者向 RabbitMQ 注册后，会建立起一个 Channel ，RabbitMQ 会用 basic.deliver 方法向消费者推送消息，这个方法携带了一个 delivery tag， 它代表了 RabbitMQ 向该 Channel 投递的这条消息的唯一标识 ID，是一个单调递增的正整数，delivery tag 的范围仅限于 Channel
                            //multiple：为了减少网络流量，手动确认可以被批处理，当该参数为 true 时，则可以一次性确认 delivery_tag 小于等于传入值的所有消息
                            bc.Model.BasicAck(ea.DeliveryTag, false);
                            //channel.BasicAck(ea.DeliveryTag, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        // throw;
                    }

                };
                return "";
                //Console.ReadLine();类似于监听服务，需等待处理。
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                return ex.Message;
                //throw;
            }
        }

        public void Stop()
        {
            try
            {
                if (channel != null && channel.IsOpen)
                {
                    try
                    {
                        channel.Close();
                    }
                    catch (Exception ex)
                    {
                        //LogHandler.WriteLog("RabbitMQ重新连接，正在尝试关闭之前的Channel[接收]，但遇到错误", ex);
                        Console.WriteLine("RabbitMQ重新连接，正在尝试关闭之前的Channel[接收]，但遇到错误:" + ex.Message);
                    }
                    channel = null;
                }

                if (connection != null && connection.IsOpen)
                {
                    try
                    {
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        //LogHandler.WriteLog("RabbitMQ重新连接，正在尝试关闭之前的连接，但遇到错误", ex);
                        Console.WriteLine("RabbitMQ重新连接，正在尝试关闭之前的连接，但遇到错误:" + ex.Message);
                    }
                    connection = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Reconnect()
        {
            try
            {
                //清除连接及频道
                Stop();

                //var mres = new ManualResetEventSlim(false); // state is initially false
                bool flag = true;
                Console.WriteLine("RabbitMQ尝试重新连接(" + Convert.ToInt32(this.AutoReconnectWaitSeconds / 1000) + "秒)");
                while (flag) // loop until state is true, checking every 3s
                {
                    try
                    {
                        //默认等待10秒
                        Thread.Sleep(this.AutoReconnectWaitSeconds);
                        //连接
                        string msg = Receive(this.Queue, this.processMethod, this.prefetchCount, this.autoAck);
                        if (msg == "")
                        {
                            flag = false;
                            Console.WriteLine("RabbitMQ连接成功！");
                        }
                        else
                        {
                            Console.Write(".");
                        }

                        // mres.Set(); // state set to true - breaks out of loop
                    }
                    catch (Exception ex)
                    {
                        //LogHandler.WriteLog("RabbitMQ尝试连接RabbitMQ服务器出现错误：" + ex.Message, ex);
                        Console.WriteLine("RabbitMQ尝试连接RabbitMQ服务器出现错误：" + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                //LogHandler.WriteLog("RabbitMQ尝试重新连接RabbitMQ服务器出现错误：" + ex.Message, ex);
                Console.WriteLine("RabbitMQ尝试重新连接RabbitMQ服务器出现错误：" + ex.Message);
            }
        }

        ~Consumer()
        {
            channel = null;
            connection = null;
        }
    }
}
