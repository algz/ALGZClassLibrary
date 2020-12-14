using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace WebLibrary.Http
{

    /// <summary>
    /// 1、UploadData方法（Content-Type：application/x-www-form-urlencoded）
    //    //创建WebClient 对象
    //    WebClient webClient = new WebClient();
    //    //地址
    //    string path = "http://******";
    //    //需要上传的数据
    //    string postString = "username=***&password=***&grant_type=***";
    //    //以form表单的形式上传
    //    webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
    //            // 转化成二进制数组
    //            byte[] postData = Encoding.UTF8.GetBytes(postString);
    //    // 上传数据
    //    byte[] responseData = webClient.UploadData(path, "POST", postData);
    //    //获取返回的二进制数据
    //    string result = Encoding.UTF8.GetString(responseData);

    //2、UploadData方法（Content-Type：application/json）

    //　　   //创建WebClient 对象
    //            WebClient webClient = new WebClient();
    //    //地址
    //    string path = "http://******";
    //    //需要上传的数据
    //    string jsonStr = "{\"pageNo\":1,\"pageSize\":3,\"keyWord\":\"\"}";

    //    //如果调用的方法需要身份验证则必须加如下请求标头
    //    string token = "eyJhbGciOiJSUzI..................";
    //    webClient.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {token}");

    //　　     //或者webClient.Headers.Add("Authorization", $"Bearer {token}");

    //　　     //以json的形式上传
    //            webClient.Headers.Add("Content-Type", "application/json");
    //            // 转化成二进制数组
    //            byte[] postData = Encoding.UTF8.GetBytes(jsonStr);
    //    // 上传数据
    //    byte[] responseData = webClient.UploadData(path, "POST", postData);
    //    //获取返回的二进制数据
    //    string result = Encoding.UTF8.GetString(responseData);

    //3、DownloadData方法

    //    WebClient webClient = new WebClient();
    //    string path = "http://******";

    //    //如果调用的方法需要身份验证则必须加如下请求标头
    //    string token = "eyJhbGciOiJSUzI1NiIs.........";
    //    webClient.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {token}");

    //　　　 // 下载数据
    //            byte[] responseData = webClient.DownloadData(path);
    //    string result = Encoding.UTF8.GetString(responseData);

    //4、DownloadString方法

    //            //创建WebClient 对象
    //            WebClient webClient = new WebClient();
    //    //地址
    //    string path = "http://******";

    //    //如果调用的方法需要身份验证则必须加如下请求标头
    //    string token = "eyJhbGciOiJSUzI1NiIsI.................";
    //    //设置请求头--名称/值对
    //    webClient.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {token}");
    //            //设置请求查询条件--名称/值对
    //            webClient.QueryString.Add("type_S", "我的类型");
    //            // 下载数据
    ///            string responseData = webClient.DownloadString(path);
    /// 
    /// 
    /// 
    /// </summary>
    public class WebClientExtClass : WebClient
    {
        public string Method;
        public CookieContainer CookieContainer { get; set; }
        public Uri Uri { get; set; }

        public WebClientExtClass()
            : this(new CookieContainer())
        {
        }

        public WebClientExtClass(CookieContainer cookies)
        {
            this.CookieContainer = cookies;
            this.Encoding = Encoding.UTF8;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = this.CookieContainer;
                (request as HttpWebRequest).ServicePoint.Expect100Continue = false;
                (request as HttpWebRequest).UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.5 Safari/537.36";
                (request as HttpWebRequest).Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                (request as HttpWebRequest).Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-CN,zh;q=0.8,en;q=0.6,nl;q=0.4,zh-TW;q=0.2");
                (request as HttpWebRequest).Referer = "some url";
                (request as HttpWebRequest).KeepAlive = true;
                (request as HttpWebRequest).AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                if (Method == "POST")
                {
                    (request as HttpWebRequest).ContentType = "application/x-www-form-urlencoded";
                }
            }
            HttpWebRequest httpRequest = (HttpWebRequest)request;
            httpRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return httpRequest;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            try
            {
                WebResponse response = base.GetWebResponse(request);
                String setCookieHeader = response.Headers[HttpResponseHeader.SetCookie];

                if (setCookieHeader != null)
                {
                    //do something if needed to parse out the cookie.
                    try
                    {
                        if (setCookieHeader != null)
                        {
                            Cookie cookie = new Cookie();
                            cookie.Domain = request.RequestUri.Host;
                            this.CookieContainer.Add(cookie);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                return response;
            }
            catch(WebException ex)
            {
                return (HttpWebResponse)ex.Response;
            }
        }
    }

}
