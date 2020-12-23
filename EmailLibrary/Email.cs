using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmailLibrary
{
    public class Email
    {
        public string FromEmail { get; set; }


        public string ToEmail { get; set; }

        public string Host { get; set; }


        private int _port = 25;
        /// <summary>
        /// 默认端口为25
        /// </summary>
        public int Port { get { return _port; } set { _port = value; } }

        public string Username { get; set; }


        public string Password { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

    }
}
