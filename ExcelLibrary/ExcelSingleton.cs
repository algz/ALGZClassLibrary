using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelLibrary
{
    public class ExcelSingleton
    {
        //#region Variables
        //private Excel.Application application = null;
        //private Excel.Workbooks workBooks = null;
        //private Excel.Workbook excelWorkBook = null;
        //private Excel.Worksheet excelWorkSheet = null;
        //private Excel.Range excelRange = null;//Excel Range Object,多种用途  
        //private int excelActiveWorkSheetIndex;          //活动工作表索引 
        //private string excelOpenFileName = "";      //操作Excel的路径 
        //#endregion


        

        /// <summary> 
        /// Excel 数据存入 DataTable
        /// </summary> 
        public static bool ProcessExcelToDataTable(string filePath,Func<DataTable,string> fun,int startCol=1, int endCol=1, int startRow=1,int endRow=1)
        {
            Excel.Application application = null;
            //检查文件是否存在 
            if (!File.Exists(filePath))
            {
                throw new Exception(filePath + "该文件不存在！");
            }
            int iGeneration = 0;
            try
            {
                application = new Excel.ApplicationClass();
                Excel.Workbooks workBooks = application.Workbooks;
                Excel.Workbook workBook = ((Excel.Workbook)workBooks.Open(filePath, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value));
                Excel.Worksheet workSheet = (Excel.Worksheet)workBook.Worksheets[1];

                /**************************************/
                bool flag = false;
                if (endRow < 1)
                {
                    endRow = workSheet.Rows.Count;
                    flag = true;
                }
                System.Data.DataTable dt = new System.Data.DataTable();
                for (int columnID = startCol; columnID <= endCol; columnID++)
                {
                    dt.Columns.Add();
                }
                object[,] obj=(object[,])workSheet.Range["A1:H20"].Value2;
                for (int rowID = startRow; rowID <= endRow; rowID++)
                {
                    DataRow dr = dt.NewRow();

                    int f=0;
                    for (int columnID = startCol,i=0; columnID <= endCol; columnID++)
                    {
                        string val= ((Excel.Range)workSheet.Cells[rowID, columnID]).Text+"";
                        dr[i++] = val;
                        f = val != "" ? ++f : f;
                    }
                    //如果不指定结束行,则以最后一行无数据(即空字符串),则退出写循环.
                    if (flag&&f==0)
                    {
                        break; //退出行循环
                    }
                    dt.Rows.Add(dr);
                }
                //回调函数
                fun(dt);
                /***************************************/
                workBooks = null;
                workBook = null;
                workSheet = null;
                if (application != null)
                {
                    application.Workbooks.Close();
                    application.Quit();
                    iGeneration = System.GC.GetGeneration(application);
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(application);
                    ProcessLibrary.ProcessCommon.KillWindow((IntPtr)application.Hwnd);
                    //ProcessLibrary.ProcessCommon.CloseWindow((IntPtr)application.Hwnd);
                    application = null;
                }
                
                return true;
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
                
            }
            finally
            {
                if (application != null)
                {
                    //ProcessLibrary.ProcessCommon.CloseWindow((IntPtr)application.Hwnd);
                    //application.Hwnd;
                }
                GC.Collect(iGeneration);
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        /// <summary> 
        /// Excel 数据存入 DataTable
        /// </summary> 
        /// <param name="filePath">Excel文件地址</param>
        /// <param name="stCell">开始单元格编号,如:A1</param>
        /// <param name="edCell">结束单元格编号,如:C100</param>
        public static object[,] GetExcelRangeData(string filePath,  string stCell,string edCell)
        {
            //检查文件是否存在 
            if (!File.Exists(filePath))
            {
                throw new Exception(filePath + "文件不存在！");
            }
            Excel.Application application = null;
            Excel.Workbook workBook = null;
            try
            {
                application = new Excel.ApplicationClass();
                workBook = application.Workbooks.Open(filePath, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                if (workBook == null)
                {
                    return null;
                }
                Excel.Worksheet workSheet = (Excel.Worksheet)workBook.Worksheets[1];

                /**************************************/
                //从头读到尾
                //var maxN = workSheet.Range["A1"].End[Excel.XlDirection.xlDown].Row;
                return (object[,])workSheet.Range[stCell + ":" + edCell].Value2;
                /***************************************/
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);

            }
            finally
            {
                if (workBook != null)
                {
                    workBook.Close(false, Missing.Value, Missing.Value);
                    Marshal.ReleaseComObject(workBook);
                    application.Workbooks.Close();
                    application.Quit();
                    Marshal.ReleaseComObject(application);
                    //ProcessLibrary.ProcessCommon.CloseWindow((IntPtr)application.Hwnd);
                }
            }
        }

        /// <summary>
        /// DataTable 数据写入 Excel
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="dt"></param>
        /// <param name="startCol">写入的开始列号</param>
        /// <param name="endCol">写入的结束列号</param>
        /// <param name="startRow">写入的开始行号</param>
        /// <param name="endRow">写入的结束行号</param>
        /// <param name="contrastCol">匹配列号</param>
        /// <returns></returns>
        public static bool ProcessDataTableToExcel(string filePath, DataTable dt, int startCol = 1, int endCol = 1, int startRow = 1, int endRow = 1,int contrastCol=-1)
        {
            Excel.Application application = null;
            //检查文件是否存在 
            if (!File.Exists(filePath))
            {
                throw new Exception(filePath + "该文件不存在！");
            }
            int iGeneration = 0;
            try
            {
                application = new Excel.ApplicationClass();
                Excel.Workbooks workBooks = application.Workbooks;
                Excel.Workbook workBook = ((Excel.Workbook)workBooks.Open(filePath, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value));
                Excel.Worksheet workSheet = (Excel.Worksheet)workBook.Worksheets[1];

                /**************************************/
                for (int rowID = startRow,i=0; rowID <= endRow; rowID++,i++)
                {
                    for (int columnID = startCol,j=1; columnID <= endCol ; columnID++,j++)
                    {
                        if (contrastCol != -1)
                        {
                            string txt = ((Excel.Range)workSheet.Cells[rowID, contrastCol]).Text + "";
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (txt == dr[0] + "")
                                {
                                    ((Excel.Range)workSheet.Cells[rowID, columnID]).Value2 = dr[j];
                                    break;
                                }
                            }
                        }else
                        {
                            ((Excel.Range)workSheet.Cells[rowID, columnID]).Value2 = dt.Rows[i][j-1];
                        }
                        
                    }
                }
                /***************************************/
                //workSheet = null;
                workBook.Save(); //保存修改的表单.
                //workBook = null;
                //workBooks = null;
                if (application != null)
                {
                    //application.SaveWorkspace();
                    application.Workbooks.Close();
                    application.Quit();
                    //iGeneration = System.GC.GetGeneration(application);
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(application);

                    ProcessLibrary.ProcessCommon.KillWindow((IntPtr)application.Hwnd);
                    application = null;
                }

                return true;
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);

            }
            finally
            {
                if (application != null)
                {
                    //ProcessLibrary.ProcessCommon.CloseWindow((IntPtr)application.Hwnd);
                    //application.Hwnd;
                }
                GC.Collect(iGeneration);
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        ///// <summary> 
        ///// 当前活动工作表中有效列数(总列数) 
        ///// </summary> 
        ///// <param></param>  
        //public int getTotalColumnCount()
        //{
        //    int columnNumber = 0;
        //    try
        //    {
        //        while (true)
        //        {
        //            if (((Excel.Range)excelWorkSheet.Cells[1, columnNumber + 1]).Text.ToString().Trim() == "" &
        //                   ((Excel.Range)excelWorkSheet.Cells[1, columnNumber + 2]).Text.ToString().Trim() == "" &
        //                   ((Excel.Range)excelWorkSheet.Cells[1, columnNumber + 3]).Text.ToString().Trim() == "")
        //                break;
        //            columnNumber++;
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        int i = columnNumber;
        //        CloseExcelApplication();
        //        throw new Exception(ex.Message);
        //    }
        //    return columnNumber;
    }

    /// <summary> 
    /// 关闭Excel文件，释放对象；最后一定要调用此函数，否则会引起异常 
    /// </summary> 
    /// <param></param>  
    //public void CloseExcelApplication()
    //{
    //    int iGeneration = 0;
    //    try
    //    {
    //        excelWorkBooks = null;
    //        excelWorkBook = null;
    //        excelWorkSheet = null;
    //        excelRange = null;
    //        if (excelApplication != null)
    //        {
    //            excelApplication.Workbooks.Close();
    //            excelApplication.Quit();
    //            iGeneration = System.GC.GetGeneration(excelApplication);
    //            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApplication);
    //            excelApplication = null;
    //        }
    //    }
    //    catch (System.Exception ex)
    //    {
    //        throw new Exception(ex.Message);
    //    }
    //    finally
    //    {

    //        GC.Collect(iGeneration);
    //        GC.WaitForPendingFinalizers();
    //        GC.Collect();
    //        GC.WaitForPendingFinalizers();
    //    }
    //}
//}
}
