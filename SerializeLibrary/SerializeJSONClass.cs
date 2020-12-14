using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace SerializeLibrary
{
    /// <summary>
    /// 使用第三方插件 Newtonsoft.Json 
    /// 主要用于前端 js 传输
    /// </summary>
    public class SerializeJSONClass
    {

        #region DataTable 

        public static string Serialize(DataTable dt)
        {
            return JsonConvert.SerializeObject(dt);

        }


        public static Object Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<Object>(json);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        #endregion
    }
}
