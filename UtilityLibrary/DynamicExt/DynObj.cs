using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace UtilityLibrary.DynamicExt
{
    /// <summary>
    /// 3) 定义一个动态对象:
    /// </summary>
    public class DynamicObjectExt : DynamicObject
    {
        //保存对象动态定义的属性值
        private Dictionary<string, object> _values;
        public DynamicObjectExt()
        {
            _values = new Dictionary<string, object>();
        }
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetPropertyValue(string propertyName)
        {
            if (_values.ContainsKey(propertyName) == true)
            {
                return _values[propertyName];
            }
            return null;
        }
        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void SetPropertyValue(string propertyName, object value)
        {
            if (_values.ContainsKey(propertyName) == true)
            {
                _values[propertyName] = value;
            }
            else
            {
                _values.Add(propertyName, value);
            }
        }
        /// <summary>
        /// 实现动态对象属性成员访问的方法，得到返回指定属性的值
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetPropertyValue(binder.Name);
            return result == null ? false : true;
        }
        /// <summary>
        /// 实现动态对象属性值设置的方法。
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetPropertyValue(binder.Name, value);
            return true;
        }
        /// <summary>
        /// 动态对象动态方法调用时执行的实际代码
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var theDelegateObj = GetPropertyValue(binder.Name) as DelegateObj;
            if (theDelegateObj == null || theDelegateObj.CallMethod == null)
            {
                result = null;
                return false;
            }
            result = theDelegateObj.CallMethod(this, args);
            return true;
        }
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            return base.TryInvoke(binder, args, out result);
        }
    }

    /// <summary>
    /// 测试代码.
    /// </summary>
    public class Test
    {
        public static void main()
        {
            dynamic theObj = new DynamicObjectExt();
            theObj.aaa = "this is a test";//动态属性
                                          //动态方法，这里不能没法定义参数，调用的时候可以是任意多参数，具体参数类型和含义就只能自己去小心处理了.
            theObj.show = DelegateObj.Function((s, pms) =>
            {
                if (pms != null && pms.Length > 0)
                {
                    //MessageBox.Show(pms[0].ToString() + ":" + s.aaa);
                }
                else
                {
                    //MessageBox.Show(s.aaa);
                }
                return null;
            }
            );
            theObj.show("hello");
        }
    }
}
