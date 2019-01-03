using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Services.Description;

namespace XCommon
{
    /// <summary>
    /// webservice动态调用
    /// </summary>
    public class WebServiceClass
    {
        private static CompilerParameters cplist = null;

        /// <summary>
        /// 静态类构造方法，在创建第一个实例或引用任何静态成员之前，由.NET自动调用,调用时只执行一次。
        /// </summary>
        static WebServiceClass()
        {
            #region 设定编译参数  
            cplist = new CompilerParameters();
            cplist.GenerateExecutable = false;
            cplist.GenerateInMemory = true;
            cplist.ReferencedAssemblies.Add("System.dll");
            cplist.ReferencedAssemblies.Add("System.XML.dll");
            cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
            cplist.ReferencedAssemblies.Add("System.Data.dll");
            #endregion
        }

        /// <summary>
        /// webservice服务执行之前,返回服务端代理类。
        /// </summary>
        /// <param name="url">url不带后缀?wsdl</param>
        /// <returns>代理类</returns>
        private static Type BeforeWebservice(string url)
        {
            //客户端代理服务命名空间，可以设置成需要的任意值。  
            string ns = string.Format("ALGZ.XCommon.ProxyServiceReference");

            #region 获取WSDL
            WebClient wc = new WebClient();
            Stream stream = wc.OpenRead(url.ToLower().EndsWith("?wsdl")?url:url + "?wsdl");
            ServiceDescription sd = ServiceDescription.Read(stream);//服务的描述信息都可以通过ServiceDescription获取  
            string classname = sd.Services[0].Name;
            

            ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
            sdi.AddServiceDescription(sd, "", "");
            CodeNamespace cn = new CodeNamespace(ns);
            #endregion

            #region 生成客户端代理类代码
            CodeCompileUnit ccu = new CodeCompileUnit();
            ccu.Namespaces.Add(cn);
            sdi.Import(cn, ccu);
            CSharpCodeProvider csc = new CSharpCodeProvider();
            #endregion



            #region 编译代理类
            CompilerResults cr = csc.CompileAssemblyFromDom(cplist, ccu);
            if (cr.Errors.HasErrors == true)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                {
                    sb.Append(ce.ToString());
                    sb.Append(System.Environment.NewLine);
                }
                throw new Exception(sb.ToString());
            }
            #endregion

            #region 通过反射，生成webservice服务端类的代理类  
            Assembly assembly = cr.CompiledAssembly;
            Type t = assembly.GetType(ns + "." + classname, true, true);
            #endregion

            return t;
        }

        /// <summary>
        /// 动态调用web服务
        /// 服务地址，该地址可以放到程序的配置文件中，这样即使服务地址改变了，也无须重新编译程序。
        /// </summary>
        /// <param name="url">服务地址。（后缀"?wsdl"程序自行判断添加）</param>
        /// <param name="methodname">方法名</param>
        /// <param name="args">参数值</param>
        /// <returns></returns>
        public static object InvokeWebService(string url, string methodname, object[] args=null)
        {
            object retValue = null;
            if (url.Equals("") || methodname.Equals(""))
            {
                return retValue;
            }
            try
            {
                Type t = BeforeWebservice(url);
                //生成webservice服务端类的代理类的实例  
                object obj = Activator.CreateInstance(t);
                //调用webservice服务端类的方法
                MethodInfo mi = obj.GetType().GetMethod(methodname);
                retValue = mi.Invoke(obj, args);
            }
            catch
            {
                ;
            }
            return retValue;
        }

        public static StringBuilder PrintWebserviceInfo(string url,String methodname)
        {
            StringBuilder strBuilder = new StringBuilder();
            Type t = BeforeWebservice(url);
            object obj = Activator.CreateInstance(t);


            ////////////////////////////////////////////////////////////////////////////////////  
            //调用方法  
            MethodInfo mi = t.GetMethod(methodname);
            //object helloWorldReturn = mi.Invoke(obj, new object[] { "1" });
            //strBuilder.AppendLine("调用"+ methodname+"方法，返回{0}" + helloWorldReturn.ToString());

            //获取方法的参数  
            ParameterInfo[] helloWorldParamInfos = mi.GetParameters();
            strBuilder.AppendFormat("HelloWorld方法有{0}个参数：\n",helloWorldParamInfos.Length);
            foreach (ParameterInfo paramInfo in helloWorldParamInfos)
            {
                strBuilder.AppendFormat("参数名：{0}，参数类型：{1}\n", paramInfo.Name, paramInfo.ParameterType.Name);
            }

            //获取方法返回的数据类型  
            strBuilder.AppendFormat("HelloWorld返回的数据类型是{0}\n", mi.ReturnType.Name);
            return strBuilder;
        }

        //static void Main(string[] args)
        //{

        //    object o= InvokeWebService("http://localhost:8080/algz/ras/ws/hello", "SayHi",new object[] { "1" });
        //    Console.Out.WriteLine(o);
        //    o=PrintWebserviceInfo("http://localhost:8080/algz/ras/ws/hello", "SayHi");
        //    Console.WriteLine(o);
        //    ////服务地址，该地址可以放到程序的配置文件中，这样即使服务地址改变了，也无须重新编译程序。  
        //    //string url = "http://localhost:8080/algz/ras/ws/hello";

        //    ////客户端代理服务命名空间，可以设置成需要的值。  
        //    //string ns = string.Format("ALGZ.XCommon.ProxyServiceReference");

        //    ////获取WSDL  
        //    //WebClient wc = new WebClient();
        //    //Stream stream = wc.OpenRead(url + "?WSDL");
        //    //ServiceDescription sd = ServiceDescription.Read(stream);//服务的描述信息都可以通过ServiceDescription获取  
        //    //string classname = sd.Services[0].Name;

        //    //ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
        //    //sdi.AddServiceDescription(sd, "", "");
        //    //CodeNamespace cn = new CodeNamespace(ns);

        //    ////生成客户端代理类代码  
        //    //CodeCompileUnit ccu = new CodeCompileUnit();
        //    //ccu.Namespaces.Add(cn);
        //    //sdi.Import(cn, ccu);
        //    //CSharpCodeProvider csc = new CSharpCodeProvider();

        //    ////设定编译参数  
        //    //CompilerParameters cplist = new CompilerParameters();
        //    //cplist.GenerateExecutable = false;
        //    //cplist.GenerateInMemory = true;
        //    //cplist.ReferencedAssemblies.Add("System.dll");
        //    //cplist.ReferencedAssemblies.Add("System.XML.dll");
        //    //cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
        //    //cplist.ReferencedAssemblies.Add("System.Data.dll");

        //    ////编译代理类  
        //    //CompilerResults cr = csc.CompileAssemblyFromDom(cplist, ccu);
        //    //if (cr.Errors.HasErrors == true)
        //    //{
        //    //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    //    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
        //    //    {
        //    //        sb.Append(ce.ToString());
        //    //        sb.Append(System.Environment.NewLine);
        //    //    }
        //    //    throw new Exception(sb.ToString());
        //    //}

        //    ////生成代理实例，并调用方法  
        //    //Assembly assembly = cr.CompiledAssembly;
        //    //Type t = assembly.GetType(ns + "." + classname, true, true);
        //    //object obj = Activator.CreateInstance(t);


        //    //////////////////////////////////////////////////////////////////////////////////////  
        //    ////调用HelloWorld方法  
        //    //MethodInfo helloWorld = t.GetMethod("SayHi");
        //    //object helloWorldReturn = helloWorld.Invoke(obj, new object[] { "1" });
        //    //Console.WriteLine("调用HelloWorld方法，返回{0}", helloWorldReturn.ToString());

        //    ////获取Add方法的参数  
        //    //ParameterInfo[] helloWorldParamInfos = helloWorld.GetParameters();
        //    //Console.WriteLine("HelloWorld方法有{0}个参数：", helloWorldParamInfos.Length);
        //    //foreach (ParameterInfo paramInfo in helloWorldParamInfos)
        //    //{
        //    //    Console.WriteLine("参数名：{0}，参数类型：{1}", paramInfo.Name, paramInfo.ParameterType.Name);
        //    //}

        //    ////获取HelloWorld返回的数据类型  
        //    //string helloWorldReturnType = helloWorld.ReturnType.Name;
        //    //Console.WriteLine("HelloWorld返回的数据类型是{0}", helloWorldReturnType);


        //    //////////////////////////////////////////////////////////////////////////////////////  
        //    //Console.WriteLine("\n==============================================================");
        //    ////调用Add方法  
        //    //MethodInfo add = t.GetMethod("Add");
        //    //int a = 10, b = 20;//Add方法的参数  
        //    //object[] addParams = new object[] { a, b };
        //    //object addReturn = add.Invoke(obj, addParams);
        //    //Console.WriteLine("调用HelloWorld方法，{0} + {1} = {2}", a, b, addReturn.ToString());

        //    ////获取Add方法的参数  
        //    //ParameterInfo[] addParamInfos = add.GetParameters();
        //    //Console.WriteLine("Add方法有{0}个参数：", addParamInfos.Length);
        //    //foreach (ParameterInfo paramInfo in addParamInfos)
        //    //{
        //    //    Console.WriteLine("参数名：{0}，参数类型：{1}", paramInfo.Name, paramInfo.ParameterType.Name);
        //    //}

        //    ////获取Add返回的数据类型  
        //    //string addReturnType = add.ReturnType.Name;
        //    //Console.WriteLine("Add返回的数据类型：{0}", addReturnType);


        //    //////////////////////////////////////////////////////////////////////////////////////  
        //    //Console.WriteLine("\n==============================================================");
        //    ////调用GetDate方法  
        //    //MethodInfo getDate = t.GetMethod("GetDate");
        //    //object getDateReturn = getDate.Invoke(obj, null);
        //    //Console.WriteLine("调用GetDate方法，返回{0}", getDateReturn.ToString());

        //    ////获取GetDate方法的参数  
        //    //ParameterInfo[] getDateParamInfos = getDate.GetParameters();
        //    //Console.WriteLine("GetDate方法有{0}个参数：", getDateParamInfos.Length);
        //    //foreach (ParameterInfo paramInfo in getDateParamInfos)
        //    //{
        //    //    Console.WriteLine("参数名：{0}，参数类型：{1}", paramInfo.Name, paramInfo.ParameterType.Name);
        //    //}

        //    ////获取Add返回的数据类型  
        //    //string getDateReturnType = getDate.ReturnType.Name;
        //    //Console.WriteLine("GetDate返回的数据类型：{0}", getDateReturnType);


        //    //Console.WriteLine("\n\n==============================================================");
        //    //Console.WriteLine("服务信息");
        //    //Console.WriteLine("服务名称：{0}，服务描述：{1}", sd.Services[0].Name, sd.Services[0].Documentation);
        //    //Console.WriteLine("服务提供{0}个方法：", sd.PortTypes[0].Operations.Count);
        //    //foreach (Operation op in sd.PortTypes[0].Operations)
        //    //{
        //    //    Console.WriteLine("方法名称：{0}，方法描述：{1}", op.Name, op.Documentation);
        //    //}

        //    //Console.ReadKey();
        //}
    }
}
