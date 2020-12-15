using DBLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DBLibrary
{
    public sealed class LocalDBCommon<C, S> : IDBCommon
    {
        //static LocalDBCommon()
        //{
        //    if (ConnectionString == null)
        //    {
        //        ConnectionString = AppConfigLibrary.AppConfigClass.GetValue("DataSource");
        //    }
        //}

        private LocalDBCommon()
        {
            //ConnectionString = AppConfigLibrary.AppConfigClass.GetValue("DataSource");
        }
        //public LocalDBCommon(string connectionString)
        //{
        //    ConnectionString = connectionString;
        //}

        private static readonly Lazy<LocalDBCommon<C, S>> lazy = new Lazy<LocalDBCommon<C, S>>(() => new LocalDBCommon<C, S>());

        public static LocalDBCommon<C, S> Instance { get { return lazy.Value; } }

        //private string _connectionString;
        /// <summary>
        /// 获取连接字符串
        /// Oracle:
        /// "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=SANY)));Persist Security Info=True;User ID=SANY;Password=SANY;"
        /// SQLite:
        /// Data Source=MyDatabase.db;Version=3;
        /// 默认情况下，外键约束是关闭的，当设置打开时并不能持久保持，foreign key 标注是基于会话的，也就是说每当连接关闭时，信息也就丢失了，SQLite连接字符串支持添加参数。可以在每次查询中用下面形式的连接字符串来都设置。foreign keys=true;
        /// Data Source=Data Source=" + DBPath + ";Version=3;foreign keys=true;";
        /// </summary>
        public string ConnectionString { get; set; }

        public string GetIP => new Regex(@"\(" + "[^*]+" + @"\)").Match(this.ConnectionString)+"";


        //public string Username{ get; set; }

        //public string Password { get; set; }




        /// <summary>
        /// 测试数据库是否连接成功.成功返回空字符串,失败返回错误原因.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public string ConnectionTest(string connectionString,params object[] obj)
        {
            try
            {
                //string connectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST="+ip+")(PORT=1521))(CONNECT_DATA=(SERVICE_NAME="+ databaseName + ")));Persist Security Info=True;User ID="+ username + ";Password="+password+";";
                DbConnection conn = Activator.CreateInstance<C>() as DbConnection;
                conn.ConnectionString = connectionString;
                conn.Open();
                ConnectionString = connectionString;
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
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
        public int ExecuteNonQuery(string commandText, params DbParameter[] commandParameters)
        {
            try
            {
                using (DbConnection conn = Activator.CreateInstance<C>() as DbConnection)
                {
                    conn.ConnectionString = ConnectionString;
                    using (DbCommand command = Activator.CreateInstance<S>() as DbCommand)
                    {
                        //commandText = commandText.ToUpper();
                        if (commandParameters != null && commandParameters.Count() != 0)
                        {
                            foreach (DbParameter dbp in commandParameters)
                            {
                                if (dbp.DbType.ToString() == "String")
                                {
                                    Regex reg = new Regex(":" + dbp.ParameterName.ToUpper(), RegexOptions.IgnoreCase);
                                    string txt = commandText = commandText = reg.Replace(commandText, "'" + dbp.Value + "'", 1);
                                }
                                else if (dbp.DbType.ToString().Contains("Int"))
                                {
                                    commandText = commandText.Replace(":" + dbp.ParameterName.ToUpper(), dbp.Value + "");

                                }

                            }
                            //command.Parameters.AddRange(commandParameters);
                        }
                        conn.Open();
                        command.CommandText = commandText;
                        command.Connection = conn;
                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        // 查询并返回datatable
        public DataTable ExecuteDataTable(string commandText, params DbParameter[] commandParameters)
        {
            try
            {
                using (DbConnection conn = Activator.CreateInstance<C>() as DbConnection)
                {
                    conn.ConnectionString = ConnectionString;
                    using (DbCommand command = Activator.CreateInstance<S>() as DbCommand)
                    {
                        conn.Open();
                        command.Connection = conn;
                        command.CommandText = commandText;
                        if (commandParameters != null && commandParameters.Count() != 0)
                            command.Parameters.AddRange(commandParameters);
                        DbDataReader dr = command.ExecuteReader();
                        //Console.WriteLine(string.Format("AwbPre:{0},AwbNo:{1}", dr["name"], dr["cname"]));
                        DataTable dt = new DataTable();
                        dt.Load(dr);//来把查询到的数据插入到DataTable中
                        return dt;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            
        }

        // 查询并返回sql语句执行后的第一行第一列的值
        public object ExecuteScalar(string commandText, params DbParameter[] commandParameters)
        {
            using (DbConnection conn = Activator.CreateInstance<C>() as DbConnection)
            {
                conn.ConnectionString = ConnectionString;
                using (DbCommand command = Activator.CreateInstance<S>() as DbCommand)
                {
                    conn.Open();
                    command.Connection = conn;
                    command.CommandText = commandText;
                    if (commandParameters != null)
                        command.Parameters.AddRange(commandParameters);
                    object obj = command.ExecuteScalar();
                    return obj;
                }
            }
        }

        //查询表是否存在
        public int IsTableExists(string tableName)
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

    }
}
