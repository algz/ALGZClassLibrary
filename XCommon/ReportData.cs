using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XCommon
{
    public class ReportData
    {
        /// <summary>
        /// 重量名称
        /// </summary>
        public string name
        {
            get;
            set;
        }

        /// <summary>
        /// 分配重量
        /// </summary>
        public string weight
        {
            get;
            set;
        }

        /// <summary>
        /// 百分比
        /// </summary>
        public string percent
        {
            get;
            set;
        }

        /// <summary>
        /// 重心前限前m
        /// </summary>
        public string topMargin
        {
            get;
            set;
        }

        /// <summary>
        /// 重心后限后m
        /// </summary>
        public string bottomMargin
        {
            get;
            set;
        }

        /// <summary>
        /// 转动惯量值
        /// </summary>
        public string inertiaValue
        {
            get;
            set;
        }

        /// <summary>
        /// 公式内容
        /// </summary>
        public FormulaData[] formulaContent
        {
            get;
            set;
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get;
            set;
        }

    }

    /// <summary>
    /// 公式数据
    /// </summary>
    public class FormulaData
    {
        /// <summary>
        /// 主标题
        /// </summary>
        public string MainTile
        {
            get;
            set;
        }

        /// <summary>
        /// 公式表达式
        /// </summary>
        public string FormulaExpression
        {
            get;
            set;
        }

        public string[] FormulaDetail
        {
            get;
            set;
        }

        public string FormulaValue
        {
            get;
            set;
        }
        
    }
}
