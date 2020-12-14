using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// C#的动态对象的属性实现比较简单,如果要实现动态语言那种动态方法就比较困难,因为对于dynamic对象,扩展方法,匿名方法都是不能用直接的,这里还是利用对象和委托来模拟这种动态方法的实现,看起来有点JavaScript的对象味道.
/// </summary>
namespace UtilityLibrary.DynamicExt
{
    /// <summary>
    /// 1)定义一个委托,参数个数可变,参数都是object类型:这里的委托多有个dynamic参数,代表调用这个委托的动态对象本身.
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="PMs"></param>
    /// <returns></returns>
    public delegate object MyDelegate(dynamic Sender, params object[] PMs);

    /// <summary>
    /// 2)定义一个委托转载对象,因为dynamic对象不能直接用匿名方法,这里用对象去承载:
    /// </summary>
    public class DelegateObj
    {
        private MyDelegate _delegate;

        public MyDelegate CallMethod
        {
            get { return _delegate; }
        }
        private DelegateObj(MyDelegate D)
        {
            _delegate = D;
        }
        /// <summary>
        /// 构造委托对象，让它看起来有点javascript定义的味道.
        /// </summary>
        /// <param name="D"></param>
        /// <returns></returns>
        public static DelegateObj Function(MyDelegate D)
        {
            return new DelegateObj(D);
        }
    }
}
