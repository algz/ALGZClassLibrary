using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace NPOILibrary
{
    public class ExcelClass
    {
        //private static XSSFWorkbook hssfworkbook;


        /// <summary>
        /// 在Excel中,把第colStart列到colEnd列,从rowStart行到rowEnd行的数据全部存储到DataTable中.
        /// DataTable数据存储方式:0列到colEnd列,0行到rowStart行,其中第colStart列到colEnd列,rowStart行到rowEnd行存储数据.
        /// 转换Excel=>DataTable.
        /// NPO对Excel列,从0开始计数.如A1=第0行第0列.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="action">返回的DataTable,从0到结束列;0到结束行</param>
        /// <param name="colStart">开始列.从指定的列(按Excel列算起)开始读取.(NPOI默认0开始)</param>
        /// <param name="colEnd">结束列.从指定的列(按Excel行算起)结束读取.(NPOI默认0开始)</param>
        /// <param name="rowStart">开始行.从指定的行(按Excel行算起)开始读取.(NPOI默认0开始)</param>
        /// <param name="rowEnd">结束行.从指定的行(按Excel行算起)结束读取.(NPOI默认0开始)</param>
        /// <returns></returns>
        public static string ConvertToDataTable(string path, Action<DataTable, string> action, string sheetName,int colStart = 0, int colEnd = 1, int rowStart = 0, int rowEnd = -1)
        {

            try
            {
                DataTable dt = new DataTable();
                //打开Excel
                IWorkbook workbook = OpenExcel(path);
                ISheet sheet = workbook.GetSheetAt(0);
                //string sheetName = sheet.SheetName;
                for (int i=0;i< workbook.NumberOfSheets;i++)
                {
                    string name=workbook.GetSheetName(i);
                    if (sheetName!=null&&name.Contains(sheetName))
                    {
                        sheet = workbook.GetSheetAt(i);
                        break;
                    }
                }
                
                System.Collections.IEnumerator rows = sheet.GetRowEnumerator();


                //创建列定义
                for (int j = 0; j < colEnd; j++)
                {
                    dt.Columns.Add(Convert.ToChar(((int)'A') + j).ToString());
                }

                //添加行数据
                for (int rowIndex = rowStart; rowIndex < rowEnd; rowIndex++)
                {

                    IRow row = (XSSFRow)sheet.GetRow(rowIndex);
                    if (row == null)
                    {
                        break;
                    }
                    DataRow dr = dt.NewRow();

                    for (int colIndex = colStart; colIndex < (colEnd == -1 ? row.LastCellNum : colEnd); colIndex++)
                    {
                        ICell cell = row.GetCell(colIndex);

                        if (cell == null)
                        {

                            dr[colIndex] = null;
                        }
                        else
                        {
                            cell.SetCellType(CellType.String);
                            dr[colIndex] = cell.ToString();
                        }
                    }
                    dt.Rows.Add(dr);
                }

                //while (rows.MoveNext())
                //{

                //    IRow row = (HSSFRow)rows.Current;
                //    DataRow dr = dt.NewRow();

                //    for (int i = 0; i < row.LastCellNum; i++)
                //    {
                //        ICell cell = row.GetCell(i);


                //        if (cell == null)
                //        {
                //            dr[i] = null;
                //        }
                //        else
                //        {
                //            dr[i] = cell.ToString();
                //        }
                //    }
                //    dt.Rows.Add(dr);
                //}
                workbook.Close();
                action(dt, sheetName);
                return "";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public static string ConvertToExcel(string path, DataTable dt, int startCol = 1, int endCol = 1, int startRow = 1, int endRow = 1, int contrastCol = -1)
        {
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    //打开Excel
                    IWorkbook workbook = OpenExcel(path); ;

                    ISheet sheet = workbook.GetSheetAt(workbook.ActiveSheetIndex);//.a.CreateSheet("Sheet0");//创建一个名称为Sheet0的表

                    int rowCount = dt.Rows.Count;//行数  
                    int columnCount = dt.Columns.Count;//列数  


                    for (int rowID = startRow, i = 0; rowID <= endRow; rowID++, i++)
                    {
                        for (int columnID = startCol, j = 1; columnID <= endCol; columnID++, j++)
                        {
                            if (contrastCol != -1)
                            {
                                //string txt = ((Excel.Range)workSheet.Cells[rowID, contrastCol]).Text + "";
                                string txt = sheet.GetRow(rowID).Cells[contrastCol].StringCellValue;
                                foreach (DataRow dr in dt.Rows)
                                {
                                    if (txt == dr[0] + "")
                                    {
                                        sheet.GetRow(rowID).Cells[contrastCol].SetCellValue(dr[j] + "");
                                        //((Excel.Range)workSheet.Cells[rowID, columnID]).Value2 = dr[j];
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                sheet.GetRow(rowID).Cells[contrastCol].SetCellValue(dt.Rows[i][j - 1] + "");
                                //((Excel.Range)workSheet.Cells[rowID, columnID]).Value2 = dt.Rows[i][j - 1];
                            }

                        }

                    }
                    //保存Excel
                    SaveAsExel(workbook, path);
                    ////sheet = workbook.a.CreateSheet("Sheet0");//创建一个名称为Sheet0的表  
                    //int rowCount = dt.Rows.Count;//行数  
                    //int columnCount = dt.Columns.Count;//列数  

                    ////设置列头  
                    //row = sheet.CreateRow(0);//excel第一行设为列头  
                    //for (int c = 0; c < columnCount; c++)
                    //{
                    //    cell = row.CreateCell(c);
                    //    cell.SetCellValue(dt.Columns[c].ColumnName);
                    //}

                    ////设置每行每列的单元格,  
                    //for (int i = 0; i < rowCount; i++)
                    //{
                    //    row = sheet.CreateRow(i + 1);
                    //    for (int j = 0; j < columnCount; j++)
                    //    {
                    //        cell = row.CreateCell(j);//excel第二行开始写入数据  
                    //        cell.SetCellValue(dt.Rows[i][j].ToString());
                    //    }
                    //}
                    //using (fs = File.OpenWrite(@"D:/myxls.xls"))
                    //{
                    //    workbook.Write(fs);//向打开的这个xls文件中写入数据  
                    //    result = true;
                    //}
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        ///// <summary>
        ///// DataTable 保存为 Excel文件.
        ///// Excel和DataTable 列和行都从0开始计数.
        ///// </summary>
        ///// <param name="path"></param>
        ///// <param name="contrastDt">输入DataTable</param>
        ///// <param name="startCol">要修改的开始列号(从0开始).Excel列开始行.(NPOI默认0开始)</param>
        ///// <param name="endCol">要修改的结束列号(从0开始).Excel列结束行.(NPOI默认0开始)</param>
        ///// <param name="startRow">要修改的开始行号(从0开始).Excel行开始.(NPOI默认0开始)</param>
        ///// <param name="endRow">要修改的结束行号(从0开始).Excel行结束行.(NPOI默认0开始)</param>
        ///// <param name="contrastExcelCol">需要对比的Excel列.(NPOI默认从0开始)</param>
        ///// <param name="contrastColDt">DataTable中需要与Excel的contrastExcelCol列比较的列号</param>
        ///// <param name="valColDt">DataTable中valColDt行号的数据赋值给需要与Excel的contrastExcelCol列比较的列号</param>
        ///// <returns></returns>
        //public static string DataTableToExcel(string path, DataTable contrastDt, int startCol = 0, int endCol = 0, int startRow = 0, int endRow = 0, int contrastExcelCol = -1, int contrastColDt = 0, int valColDt = 0)
        //{
        //    try
        //    {
        //        //打开Excel
        //        IWorkbook workbook = OpenExcel(path);

        //        ISheet sheet = workbook.GetSheetAt(0);//.a.CreateSheet("Sheet0");//创建一个名称为Sheet0的表
        //        for (int rowID = startRow; rowID <= endRow; rowID++)
        //        {

        //            //指定对比列赋值.
        //            if (contrastExcelCol != -1)
        //            {
        //                bool flag = false;
        //                //获得Excel中的对比列的值
        //                string txt = sheet.GetRow(rowID).Cells[contrastExcelCol].ToString();//.SetCellValue(dr[j] + "");
        //                foreach (DataRow dr in contrastDt.Rows)
        //                {
        //                    //查找DataTable中的与Exce的列相匹配的数据.
        //                    if (txt == dr[contrastColDt] + "")
        //                    {
        //                        flag = true;
        //                        break;
        //                    }
        //                }
        //                if (!flag)
        //                {
        //                    continue;
        //                }
        //            }

        //            for (int columnID = startCol; columnID <= endCol && columnID < sheet.GetRow(rowID).Cells.Count; columnID++)
        //            {
        //                if (!(contrastDt.Rows[rowID][columnID] is DBNull))
        //                {
        //                    sheet.GetRow(rowID).Cells[columnID].SetCellValue(contrastDt.Rows[rowID][columnID] + "");
        //                }
        //            }
        //        }
        //        //保存Excel
        //        SaveAsExel(workbook, path);
        //        return "";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}

        public static string ModifyExcelTemplate(string templateFilePath, DataTable contrastDt, int startCol = 0, int endCol = 0, int startRow = 0, int endRow = 0, int contrastExcelCol = -1, int contrastColDt = 0, int valColDt = 0)
        {
            try
            {
                //打开Excel
                IWorkbook workbook = OpenExcel(templateFilePath);

                ISheet sheet = workbook.GetSheetAt(0);//.a.CreateSheet("Sheet0");//创建一个名称为Sheet0的表
                for (int rowID = startRow; rowID <= endRow; rowID++)
                {
                    DataRow cur = contrastDt.Rows[rowID];
                    //指定对比列赋值.
                    if (contrastExcelCol != -1)
                    {
                        cur = null;
                        //获得Excel中的对比列的值
                        string txt = sheet.GetRow(rowID).Cells[contrastExcelCol].ToString();//.SetCellValue(dr[j] + "");
                        foreach (DataRow dr in contrastDt.Rows)
                        {
                            //查找DataTable中的与Exce的列相匹配的数据.
                            if (txt == dr[contrastColDt] + "")
                            {
                                cur = dr;
                                break;
                            }
                        }
                        if (cur == null)
                        {
                            continue;
                        }
                    }

                    for (int columnID = startCol; columnID <= endCol && columnID < sheet.GetRow(rowID).Cells.Count; columnID++)
                    {
                        if (!(contrastDt.Rows[rowID][columnID] is DBNull))
                        {
                            sheet.GetRow(rowID).Cells[columnID].SetCellValue(cur[columnID] + "");
                        }
                    }
                }
                //保存Excel
                SaveAsExel(workbook, templateFilePath);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// DataTable 保存为 Excel文件.
        /// Excel和DataTable 列和行都从0开始计数.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="contrastDt">输入DataTable</param>
        /// <param name="startCol">要修改的开始列号(从0开始).Excel列开始行.(NPOI默认0开始)</param>
        /// <param name="endCol">要修改的结束列号(从0开始).Excel列结束行.(NPOI默认0开始)</param>
        /// <param name="startRow">要修改的开始行号(从0开始).Excel行开始.(NPOI默认0开始)</param>
        /// <param name="endRow">要修改的结束行号(从0开始).Excel行结束行.(NPOI默认0开始)</param>
        /// <param name="contrastExcelCol">需要对比的Excel列.(NPOI默认从0开始)</param>
        /// <param name="contrastColDt">DataTable中需要与Excel的contrastExcelCol列比较的列号</param>
        /// <param name="valColDt">DataTable中valColDt行号的数据赋值给需要与Excel的contrastExcelCol列比较的列号</param>
        /// <returns></returns>
        public static string ModifyExcelForDataTable(string path, DataTable contrastDt,string sheetName, int startCol = 0, int endCol = 0, int startRow = 0, int endRow = 0, int contrastExcelCol = -1, int contrastColDt = 0, int valColDt = 0)
        {
            try
            {
                //打开Excel
                IWorkbook workbook = OpenExcel(path);

                ISheet sheet = workbook.GetSheetAt(0);//.a.CreateSheet("Sheet0");//创建一个名称为Sheet0的表
                for (int i = 0; i < workbook.NumberOfSheets&&sheetName!=null; i++)
                {
                    string name = workbook.GetSheetName(i);
                    if (name.Contains(sheetName))
                    {
                        sheet = workbook.GetSheetAt(i);
                        break;
                    }
                }

                for (int rowID = startRow; rowID <= endRow; rowID++)
                {
                    int i=sheet.LastRowNum;
                    DataRow cur = contrastDt.Rows[rowID];
                    //指定对比列赋值.
                    if (contrastExcelCol != -1)
                    {
                        cur = null;
                        //获得Excel中的对比列的值
                        string txt = sheet.GetRow(rowID).Cells[contrastExcelCol].ToString();//.SetCellValue(dr[j] + "");
                        foreach (DataRow dr in contrastDt.Rows)
                        {
                            //查找DataTable中的与Exce的列相匹配的数据.
                            if (txt == dr[contrastColDt] + "")
                            {
                                cur = dr;
                                break;
                            }
                        }
                        if (cur == null)
                        {
                            continue;
                        }
                    }

                    for (int columnID = startCol; columnID <= endCol && columnID < sheet.GetRow(rowID).Cells.Count; columnID++)
                    {
                        if (!(contrastDt.Rows[rowID][columnID] is DBNull))
                        {
                            sheet.GetRow(rowID).Cells[columnID].SetCellValue(cur[columnID] + "");
                        }
                    }
                }
                //保存Excel
                SaveAsExel(workbook, path);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        
        /// <summary>
        /// DataTable 数据添加到Excel模板中.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dt">输入DataTable</param>
        /// <param name="startRow">NPOI开始的行号,从0开始.(对应的Excel列从0开始行).</param>
        /// <param name="rowHeight">行高.(Excel中设置的值,方法中已进行x20计算)</param>
        /// <param name="styleCell_row">样式单元的行号,默认0行.</param>
        /// <param name="styleCell_col">样式单元的列号,默认0列.</param>
        /// <returns></returns>
        public static string CreateExcelForDataTable(string path, DataTable dt, int startRow = 0, int rowHeight=0,int styleCell_row=0, int styleCell_col=0,Dictionary<string,string> replaceStr=null)
        {
            try
            {
                //将文件读到内存，在内存中操作excel
                //打开Excel
                IWorkbook workbook = OpenExcel(path);

                ISheet sheet = workbook.GetSheetAt(0);//.a.CreateSheet("Sheet0");//创建一个名称为Sheet0的表
                #region 替换表单中指定字符串
                if (replaceStr != null)
                {
                    for(int i=sheet.FirstRowNum;i<=sheet.LastRowNum;i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row != null)
                        {
                            for (int j = row.FirstCellNum; j < sheet.GetRow(i).LastCellNum; j++)
                            {
                                string str = row.GetCell(j).StringCellValue;
                                foreach (string k in replaceStr.Keys)
                                {
                                    if (str.Contains(k))
                                    {
                                        row.GetCell(j).SetCellValue(str.Replace(k, replaceStr[k]));
                                        replaceStr.Remove(k);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                //插入行.第"startRow"到第"sheet.LastRowNum"行移动"dt.Rows.Count"行,bool copyRowHeight, bool resetOriginalRowHeight
                sheet.ShiftRows(startRow, sheet.LastRowNum, dt.Rows.Count, true, true);
                foreach (DataRow dr in  dt.Rows)
                {
                    IRow excelRow = sheet.CreateRow(startRow);
                    //设置行高 ,excel行高度每个像素点是1/20
                    if (rowHeight > 0)
                    {
                        excelRow.Height = (short)(rowHeight * 20);
                    }
                    //int curStartCol = startCol;
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        var excelCell = excelRow.CreateCell(j, CellType.String);
                        excelCell.CellStyle = sheet.GetRow(styleCell_row).Cells[styleCell_col].CellStyle;
                        var value = dr[j].ToString();
                        if (File.Exists(value))
                        {
                            byte[] bytes = File.ReadAllBytes(value);
                            int pictureIdx = workbook.AddPicture(bytes, PictureType.JPEG);
                            IDrawing patriarch= sheet.CreateDrawingPatriarch();
                            // 插图片的位置  HSSFClientAnchor（dx1,dy1,dx2,dy2,col1,row1,col2,row2)
                            //函数原型：XSSFClientAnchor(int dx1, int dy1, int dx2, int dy2, int col1, int row1, int col2, int row2)；
                            //坐标(col1,row1)表示图片左上角所在单元格的位置，均从0开始，比如(5,2)表示(第五列，第三行),即F3；注意：图片左上角坐标与(col1,row1)单元格左上角坐标重合
                            //坐标(col2,row2)表示图片右下角所在单元格的位置，均从0开始，比如(10,3)表示(第十一列，第四行),即K4；注意：图片右下角坐标与(col2,row2)单元格左上角坐标重合
                            //坐标(dx1,dy1)表示图片左上角在单元格(col1,row1)基础上的偏移量(往右下方偏移)；(dx1，dy1)的最大值为(1023, 255),为一个单元格的大小
                            //坐标(dx2,dy2)表示图片右下角在单元格(col2,row2)基础上的偏移量(往右下方偏移)；(dx2,dy2)的最大值为(1023, 255),为一个单元格的大小


                            //参数的解析: HSSFClientAnchor（int dx1,int dy1,int dx2,int dy2,int col1,int row1,int col2,int row2)
                            //dx1: 图片左边相对excel格的位置(x偏移) 范围值为: 0~1023; 即输100 偏移的位置大概是相对于整个单元格的宽度的100除以1023大概是10分之一
                            //dy1: 图片上方相对excel格的位置(y偏移) 范围值为: 0~256 原理同上。
                            //dx2: 图片右边相对excel格的位置(x偏移) 范围值为: 0~1023; 原理同上。
                            //dy2: 图片下方相对excel格的位置(y偏移) 范围值为: 0~256 原理同上。
                            //col1和row1: 图片左上角的位置，以excel单元格为参考,比喻这两个值为(1, 1)，那么图片左上角的位置就是excel表(1, 1)单元格的右下角的点(A, 1)右下角的点。
                            //col2和row2: 图片右下角的位置，以excel单元格为参考,比喻这两个值为(2, 2)，那么图片右下角的位置就是excel表(2, 2)单元格的右下角的点(B, 2)右下角的点。
                            IClientAnchor anchor = null;
                            if (path.IndexOf(".xlsx") > 0)
                            {
                                // 2007版本 
                                anchor = new XSSFClientAnchor(0, 0, 0, 0, j, startRow, j+1, startRow + 1);
                            }
                            else if (path.IndexOf(".xls") > 0)
                            {
                                // 2003版本 
                                anchor = new HSSFClientAnchor(0, 0, 0, 0, j, startRow, j+1, startRow + 1);
                            }
                            //把图片插到相应的位置
                            IPicture pict = patriarch.CreatePicture(anchor, pictureIdx);
                        }
                        else
                        {
                            excelCell.SetCellValue(value);
                        }
                    }
                    startRow++;
                }
                sheet.ForceFormulaRecalculation = true;

                //保存Excel
                SaveAsExel(workbook, path);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        /// <summary>
        /// DataTable 数据添加到Excel模板中.
        /// </summary>
        /// <param name="newFilePath"></param>
        /// <param name="dt">输入DataTable</param>
        /// <param name="startRow">NPOI开始的行号,从0开始.(对应的Excel列从0开始行).</param>
        /// <param name="rowHeight">行高.(Excel中设置的值,方法中已进行x20计算)</param>
        /// <param name="styleCell_row">样式单元的行号,默认0行.</param>
        /// <param name="styleCell_col">样式单元的列号,默认0列.</param>
        /// <returns></returns>
        public static string CreateExcelTemplate(string newFilePath, DataSet ds, string sheetName = "sheet1")
        {
            try
            {
                IWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet(sheetName);

                //单元格的四周边框设置为细边框.
                ICellStyle style = workbook.CreateCellStyle();//.CreateCellStyle();
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                //cell.CellStyle = style;

                int startRowNum = 0;
                foreach (DataTable dt in ds.Tables)
                {
                    //if (dt.Rows[0][1] + "" == dt.Rows[0][2] + "")
                    //{
                    //    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(startRow, startRow + dt.Rows.Count - 1, 1, 2));
                    //}
                    //else
                    //{
                    //    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(startRow, startRow + dt.Rows.Count - 1, 1, 1));
                    //    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(startRow, startRow + dt.Rows.Count - 1, 2, 2));
                    //}

                    //DataRow preRow = null;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow curRow = dt.Rows[i];
                        //preRow = i == 0 ? dt.Rows[0] : dt.Rows[i - 1];
                        //if(curRow[1]+""==preRow[1]+""&& curRow[2] + ""==preRow[2] + "" && curRow[1] + ""==curRow[1] + "")
                        //{
                        //    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i-1<0?0:i, i, 1, 2));
                        //}else 
                        //{
                        //    if (curRow[0] + ""==preRow[0] + "")
                        //    {
                        //        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i - 1, i, 1, 1));
                        //    }
                        //    if (curRow[1] + ""==preRow[1] + "")
                        //    {
                        //        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i - 1, i, 2, 2));
                        //    }
                        //}


                        //DataRow dr = dt.Rows[i];
                        IRow excelRow = sheet.CreateRow(startRowNum + i);


                        //插入行.第"startRow"到第"sheet.LastRowNum"行移动"dt.Rows.Count"行,bool copyRowHeight, bool resetOriginalRowHeight
                        //sheet.ShiftRows(i, i + 1, dt.Rows.Count, true, true);

                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            
                            var excelCell = excelRow.CreateCell(j, CellType.String);

                            #region 单元格样式
                            excelCell.CellStyle = style;// sheet.GetRow(styleCell_row).Cells[styleCell_col].CellStyle;
                            #endregion

                            excelCell.SetCellValue(curRow[j].ToString());

                        }
                    }
                    startRowNum += dt.Rows.Count;
                }

                Dictionary<string, List<int>> dic = new Dictionary<string, List<int>>();
                List<int[]> List = new List<int[]>();
                int startRowNo=0, endRowNo=0, startColNo, endColNo;
                ICell preCel, curCel;
                //合并相同的单元格
                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {

                    IRow row = sheet.GetRow(rowIndex);
                    string cel1Val = row.GetCell(1).StringCellValue;
                    string cel2Val = row.GetCell(2).StringCellValue;

                    string precel1Val = sheet.GetRow(rowIndex-1).GetCell(1).StringCellValue;
                    string precel2Val = sheet.GetRow(rowIndex-1).GetCell(2).StringCellValue;

                    if (!dic.ContainsKey(cel1Val))
                    {
                        dic.Add(cel1Val, new List<int>());
                    }
                    if (!dic.ContainsKey(cel2Val))
                    {
                        dic.Add(cel2Val, new List<int>());
                    }


                    if (precel1Val== cel1Val&&precel1Val==cel2Val)
                    {
                        if (dic[precel1Val].Count>0)
                        {
                            dic[precel1Val][1] = rowIndex;
                        }
                        
                        endRowNo = rowIndex;
                    }
                    else
                    {
                        List.Add(new int[] {startRowNo,endRowNo });
                    }

                    if (cel1Val == cel2Val)
                    {
                        startColNo = 1;
                        endColNo = 2;
                    }


                    //if (cellVal + "" == cel2 + "" && curRow[2] + "" == preRow[2] + "" && curRow[1] + "" == curRow[1] + "")
                    //{
                    //    //sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i - 1 < 0 ? 0 : i, i, 1, 2));
                    //}
                    //else
                    //{
                    //    //if (curRow[0] + "" == preRow[0] + "")
                    //    //{
                    //    //    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i - 1, i, 1, 1));
                    //    //}
                    //    //if (curRow[1] + "" == preRow[1] + "")
                    //    //{
                    //    //    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i - 1, i, 2, 2));
                    //    //}
                    //}

                    //if (cell == null)
                    //{
                    //    continue;
                    //}
                    //int contextLength = Encoding.UTF8.GetBytes(cell.ToString()).Length;//获取当前单元格的内容宽度
                    //columnWidth = columnWidth < contextLength ? contextLength : columnWidth;

                }

                //自适应列宽
                AutoColumnWidth(sheet, 8);
                //保存Excel
                workbook.SetSheetOrder(sheet.SheetName, 0);//设置表格位置
                workbook.SetActiveSheet(0);
                using (FileStream fs = new FileStream(newFilePath, FileMode.Create))
                {
                    workbook.Write(fs);
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 自适应列宽
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="cols"></param>
        public static void AutoColumnWidth(ISheet sheet, int cols)
        {
            for (int col = 0; col <= cols; col++)
            {
                sheet.AutoSizeColumn(col);//自适应宽度，但是其实还是比实际文本要宽
                int columnWidth = sheet.GetColumnWidth(col) / 256;//获取当前列宽度
                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    ICell cell = row.GetCell(col);
                    if (cell == null)
                    {
                        continue;
                    }
                    int contextLength = Encoding.UTF8.GetBytes(cell.ToString()).Length;//获取当前单元格的内容宽度
                    columnWidth = columnWidth < contextLength ? contextLength : columnWidth;

                }
                sheet.SetColumnWidth(col, columnWidth * 200);//

            }
        }

        /// <summary>
        /// DataTable 数据添加到Excel模板中.
        /// </summary>
        /// <param name="newFilePath"></param>
        /// <param name="dt">输入DataTable</param>
        /// <param name="startRow">NPOI开始的行号,从0开始.(对应的Excel列从0开始行).</param>
        /// <param name="rowHeight">行高.(Excel中设置的值,方法中已进行x20计算)</param>
        /// <param name="styleCell_row">样式单元的行号,默认0行.</param>
        /// <param name="styleCell_col">样式单元的列号,默认0列.</param>
        /// <returns></returns>
        public static string CreateNewExcelForDataTable(string newFilePath, DataSet ds, string sheetName="sheet1")
        {
            try
            {
                int rowHeight = 0;
                int styleCell_row = 0;
                int styleCell_col = 0;
                IWorkbook workbook = OpenExcel(newFilePath);
                //IWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.GetSheetAt(0);
                //ISheet sheet = workbook.CreateSheet(sheetName);
                int curRow=0, curCol=0;

                foreach(DataTable dt in ds.Tables)
                {
                    for (int i = curRow++; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        IRow excelRow = sheet.CreateRow(i);
                        

                        //插入行.第"startRow"到第"sheet.LastRowNum"行移动"dt.Rows.Count"行,bool copyRowHeight, bool resetOriginalRowHeight
                        sheet.ShiftRows(i, i+1, dt.Rows.Count, true, true);

                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            var excelCell = excelRow.CreateCell(j, CellType.String);

                            #region 单元格样式
                            excelCell.CellStyle = sheet.GetRow(styleCell_row).Cells[styleCell_col].CellStyle;
                            #endregion

                            excelCell.SetCellValue(dr[j].ToString());

                        }
                    }
                }

                ;
                //插入行.第"startRow"到第"sheet.LastRowNum"行移动"dt.Rows.Count"行,bool copyRowHeight, bool resetOriginalRowHeight
                //sheet.ShiftRows(startRow, sheet.LastRowNum, dt.Rows.Count, true, true);
                //for (int i=0;i<dt.Rows.Count;i++)
                //{
                //    DataRow dr = dt.Rows[i];
                //    IRow excelRow = sheet.CreateRow(i);

                //    #region 行样式

                //    //设置行高 ,excel行高度每个像素点是1/20
                //    if (rowHeight > 0)
                //    {
                //        excelRow.Height = (short)(rowHeight * 20);
                //    }
                //    #endregion

                //    for (int j = 0; j < dt.Columns.Count; j++)
                //    {
                //        var excelCell = excelRow.CreateCell(j, CellType.String);

                //        #region 单元格样式
                //        excelCell.CellStyle = sheet.GetRow(styleCell_row).Cells[styleCell_col].CellStyle;
                //        #endregion

                //        excelCell.SetCellValue(dr[j].ToString());

                //    }
                //}

                sheet.ForceFormulaRecalculation = true;

                //保存Excel
                SaveAsExel(workbook, newFilePath);
                return "";
                ////保存Excel
                //workbook.SetSheetOrder(sheet.SheetName ,0);//设置表格位置
                //workbook.SetActiveSheet(0);
                //using (FileStream fs = new FileStream(newFilePath, FileMode.Create))
                //{
                //    workbook.Write(fs);
                //}
                //return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public static string ExportSheet(string path, int sheetIndex)
        {
            try
            {
                //打开Excel
                IWorkbook workbook = OpenExcel(path);
                ISheet sheet = workbook.GetSheetAt(sheetIndex);
                workbook.SetActiveSheet(sheetIndex);
                int i = 0;
                while (workbook.NumberOfSheets != 1)
                {
                    if (workbook.GetSheetName(i) != sheet.SheetName)
                    {
                        workbook.RemoveSheetAt(i);

                    }
                    else
                    {
                        i++;
                    }

                }

                //保存Excel
                SaveAsExel(workbook, path);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 打开Excel
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static IWorkbook OpenExcel(string path)
        {
            //read the template via FileStream, it is suggested to use FileAccess.Read to prevent file lock.
            //book1.xls is an Excel-2007-generated file, so some new unknown BIFF records are added. 

            IWorkbook workbook = null;
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                if (path.IndexOf(".xlsx") > 0)
                {
                    // 2007版本 
                    workbook = new XSSFWorkbook(file);
                }
                else if (path.IndexOf(".xls") > 0)
                {
                    // 2003版本 
                    workbook = new HSSFWorkbook(file);
                }
            }
            return workbook;
        }

        /// <summary>
        /// 保存/另存为 Excel文件.
        /// 必须使用File.Create()函数,另存为文件覆盖,不然保存后的文件无法打开.
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="path"></param>
        private static void SaveAsExel(IWorkbook workbook, string path)
        {
            if (workbook != null)
            {
                //必须使用File.Create(path)函数,不然文件保存后,用EXCEL打开会提示:"文件格式已被损坏,内容有问题",无法打文件.
                using (FileStream fsWrite = File.Create(path))
                {
                    workbook.Write(fsWrite);
                }
                workbook.Close();
            }
        }




        //switch(cell.CellType)
        //{
        //    case HSSFCellType.BLANK:
        //        dr[i] = "[null]";
        //        break;
        //    case HSSFCellType.BOOLEAN:
        //        dr[i] = cell.BooleanCellValue;
        //        break;
        //    case HSSFCellType.NUMERIC:
        //        dr[i] = cell.ToString();    //This is a trick to get the correct value of the cell. NumericCellValue will return a numeric value no matter the cell value is a date or a number.
        //        break;
        //    case HSSFCellType.STRING:
        //        dr[i] = cell.StringCellValue;
        //        break;
        //    case HSSFCellType.ERROR:
        //        dr[i] = cell.ErrorCellValue;
        //        break;
        //    case HSSFCellType.FORMULA:
        //    default:
        //        dr[i] = "="+cell.CellFormula;
        //        break;
        //}

    }
}
