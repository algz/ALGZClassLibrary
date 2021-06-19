using DBLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DBLibrary
{


    /// <summary>
    /// 单例模式，不缓存，执行完后关闭数据库连接。不支持left join等级联，建议用缓存模式。
    /// </summary>
    public class DBSingletonCommon<C, S>
    {
        static DBSingletonCommon()
        {
            string connectionString = AppConfigLibrary.AppConfigClass.GetValue("DataSource");
            //ConectionMode=(DBConectionMode)Enum.Parse(typeof(DBConectionMode), "", true);
            string tem = AppConfigLibrary.AppConfigClass.GetValue("DBConectionMode");
            ConectionMode = (DBConectionMode)Convert.ToInt32(tem==""?"0":tem);
            switch (ConectionMode)
            {
                case DBConectionMode.LocalDB:
                    dbcommon = LocalDBCommon<C,S>.Instance;
                    break;
                case DBConectionMode.WebDB:
                    dbcommon = WebDBCommon.Instance;

                    //string webConnectionString = AppConfigLibrary.AppConfigClass.GetValue("WebDataSource");
                    //if (webConnectionString != "")
                    //{
                    //    connectionString = webConnectionString;
                    //}

                    //((WebDBCommon)dbcommon).ExecuteDatatableUrl = AppConfigLibrary.AppConfigClass.GetValue("ExecuteDatatableUrl");
                    //((WebDBCommon)dbcommon).ExecuteScalarUrl = AppConfigLibrary.AppConfigClass.GetValue("ExecuteScalarUrl");
                    //((WebDBCommon)dbcommon).ExecuteNonQueryUrl = AppConfigLibrary.AppConfigClass.GetValue("ExecuteNonQueryUrl");
                    //string username = AppConfigLibrary.AppConfigClass.GetValue("LoginUsername");
                    //string password = AppConfigLibrary.AppConfigClass.GetValue("LoginPassword");
                    //string loginUrl = AppConfigLibrary.AppConfigClass.GetValue("LoginUrl");
                    //string csrf = AppConfigLibrary.AppConfigClass.GetValue("CSRF");
                    //((WebDBCommon)dbcommon).Login(loginUrl, username, password,csrf==""||csrf=="true"?true:false);
                    break;
                case DBConectionMode.MixDB:
                    dbcommon = MixDBCommon<C,S>.Instance;

                    
                    //Data Source =C:\\Users\\algz\\documents\\visual studio 2015\\Projects\\SANY_WINFORM\\Test\\bin\\Debug/test.db
                    string dbPath = "Data Source =" + Environment.CurrentDirectory + "/test.db";
                    ((MixDBCommon<C, S>)dbcommon).localDbCommon =LocalDBCommon<C,S>.Instance;

                    ((MixDBCommon<C, S>)dbcommon).remoteDbCommon = WebDBCommon.Instance;
                    ((MixDBCommon<C, S>)dbcommon).remoteDbCommon.ConnectionString = AppConfigLibrary.AppConfigClass.GetValue("WebDataSource");
                    //((WebDBCommon)dbcommon).ExecuteDatatableUrl = AppConfigLibrary.AppConfigClass.GetValue("ExecuteDatatableUrl");
                    //((WebDBCommon)dbcommon).ExecuteScalarUrl = AppConfigLibrary.AppConfigClass.GetValue("ExecuteScalarUrl");
                    //((WebDBCommon)dbcommon).ExecuteNonQueryUrl = AppConfigLibrary.AppConfigClass.GetValue("ExecuteNonQueryUrl");
                    //string username1 = AppConfigLibrary.AppConfigClass.GetValue("LoginUsername");
                    //string password1 = AppConfigLibrary.AppConfigClass.GetValue("LoginPassword");
                    //string loginUrl1 = AppConfigLibrary.AppConfigClass.GetValue("LoginUrl");
                    //string csrf1 = AppConfigLibrary.AppConfigClass.GetValue("CSRF");
                    //((WebDBCommon)dbcommon).Login(loginUrl1, username1, password1, csrf1 == "" || csrf1 == "true" ? true : false);
                    break;
            }
                dbcommon.ConnectionString = connectionString;
        }

        private static IDBCommon dbcommon;

        public static DBConectionMode ConectionMode { get; set; }

        /// <summary>
        /// 获取连接字符串
        /// Oracle:
        /// "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=SANY)));Persist Security Info=True;User ID=SANY;Password=SANY;"
        /// SQLite:
        /// Data Source=MyDatabase.db;Version=3;
        /// 默认情况下，外键约束是关闭的，当设置打开时并不能持久保持，foreign key 标注是基于会话的，也就是说每当连接关闭时，信息也就丢失了，SQLite连接字符串支持添加参数。可以在每次查询中用下面形式的连接字符串来都设置。foreign keys=true;
        /// Data Source=Data Source=" + DBPath + ";Version=3;foreign keys=true;";
        /// </summary>
        public static string ConnectionString{ get => dbcommon.ConnectionString;}


        /// <summary>
        /// 测试数据库是否连接成功.
        /// </summary>
        /// <param name="connectionString">连接字符串（可选）</param>
        /// <param name="obj"></param>
        /// <returns>连接成功，返回空字符串；失败，返回错误原因.</returns>
        public static string ConnectionTest(string connectionString=null,params object[] obj)
        {
            return dbcommon.ConnectionTest(connectionString??dbcommon.ConnectionString,obj);
        }


        //public static void test()
        //{
        //    DbConnection conn = null;
        //    try
        //    {
        //        conn = OpenConn();
        //        var cmd = conn.CreateCommand();
        //        cmd.CommandText = "select * from S_STANDARDS";
        //        cmd.CommandType = CommandType.Text;
        //        var reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            Console.WriteLine(string.Format("AwbPre:{0},AwbNo:{1}", reader["name"], reader["cname"]));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    finally
        //    {
        //        CloseConn(conn);
        //    }
        //    Console.Read();
        //}

        //private static OracleConnection OpenConn(string connectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=SANY)));Persist Security Info=True;User ID=SANY;Password=SANY;")
        //{
        //    OracleConnection conn = new OracleConnection();
        //    conn.ConnectionString = connectionString;
        //    conn.Open();
        //    return conn;
        //}

        //public static void CloseConn(OracleConnection conn)
        //{
        //    if (conn == null) { return; }
        //    try
        //    {
        //        if (conn.State != ConnectionState.Closed)
        //        {
        //            conn.Close();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //    finally
        //    {
        //        conn.Dispose();
        //    }
        //}

        //////////////////////////////////////////////////////////

        

        /// <summary>
        /// MySQL/SQLite: select * from tablename where name=@name;
        /// Oracle:  select * from tablename where name=:name; (实际中,命名参数按顺序绑定,与参数名无关.)
        /// 参数里面标记为params即可以传0个，也可以传null
        /// SQLiteParameter name = new SQLiteParameter("name",  "SUSAN LI");
        /// SQLiteParameter sex = new SQLiteParameter("sex", DbType.Int16, 1);
        /// SQLiteParameter age = new SQLiteParameter("age", DbType.Int16, 30);
        /// SQLiteParameter[] pa = { name, sex, age };
        /// SQLiteSingletonCommon.ExecuteNonQuery("INSERT INTO USER6 (NAME,SEX,AGE) values (@name,@sex,@age)",pa);
        /// 在select查询的where条件查询中定义的命名参数不支持"ID"字段(但insert,update,delete 支持ID字段的命名参数):
        /// select * from tablename where id=:id;
        /// SQLiteParameter name = new SQLiteParameter("ID",  "1"); x (命名参数不支持表的字段名为ID字符,所以只能在SQL语句中连接使用.
        /// SQL语法：
        /// 添加：INSERT INTO TABLE_NAME [(column1,...,columnN)] VALUES(value1,...,valueN);
        /// 修改：UPDATE table_name SET column1 = value1,...., columnN = valueN WHERE[condition];
        /// 删除：DELETE FROM table_name WHERE[condition];
        /// </summary>
        /// <param name="commandText">SQL语句（SQLite/MySQL参数占位符用‘@参数占位符名’符号,Oracle参数占位符前用':参数占位符名'）</param>
        /// <param name="commandParameters">可以传0个，也可以传null，或者不传值</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string commandText, params DbParameter[] commandParameters)
        {
            return dbcommon.ExecuteNonQuery(commandText, commandParameters);
        }

        /// <summary>
        /// 获取对象的属性(string,int),自动装载到SQL查询中,执行新增.
        /// </summary>
        /// <param name="t">IDictionary<string,object> 类型；实体对象类型</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public static int ExecuteNonQuery_Add(object t, string tableName)
        {
            //获得所有列名
            DataTable dt = GetColumnNames(tableName);
            

            //string tableName = t.GetType().Name;
            List<string> nameList = new List<string>();
            List<string> valList = new List<string>();
            if (t is IDictionary)
            {
                //IDictionary 类型
                IDictionary dic = t as IDictionary;
                foreach (object obj in dic.Keys)
                {
                    string colName = obj.ToString();
                    if (dt.Select("COLUMN_NAME='" + colName.ToUpper() + "'").Length > 0)
                    {
                        nameList.Add(colName);
                        if (dic[obj].GetType() == typeof(string))
                        {
                            valList.Add("'" + dic[obj] + "'");
                        }
                        else if (dic[obj].GetType() == typeof(int))
                        {
                            valList.Add(dic[obj] + "");
                        }
                    }

                }
            }
            else
            {
                //其它实体对象类型
                foreach (PropertyInfo pi in t.GetType().GetProperties())
                {
                    if (dt.Select("COLUMN_NAME='" + pi.Name.ToUpper() + "'").Length > 0)
                    {
                        nameList.Add(pi.Name);
                        if (pi.PropertyType == typeof(string))
                        {
                            valList.Add("'" + pi.GetValue(t, null) + "'");
                        }
                        else if (pi.PropertyType == typeof(int))
                        {
                            valList.Add(pi.GetValue(t, null) + "");
                        }
                    }

                }
            }
            
            string commandText = "insert into "+ tableName + " ("+string.Join(",",nameList)+")values("+string.Join(",",valList)+")";
            return dbcommon.ExecuteNonQuery(commandText);
        }

        public static int ExecuteNonQuery_Add(DataRow dr, string tableName)
        {
            
            //获得所有列名
            DataTable dt = GetColumnNames(tableName);


            //string tableName = t.GetType().Name;
            List<string> nameList = new List<string>();
            List<string> valList = new List<string>();
            foreach (DataColumn col in dr.Table.Columns)
            {
                string colName = col.ColumnName;
                if (dt.Select("COLUMN_NAME='" + colName.ToUpper() + "'").Length > 0)
                {
                    
                    if (dr[colName].GetType() == typeof(string))
                    {
                        nameList.Add(col.ColumnName);
                        valList.Add("'" + dr[colName] + "'");
                    }
                    else if (dr[colName].GetType() == typeof(int))
                    {
                        nameList.Add(col.ColumnName);
                        valList.Add(dr[colName] + "");
                    }
                    //else if (t[colName].GetType() == typeof(DBNull))
                    //{
                    //    valList.Add("''");
                    //}
                }

            }
            string commandText = "insert into " + tableName + " (" + string.Join(",", nameList) + ")values(" + string.Join(",", valList) + ")";
            return dbcommon.ExecuteNonQuery(commandText);
        }



        private static DataTable GetColumnNames(string tableName)
        {
            string sql = "SELECT T.COLUMN_NAME FROM USER_TAB_COLUMNS T WHERE UPPER(T.TABLE_NAME)=UPPER('" + tableName + "')";
            return dbcommon.ExecuteDataTable(sql);
        }

        /// <summary>
        /// 保存数据.
        /// 表格字段必须与方法名相同,主键名称必须是"ID".
        /// </summary>
        /// <param name="t">IDictionary类型和实体类型</param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery_Update(object t, string tableName)
        {
            //获得所有列名
            DataTable dt = GetColumnNames(tableName);

            List<string> valList = new List<string>();
            string id = null;
            if (t is IDictionary)
            {
                //IDictionary 类型
                IDictionary dic = t as IDictionary;
                foreach (object obj in dic.Keys)
                {
                    string colName = obj.ToString();
                    if (dt.Select("COLUMN_NAME='" + colName.ToUpper() + "'").Length > 0)
                    {
                        var val = dic[obj];
                        if (colName.ToUpper() == "ID")
                        {
                            id = val.ToString();
                        }
                        if (val != null)
                        {
                            if (dic[obj].GetType() == typeof(string))
                            {
                                valList.Add(colName + "='" + val + "'");
                            }
                            else if (dic[obj].GetType() == typeof(int))
                            {
                                valList.Add(colName + "=" + val);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (PropertyInfo pi in t.GetType().GetProperties())
                {
                    if (dt.Select("COLUMN_NAME='" + pi.Name.ToUpper() + "'").Length > 0)
                    {
                        var val = pi.GetValue(t, null);
                        if (pi.Name.ToUpper() == "ID")
                        {
                            id = val.ToString();
                        }
                        if (val != null)
                        {
                            if (pi.PropertyType == typeof(string))
                            {
                                valList.Add(pi.Name + "='" + val + "'");
                            }
                            else if (pi.PropertyType == typeof(int))
                            {
                                valList.Add(pi.Name + "=" + val);
                            }
                        }
                    }

                }
            }
            
            string commandText = "update " + tableName + " set "+string.Join(",",valList)+" where id='"+id+"'";
            return dbcommon.ExecuteNonQuery(commandText);
        }

        public static int ExecuteNonQuery_Update(DataRow dr, string tableName)
        {
            //获得所有列名
            DataTable dt = GetColumnNames(tableName);

            List<string> valList = new List<string>();
            string id = null;
            foreach (DataColumn col in dr.Table.Columns)
            {
                string colName = col.ColumnName;
                if (dt.Select("COLUMN_NAME='" + col.ColumnName.ToUpper() + "'").Length > 0)
                {
                    
                    var val = dr[col];
                    if (colName.ToUpper() == "ID")
                    {
                        id = val.ToString();
                        continue;
                    }
                    if (val != null)
                    {
                        if (dr[colName].GetType() == typeof(string))
                        {
                            valList.Add(colName + "='" + val + "'");
                        }
                        else if (dr[colName].GetType() == typeof(int))
                        {
                            valList.Add(colName + "=" + val);
                        }
                    }
                }

            }
            string commandText = "update " + tableName + " set " + string.Join(",", valList) + " where id='" + id + "'";
            return dbcommon.ExecuteNonQuery(commandText);
        }

        public static int ExecuteNonQuery_Del(string id,string tableName,string column="id")
        {
            string sql = "delete from "+tableName+" where "+ column + "='"+id+"'";
            return dbcommon.ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 删除数据.
        /// 表格字段必须与方法名相同,主键名称必须是"ID".
        /// </summary>
        /// <param name="t"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery_Del(object t, string tableName)
        {
            //获得所有列名
            DataTable dt = GetColumnNames(tableName);

            List<string> valList = new List<string>();
            string id = null;
            foreach (PropertyInfo pi in t.GetType().GetProperties())
            {
                if (dt.Select("COLUMN_NAME='" + pi.Name.ToUpper() + "'").Length > 0)
                {
                    var val = pi.GetValue(t, null);
                    if (pi.Name.ToUpper() == "ID")
                    {
                        id = val.ToString();
                    }
                    if (val != null)
                    {
                        if (pi.PropertyType == typeof(string))
                        {
                            valList.Add(pi.Name + "='" + val + "'");
                        }
                        else if (pi.PropertyType == typeof(int))
                        {
                            valList.Add(pi.Name + "=" + val);
                        }
                    }
                }

            }
            string commandText = "delete from " + tableName + " where "+ string.Join(" and ", valList);
            return dbcommon.ExecuteNonQuery(commandText);
        }




        // 查询并返回datatable
        public static DataTable ExecuteDataTable(string commandText, params DbParameter[] commandParameters)
        {
            return dbcommon.ExecuteDataTable(commandText, commandParameters);
        }



        /// <summary>
        /// 查询并把结果装入T类型返回实例对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static List<T> ExecuteDataTable<T>(string commandText, params DbParameter[] commandParameters)
        {
            try
            {
                List<T> list = new List<T>();
                DataTable dt = dbcommon.ExecuteDataTable(commandText, commandParameters);
                if (dt == null)
                {
                    return null;
                }
                foreach (DataRow dr in dt.Rows)
                {
                    T t = Activator.CreateInstance<T>();
                    foreach (PropertyInfo pi in t.GetType().GetProperties())
                    {
                        if (dt.Columns.Contains(pi.Name.ToUpper()))
                        {
                            if (pi.PropertyType == typeof(string))
                            {
                                pi.SetValue(t, dr[pi.Name]+"", null);
                            }
                            else if (pi.PropertyType == typeof(int))
                            {
                                pi.SetValue(t, Convert.ToInt32(dr[pi.Name]), null);
                            }
                        }
                    }
                    //foreach (MethodInfo mi in t.GetType().GetMethods())
                    //{
                    //    if (mi.Name.Contains("set_"))
                    //    {
                    //        string str = mi.Name.Substring(4);
                    //        if (dt.Columns.Contains(str.ToUpper()))
                    //        {
                    //            ParameterInfo pi = mi.GetParameters()[0];
                    //            if(pi.ParameterType == typeof(string))
                    //            {
                    //                mi.Invoke(t, new object[] { dr[str] + "" });
                    //            }else if (pi.ParameterType == typeof(int))
                    //            {
                    //                mi.Invoke(t, new object[] { Convert.ToInt32(dr[str]) });
                    //            }
                                
                    //        }
                    //    }
                    //}
                    list.Add(t);
                }
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 使用此函数，对象名称必须与数据字段名称一致。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">如果t为null，查询所有值;对象属性值如果为null,不查询此属性，但属性值为空字符串，则查询。</param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static List<T> ExecuteDataTable<T>(T t, string tableName,string sqlParam="")
        {
            try
            {
                //获得所有列名
                DataTable dt = GetColumnNames(tableName);
                List<string> valList = new List<string>();
                string commandText = "";
                if (t != null)
                {
                    foreach (PropertyInfo pi in t.GetType().GetProperties())
                    {
                        if (dt.Select("COLUMN_NAME='" + pi.Name.ToUpper() + "'").Length > 0)
                        {
                            var val = pi.GetValue(t, null);
                            if (val != null)
                            {
                                if (pi.PropertyType == typeof(string))
                                {
                                    valList.Add(" and " + pi.Name + "='" + val + "'");
                                }
                                else if (pi.PropertyType == typeof(int))
                                {
                                    valList.Add(" and " + pi.Name + "=" + val);
                                }
                            }
                        }

                    }
                    commandText = "select * from " + tableName + " where 1=1  " + string.Join(" ", valList) + " " + sqlParam;
                }
                else
                {
                    commandText = "select * from " + tableName;
                }

                
                return ExecuteDataTable<T>(commandText);


                //List<T> list = ExecuteDataTable<T>(commandText, commandParameters);
                //return list.Count > 0 ? list[0] : default(T);
            }
            catch (Exception)
            {

                return null;
            }
            
        }

        public static T ExecuteSingleData<T>(string commandText, params DbParameter[] commandParameters)
        {
            List<T> list= ExecuteDataTable<T>(commandText, commandParameters);
            return list.Count > 0? list[0]:default(T);
        }

        /// <summary>
        /// 使用此函数，对象名称必须与数据字段名称一致。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static T ExecuteSingleData<T>(T t, string tableName,string sqlParam="")
        {
            List<T> list = ExecuteDataTable<T>(t, tableName, sqlParam);
            return list.Count > 0 ? list[0] : default(T);
        }

        // 查询并返回sql语句执行后的第一行第一列的值
        public static object ExecuteScalar(string commandText, params DbParameter[] commandParameters)
        {
            return dbcommon.ExecuteScalar(commandText, commandParameters);
        }

        //查询表是否存在
        public static int IsTableExists(string tableName)
        {
            
            DbConnection conn = Activator.CreateInstance<C>() as DbConnection;
            conn.Open();

            DbCommand command = Activator.CreateInstance<S>() as DbCommand;
            command.Connection = conn;
            command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='" + tableName + "'";
            int val = Convert.ToInt32(command.ExecuteScalar());

            command.Dispose();
            conn.Close();
            conn.Dispose();
            return val;
        }

        /// <summary>
        /// 删除字段
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static int DropField(string fieldName,string tableName)
        {
            string sql = "ALTER TABLE WL_PARAMETERDATA drop (\"" + fieldName + "\")";
            return dbcommon.ExecuteNonQuery(sql);

        }

    }

    internal class DBSingletonCommon
    {
    }
}
