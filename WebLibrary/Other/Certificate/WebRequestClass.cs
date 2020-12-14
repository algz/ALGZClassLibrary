using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebLibrary.Certificate
{
    /// <summary>
    /// 根据URL地址获取需要验证证书才能访问的网页信息
    /// 这招是学会算是进了大门了，凡是需要验证证书才能进入的页面都可以使用这个方法进入，我使用的是证书回调验证的方式，证书验证是否通过在客户端验证，这样的话我们就可以使用自己定义一个方法来验证了，有的人会说那也不清楚是怎么样验证的啊，其它很简单，代码是自己写的为什么要那么难为自己呢，直接返回一个True不就完了，永远都是验证通过，这样就可以无视证书的存在了， 特点：

////1.入门前的小难题，初级课程。

////2.适应于无需登录，明文但需要验证证书才能访问的页面。

////3.获取的数据类型为HTML文档。

////4.请求方法为Get/Post
    /// </summary>
    public class WebRequestClass
    {
        //回调验证证书问题
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            // 总是接受 
            return true;
        }
        /// <summary>
        /// get方法,传入URL返回网页的html代码
        /// </summary>
        public string GetUrltoHtml(string Url)
        {
            StringBuilder content = new StringBuilder();
            try
            {
                //这一句一定要写在创建连接的前面。使用回调的方法进行证书验证。
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                // 与指定URL创建HTTP请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                //创建证书文件
                X509Certificate objx509 = new X509Certificate(Application.StartupPath + "\\123.cer");
                //添加到请求里
                request.ClientCertificates.Add(objx509);
                // 获取对应HTTP请求的响应
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // 获取响应流
                Stream responseStream = response.GetResponseStream();
                // 对接响应流(以"GBK"字符集)
                StreamReader sReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                // 开始读取数据
                Char[] sReaderBuffer = new Char[256];
                int count = sReader.Read(sReaderBuffer, 0, 256);
                while (count > 0)
                {
                    String tempStr = new String(sReaderBuffer, 0, count);
                    content.Append(tempStr);
                    count = sReader.Read(sReaderBuffer, 0, 256);
                }
                // 读取结束
                sReader.Close();
            }
            catch (Exception)
            {
                content = new StringBuilder("Runtime Error");
            }
            return content.ToString();
        }

        //回调验证证书问题
        //public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        //{
        //    // 总是接受 
        //    return true;
        //}

        ///<summary>
        ///post方法,采用https协议访问网络
        ///</summary>
        public string OpenReadWithHttps(string URL, string strPostdata, string strEncoding)
        {
            // 这一句一定要写在创建连接的前面。使用回调的方法进行证书验证。
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            //创建证书文件
            X509Certificate objx509 = new X509Certificate(System.Windows.Forms.Application.StartupPath + "\\123.cer");
            //加载Cookie
            request.CookieContainer = new CookieContainer();
            //添加到请求里
            request.ClientCertificates.Add(objx509);
            request.Method = "post";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] buffer = encoding.GetBytes(strPostdata);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding(strEncoding)))
            {
                return reader.ReadToEnd();
            }
        }

    }


}
