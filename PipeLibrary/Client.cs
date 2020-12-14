using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;

namespace PipeLibrary
{
    public class Client
    {
        //private const string PipeServerName = "PipeServer.exe";
        

        private static NamedPipeClientStream pipeClient;

        //private bool _starting = false;


        public static string Send(string message, AsyncCallback PipeWritedCallback=null,  string PipeServerName=".",string pipeName="ALGZ_PIPE")
        {
            //if (_starting)
            //{
            //    return;
            //}

            //var path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, PipeServerName);

            //var startInfo = new ProcessStartInfo(path)
            //{
            //    UseShellExecute = false,
            //    CreateNoWindow = true
            //};

            try
            {
                //var process = Process.Start(startInfo);

                pipeClient = new NamedPipeClientStream
                (
                    PipeServerName,
                    pipeName,
                    PipeDirection.InOut,
                    PipeOptions.Asynchronous | PipeOptions.WriteThrough
                );

                pipeClient.Connect(500);

                //pipeClient.ReadMode = PipeTransmissionMode.Message;

                //message = "Connected!";

                byte[] data = Encoding.UTF8.GetBytes(message);

                pipeClient.BeginWrite(data, 0, data.Length, PipeWritedCallback==null?PipeWriteCallback: PipeWritedCallback, pipeClient);

                //_starting = true;
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static void PipeWriteCallback(IAsyncResult ar)
        {
            //var pipe = (NamedPipeClientStream)ar.AsyncState;

            pipeClient.EndWrite(ar);
            pipeClient.Flush();
            pipeClient.WaitForPipeDrain();
            pipeClient.Close();
        }
    }
}
