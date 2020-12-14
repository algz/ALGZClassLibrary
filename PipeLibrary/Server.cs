using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;

namespace PipeLibrary
{
    /// <summary>
    /// 单例模式(只允许创建一个管道,如果想重新开启,需调用Close()方法结束服务,再调用Start()方法开启服务)
    /// 调用者窗口关闭时,要调用Close()结束服务.
    /// </summary>
    public class Server
    {
        private static NamedPipeServerStream pipeServer = null;

        private const int PipeInBufferSize = 4096;

        private const int PipeOutBufferSize = 65535;

        private Encoding encoding = Encoding.UTF8;

        public static string ErrMsg { get; set; }

        //static Server()
        //{
        //    pipeServer = new NamedPipeServerStream
        //        (
        //            pipeName,
        //            PipeDirection.InOut,
        //            1,
        //            PipeTransmissionMode.Message,
        //            PipeOptions.Asynchronous | PipeOptions.WriteThrough,
        //            PipeInBufferSize,
        //            PipeOutBufferSize
        //         );

        //    //_pipe.BeginWaitForConnection(WaitForConnectionCallback, _pipe);
        //}


        private static void Load(string pipeName = "ALGZ_PIPE")
        {
            pipeServer = new NamedPipeServerStream
                (
                    pipeName,
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Message,
                    PipeOptions.Asynchronous //| PipeOptions.WriteThrough//,
                                             //PipeInBufferSize,
                                             //PipeOutBufferSize
                 );
        }
        /// <summary>
        /// 启动命名管道服务
        /// </summary>
        /// <param name="a">客户端连接到 System.IO.Pipes.NamedPipeServerStream 对象时调用的方法。</param>
        /// <param name="pipeName"></param>
        public static void Start(Action<StreamReader> action, string pipeName = "ALGZ_PIPE")
        {
            Close();
            if (pipeServer == null)
            {
                Load(pipeName); //重新加载命名管道

                IAsyncResult result = pipeServer.BeginWaitForConnection((o) =>
                  {
                      try
                      {
                          //NamedPipeServerStream pServer = (NamedPipeServerStream)o.AsyncState;

                          pipeServer?.EndWaitForConnection(o);//阻塞,等待连接.
                          if (pipeServer != null)
                          {
                              StreamReader sr = new StreamReader(pipeServer);
                              action(sr);
                              Close();
                              Start(action);
                          }
                      }
                      catch (Exception ex)
                      {
                          ErrMsg = ex.Message;
                      }
                  }, pipeServer);
            }
        }

        /// <summary>
        /// 关闭命名管道服务.
        /// (当调用者的窗口关闭时,一定要调用此方法结束服务)
        /// </summary>
        public static void Close()
        {
            if (pipeServer != null)
            {
                if (pipeServer.IsConnected)
                {
                    pipeServer.Disconnect();
                }
                pipeServer.Dispose();
                pipeServer = null;
            }
        }

        private static void WaitForConnectionCallback(IAsyncResult ar)
        {
            //var pipeServer = (NamedPipeServerStream)ar.AsyncState;

            pipeServer.EndWaitForConnection(ar);

            var data = new byte[PipeInBufferSize];

            var count = pipeServer.Read(data, 0, PipeInBufferSize);
            string message = Encoding.UTF8.GetString(data, 0, count);

            //StreamReader sr = new StreamReader(pipeServer);
            //using (StreamReader sr = new StreamReader(pipeServer))
            //{
            //    while (!sr.EndOfStream)
            //    {
            //        string str = sr.ReadLine();
            //    }
            //    //sw.AutoFlush = true;
            //    //sw.WriteLine("hello " + DateTime.Now.ToString());
            //    //while (true)
            //    //{
            //    //    string str = sr.ReadLine();
            //    //    File.AppendAllText(Application.StartupPath + "//log.txt", DateTime.Now.ToLocalTime().ToString() + " " + str + Environment.NewLine);
            //    //    txtInfo.AppendText(str + Environment.NewLine);
            //    //    sw.WriteLine("send to client " + DateTime.Now.ToString());
            //    //    Thread.Sleep(1000);
            //    //}
            //}

            //if (count > 0)
            //{
            //    // 通信双方可以约定好传输内容的形式，例子中我们传输简单文本信息。

            //    string message = Encoding.UTF8.GetString(data, 0, count);

            //    //Dispatcher.BeginInvoke(new Action(() =>
            //    //{
            //    //    tblRecMsg.Text = message;
            //    //}));
            //}
        }

        //private void OnSend(object sender, RoutedEventArgs e)
        //{
        //    if (_pipe.IsConnected)
        //    {
        //        try
        //        {
        //            string message = txtSendMsg.Text;

        //            byte[] data = encoding.GetBytes(message);

        //            _pipe.Write(data, 0, data.Length);
        //            _pipe.Flush();
        //            _pipe.WaitForPipeDrain();
        //        }
        //        catch { }
        //    }

        //    Close();
        //}
    }
}
