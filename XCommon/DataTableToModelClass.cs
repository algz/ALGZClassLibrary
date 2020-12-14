using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace XCommon
{
    /// <summary>
    /// DataTableToModel 的摘要说明
    /// </summary>
    public static class DataTableToModel
    {
        /// <summary>
        /// DataTable通过反射获取单个像
        /// </summary>
        public static T ToSingleModel<T>(this DataTable data) where T : new()
        {
            T t = data.GetList<T>(null, true).Single();
            return t;
        }


        /// <summary>
        /// DataTable通过反射获取单个像
        /// <param name="prefix">前缀</param>
        /// <param name="ignoreCase">是否忽略大小写，默认不区分</param>
        /// </summary>
        public static T ToSingleModel<T>(this DataTable data, string prefix, bool ignoreCase = true) where T : new()
        {
            T t = data.GetList<T>(prefix, ignoreCase).Single();
            return t;
        }

        /// <summary>
        /// DataTable通过反射获取多个对像
        /// </summary>
        /// <typeparam name="type"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<T> ToListModel<T>(this DataTable data) where T : new()
        {
            List<T> t = data.GetList<T>(null, true);
            return t;
        }


        /// <summary>
        /// DataTable通过反射获取多个对像
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <param name="ignoreCase">是否忽略大小写，默认不区分</param>
        /// <returns></returns>
        private static List<T> ToListModel<T>(this DataTable data, string prefix, bool ignoreCase = true) where T : new()
        {
            List<T> t = data.GetList<T>(prefix, ignoreCase);
            return t;
        }



        private static List<T> GetList<T>(this DataTable data, string prefix, bool ignoreCase = true) where T : new()
        {
            List<T> t = new List<T>();
            int columnscount = data.Columns.Count;
            if (ignoreCase)
            {
                for (int i = 0; i < columnscount; i++)
                    data.Columns[i].ColumnName = data.Columns[i].ColumnName.ToUpper();
            }
            try
            {
                var properties = new T().GetType().GetProperties();

                var rowscount = data.Rows.Count;
                for (int i = 0; i < rowscount; i++)
                {
                    var model = new T();
                    foreach (var p in properties)
                    {
                        var keyName = prefix + p.Name + "";
                        if (ignoreCase)
                            keyName = keyName.ToUpper();
                        for (int j = 0; j < columnscount; j++)
                        {
                            if (data.Columns[j].ColumnName == keyName && data.Rows[i][j] != null)
                            {
                                string pval = data.Rows[i][j].ToString();
                                if (!string.IsNullOrEmpty(pval))
                                {
                                    try
                                    {
                                        // We need to check whether the property is NULLABLE
                                        if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                        {
                                            p.SetValue(model, Convert.ChangeType(data.Rows[i][j], p.PropertyType.GetGenericArguments()[0]), null);
                                        }
                                        else
                                        {
                                            p.SetValue(model, Convert.ChangeType(data.Rows[i][j], p.PropertyType), null);
                                        }
                                    }
                                    catch (Exception x)
                                    {
                                        throw x;
                                    }
                                }
                                break;
                            }
                        }
                    }
                    t.Add(model);
                }
            }
            catch (Exception ex)
            {


                throw ex;
            }


            return t;
        }
    }
}
