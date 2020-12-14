using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UtilityLibrary
{
    public class ReflectionClass
    {
        /// <summary>
        /// 动态加载DLL文件
        /// </summary>
        /// <param name="dllPath"></param>
        /// <returns></returns>
        public static Assembly DynamicLoadDll(string dllPath)
        {
            //1、利用反射进行动态加载和调用.
            Assembly assembly = Assembly.LoadFrom(dllPath); //利用dll的路径加载,同时将此程序集所依赖的程序集加载进来,需后辍名.dll
            return assembly;

        }

        /// <summary>
        /// 通过Assembly,获得关联的Type.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static Type GetTypeByAssembly(Assembly assembly, string className)
        {
            //2、加载dll后,需要使用dll中某类.
            Type type = assembly.GetType(className);//用类型的命名空间和名称获得类型
            return type;
        }


        public static Type DynamicLoadDllClass(string dllPath, string className)
        {
            return GetTypeByAssembly(DynamicLoadDll(dllPath), className);
        }

        /// <summary>
        /// 调用DLL中类的方法.
        /// </summary>
        /// <param name="dllPath">dll文件</param>
        /// <param name="className">全类名(包括命名空间)</param>
        /// <param name="methodName">类的方法名</param>
        /// <param name="instanceArgs">类的构造函数的参数集.(如果没有参数,则设置null.)</param>
        /// <param name="methodTypes">方法类型.
        /// a.如果调用重载方法，会报异常.
        /// 解决方案如下：
        /// GetMethod("MethodName", new Type[] { typeof(参数类型) });
        ///其中type数组中的项的个数是由要调用的方法的参数个数来决定的。
        ///如果无参数，则new Type[]{ }，使Type数组中的项个数为0
        /// </param>
        /// <param name="methodArgs">方法的形参集.(如果没有参数,则设置null.)</param>
        /// <returns></returns>
        public static object Invoke(string dllPath, string className, string methodName, string[] instanceArgs = null, Type[] methodTypes = null, object[] methodArgs = null)
        {
            try
            {
                //1、加载dll后,需要使用dll中某类.
                Type type = DynamicLoadDllClass(dllPath, className);//用类型的命名空间和名称获得类型

                //2、需要实例化类型,才可以使用,参数可以人为的指定,也可以无参数,静态实例可以省略
                Object obj = Activator.CreateInstance(type, instanceArgs);//利用指定的参数实例话类型

                //3、调用类型中的某个方法:
                //需要首先得到此方法,通过方法名称获得方法
                MethodInfo mi = type.GetMethod(methodName, methodTypes == null ? new Type[] { } : methodTypes);

                //4、然后对方法进行调用,多态性利用参数进行控制
                return mi.Invoke(obj, methodArgs);//根据参数直线方法,返回值就是原方法的返回值
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        //public static object Invoke_Test(string className, string methodName)
        //{
        //    //1、利用反射进行动态加载和调用.
        //    Assembly assembly = Assembly.LoadFrom(DllPath); //利用dll的路径加载,同时将此程序集所依赖的程序集加载进来,需后辍名.dll
        //    //Assembly.LoadFile 只加载指定文件，并不会自动加载依赖程序集.Assmbly.Load无需后辍名

        //    //2、加载dll后,需要使用dll中某类.
        //    Type type = assembly.GetType("TypeName");//用类型的命名空间和名称获得类型

        //    //3、需要实例化类型,才可以使用,参数可以人为的指定,也可以无参数,静态实例可以省略
        //    Object obj = Activator.CreateInstance(type,params[]);//利用指定的参数实例话类型

        //    //4、调用类型中的某个方法:
        //    //需要首先得到此方法
        //    MethodInfo mi = type.GetMethod("MehtodName");//通过方法名称获得方法

        //    //5、然后对方法进行调用,多态性利用参数进行控制
        //    return mi.Invoke(obj,params[]);//根据参数直线方法,返回值就是原方法的返回值
        //}

        /// <summary>
        /// 将父类的值赋值到子类中
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static TChild AutoCopy<TParent, TChild>(TParent parent) where TChild : TParent, new()
        {
            TChild child = new TChild();
            var ParentType = typeof(TParent);
            var Properties = ParentType.GetProperties();
            foreach (var Propertie in Properties)
            {
                //循环遍历属性
                if (Propertie.CanRead && Propertie.CanWrite)
                {
                    //进行属性拷贝
                    Propertie.SetValue(child, Propertie.GetValue(parent, null), null);
                }
            }
            return child;
        }
    }
}
