using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XCommon
{
    public class CommonMessage
    {

        #region 操作类型

        /// <summary>
        /// 没有操作
        /// </summary>
        public static string operNone = string.Empty;

        /// <summary>
        /// 新建
        /// </summary>
        public static string operNew = "new";
        /// <summary>
        /// 基于新建
        /// </summary>
        public static string operJYNew = "JYNew";
        /// <summary>
        /// 编辑
        /// </summary>
        public static string operEdit = "edit";
        /// <summary>
        /// 删除
        /// </summary>
        public static string operDelete = "delete";
        /// <summary>
        /// 确认
        /// </summary>
        public static string operConfirm = "confirm";
        /// <summary>
        /// 取消
        /// </summary>
        public static string operCancle = "cancle";

        #endregion

        /// <summary>
        /// 是否重量数据导入
        /// </summary>
        public static bool IsWeightDataImport = false;

    }
}
