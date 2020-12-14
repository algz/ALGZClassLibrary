using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestWinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Form2().Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                PipeLibrary.Server.Start(WaitForConnectionCallback);
                MessageBox.Show("启动成功!");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void WaitForConnectionCallback(IAsyncResult ar)
        {
            var pipeServer = (NamedPipeServerStream)ar.AsyncState;


            pipeServer.EndWaitForConnection(ar);
            var data = new byte[10000];

            var count = pipeServer.Read(data, 0, 10000);
            string message = Encoding.UTF8.GetString(data, 0, count);
            MessageBox.Show(message);
            //            _pipe.Write(data, 0, data.Length);
            //            _pipe.Flush();
            //            _pipe.WaitForPipeDrain();
            pipeServer.Flush();
            pipeServer.WaitForPipeDrain();
            pipeServer.Dispose();
        }
    }
}
