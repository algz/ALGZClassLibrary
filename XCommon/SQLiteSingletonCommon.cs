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
    /// 单例模式，不缓存，执行完后关闭数据库连接。不支持left join等级联，建议用缓存模式。
    /// </summary>
    public class SQLiteSingletonCommon
    {

        /// <summary>
        /// 获取连接字符串
        /// Data Source=MyDatabase.db;Version=3;
        /// 默认情况下，外键约束是关闭的，当设置打开时并不能持久保持，foreign key 标注是基于会话的，也就是说每当连接关闭时，信息也就丢失了，SQLite连接字符串支持添加参数。可以在每次查询中用下面形式的连接字符串来都设置。foreign keys=true;
        /// Data Source=Data Source=" + DBPath + ";Version=3;foreign keys=true;";
        /// </summary>
        public static string ConnectionString
        {
            get;set;
        }

        /// <summary>
        /// 参数里面标记为params即可以传0个，也可以传null
        /// SQLiteParameter name = new SQLiteParameter("name",  "SUSAN LI");
        /// SQLiteParameter sex = new SQLiteParameter("sex", DbType.Int16, 1);
        /// SQLiteParameter age = new SQLiteParameter("age", DbType.Int16, 30);
        /// SQLiteParameter[] pa = { name, sex, age };
        /// SQLiteSingletonCommon.ExecuteNonQuery("INSERT INTO USER6 (NAME,SEX,AGE) values (@name,@sex,@age)",pa);
        /// SQL语法：
        /// 添加：INSERT INTO TABLE_NAME [(column1,...,columnN)] VALUES(value1,...,valueN);
        /// 修改：UPDATE table_name SET column1 = value1,...., columnN = valueN WHERE[condition];
        /// 删除：DELETE FROM table_name WHERE[condition];
        /// </summary>
        /// <param name="commandText">SQL语句（命名参数前用‘@’符号）</param>
        /// <param name="commandParameters">可以传0个，也可以传null，或者不传值</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string commandText, params SQLiteParameter[] commandParameters)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand(commandText, conn))
                {
                    conn.Open();
                    if (commandParameters != null&&commandParameters.Count()!=0)
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
                    if (commandParameters != null&&commandParameters.Count()!=0)
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
