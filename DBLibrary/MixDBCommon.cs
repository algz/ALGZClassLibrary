using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace DBLibrary
{
    /// <summary>
    /// 混合模式数据库管理.
    /// 本地sqlite,远程oracle
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="S"></typeparam>
    public class MixDBCommon<C, S> : IDBCommon
    {
        public LocalDBCommon<C,S> localDbCommon { get; set; }
        public WebDBCommon remoteDbCommon { get; set; }

        public MixDBCommon()
        {
        }
        public MixDBCommon(string connectionString)
        {
            ConnectionString = connectionString;
        }

        private static readonly Lazy<MixDBCommon<C, S>> lazy = new Lazy<MixDBCommon<C, S>>(() => new MixDBCommon<C, S>());

        public static MixDBCommon<C, S> Instance { get { return lazy.Value; } }


        public string ConnectionString { get; set; }

        //创建表
        private const string CREATE_TABLE = "CREATE TABLE {0} ({1})";
        //删除表
        private const string DROP_TABLE = "drop table 表名称";
        //查看所有表(TABLE)和视图(VIEW)
        private const string ALL_TABLE = "select table_name,table_type,comments from  user_tab_comments where 1=1 and {0}";
        //查看表的所有字段
        private const string TABLE_COLUMNS = "select * from user_tab_columns where table_name='{0}' ORDER BY COLUMN_ID";
        //查看所有视图及内容
        private const string USER_VIEWS = "select view_name,text from user_views t";

        private DataTable GetTablesAndViews()
        {
            return remoteDbCommon.ExecuteDataTable(ALL_TABLE);
        }

        private void CreateTable(string tableName)
        {
            string sql = string.Format(ALL_TABLE, "table_type='TABLE'");
            DataTable tables = remoteDbCommon.ExecuteDataTable(sql);
            foreach(DataRow dr in tables.Rows)
            {
                sql = string.Format(TABLE_COLUMNS, dr[0]);
                DataTable columns = remoteDbCommon.ExecuteDataTable(sql);
            }
            
            
        }

        private void CreateView(string tableName)
        {
            string sql = string.Format(USER_VIEWS, tableName);
            DataTable views = remoteDbCommon.ExecuteDataTable(sql);
        }

        /// <summary>
        /// 初始化本地数据库,把远程数据库的表映射为本地数据库表.
        /// </summary>
        public void InitLocalDB(DataTable dt)
        {
            DataTable tableview = GetTablesAndViews();
            foreach(DataRow dr in tableview.Rows)
            {
                switch (dr[1] + "")
                {
                    case "TABLE":
                        CreateTable(dr[1]+"");
                        break;
                    case "VIEW":
                        CreateView(dr[1] + "");
                        break;
                }
            }
            //string.Format(CREATE_TABLE,"tablename", "id NUMBER NOT NULL ,PRIMARY KEY("id")")
        }

        /// <summary>
        /// 同步数据-上传.
        /// </summary>
        /// <param name="localDt"></param>
        /// <returns></returns>
        public string SyncUpDataTable(DataTable localDt,SQLConvertPattern patten)
        {
            List<string> sqlList=ConvertToSql(localDt, patten);
            foreach(string sql in sqlList)
            {
                remoteDbCommon.ExecuteNonQuery(sql);
            }
            
            //string postString=WebLibrary.Serialize.SerializeJSONClass.Serialize(localDt);
            //byte[] postData = Encoding.ASCII.GetBytes(postString);
            //remoteDbCommon.GetWebClientExt.UploadData("", postData);
            //remoteDbCommon.ExecuteNonQuery(remoteDbCommon);
            return "";
        }

        /// <summary>
        /// 同步数据-下载
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public DataTable SyncDownDataTable(string commandText, params DbParameter[] commandParameters)
        {
            DataTable remoteDt=remoteDbCommon.ExecuteDataTable(commandText, commandParameters);
            //1.删除原数据
            List<string> list = ConvertToSql(remoteDt, SQLConvertPattern.Delete);
            foreach (string sql in list)
            {
                try
                {
                    localDbCommon.ExecuteNonQuery(sql);
                }
                catch
                {
                    continue;
                }
            }
            //2.添加新数据
            list = ConvertToSql(remoteDt,SQLConvertPattern.Insert);
            foreach(string sql in list)
            {
                try
                {
                    localDbCommon.ExecuteNonQuery(sql);
                }
                catch
                {
                    continue;
                }
            }
            return remoteDt;
        }

       

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

        public DataTable ExecuteDataTable(string commandText, params DbParameter[] commandParameters)
        {
            //string msg;
            //string url = "http://localhost:8080/algz/common/sql/executedatatable?sql="+ commandText;
            //WebLibrary.Http.WebClientExtClass ca = WebLibrary.Http.HttpClass.Login_CSRF("http://127.0.0.1:8080/algz/login", "test", "test", out msg);
            //string txt=ca.DownloadString(url);
            //var obj = WebLibrary.Serialize.SerializeJSONClass.Deserialize<DataTable>(txt);
            //return obj;
            //return new DataTable();
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

        public int ExecuteNonQuery(string commandText, params DbParameter[] commandParameters)
        {
            using (DbConnection conn = Activator.CreateInstance<C>() as DbConnection)
            {
                conn.ConnectionString = ConnectionString;
                using (DbCommand command = Activator.CreateInstance<S>() as DbCommand)
                {
                    conn.Open();
                    command.CommandText = commandText;
                    command.Connection = conn;
                    if (commandParameters != null && commandParameters.Count() != 0)
                        command.Parameters.AddRange(commandParameters);
                    return command.ExecuteNonQuery();
                }
            }
        }

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


        #region DataTable ConvertTo SQL

        public enum SQLConvertPattern
        {
            Insert,Update,Delete
        }

        private List<string> ConvertToSql(DataTable dt, SQLConvertPattern pattern)
        {
            List<string> list = new List<string>();
            switch (pattern)
            {
                case (SQLConvertPattern.Insert):
                    list = GenerateInserSql(dt);
                    break;
                case (SQLConvertPattern.Update):
                    list = GenerateUpdateSql(dt);
                    break;
                case (SQLConvertPattern.Delete):
                    list = GenerateDelSql(dt);
                    break;
            }
            return list;
        }

        /// <summary>
        /// 生成插入数据的sql语句。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="command"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<string> GenerateInserSql(DataTable dt)
        {
            List<string> list = new List<string>();
            StringBuilder names = new StringBuilder();
            StringBuilder values = new StringBuilder();

            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    if (names.Length > 0)
                    {
                        names.Append(",");
                        values.Append(",");
                    }
                    names.AppendFormat("{0}", col.ColumnName);
                    values.AppendFormat("{0}", row[col.ColumnName]);
                }
                list.Add(string.Format("INSERT INTO {0}({1}) VALUES ({2})", dt.TableName, names, values));
            }
            return list;
        }

        /// <summary>
        /// 生成更新数据的sql语句。(仅支持单主键.)
        /// </summary>
        /// <param name="database"></param>
        /// <param name="command"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<string> GenerateUpdateSql(DataTable dt)
        {
            List<string> list = new List<string>();
            StringBuilder values = new StringBuilder();
            DataColumn[] keys = dt.PrimaryKey;
            DataColumn keyCol = keys.Length == 0 ? dt.Columns[0] : keys[0];
            foreach (DataRow row in dt.Rows)
            {
                string keyVal = keyCol.ColumnName + "='" + row[keyCol.ColumnName] + "'";
                foreach (DataColumn col in dt.Columns)
                {
                    if (values.Length > 0)
                    {
                        values.Append(",");
                    }
                    values.AppendFormat("{0}", col.ColumnName + "='" + row[col.ColumnName] + "'");
                }
                list.Add(string.Format("update {0} set {1} where {2}", dt.TableName, values, keyVal));
            }
            return list;
        }

        /// <summary>
        /// 生成删除数据的sql语句。(仅支持单主键.)
        /// </summary>
        /// <param name="database"></param>
        /// <param name="command"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<string> GenerateDelSql(DataTable dt)
        {
            List<string> list = new List<string>();
            StringBuilder values = new StringBuilder();
            DataColumn[] keys = dt.PrimaryKey;
            DataColumn keyCol = keys.Length == 0 ? dt.Columns[0] : keys[0];
            foreach (DataRow row in dt.Rows)
            {
                string keyVal = keyCol.ColumnName + "='" + row[keyCol.ColumnName] + "'";
                list.Add(string.Format("delete from {0} where {1}", dt.TableName, keyVal));
            }
            return list;
        }

        #endregion

    }
}
