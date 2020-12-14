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

namespace WebLibrary.Login
{
    /// <summary>
    /// 根据URL地址获取需要登录才能访问的网页信息
    /// 我们先来分析一下这种类型的网页，需要登录才能访问的网页，其它呢也是一种验证，验证什么呢，验证客户端是否登录，是否具用相应的凭证，需要登录的都要验证SessionID这是每一个需要登录的页面都需要验证的，那我们怎么做的，我们第一步就是要得存在Cookie里面的数据包括SessionID，那怎么得到呢，这个方法很多，使用ID9或者是火狐浏览器很容易就能得到。

//    提供一个网页抓取hao123手机号码归属地的例子 这里面针对ID9有详细的说明。

//如果我们得到了登录的Cookie信息之后那个再去访问相应的页面就会非常的简单了，其它说白了就是把本地的Cookie信息在请求的时候捎带过去就行了。
    /// </summary>
    public class WebRequestClass
    {
        /// <summary>
        /// get方法,传入URL返回网页的html代码带有证书的方法
        /// </summary>
        public string GetUrltoHtml(string Url)
        {
            StringBuilder content = new StringBuilder();
            try
            {
                // 与指定URL创建HTTP请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                request.Method = "GET";
                request.Accept = "*/*";
                //如果方法验证网页来源就加上这一句如果不验证那就可以不写了
                request.Referer = "http://txw1958.cnblogs.com";
                CookieContainer objcok = new CookieContainer();
                objcok.Add(new Uri("http://txw1958.cnblogs.com"), new Cookie("键", "值"));
                objcok.Add(new Uri("http://txw1958.cnblogs.com"), new Cookie("键", "值"));
                objcok.Add(new Uri("http://txw1958.cnblogs.com"), new Cookie("sidi_sessionid", "360A748941D055BEE8C960168C3D4233"));
                request.CookieContainer = objcok;
                //不保持连接
                request.KeepAlive = true;
                // 获取对应HTTP请求的响应
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // 获取响应流
                Stream responseStream = response.GetResponseStream();
                // 对接响应流(以"GBK"字符集)
                StreamReader sReader = new StreamReader(responseStream, Encoding.GetEncoding("gb2312"));
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


        ///<summary>
        ///post方法,采用https协议访问网络
        ///</summary>
        public string OpenReadWithHttps(string URL, string strPostdata)
        {
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "post";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/x-www-form-urlencoded";
            CookieContainer objcok = new CookieContainer();
            objcok.Add(new Uri("http://txw1958.cnblogs.com"), new Cookie("键", "值"));
            objcok.Add(new Uri("http://txw1958.cnblogs.com"), new Cookie("键", "值"));
            objcok.Add(new Uri("http://txw1958.cnblogs.com"), new Cookie("sidi_sessionid", "360A748941D055BEE8C960168C3D4233"));
            request.CookieContainer = objcok;
            byte[] buffer = encoding.GetBytes(strPostdata);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("utf-8"));
            return reader.ReadToEnd();
        }
    }


}
