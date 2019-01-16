using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace XCommon
{
    public class SQLiteClass
    {


        #region



        /// <summary>
        /// 创建数据库文件
        /// </summary>
        /// <param name="fileName">文件目录</param>
        public static void CreateDBFile(string fileName)
        {
            string path = System.Environment.CurrentDirectory + @"/Data/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string databaseFileName = path + fileName;
            if (!File.Exists(databaseFileName))
            {
                SQLiteConnection.CreateFile(databaseFileName);
            }
        }

        //生成连接字符串
        private static string CreateConnectionString()
        {
            SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder();
            connectionString.DataSource = @"data/ScriptHelper.db";

            string conStr = connectionString.ToString();
            return conStr;
        }

        /// <summary>
        /// 对插入到数据库中的空值进行处理
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ToDbValue(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// 对从数据库中读取的空值进行处理
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object FromDbValue(object value)
        {
            if (value == DBNull.Value)
            {
                return null;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// 执行非查询的数据库操作
        /// </summary>
        /// <param name="sqlString">要执行的sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回受影响的条数</returns>
        public static int ExecuteNonQuery1(string sqlString, params SQLiteParameter[] parameters)
        {
            string connectionString = CreateConnectionString();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlString;
                    foreach (SQLiteParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 执行查询并返回查询结果第一行第一列
        /// </summary>
        /// <param name="sqlString">SQL语句</param>
        /// <param name="sqlparams">参数列表</param>
        /// <returns></returns>
        public static object ExecuteScalar(string sqlString, params SQLiteParameter[] parameters)
        {
            string connectionString = CreateConnectionString();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlString;
                    foreach (SQLiteParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    return cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// 查询多条数据
        /// </summary>
        /// <param name="sqlString">SQL语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回查询的数据表</returns>
        public static DataTable GetDataTable(string sqlString, params SQLiteParameter[] parameters)
        {
            string connectionString = CreateConnectionString();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlString;
                    foreach (SQLiteParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    DataSet ds = new DataSet();
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                    adapter.Fill(ds);
                    return ds.Tables[0];
                }
            }
        }

        #endregion

        //private static string connectionString = "";

        /// <summary>
        /// 获取连接字符串
        /// Data Source=MyDatabase.db;Version=3;
        /// </summary>
        public static string ConnectionString
        {
            get;set;
        }

        // 创建表，创建表连接只需要用一次，所以新建并释放就可以了
        // 直接传入要执行的sql语句就可以，因为将来可能涉及添加索引等复杂需求，如果用动态创建的方案，局限性比较大
        public static int CreateTable(string commandText)
        {
            SQLiteConnection conn = new SQLiteConnection(ConnectionString);
            conn.Open();

            SQLiteCommand command = new SQLiteCommand();
            command.Connection = conn;
            command.CommandText = commandText;
            int val = command.ExecuteNonQuery();

            command.Dispose();// 释放command
            conn.Close();
            conn.Dispose();

            return val;
        }

        /// <summary>
        /// 参数里面标记为params即可以传0个，也可以传null
        /// SQLiteParameter name = MySQLiteHelper.CreateParameter("name", DbType.String, "SUSAN LI");
        /// SQLiteParameter sex = MySQLiteHelper.CreateParameter("sex", DbType.Int16, 1);
        /// SQLiteParameter age = MySQLiteHelper.CreateParameter("age", DbType.Int16, 30);
        /// SQLiteParameter[] pa = new SQLiteParameter[3] { name, sex, age };
        /// MySQLiteHelper.ExecuteNonQuery("INSERT INTO USER6 (NAME,SEX,AGE) values (@name,@sex,@age)",pa);
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters">可以传0个，也可以传null</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string commandText, params SQLiteParameter[] commandParameters)
        {
            SQLiteConnection conn = new SQLiteConnection(ConnectionString);
            conn.Open();

            SQLiteCommand command = new SQLiteCommand(commandText,conn);
            if (commandParameters != null)
                command.Parameters.AddRange(commandParameters);
            int val = command.ExecuteNonQuery();

            command.Dispose();
            conn.Close();
            conn.Dispose();
            return val;
        }

        // 使用传入已初始化完成并且配置了conn的command
        // 此函数的作用是重用，即如果一个频繁的数据库操作，不要总是关闭及开启，而是要执行完毕再关闭即可
        // 所以在这里不要执行关闭了
        // 在调用之前先要配置conn和command：

        // SQLiteConnection conn = new SQLiteConnection(ConnectionString);
        // conn.Open();
        // SQLiteCommand command = new SQLiteCommand();
        // command.Connection = conn;
        public static int ExecuteNonQuery(SQLiteCommand command, string commandText, params SQLiteParameter[] commandParameters)
        {
            if (command.Connection.State == ConnectionState.Closed)
                command.Connection.Open();
            command.CommandText = commandText;
            command.Parameters.Clear();
            if (commandParameters != null)
                command.Parameters.AddRange(commandParameters);
            return command.ExecuteNonQuery();
        }

        // 查询并返回datatable
        public static DataTable ExecuteDataTable(string commandText, params SQLiteParameter[] commandParameters)
        {
            SQLiteConnection conn = new SQLiteConnection(ConnectionString);
            conn.Open();

            SQLiteCommand command = new SQLiteCommand();
            command.Connection = conn;
            command.CommandText = commandText;
            if (commandParameters != null)
                command.Parameters.AddRange(commandParameters);
            // 开始读取
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            DataTable data = new DataTable();
            adapter.Fill(data);
            // dispose
            adapter.Dispose();
            command.Dispose();
            conn.Close();
            conn.Dispose();

            return data;
        }

        // 创建参数
        public static SQLiteParameter CreateParameter(string parameterName, System.Data.DbType parameterType, object parameterValue)
        {
            SQLiteParameter parameter = new SQLiteParameter();
            parameter.DbType = parameterType;
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;
            return parameter;
        }

        //查询表是否存在
        public static int IsTableExists(string tableName)
        {
            SQLiteConnection conn = new SQLiteConnection(ConnectionString);
            conn.Open();

            SQLiteCommand command = new SQLiteCommand();
            command.Connection = conn;
            command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='" + tableName + "'";
            int val = Convert.ToInt32(command.ExecuteScalar());

            command.Dispose();
            conn.Close();
            conn.Dispose();
            return val;
        }

        ////////////////////////////////

        
    }
}
