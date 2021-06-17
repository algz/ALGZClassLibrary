using RegistryKeyLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WebLibrary;

namespace Server
{
    public partial class Server : Form
    {
        NamedPipeServerStream pipeServer =null;

        public Server()
        {
            InitializeComponent();
        }

        
        private void Server_Load(object sender, EventArgs e)
        {
            pipeServer =
                new NamedPipeServerStream("testpipe", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pipeServer.BeginWaitForConnection((o) =>
            {
                NamedPipeServerStream pServer = (NamedPipeServerStream)o.AsyncState;
                pServer.EndWaitForConnection(o);
                if (pipeServer.IsConnected)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (pipeServer.IsConnected)
                        {
                            using (StreamReader sr = new StreamReader(pServer))
                            {
                                this.textBox1.Text += sr.ReadLine();
                            }
                            
                            //pipeServer.Disconnect();
                        }
                    });
                }
                //button1_Click(null,null);
            }, pipeServer);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            PipeLibrary.Server.Start((sr) =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    //if (PipeLibrary.Server.pipeServer.IsConnected)
                    //{
                        //StreamReader sr1 = new StreamReader(sr);
                        this.textBox1.Text += sr.ReadLine();
                        //PipeLibrary.Server.pipeServer.Disconnect();
                    //}
                });
            }, "ALGZ_PIPE");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PipeLibrary.Server.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //string s=WebRequestClass.GetUrltoHtml("http://localhost:8080/algz/login", "utf-8");
            //Console.WriteLine(s);
        }

        //创建套接字
        private static byte[] result = new byte[1024];
        static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private void socketBtn_Click(object sender, EventArgs e)
        {
            Console.WriteLine("服务端已启动");
            string host = "127.0.0.1";//IP地址
            int port = 2000;//端口
            socket.Bind(new IPEndPoint(IPAddress.Parse(host), port));
            socket.Listen(100);//设定最多100个排队连接请求   
            Thread myThread = new Thread(ListenClientConnect);//通过多线程监听客户端连接  
            myThread.Start();
            Console.ReadLine();
        }

        /// <summary>  
        /// 监听客户端连接  
        /// </summary>  
        private static void ListenClientConnect()
        {
            while (true)
            {
                Socket clientSocket = socket.Accept();
                //clientSocket.Send(Encoding.UTF8.GetBytes("服务器连接成功"));
                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start(clientSocket);
            }
        }

        /// <summary>  
        /// 接收消息  
        /// </summary>  
        /// <param name="clientSocket"></param>  
        private static void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    //通过clientSocket接收数据  
                    int receiveNumber = myClientSocket.Receive(result);
                    if (receiveNumber == 0)
                        return;
                    Console.WriteLine("接收客户端{0} 的消息：{1}", myClientSocket.RemoteEndPoint.ToString(), Encoding.UTF8.GetString(result, 0, receiveNumber));
                    //给Client端返回信息
                    string sendStr = "已成功接到您发送的消息";
                    byte[] bs = Encoding.UTF8.GetBytes(sendStr);//Encoding.UTF8.GetBytes()不然中文会乱码
                    myClientSocket.Send(bs, bs.Length, 0);  //返回信息给客户端
                    myClientSocket.Close(); //发送完数据关闭Socket并释放资源
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);//禁止发送和上传
                    myClientSocket.Close();//关闭Socket并释放资源
                    break;
                }
            }
        }

        private void RegProtocolBtn_Click(object sender, EventArgs e)
        {
            RegistryProtocol.RegistryURLProtocol("ALGZ", @"D:\Source\c#\Repos\ALGZClassLibrary\TestWinForm\bin\Debug\TestWinForm.exe");
        }
    }
}
