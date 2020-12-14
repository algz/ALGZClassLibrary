//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace PipeLibrary
//{
//    public class Class1
//    {
////        最近在一次项目中使用到了C#中命名管道，所以在此写下一篇小结备忘。

////为什么要使用命名管道呢？为了实现两个程序之间的数据交换。假设下面一个场景。在同一台PC上，程序A与程序B需要进行数据通信，此时我们就可以使用命名管道技术来实现。命名管道的两个对象。NamedPipeClientStream 和 NamedPipeServerStream 对象。请求通信的一方为Client端，发送数据的一方为Server端。

////使用NamedPipe来通信，如果Server端崩溃了，不会影响到客户端。
//        public partial class Form1 : Form
//        {
//            // 命名管道客户端
//            NamedPipeClientStream pipeClient = null;
//            StreamWriter swClient = null;
//            StreamReader srClient = null;

//            public Form1()
//            {
//                InitializeComponent();
//                Control.CheckForIllegalCrossThreadCalls = false;
//            }

//            // 创建命名管道
//            private void button1_Click(object sender, EventArgs e)
//            {
//                backgroundWorker1.RunWorkerAsync();
//                txtInfo.AppendText("创建命名管道" + Environment.NewLine);
//            }

//            private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
//            {
//                using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("testPipe", PipeDirection.InOut))
//                {
//                    pipeServer.WaitForConnection();

//                    var data = new byte[10240];
//                    var count = pipeServer.Read(data, 0, 10240);
//                    StreamReader sr = new StreamReader(pipeServer);
//                    using (StreamWriter sw = new StreamWriter(pipeServer))
//                    {
//                        sw.AutoFlush = true;
//                        sw.WriteLine("hello " + DateTime.Now.ToString());
//                        while (true)
//                        {
//                            string str = sr.ReadLine();
//                            File.AppendAllText(Application.StartupPath + "//log.txt", DateTime.Now.ToLocalTime().ToString() + " " + str + Environment.NewLine);
//                            txtInfo.AppendText(str + Environment.NewLine);
//                            sw.WriteLine("send to client " + DateTime.Now.ToString());
//                            Thread.Sleep(1000);
//                        }
//                    }
//                }
//            }

//            // 连接命名管道
//            private void button2_Click(object sender, EventArgs e)
//            {
//                try
//                {
//                    pipeClient = new NamedPipeClientStream("localhost", "testPipe", PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.None);
//                    pipeClient.Connect(5000);
//                    swClient = new StreamWriter(pipeClient);
//                    srClient = new StreamReader(pipeClient);
//                    swClient.AutoFlush = true;
//                    backgroundWorker2.RunWorkerAsync();

//                    txtInfo.AppendText("连接命名管道" + Environment.NewLine);
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show("连接建立失败，请确保服务端程序已经被打开。" + ex.ToString());
//                }
//            }

//            // 发送消息
//            private void button3_Click(object sender, EventArgs e)
//            {
//                if (swClient != null)
//                {
//                    swClient.WriteLine(this.textBox1.Text);
//                }
//                else
//                {
//                    MessageBox.Show("未建立连接，不能发送消息。");
//                }
//            }

//            // 接收消息
//            private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
//            {
//                while (true)
//                {
//                    if (srClient != null)
//                    {
//                        txtInfo.AppendText(srClient.ReadLine() + System.Environment.NewLine);
//                    }
//                }
//            }

//            private void Form1_Load(object sender, EventArgs e)
//            {
//                // button1.PerformClick();
//            }
//        }
//    }

//}
