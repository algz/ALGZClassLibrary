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
    /// <summary>
    /// 单例模式，不缓存，执行完后关闭数据库连接。
    /// </summary>
    public class SQLiteSingletonCommon
    {

        /// <summary>
        /// 获取连接字符串
        /// Data Source=MyDatabase.db;Version=3;
        /// </summary>
        public static string ConnectionString
        {
            get;set;
        }

        /// <summary>
        /// 参数里面标记为params即可以传0个，也可以传null
        /// SQLiteParameter name = MySQLiteHelper.CreateParameter("name", DbType.String, "SUSAN LI");
        /// SQLiteParameter sex = MySQLiteHelper.CreateParameter("sex", DbType.Int16, 1);
        /// SQLiteParameter age = MySQLiteHelper.CreateParameter("age", DbType.Int16, 30);
        /// SQLiteParameter[] pa = new SQLiteParameter[3] { name, sex, age };
        /// MySQLiteHelper.ExecuteNonQuery("INSERT INTO USER6 (NAME,SEX,AGE) values (@name,@sex,@age)",pa);
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">可以传0个，也可以传null</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string commandText, params SQLiteParameter[] commandParameters)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand(commandText, conn))
                {
                    conn.Open();
                    if (commandParameters != null)
                        command.Parameters.AddRange(commandParameters);
                    return command.ExecuteNonQuery();
                }
            }
        }

        // 查询并返回datatable
        public static DataTable ExecuteDataTable(string commandText, params SQLiteParameter[] commandParameters)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                using(SQLiteCommand command = new SQLiteCommand())
                {
                    conn.Open();
                    command.Connection = conn;
                    command.CommandText = commandText;
                    if (commandParameters != null)
                        command.Parameters.AddRange(commandParameters);
                    SQLiteDataReader dr=command.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(dr);//来把查询到的数据插入到DataTable中
                    return dt;
                }
            }
        }

        // 查询并返回sql语句执行后的第一行第一列的值
        public static object ExecuteScalar(string commandText, params SQLiteParameter[] commandParameters)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand())
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
    }
}
