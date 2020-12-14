using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace SerializeLibrary
{
    public class Class1
    {
        /// <summary>
        /// 手动拼接转换JSON
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string DataTableToJson(DataTable table)
        {
            if (table.Rows.Count == 0) return "";

            StringBuilder sbJson = new StringBuilder();
            sbJson.Append("[");
            for (int i = 0; i < table.Rows.Count; i++)
            {
                sbJson.Append("{");
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    string colName = table.Columns[j].ColumnName.ToString();
                    string colData = table.Rows[i][j].ToString();
                    if (table.Columns[j].DataType.ToString() == "System.DateTime")
                    {
                        colData = Convert.ToDateTime(colData).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    sbJson.Append("\"" + colName + "\":\"" + colData + "\"");
                    if (j < table.Columns.Count - 1)
                    {
                        sbJson.Append(",");
                    }
                }
                sbJson.Append("}");
                if (i < table.Rows.Count - 1)
                {
                    sbJson.Append(",");
                }
            }
            sbJson.Append("]");

            return sbJson.ToString();
        }

        /// <summary>
        /// 序列化DataTable
        /// </summary>
        public static string SerializeJSON(DataTable dt)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc].ToString());
                }
                list.Add(result);
            }
            return serializer.Serialize(list);
        }

        /// <summary>
        /// 反序列化DataTable
        /// </summary>
        public static DataTable DeserializeJSON<T>(string str)
        {

            DataTable dt = new DataTable();
            if (str[0] == '[')//如果str的第一个字符是'['，则说明str里存放有多个model数据
            {
                //删除最后一个']'和第一个'['，顺序不能错。不然字符串的长度就不对了。
                //因为每个model与model之间是用 ","分隔的，所以改为用 ";"分隔
                str = str.Remove(str.Length - 1, 1).Remove(0, 1).Replace("},{", "};{");
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            string[] items = str.Split(';');//用";"分隔开多条数据
            foreach (PropertyInfo property in typeof(T).GetProperties())//反射，获得T类型的所有属性
            {
                //创建一个新列，列名为属性名，类型为属性的类型。
                DataColumn col = new DataColumn(property.Name, property.PropertyType);
                dt.Columns.Add(col);
            }

            //循环，一个一个的反序列化
            for (int i = 0; i < items.Length; i++)
            {
                //创建新行
                DataRow dr = dt.NewRow();

                //反序列化为一个T类型对象
                T temp = js.Deserialize<T>(items[i]);
                foreach (PropertyInfo property in typeof(T).GetProperties())
                {
                    //赋值
                    dr[property.Name] = property.GetValue(temp, null);
                }
                dt.Rows.Add(dr);
            }

            return dt;
            //List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            //foreach (DataRow dr in dt.Rows)
            //{
            //    Dictionary<string, object> result = new Dictionary<string, object>();
            //    foreach (DataColumn dc in dt.Columns)
            //    {
            //        result.Add(dc.ColumnName, dr[dc].ToString());
            //    }
            //    list.Add(result);
            //}
            //return serializer.Serialize(list);
        }

       



    }
}
