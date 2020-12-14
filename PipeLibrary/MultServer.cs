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
    public class MultServer
    {
        private NamedPipeServerStream pipeServer=null;

        private const int PipeInBufferSize = 4096;

        private const int PipeOutBufferSize = 65535;

        private Encoding encoding = Encoding.UTF8;

        public string ErrMsg { get; set; }

        public MultServer(string pipeName = "ALGZ_PIPE")
        {
            Load(pipeName);
        }

        private void Load(string pipeName = "ALGZ_PIPE")
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
        public void Start(Action<StreamReader> action, string pipeName = "ALGZ_PIPE")
        {
            //Close();
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
        public void Close()
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

        ~MultServer(){
            Close();
        }
    }
}
