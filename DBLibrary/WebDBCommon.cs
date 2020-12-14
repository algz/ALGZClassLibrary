using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WebLibrary.Http;

namespace DBLibrary
{
    public sealed class WebDBCommon : IDBCommon
    {
        //static WebDBCommon()
        //{
        //    if (ConnectionUrl == null)
        //    {
        //        //string url = "http://localhost:8080/algz/common/sql/executedatatable?sql=";

        //        ConnectionUrl = AppConfigLibrary.AppConfigClass.GetValue("WEBDataSource");
        //        //WebClientExtClass ca =HttpClass.Login_CSRF("http://127.0.0.1:8080/algz/login", "test", "test", out msg);

        //    }
        //}

        //public WebDBCommon()
        //{

        //}

        //public WebDBCommon(string executeDatatableUrl,string executeScalarUrl,string executeNonQueryUrl) : this()
        //{
        //    this._executeDatatableUrl = executeDatatableUrl;
        //    this._executeScalarUrl = executeScalarUrl;
        //    this._executeNonQueryUrl = executeNonQueryUrl;
        //}

        private static readonly Lazy<WebDBCommon> lazy =new Lazy<WebDBCommon>(() => new WebDBCommon());

        public static WebDBCommon Instance { get { return lazy.Value; } }


        //private static WebClientExtClass _ca = null;
        //public WebClientExtClass GetWebClientExt { get { return WebDBCommon._ca; } }

        private string loginUrl;
        private string executeDatatableUrl;
        private string executeScalarUrl;
        private string executeNonQueryUrl;


        private string _loginUsername;
        public string LoginUsername { get { return _loginUsername; } }

        private string _loginPassword;
        public string LoginPassword { get { return _loginPassword; } }

        private static readonly HttpClass http = new HttpClass();

        private string _connectionString;
        /// <summary>
        /// 获取连接字符串
        /// Oracle:
        /// "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=SANY)));Persist Security Info=True;User ID=SANY;Password=SANY;"
        /// SQLite:
        /// Data Source=MyDatabase.db;Version=3;
        /// 默认情况下，外键约束是关闭的，当设置打开时并不能持久保持，foreign key 标注是基于会话的，也就是说每当连接关闭时，信息也就丢失了，SQLite连接字符串支持添加参数。可以在每次查询中用下面形式的连接字符串来都设置。foreign keys=true;
        /// Data Source=Data Source=" + DBPath + ";Version=3;foreign keys=true;";
        /// </summary>
        public string ConnectionString{
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString=value;
                loginUrl = value + "/login";
                executeDatatableUrl = value + "/executedatatable";
                executeScalarUrl = value + "/executescalar";
                executeNonQueryUrl = value + "/executenonquery";
            }
        }

        /// <summary>
        /// 测试数据库是否连接成功.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public string ConnectionTest(string ConnectionUrl,params object[] obj)
        {
            if (obj.Length >= 2)
            {
                _loginUsername = obj[0] + "";
                _loginPassword = obj[1] + "";
                //"http://127.0.0.1:8080/algz/login"  test  test
                string msg = http.Login(loginUrl, _loginUsername, _loginPassword);
                return msg;
            }else
            {
                return "{0}用户名和{1}密码没有输入.";
            }
        }

        
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
        public  int ExecuteNonQuery(string commandText, params DbParameter[] commandParameters)
        {

            string url = this.executeNonQueryUrl+"?sql=" + ConvertDBParameters(commandText, commandParameters);
            string txt = http.GetString(url);// _ca.DownloadString(url);
            //var obj = WebLibrary.Serialize.SerializeJSONClass.Deserialize<DataTable>(txt);
            return 1;
        }

        public int ExecuteBatchNonQuery(string[] commandText)
        {
            string txt= http.PostString(this.executeNonQueryUrl+"/batch", commandText);
            return 1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public  DataTable ExecuteDataTable(string commandText, params DbParameter[] commandParameters)
        {
            string url = this.executeDatatableUrl + "?sql=" + ConvertDBParameters(commandText, commandParameters);
            string txt = http.GetString(url);// _ca.DownloadString(url);
            if (txt == "[]")
            {
                //string str1 = "select column_name from user_tab_columns where";
                //Regex rgx = new Regex(@"from[\s]+([\w]+)[\s]*");
                //string tableName = rgx.Match(commandText).Value.Substring(4);

                //string sql = "select column_name from user_tab_columns  where TABLE_NAME = '"+tableName+"' order by column_id";
                //url = ExecuteDatatableUrl + sql;
                //txt = ca.DownloadString(url);
                //var v=WebLibrary.Serialize.SerializeJSONClass.Deserialize<DataTable>(txt);
                //DataTable dt = new DataTable();
                //foreach()
                return null;

            }
            else
            {
                var obj = WebLibrary.Serialize.SerializeJSONClass.Deserialize<DataTable>(txt);
                return obj;
            }

        }


        /// <summary>
        /// 查询并返回sql语句执行后的第一行第一列的值
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public  object ExecuteScalar(string commandText, params DbParameter[] commandParameters)
        {
            string url = this.executeScalarUrl + "?sql=" + ConvertDBParameters(commandText, commandParameters);
            string txt = http.GetString(url);// _ca.DownloadString(url);
            //var dt = WebLibrary.Serialize.SerializeJSONClass.Deserialize<DataTable>(txt);
            Decimal d = Convert.ToDecimal(txt);
            return d;
            
        }

        //查询表是否存在
        public  int IsTableExists(string tableName)
        {
            return 0;
            //DbConnection conn = Activator.CreateInstance<C>() as DbConnection;
            //conn.Open();

            //DbCommand command = Activator.CreateInstance<S>() as DbCommand;
            //command.Connection = conn;
            //command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='" + tableName + "'";
            //int val = Convert.ToInt32(command.ExecuteScalar());

            //command.Dispose();
            //conn.Close();
            //conn.Dispose();
            //return val;
        }

        private static string ConvertDBParameters(string commandText, params DbParameter[] commandParameters)
        {
            if (commandParameters.Length == 0)
            {
                return commandText;
            }
            else
            {
                StringBuilder str = new StringBuilder(commandText);
                foreach (DbParameter dbp in commandParameters)
                {
                    string val = dbp.Value + "";
                    if (dbp.DbType == DbType.String)
                    {
                        val = "'" + val + "'";
                    }
                    str.Replace(":" + dbp.ParameterName, val);
                }
                return str.ToString();
            }
        }
    }
}
