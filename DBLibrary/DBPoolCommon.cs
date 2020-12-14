//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Data.SQLite;
//using System.IO;
//using System.Linq;
//using System.Text;

//namespace XCommon
//{
//    public class SQLitePoolCommon
//    {
//        private SQLiteConnection conn = new SQLiteConnection();
//        private SQLiteCommand command = new SQLiteCommand();
//        private SQLiteDataAdapter adapter = new SQLiteDataAdapter();

//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        /// <param name="ConnectionString">例:"Data Source=MyDatabase.db;Version=3;"</param>
//        public SQLitePoolCommon(string ConnectionString)
//        {
//            conn.ConnectionString= ConnectionString;
//            conn.Open();
//            command.Connection = conn;
//        }

//        /// <summary>
//        /// 析构函数
//        /// </summary>
//        ~SQLitePoolCommon()
//        {
//            adapter.Dispose();
//            command.Dispose();
//            //conn.Close();
//            conn.Dispose();
//        }




//        /// <summary>
//        /// 使用传入已初始化完成并且配置了conn的command
//        /// 此函数的作用是重用，即如果一个频繁的数据库操作，不要总是关闭及开启，而是要执行完毕再关闭即可
//        /// 所以在这里不要执行关闭了
//        /// 在调用之前先要配置conn和command：

//        // SQLiteConnection conn = new SQLiteConnection(ConnectionString);
//        // conn.Open();
//        // SQLiteCommand command = new SQLiteCommand();
//        // command.Connection = conn;
//        /// </summary>
//        /// <param name="commandText"></param>
//        /// <param name="commandParameters"></param>
//        /// <returns></returns>
//        public int ExecuteNonQuery(string commandText, params SQLiteParameter[] commandParameters)
//        {
//            if (command.Connection.State == ConnectionState.Closed)
//                command.Connection.Open();
//            command.CommandText = commandText;
//            command.Parameters.Clear();
//            if (commandParameters != null)
//                command.Parameters.AddRange(commandParameters);
//            return command.ExecuteNonQuery();
//        }

//        // 
//        /// <summary>
//        /// 查询并返回datatable.
//        /// 注意; 一个Form窗口中仅一个GridView,且只能用一个“缓存对象”。不然无法使用update()更新，并报“对于多个基表不支持动态SQL生成”异常。
//        /// 多个GridView，其它GridView请使用“单例模式”。
//        /// 
//        /// sql语句，不支直接传数组参数。
//        /// 例：
//        /// select * from tablename where id in (@ids)
//        /// ids必须自行组合成字符串，在通过SQLiteParameter传字符串类型绑定参数，不能直接把数组传给ids.
//        /// </summary>
//        /// <param name="commandText">SQL语句</param>
//        /// <param name="commandParameters">不支持数组。对于in子句的数组查询，需要自动组合成字符串</param>
//        /// <returns></returns>
//        public DataTable ExecuteDataTable(string commandText, params SQLiteParameter[] commandParameters)
//        {
//            if (command.Connection.State == ConnectionState.Closed)
//                command.Connection.Open();
//            command.CommandText = commandText;
//            if (commandParameters != null)
//                command.Parameters.AddRange(commandParameters);
//            adapter = new SQLiteDataAdapter(command);

//            //必须设置,不然不能用 adapter.Update() 方法
//            SQLiteCommandBuilder scb = new SQLiteCommandBuilder(adapter);
//            //adapter.UpdateCommand = scb;

//            DataTable data = new DataTable(); 
//            adapter.Fill(data);
//            return data;
//        }

//        /// <summary>
//        /// 如果无法更新到数据库或报“对于多个基表不支持动态SQL生成”异常，
//        /// 请确定一个Form窗口中是否有多个GridView，并且都在使用“缓存对象”？如果是，其它GridView请使用“单例模式”。
//        /// 
//        ////*有三种方法可以删除 DataTable 中的 DataRow：
//        /// Delete 方法和 Remove 方法和 RemoveAt 方法
//        /// 其区别是：
//        /// Delete 方法实际上不是从 DataTable 中删除掉一行，而是将其标志为删除，仅仅是做个记号，
//        /// Remove 方法则是真正的从 DataRow 中删除一行，
//        /// RemoveAt 方法是根据行的索引来删除。*/
//        /// //dt.Rows[index].Delete(); //数据双向绑定，必须使用delete标记，然后数据源才能提交。
//        /// </summary>
//        /// <param name="dt"></param>
//        public void Update(DataTable dt)
//        {
//            try
//            {
//                adapter.Update(dt);
//            }
//            catch
//            {
//            }
            
//            //不添加，在多次更新时，会报异常：违反并发性: UpdateCommand 影响了预期 1 条记录中的 0 条。
//            //原因是数据库里数据修改了，但内存的数据未修改，导致数据库内存的数据不一致。
//            //dt.AcceptChanges();
//        }

//        /// <summary>
//        ///  创建参数
//        /// </summary>
//        /// <param name="parameterName"></param>
//        /// <param name="parameterType"></param>
//        /// <param name="parameterValue"></param>
//        /// <returns></returns>
//        public static SQLiteParameter CreateParameter(string parameterName, System.Data.DbType parameterType, object parameterValue)
//        {
//            SQLiteParameter parameter = new SQLiteParameter();
//            parameter.DbType = parameterType;
//            parameter.ParameterName = parameterName;
//            parameter.Value = parameterValue;
//            return parameter;
//        } 
//    }
//}
