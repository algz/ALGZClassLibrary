using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace ZaeroModelSystem
{
    public class ExcelLib
    {
        #region Variables
        private Excel.Application excelApplication = null;
        private Excel.Workbooks excelWorkBooks = null;
        private Excel.Workbook excelWorkBook = null;
        private Excel.Worksheet excelWorkSheet = null;
        private Excel.Range excelRange = null;//Excel Range Object,多种用途  
        private int excelActiveWorkSheetIndex;          //活动工作表索引 
        private string excelOpenFileName = "";      //操作Excel的路径 
        #endregion

        #region Properties
        public int ActiveSheetIndex
        {
            get
            {
                return excelActiveWorkSheetIndex;
            }
            set
            {
                excelActiveWorkSheetIndex = value;
            }
        }
        public string OpenFileName
        {
            get
            {
                return excelOpenFileName;
            }
            set
            {
                excelOpenFileName = value;
            }
        }
        #endregion

        // 
        //-------------------------------------------------------------------------------------------------------- 
        /// <summary> 
        /// 构造函数;
        /// </summary> 
        public ExcelLib()
        {
            excelApplication = null;//Excel Application Object 
            excelWorkBooks = null;//Workbooks 
            excelWorkBook = null;//Excel Workbook Object 
            excelWorkSheet = null;//Excel Worksheet Object 
            ActiveSheetIndex = 1;           //默认值活动工作簿为第一个；设置活动工作簿请参阅SetActiveWorkSheet()    
        }
        /// <summary> 
        /// 以excelOpenFileName为模板新建Excel文件 
        /// </summary> 
        public bool OpenExcelFile()
        {
            if (excelApplication != null) CloseExcelApplication();

            //检查文件是否存在 
            if (excelOpenFileName == "")
            {
                throw new Exception("请选择文件！");
            }
            if (!File.Exists(excelOpenFileName))
            {
                throw new Exception(excelOpenFileName + "该文件不存在！");
            }
            try
            {
                excelApplication = new Excel.ApplicationClass();
                excelWorkBooks = excelApplication.Workbooks;
                excelWorkBook = ((Excel.Workbook)excelWorkBooks.Open(excelOpenFileName, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value));
                excelWorkSheet = (Excel.Worksheet)excelWorkBook.Worksheets[excelActiveWorkSheetIndex];
                excelApplication.Visible = false;
                //MessageBox.Show(((Excel.Worksheet)excelWorkBook.Worksheets[excelActiveWorkSheetIndex]).Name);
                return true;
            }
            catch (Exception e)
            {
                CloseExcelApplication();
                //MessageBox.Show(e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information); 
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 取得所有工作表名
        /// </summary>
        /// <returns></returns>
        public string[] getWorkSheetsName()
        {
            try
            {
                string[] strSheetsName = null;
                int iSheetsNum = excelWorkBook.Worksheets.Count;
                strSheetsName = new string[iSheetsNum];
                for (int i = 1; i <= iSheetsNum; i++)
                {
                    strSheetsName[i - 1] = ((Excel.Worksheet)excelWorkBook.Worksheets[i]).Name;
                }
                return strSheetsName;
            }
            catch (System.Exception ex)
            {
                CloseExcelApplication();
                throw new Exception(ex.Message);
            }

        }

        /// <summary> 
        /// 读取一个Cell的值 
        /// </summary> 
        /// <param name="CellRowID">要读取的Cell的行索引</param> 
        /// <param name="CellColumnID">要读取的Cell的列索引</param> 
        /// <returns>Cell的值</returns> 
        public string getOneCellValue(int CellRowID, int CellColumnID)
        {
            if (CellRowID <= 0)
            {
                throw new Exception("行索引超出范围！");
            }
            string sValue = "";
            try
            {
                sValue = ((Excel.Range)excelWorkSheet.Cells[CellRowID, CellColumnID]).Text.ToString();
            }
            catch (Exception e)
            {
                CloseExcelApplication();
                throw new Exception(e.Message);
            }
            return (sValue);
        }
        /// <summary> 
        /// 读取一个连续区域的Cell的值(矩形区域，包含一行或一列,或多行，多列)，返回一个一维字符串数组。 
        /// </summary> 
        /// <param name="StartCell">StartCell是要写入区域的左上角单元格</param> 
        /// <param name="EndCell">EndCell是要写入区域的右下角单元格</param> 
        /// <returns>值的集合</returns> 
        public string[] getCellsValue(string StartCell, string EndCell)
        {
            string[] sValue = null;
            try
            {
                excelRange = (Excel.Range)excelWorkSheet.get_Range(StartCell, EndCell);
                sValue = new string[excelRange.Count];
                int rowStartIndex = ((Excel.Range)excelWorkSheet.get_Range(StartCell, StartCell)).Row;      //起始行号 
                int columnStartIndex = ((Excel.Range)excelWorkSheet.get_Range(StartCell, StartCell)).Column;    //起始列号 
                int rowNum = excelRange.Rows.Count;                 //行数目 
                int columnNum = excelRange.Columns.Count;               //列数目 
                int index = 0;
                for (int i = rowStartIndex; i < rowStartIndex + rowNum; i++)
                {
                    for (int j = columnStartIndex; j < columnNum + columnStartIndex; j++)
                    {
                        //读到空值null和读到空串""分别处理 
                        sValue[index] = ((Excel.Range)excelWorkSheet.Cells[i, j]).Text.ToString();
                        index++;
                    }
                }
            }
            catch (Exception e)
            {
                CloseExcelApplication();
                throw new Exception(e.Message);
            }

            return (sValue);
        }

        public string[] getCellsValue2(string StartCell, string EndCell, int columnNum)
        {
            try
            {
                string[] sValue = null;
                excelRange = (Excel.Range)excelWorkSheet.get_Range(StartCell, EndCell);
                int rowStartIndex = ((Excel.Range)excelWorkSheet.get_Range(StartCell, StartCell)).Row;      //起始行号 
                int columnStartIndex = ((Excel.Range)excelWorkSheet.get_Range(StartCell, StartCell)).Column;    //起始列号 
                //int columnNum = excelRange.Columns.Count;               //列数目 
                sValue = new string[columnNum];

                for (int j = columnStartIndex; j < columnNum + columnStartIndex; j++)
                {
                    //读到空值null和读到空串""分别处理 
                    sValue[j - 1] = ((Excel.Range)excelWorkSheet.Cells[rowStartIndex, j]).Text.ToString();
                }

                return (sValue);
            }
            catch (System.Exception ex)
            {
                CloseExcelApplication();
                throw new Exception(ex.Message);
            }

        }
        /// <summary> 
        /// 读取所有单元格的数据(矩形区域)，返回一个datatable.假设所有单元格靠工作表左上区域。 
        /// </summary> 
        public System.Data.DataTable getAllCellsValue()
        {
            try
            {
                int columnCount = getTotalColumnCount();
                int rowCount = getTotalRowCount(columnCount);
                System.Data.DataTable dt = new System.Data.DataTable();
                //设置datatable列的名称 
                for (int columnID = 1; columnID <= columnCount; columnID++)
                {
                    dt.Columns.Add(((Excel.Range)excelWorkSheet.Cells[1, columnID]).Text.ToString());
                }

                if (rowCount == 1)
                {
                    DataRow dr = dt.NewRow();
                    for (int columnID = 1; columnID <= columnCount; columnID++)
                    {
                        dr[columnID - 1] = ((Excel.Range)excelWorkSheet.Cells[2, columnID]).Text.ToString();
                        //读到空值null和读到空串""分别处理 
                    }
                    dt.Rows.Add(dr);
                }
                for (int rowID = 2; rowID <= rowCount; rowID++)
                {
                    DataRow dr = dt.NewRow();
                    for (int columnID = 1; columnID <= columnCount; columnID++)
                    {
                        dr[columnID - 1] = ((Excel.Range)excelWorkSheet.Cells[rowID, columnID]).Text.ToString();
                        //读到空值null和读到空串""分别处理 
                    }
                    dt.Rows.Add(dr);
                }
                return (dt);
            }
            catch (System.Exception ex)
            {
                CloseExcelApplication();
                throw new Exception(ex.Message);
            }

        }

        /// <summary> 
        /// 读取所有单元格的数据(矩形区域)，返回一个datatable.假设所有单元格靠工作表左上区域。 
        /// </summary> 
        public System.Data.DataTable getAllCellsValue(string strType)
        {
            try
            {
                int columnCount = getTotalColumnCount();
                int rowCount = getTotalRowCount(columnCount);
                System.Data.DataTable dt = new System.Data.DataTable();
                //设置datatable列的名称 
                for (int columnID = 1; columnID <= columnCount; columnID++)
                {
                    //列标题重复
                    if (strType == "recover")
                    {
                        DataColumn column = new DataColumn();
                        column.ColumnName = "column" + columnID.ToString();
                        column.Caption = ((Excel.Range)excelWorkSheet.Cells[1, columnID]).Text.ToString();
                        dt.Columns.Add(column);
                    }
                    else
                    {
                        dt.Columns.Add(((Excel.Range)excelWorkSheet.Cells[1, columnID]).Text.ToString());
                    }
                }

                for (int rowID = 2; rowID <= rowCount; rowID++)
                {
                    DataRow dr = dt.NewRow();
                    for (int columnID = 1; columnID <= columnCount; columnID++)
                    {
                        dr[columnID - 1] = ((Excel.Range)excelWorkSheet.Cells[rowID, columnID]).Text.ToString();
                        //读到空值null和读到空串""分别处理 
                    }
                    dt.Rows.Add(dr);
                }
                return (dt);
            }
            catch (System.Exception ex)
            {
                CloseExcelApplication();
                throw new Exception(ex.Message);
            }

        }

        public int getTotalRowCount(int count)
        {//当前活动工作表中有效行数(总行数) 

            int rowsNumber = 0;
            try
            {
                while (true)
                {
                    if (((Excel.Range)excelWorkSheet.Cells[rowsNumber + 1, count]).Text.ToString().Trim() == "" &
                           ((Excel.Range)excelWorkSheet.Cells[rowsNumber + 2, count]).Text.ToString().Trim() == "" &
                           ((Excel.Range)excelWorkSheet.Cells[rowsNumber + 3, count]).Text.ToString().Trim() == "")
                        break;

                    rowsNumber++;

                }
            }
            catch (System.Exception ex)
            {
                CloseExcelApplication();
                throw new Exception(ex.Message);
            }
            return rowsNumber;
        }
        /// <summary> 
        /// 当前活动工作表中有效列数(总列数) 
        /// </summary> 
        /// <param></param>  
        public int getTotalColumnCount()
        {
            int columnNumber = 0;
            try
            {
                while (true)
                {
                    if (((Excel.Range)excelWorkSheet.Cells[1, columnNumber + 1]).Text.ToString().Trim() == "" &
                           ((Excel.Range)excelWorkSheet.Cells[1, columnNumber + 2]).Text.ToString().Trim() == "" &
                           ((Excel.Range)excelWorkSheet.Cells[1, columnNumber + 3]).Text.ToString().Trim() == "")
                        break;
                    columnNumber++;
                }
            }
            catch (System.Exception ex)
            {
                int i = columnNumber;
                CloseExcelApplication();
                throw new Exception(ex.Message);
            }
            return columnNumber;
        }

        /// <summary> 
        /// 设置活动工作表 
        /// </summary> 
        /// <param name="SheetIndex">要设置为活动工作表的索引值</param> 
        public void SetActiveWorkSheet(int SheetIndex)
        {
            if (SheetIndex <= 0)
            {
                throw new Exception("索引超出范围！");
            }
            try
            {
                ActiveSheetIndex = SheetIndex;
                excelWorkSheet = (Excel.Worksheet)excelWorkBook.Worksheets[ActiveSheetIndex];
            }
            catch (Exception e)
            {
                CloseExcelApplication();
                throw new Exception(e.Message);
            }
        }
        /// <summary> 
        /// 向连续区域一次性写入数据；只有在区域连续和写入的值相同的情况下可以使用方法 
        /// </summary> 
        /// <param name="StartCell">StartCell是要写入区域的左上角单元格</param> 
        /// <param name="EndCell">EndCell是要写入区域的右下角单元格</param> 
        /// <param name="Value">要写入指定区域所有单元格的数据值</param> 
        public void setCellsValue(string StartCell, string EndCell, string Value)
        {
            try
            {
                excelRange = excelWorkSheet.get_Range(StartCell, EndCell);
                excelRange.Value2 = Value;
                excelRange = null;
            }
            catch (Exception e)
            {
                CloseExcelApplication();
                throw new Exception(e.Message);
            }
        }

        /// <summary> 
        /// 向一个Cell写入数据 
        /// </summary> 
        /// <param name="CellRowID">CellRowID是cell的行索引</param> 
        /// <param name="CellColumnID">CellColumnID是cell的列索引</param> 
        ///<param name="Value">要写入该单元格的数据值</param> 
        public void setOneCellValue(int CellRowID, int CellColumnID, string Value)
        {
            try
            {
                excelRange = (Excel.Range)excelWorkSheet.Cells[CellRowID, CellColumnID];
                excelRange.Value2 = Value;
                //Gets or sets the value of the NamedRange control.  
                //The only difference between this property and the Value property is that Value2 is not a parameterized property.  
                excelRange = null;
            }
            catch (Exception e)
            {
                CloseExcelApplication();
                throw new Exception(e.Message);
            }
        }

        /// <summary> 
        /// 给一行写数据 
        /// </summary> 
        public void setOneLineValues(int LineID, int StartCellColumnID, int EndCellColumnID, string[] Values)////已经测试 
        {
            for (int i = StartCellColumnID; i <= EndCellColumnID; i++)
            {
                setOneCellValue(LineID, i, Values[i - 1]);
            }
        }

        /// <summary> 
        /// 插入一行 
        /// </summary> 
        /// <param name="CellRowID">要插入所在行的索引位置，插入后其原有行下移</param>  
        /// <param name="RowNum">要插入行的个数</param>  
        public void InsertRow(int CellRowID, int RowNum)//插入空行 
        {
            if (CellRowID <= 0)
            {
                throw new Exception("行索引超出范围！");
            }
            if (RowNum <= 0)
            {
                throw new Exception("插入行数无效！");
            }
            try
            {
                excelRange = (Excel.Range)excelWorkSheet.Rows[CellRowID, Missing.Value];
                for (int i = 0; i < RowNum; i++)
                {
                    excelRange.Insert(Excel.XlDirection.xlDown, Missing.Value);
                }
                excelRange = null;
            }
            catch (Exception e)
            {
                CloseExcelApplication();
                throw new Exception(e.Message);
            }
        }

        /// <summary> 
        /// 删除一行 
        /// </summary> 
        /// <param name="CellRowID">第一个要删除行的索引位置，删除后其原有行上移</param>  
        /// <param name="RowNum">要删除行的个数</param>  
        public void DeleteRow(int CellRowID, int RowNum)
        {
            if (CellRowID <= 0)
            {
                throw new Exception("行索引超出范围！");
            }
            if (RowNum <= 0)
            {
                throw new Exception("插入行数无效！");
            }
            try
            {
                excelRange = (Excel.Range)excelWorkSheet.Rows[CellRowID, Missing.Value];
                for (int i = 0; i < RowNum; i++)
                {
                    excelRange.Delete(Excel.XlDirection.xlUp);
                }
                excelRange = null;
            }
            catch (Exception e)
            {
                CloseExcelApplication();
                throw new Exception(e.Message);
            }
        }

        /// <summary> 
        /// 判断单元格是否有数据 
        /// </summary> 
        public bool CellValueIsNull(int CellLineID, int CellColumnID)////已经测试 
        {
            try
            {//判断单元格是否有数据 
                if ((((Excel.Range)excelWorkSheet.Cells[CellLineID, CellColumnID]).Text.ToString().Trim() != ""))
                    return false;
                return true;
            }
            catch (System.Exception ex)
            {
                CloseExcelApplication();
                throw new Exception(ex.Message);
            }

        }

        //将column index转化为字母
        public string GetColumnNameByColumnIndex(int columnIndex)
        {
            try
            {
                string[] alphabet = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

                string result = "";
                if (columnIndex > 676)
                {
                    MessageBox.Show("列数不能大于676！");
                    return "";
                }

                int temp = (columnIndex - 1) / 26;

                int temp2 = (columnIndex - 1) % 26;

                if (temp > 0)
                {
                    result = alphabet[temp];
                }

                result += alphabet[temp2];

                return result;

            }
            catch (System.Exception ex)
            {
                CloseExcelApplication();
                throw new Exception(ex.Message);
            }

        }

        public void SaveExcel2()
        {
            try
            {
                excelApplication.DisplayAlerts = false;
                excelApplication.AlertBeforeOverwriting = false;
                excelApplication.ActiveWorkbook.Save();
            }
            catch (System.Exception ex)
            {
                CloseExcelApplication();
                throw new Exception(ex.Message);
            }

        }

        /// <summary> 
        /// 关闭Excel文件，释放对象；最后一定要调用此函数，否则会引起异常 
        /// </summary> 
        /// <param></param>  
        public void CloseExcelApplication()
        {
            int iGeneration = 0;
            try
            {
                excelWorkBooks = null;
                excelWorkBook = null;
                excelWorkSheet = null;
                excelRange = null;
                if (excelApplication != null)
                {
                    excelApplication.Workbooks.Close();
                    excelApplication.Quit();
                    iGeneration = System.GC.GetGeneration(excelApplication);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApplication);
                    excelApplication = null;
                }
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {

                GC.Collect(iGeneration);
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


    }
}
