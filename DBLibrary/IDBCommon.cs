﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace DBLibrary
{
    /// <summary>
    /// WebDB,通过web端连接远程数据库;LocalDB,本地直连(远程)数据库;MixDB,连接本地数据库,可通过web提交给远程数据库.
    /// 1.使用接口时,必须先调用ConnectionTest 方法.
    /// </summary>
    public enum DBConectionMode
    {
        [Description("本地数据库")]
        LocalDB,
        [Description("网络数据库")]
        WebDB,
        [Description("混和数据库")]
        MixDB
    }

    public interface IDBCommon
    {
        //string LoginUrl { get; set; }

        //string Username { get; set; }

        //string Password { get; set; }
        //string ConnectionUrl { get; set; }

        /// <summary>
        /// 获取连接字符串
        /// 1.Oracle:
        /// "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=SANY)));Persist Security Info=True;User ID=SANY;Password=SANY;"
        /// 2.SQLite:
        /// "Data Source=SANY.db;Version=3;"
        /// 默认情况下，外键约束是关闭的，当设置打开时并不能持久保持，foreign key 标注是基于会话的，也就是说每当连接关闭时，信息也就丢失了，SQLite连接字符串支持添加参数。可以在每次查询中用下面形式的连接字符串来都设置。foreign keys=true;
        /// Data Source=Data Source=" + DBPath + ";Version=3;foreign keys=true;";
        /// 3.MySql:
        /// "Database=SANY;Data Source=127.0.0.1;Port=3306;User Id=SANY;Password=SANY;Charset=utf8;TreatTinyAsBoolean=false;"
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// 使用接口前,必须先调用此方法.
        /// 测试数据库是否连接成功.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        string ConnectionTest(string connectionString,params object[] obj);

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
         int ExecuteNonQuery(string commandText, params DbParameter[] commandParameters);

        // 查询并返回datatable
         DataTable ExecuteDataTable(string commandText, params DbParameter[] commandParameters);

        // 查询并返回sql语句执行后的第一行第一列的值
        object ExecuteScalar(string commandText, params DbParameter[] commandParameters);

        //string GetIP { get; }

        //string GetUserName { get; }

        //string GetPassword { get; }

        //查询表是否存在
        //int IsTableExists(string tableName);
    }
}