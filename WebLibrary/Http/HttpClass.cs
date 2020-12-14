using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using WebLibrary.Http;

namespace WebLibrary.Http
{
    public class HttpClass
    {
        public enum ContentType
        {
            Form, JSON
        }
        private static readonly String Form = "application/x-www-form-urlencoded; charset=UTF-8";
        private static readonly String JSON = "application/json; charset=UTF-8";

        public static CookieContainer myCookieContainer = new CookieContainer();

        public static HttpWebRequest request = null;
        public static HttpWebResponse response = null;

        public WebClientExtClass webClientExt=new WebClientExtClass();

        /// <summary>
        /// 登陆系统,获得访问资源的权限.
        /// </summary>
        /// <param name="url">登陆URL</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="msg">登陆之后的返回值</param>
        /// <param name="csrf">服务器开启CSRF,设置为true</param>
        /// <returns>返回已登陆的HttpWeb对象</returns>
        public string Login(string url, string username, string password,bool csrf = true)
        {
            //var cookieJar = new CookieContainer();
            webClientExt = new WebClientExtClass(myCookieContainer);
            string msg = "";
            string csrf_token = "";
            try
            {
                if (url == "")
                {
                    msg = "登陆URL为空.";
                    return msg;
                }
                if (csrf)
                {
                    // the website sets some cookie that is needed for login, and as well the 'lt' is always different
                    //"http://127.0.0.1:8080/algz/login"
                    //1.获取登陆页面
                    msg = webClientExt.DownloadString(url);
                    //response = "<input name=\"_csrf\" type=\"hidden\" value=\"35aec30e-8ac7-49c2-a4e2-75e8f894d43c\" />";
                    string regx = "<input[^<]*name=\"_csrf\"[^<]*type=\"hidden\"[^<]*value=\"([^<]*)\"[^<]*>";
                    // parse the 'lt' and cookie is auto handled by the cookieContainer
                    //bool flag = Regex.IsMatch(msg, regx);
                    csrf_token = Regex.Match(msg, regx).Groups[1].Value;
                }

                string postData =string.Format("username={0}&password={1}{2}", username, password, csrf ? ("&_csrf=" + csrf_token) : "");
                webClientExt.Method = "POST";
                //2.发送登陆请求
                msg = webClientExt.UploadString(url, postData);

                //3.登陆成功,返回当前Web对象
                return msg;
                //client.Method = "GET";
                //string text = client.DownloadString("http://127.0.0.1:8080/algz/hello");
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return msg;
            }

        }

        /// <summary>
        /// Post发送数据.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public string PostString(string url,string postData)
        {
            return webClientExt.UploadString(url, postData);
        }

        public string PostString(string url, string[] postData)
        {
            string str = string.Join("&", postData);
            return webClientExt.UploadString(url, str);
        }

        /// <summary>
        /// Get获取数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetString(string url)
        {
            string msg= webClientExt.DownloadString(url);
            return msg;
        }

        /// <summary>
        /// 上传文件.
        /// </summary>
        /// <param name="localFilePath">本地上传的文件路径(含文件名).</param>
        /// <param name="url">请求的URL</param>
        /// <param name="nvcObj">
        /// 附加信息
        /// NameValueCollection nvcObj = new NameValueCollection();
        ///  nvcObj.Add("key", "val");
        /// </param>
        /// <param name="webClientExt">默认为空,创建新的webClient.</param>
        /// <param name="isAsync">false,同步方式(默认);true,异步方式.</param>
        /// <param name="arr"></param>
        /// <returns>返回服务端的信息</returns>
        public string UploadFile(string localFilePath, string url, NameValueCollection nvcObj = null, bool isAsync = false, byte[] arr = null)
        {
            try
            {
                //webClientExt = webClientExt ?? new WebClientExtClass();

                if (nvcObj != null)
                {
                    //NameValueCollection nvcObj = new NameValueCollection();
                    ////附加信息
                    //nvcObj.Add("extra", "1111");
                    //将附加信息添加至webclient对象
                    webClientExt.QueryString = nvcObj;
                }
                if (isAsync)
                {

                    this.webClientExt.UploadDataAsync(new Uri(url), "post", arr);
                    return "已发送上传.";
                }
                else
                {
                    byte[] byteArr = webClientExt.UploadFile(url, "post", localFilePath);
                    return System.Text.Encoding.Default.GetString(byteArr);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        /// <summary>
        /// 下载文件.
        /// </summary>
        /// <param name="url">请求下载的URL</param>
        /// <param name="localFilePath">保存到本地的文件路径(含文件名).</param>
        /// <param name="nvcObj">
        /// 附加信息
        /// NameValueCollection nvcObj = new NameValueCollection();
        /// nvcObj.Add("key", "val");
        /// </param>
        /// <param name="webClientExt"></param>
        /// <param name="isAsync">false,同步方式(默认);true,异步方式.</param>
        public void DownFile(string url, string localFilePath, NameValueCollection nvcObj = null, bool isAsync = false, byte[] arr = null, Object userToken = null)
        {

            //webClientExt = webClientExt ?? new WebClientExtClass();

            if (nvcObj != null)
            {
                //NameValueCollection nvcObj = new NameValueCollection();
                ////附加信息
                //nvcObj.Add("extra", "1111");
                //将附加信息添加至webclient对象
                webClientExt.QueryString = nvcObj;
            }
            if (isAsync)
            {
                webClientExt.DownloadFileAsync(new Uri(url), localFilePath, userToken);
            }
            else
            {
                webClientExt.DownloadFile(url, localFilePath);
            }


        }


        /// <summary>
        /// Post方式访问没有权限的资源URI.(也可以登陆没有开启CSRF的服务器)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postString">
        /// Form Data: 数组[1,2]=>sql=1&sql=2;
        /// </param>
        /// <returns></returns>
        public static string Post(string url, string postString ,ContentType contentType=0)
        {
            try
            {
                //新建一个CookieContainer来存放Cookie集合 
                CookieContainer myCookieContainer = new CookieContainer();

                //1.创建请求URL: Request URL: http://127.0.0.1:8080/algz/login
                request = (HttpWebRequest)HttpWebRequest.Create(url);
                //2.设置请求方式
                request.Method = "Post";
                //3.设置请求类型: request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                switch (contentType)
                {
                    case ContentType.Form:
                        request.ContentType = Form;
                        break;
                    case ContentType.JSON:
                        request.ContentType = JSON;
                        break;
                }
                //4.设置请求的用户代理:    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36";

                //request.AllowAutoRedirect = true;
                //request.CookieContainer = myCookieContainer;//获取验证码时候获取到的cookie会附加在这个容器里面
                //request.KeepAlive = true;//建立持久性连

                //Post数据: "username=test&password=test"
                string data = string.Format(postString);
                //byte[] bytepostData =System.Text.Encoding.UTF8.GetBytes(postString);
                //5.设置请求的post数据长度:  Content-Length: 70
                request.ContentLength = data.Length;//bytepostData.Length;

                //6.发送数据  using结束代码段释放
                using (StreamWriter requestStm = new StreamWriter(request.GetRequestStream()))
                {
                    //把数据写入HttpWebRequest的Request流
                    requestStm.Write(data, 0, data.Length);
                    //requestStm.Write(bytepostData, 0, bytepostData.Length);
                }

                //7.新建一个HttpWebResponse 接收网页传回的数据
                response = (HttpWebResponse)request.GetResponse();
                string text = string.Empty;
                using (Stream responseStm = response.GetResponseStream())
                {
                    StreamReader redStm = new StreamReader(responseStm, Encoding.UTF8);
                    text = redStm.ReadToEnd();
                }

                //必须在获得Response之后,才能获取一个包含url,JSESSIONID的Cookie集合的CookieCollection,
                response.Cookies = myCookieContainer.GetCookies(request.RequestUri);

                return text;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Get方式获取没有权限的资源URI
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Get(string url)
        {
            try
            {
                string text = "";
                //这里的URL就是我们需要登录之后才可以访问的URL,"http://127.0.0.1:8080/algz/hello"
                request = (HttpWebRequest)HttpWebRequest.Create(url);
                //request.Method = "Get";//默认Get
                request.CookieContainer = myCookieContainer;//*解析到的CookieContainer 
                response = (HttpWebResponse)request.GetResponse();
                //response.Cookies = myCookieContainer.GetCookies(request.RequestUri);
                using (Stream responseStm = response.GetResponseStream())
                {
                    StreamReader redStm = new StreamReader(responseStm, Encoding.UTF8);
                    text = redStm.ReadToEnd();
                }
                return text;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        /// <summary>
        /// 
        /// https://www.cnblogs.com/srx121201/p/8134361.html
        /// winfrom 使用DownloadFileAsync下载并显示进度条.
        ///  WebClient wed = new WebClient();//初始化新实例            
        //   wed.DownloadProgressChanged += DownloadProgressChanged;//
        //    wed.DownloadFileAsync(new Uri(下载地址), 保存地址);      //保存地址  异步
        //    public void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        //   {
        //       Action<DownloadProgressChangedEventArgs> on = PeceChange;
        //    on.Invoke(e);
        //   }

        //protected void PeceChange(DownloadProgressChangedEventArgs e)
        //{
        //    progressBar1.Value = e.ProgressPercentage;
        //    label1.Text = e.ProgressPercentage.ToString() + "%";
        //}
    }
}
