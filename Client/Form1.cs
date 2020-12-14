using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string pipeName = this.radioButton1.Checked ? "ALGZ_PIPE" : "testpipe";
            
            NamedPipeClientStream pipeClient =
           new NamedPipeClientStream("localhost", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.None);
            StreamWriter sw = null;


            try
            {
                pipeClient.Connect(1000);
                sw = new StreamWriter(pipeClient);
                sw.AutoFlush = true;


                if (sw != null)
                {
                    sw.WriteLine("ok");
                    this.label1.Text = "ok";
                }
                else
                {
                    this.label1.Text = "未建立连接，不能发送消息。";
            
                }
            }
            catch (Exception ex)
            {
                this.label1.Text = ex.Message;
                //MessageBox.Show("连接建立失败，请确保服务端程序已经被打开。");
                
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
