using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XCommon
{

    /// <summary>
    /// 学生信息实体
    /// </summary>
    public class Student
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Class Class { get; set; }
    }
    /// <summary>
    /// 学生班级实体
    /// </summary>
    public class Class
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }


    public class JsonClass
    {

        /// <summary>
        /// 实体序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static string SerializeObject(object obj, JsonSerializerSettings settings=null)
        {
            string json1 = JsonConvert.SerializeObject(obj);
            return json1;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonstr"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string jsonstr)
        {
           return JsonConvert.DeserializeObject<T>(jsonstr);
        }

        public static void main()
        {

            //序列化，反序列化 实体对象，实体集合，匿名对象：

            Student stu = new Student();
            stu.ID = 1;
            stu.Name = "张三";
            stu.Class = new Class() { ID = 0121, Name = "CS0121" };

            //使用方法1
            //实体序列化、反序列化
            //结果：{"ID":1,"Name":"张三","Class":{"ID":121,"Name":"CS0121"}}
            string json1 = JsonConvert.SerializeObject(stu);
            Console.WriteLine(json1);
            Student stu2 = JsonConvert.DeserializeObject<Student>(json1);
            Console.WriteLine(stu2.Name + "---" + stu2.Class.Name);

            //实体集合，序列化和反序列化
            List<Student> stuList = new List<Student>() { stu, stu2 };
            string json2 = JsonConvert.SerializeObject(stuList);
            Console.WriteLine(json2);
            List<Student> stuList2 = JsonConvert.DeserializeObject<List<Student>>(json2);
            foreach (var item in stuList2)
            {
                Console.WriteLine(item.Name + "----" + item.Class.Name);
            }

            //方法2，匿名对象的解析,
            //匿名独享的类型  obj.GetType().Name： "<>f__AnonymousType0`2"
            var obj = new { ID = 2, Name = "李四" };
            string json3 = JsonConvert.SerializeObject(obj);
            Console.WriteLine(json3);
            object obj2 = JsonConvert.DeserializeAnonymousType(json3, obj);
            //Console.WriteLine(obj2.GetType().GetProperty("ID").GetValue(obj2));
            object obj3 = JsonConvert.DeserializeAnonymousType(json3, new { ID = default(int), Name = default(string) });
            //Console.WriteLine(obj3.GetType().GetProperty("ID").GetValue(obj3));
            //匿名对象解析，可以传入现有类型，进行转换
            Student stu3 = new Student();
            stu3 = JsonConvert.DeserializeAnonymousType(json3, new Student());
            Console.WriteLine(stu3.Name);

            //方法3，定义序列化格式
            //var user = new { Name = "john", Age = 19 };
            //var serializerSettings = new JsonSerializerSettings
            //{
            //    // 设置为驼峰命名
            //    ContractResolver = new CamelCasePropertyNamesContractResolver()
            //};
            //var userStr = JsonConvert.SerializeObject(user, Formatting.None, serializerSettings);
        }
    }
}
