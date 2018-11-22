using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Reflection;

namespace XCommon
{
    public class CommonExcel
    {

        public List<string> lstColumn = null;
        /// <summary>
        /// 创建Excel
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="fileName">文件名</param>
        /// <param name="sheetNames">工作表名</param>
        public void CreateExcel(string title, string FilePath, string sheetNames, System.Data.DataTable dt)
        {
            ////待生成的文件名称
            //FileInfo fi = new FileInfo(FilePath);
            //if (fi.Exists)     //判断文件是否已经存在,如果存在就删除!
            //{
            //    fi.Delete();
            //}
            if (sheetNames != null && sheetNames != "")
            {
                Excel.Application m_Excel = new Excel.ApplicationClass();//创建一个Excel对象(同时启动EXCEL.EXE进程) 
                m_Excel.SheetsInNewWorkbook = 1;//工作表的个数 

                Excel._Workbook m_Book = (Excel._Workbook)(m_Excel.Workbooks.Add(Missing.Value));//添加新工作簿
                Excel.Worksheet m_Sheet = (Excel.Worksheet)m_Book.Worksheets[1];//定义ws为工作文档中的第一个sheet

                ListToSheet(title, dt, m_Sheet, m_Book, 0);

                #region 保存Excel,清除进程

                //屏蔽覆盖和询问框
                m_Excel.DisplayAlerts = false;
                m_Excel.AlertBeforeOverwriting = false;

                m_Book.SaveAs(FilePath, Excel.XlFileFormat.xlWorkbookNormal, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                m_Book.Close(false, Missing.Value, Missing.Value);
                m_Excel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(m_Book);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(m_Excel);

                m_Book = null;
                m_Sheet = null;
                m_Excel = null;

                #endregion
            }
        }

        /// <summary>
        /// 将List中的数据写到Excel的指定Sheet中
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="m_Sheet"></param>
        private void ListToSheet(string title, System.Data.DataTable dt, Excel._Worksheet m_Sheet,
        Excel._Workbook m_Book, int startrow)
        {
            //以下是填写EXCEL中数据
            Excel.Range range = m_Sheet.get_Range(m_Sheet.Cells[1, 1], m_Sheet.Cells[1, dt.Columns.Count]);
            range.Font.Bold = true;   //加粗单元格内字符

            int rownum = dt.Rows.Count;//行数
            int columnnum = dt.Columns.Count;//列数

            //写入列标题
            for (int j = 0; j < columnnum; j++)
            {
                int bt_startrow = startrow + 1;

                //将字段名写入文档
                m_Sheet.Cells[bt_startrow, j + 1] = lstColumn[j];
            }
            //逐行写入数据 
            for (int i = 0; i < rownum; i++)
            {
                for (int j = 0; j < columnnum; j++)
                {

                    if (dt.Rows[i][j] == DBNull.Value)
                    {
                        m_Sheet.Cells[startrow + 2 + i, 1 + j] = string.Empty;

                    }
                    else
                    {
                        if (dt.Rows[i][j].ToString() == string.Empty)
                        {
                            m_Sheet.Cells[startrow + 2 + i, 1 + j] = dt.Rows[i][j].ToString();
                        }
                        else
                        {
                            if (Verification.IsDoubleNumer(dt.Rows[i][j].ToString()))
                            {
                                m_Sheet.Cells[startrow + 2 + i, 1 + j] = Convert.ToDouble(dt.Rows[i][j]);
                            }
                            else
                            {
                                m_Sheet.Cells[startrow + 2 + i, 1 + j] = dt.Rows[i][j].ToString();
                            }
                        }
                    }
                }
            }
            m_Sheet.Columns.AutoFit();
        }
    }
}
