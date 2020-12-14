using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQLibrary
{
    public class Producer
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
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        private ConnectionFactory factory = new ConnectionFactory();

        public Producer()
        {

        }

        public Producer(string HostName, int Port, string VirtualHost, string UserName, string Password)
        {
            this.HostName = HostName;
            this.Port = Port;
            this.VirtualHost = VirtualHost;
            this.UserName = UserName;
            this.Password = Password;
        }

        public string Send(string queue, string msg, string exchange = "algz.exchange", string exchangeType = "direct")
        {
            ////1、定义连接工厂

            //2、设置服务器地址
            factory.HostName = this.HostName ?? "127.0.0.1";
            //3、设置端口
            factory.Port = this.Port;// 5672;
            //4、设置虚拟主机、用户名、密码
            factory.VirtualHost = this.VirtualHost ?? "/";
            factory.UserName = this.UserName ?? "guest";
            factory.Password = this.Password ?? "guest";

            try
            {
                //5、通过连接工厂获取连接
                using (IConnection connection = factory.CreateConnection())
                {
                    using (IModel channel = connection.CreateModel())
                    {
                        //string QueueName = queue;
                        //string ExchangeName = exchange;// "topic";
                        string RoutingKey = "routingKey";

                        //1.声明交换机
                        channel.ExchangeDeclare(exchange, exchangeType);
                        //2.声明队列
                        channel.QueueDeclare(queue, true, false, false, null);
                        //3.路由绑定队列
                        channel.QueueBind(queue, exchange, RoutingKey);
                        //3.发送频道确认模式。发送了消息后，可以收到服务端回应.
                        channel.ConfirmSelect();

                        //设置消息持久性
                        IBasicProperties props = channel.CreateBasicProperties();
                        props.ContentType = "text/plain";
                        props.DeliveryMode = 2;//持久性

                        //消息内容转码，并发送至服务器
                        var messageBody = Encoding.UTF8.GetBytes(msg);
                        channel.BasicPublish(exchange, RoutingKey, props, messageBody);

                        //等待确认
                        if (channel.WaitForConfirms())
                        {
                            Console.WriteLine("已发送： {0}", msg);
                            return "";
                        }
                        else
                        {
                            string str = string.Format("发送但未收到回复： {0}", msg);
                            Console.WriteLine(str);
                            return str;
                        }
                        //Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}
