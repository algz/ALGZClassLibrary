using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThreadLibrary
{
    public class ThreadClass
    {
        /// <summary>
        /// 创建线程,调用无参的委托方法
        /// </summary>
        /// <param name="method"></param>
        public void Thread(object method)
        {
            //Thread类接收一个ThreadStart委托或ParameterizedThreadStart委托的构造函数，该委托包装了调用Start方法时由新线程调用的方法，示例代码如下：
            //Thread thread = new Thread(new ThreadStart(method));//创建线程
            //thread.Start();

            //通过匿名委托创建
            Thread thread1 = new Thread(delegate() { Console.WriteLine("我是通过匿名委托创建的线程"); });
            thread1.Start();
            //通过Lambda表达式创建
            Thread thread2 = new Thread(() => Console.WriteLine("我是通过Lambda表达式创建的委托"));
            thread2.Start();
            Console.ReadKey();

        }

        /// <summary>
        /// 创建线程,调用有参的委托方法
        /// </summary>
        public void ThreadParameter()
        {
            //上面代码实例化了一个Thread对象，并指明将要调用的方法method()，然后启动线程。ThreadStart委托中作为参数的方法不需要参数，并且没有返回值。ParameterizedThreadStart委托一个对象作为参数，利用这个参数可以很方便地向线程传递参数，示例代码如下：
            //Thread thread = new Thread(new ParameterizedThreadStart(method));//创建线程
            //thread.Start(3);
        }
                                                         

 
    }
}
