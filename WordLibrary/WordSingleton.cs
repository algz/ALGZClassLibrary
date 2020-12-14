using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WordLibrary
{
    public class WordSingleton
    {
        public static bool AddPictureAndTable(string docPath,string picPath,System.Data.DataTable dt)
        {
            try
            {
                Object oMissing = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Word._Application WordApp = new Application();
                WordApp.Visible = true;
                object filename = docPath;

                Microsoft.Office.Interop.Word._Document WordDoc = WordApp.Documents.Add();

                #region 填充图片
                //移动光标文档末尾
                object count = WordDoc.Paragraphs.Count;
                object WdLine = Microsoft.Office.Interop.Word.WdUnits.wdParagraph;
                WordApp.Selection.MoveDown(ref WdLine, ref count, ref oMissing);//移动焦点
                WordApp.Selection.TypeParagraph();//插入段落

                object LinkToFile = false;
                object SaveWithDocument = true;
                object Anchor = WordDoc.Application.Selection.Range;
                WordDoc.Application.ActiveDocument.InlineShapes.AddPicture(picPath, ref LinkToFile, ref SaveWithDocument, ref Anchor);
                #endregion

                #region 填充表格
                //插入表格
                Microsoft.Office.Interop.Word.Table newTable = WordDoc.Tables.Add(WordApp.Selection.Range, dt.Rows.Count+1, dt.Columns.Count, ref oMissing, ref oMissing);
                newTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;//.wdLineStyleThickThinLargeGap;
                newTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                //垂直居中
                WordApp.Selection.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                //水平居中
                WordApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                for (int rowIndex = 0; rowIndex <= dt.Rows.Count; rowIndex++)
                {
                    for (int colIndex = 0; colIndex < dt.Columns.Count; colIndex++)
                    {
                        if (rowIndex == 0)
                        {
                            newTable.Cell(rowIndex + 1, colIndex + 1).Range.Text = dt.Columns[colIndex].ColumnName;
                        }
                        else
                        {
                            newTable.Cell(rowIndex + 1, colIndex + 1).Range.Text = dt.Rows[rowIndex-1][colIndex] + "";
                        }

                    }
                }
                #endregion

                //保存
                WordDoc.SaveAs2(ref filename);
                WordDoc.Close(ref oMissing, ref oMissing, ref oMissing);
                WordApp.Quit(ref oMissing, ref oMissing, ref oMissing);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }


        #region 文档中添加图片
        /// <summary>
        /// 文档中添加图片
        /// </summary>
        /// <param name="filePath">word文件名</param>
        /// <param name="picPath">picture文件名</param>
        /// <returns></returns>
        public static bool AddPicture(string filePath, string picPath)
        {
            try
            {
                Object oMissing = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Word._Application WordApp = new Application();
                WordApp.Visible = true;
                object filename = filePath;

                Microsoft.Office.Interop.Word._Document WordDoc = WordApp.Documents.Add();
                    //WordApp.Documents.Open(ref filename, ref oMissing,
                    //ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    //ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                //移动光标文档末尾
                object count = WordDoc.Paragraphs.Count;
                object WdLine = Microsoft.Office.Interop.Word.WdUnits.wdParagraph;
                WordApp.Selection.MoveDown(ref WdLine, ref count, ref oMissing);//移动焦点
                WordApp.Selection.TypeParagraph();//插入段落

                object LinkToFile = false;
                object SaveWithDocument = true;
                object Anchor = WordDoc.Application.Selection.Range;
                WordDoc.Application.ActiveDocument.InlineShapes.AddPicture(picPath, ref LinkToFile, ref SaveWithDocument, ref Anchor);

                //保存
                WordDoc.SaveAs2(ref filename);
                WordDoc.Close(ref oMissing, ref oMissing, ref oMissing);
                WordApp.Quit(ref oMissing, ref oMissing, ref oMissing);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }
        #endregion 文档中添加图片

        #region 表格处理（插入表格、设置格式、填充内容）
        /// <summary>
        /// 表格处理
        /// </summary>
        /// <param name="filePath">word文件名</param>
        /// <returns></returns>
        public static bool AddTable(string filePath,System.Data.DataTable dt)
        {
            try
            {
                Object oMissing = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Word._Application WordApp = new Application();
                WordApp.Visible = true;
                object filename = filePath;
                Microsoft.Office.Interop.Word._Document WordDoc;
                if (File.Exists(filePath))
                {
                    WordDoc = WordApp.Documents.Open(ref filename, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                }
                else
                {
                    WordDoc = WordApp.Documents.Add();
                }



                //插入表格
                Microsoft.Office.Interop.Word.Table newTable = WordDoc.Tables.Add(WordApp.Selection.Range, 12, 3, ref oMissing, ref oMissing);
                ////设置表格
                newTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleThickThinLargeGap;
                newTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                newTable.Columns[1].Width = 100f;
                newTable.Columns[2].Width = 220f;
                newTable.Columns[3].Width = 105f;

                //填充表格内容
                newTable.Cell(1, 1).Range.Text = "我的简历";
                //设置单元格中字体为粗体
                newTable.Cell(1, 1).Range.Bold = 2;

                //合并单元格
                newTable.Cell(1, 1).Merge(newTable.Cell(1, 3));

                //垂直居中
                WordApp.Selection.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                //水平居中
                WordApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                //填充表格内容
                newTable.Cell(2, 1).Range.Text = "座右铭：...";
                //设置单元格内字体颜色
                newTable.Cell(2, 1).Range.Font.Color = Microsoft.Office.Interop.Word.WdColor.wdColorDarkBlue;
                //合并单元格
                newTable.Cell(2, 1).Merge(newTable.Cell(2, 3));
                WordApp.Selection.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                //填充表格内容
                newTable.Cell(3, 1).Range.Text = "姓名：";
                newTable.Cell(3, 2).Range.Text = "雷鑫";
                //纵向合并单元格
                newTable.Cell(3, 3).Select();
                //选中一行
                object moveUnit = Microsoft.Office.Interop.Word.WdUnits.wdLine;
                object moveCount = 3;
                object moveExtend = Microsoft.Office.Interop.Word.WdMovementType.wdExtend;
                WordApp.Selection.MoveDown(ref moveUnit, ref moveCount, ref moveExtend);
                WordApp.Selection.Cells.Merge();

                //表格中插入图片
                string pictureFileName = System.IO.Directory.GetCurrentDirectory() + @"\picture.jpg";
                object LinkToFile = false;
                object SaveWithDocument = true;
                object Anchor = WordDoc.Application.Selection.Range;
                WordDoc.Application.ActiveDocument.InlineShapes.AddPicture(pictureFileName, ref LinkToFile, ref SaveWithDocument, ref Anchor);
                //图片宽度
                WordDoc.Application.ActiveDocument.InlineShapes[1].Width = 100f;
                //图片高度
                WordDoc.Application.ActiveDocument.InlineShapes[1].Height = 100f;
                //将图片设置为四周环绕型
                Microsoft.Office.Interop.Word.Shape s = WordDoc.Application.ActiveDocument.InlineShapes[1].ConvertToShape();
                s.WrapFormat.Type = Microsoft.Office.Interop.Word.WdWrapType.wdWrapSquare;

                newTable.Cell(12, 1).Range.Text = "备注：";
                newTable.Cell(12, 1).Merge(newTable.Cell(12, 3));
                //在表格中增加行
                WordDoc.Content.Tables[1].Rows.Add(ref oMissing);

                //保存
                WordDoc.SaveAs2(filePath) ;
                WordDoc.Close(ref oMissing, ref oMissing, ref oMissing);
                WordApp.Quit(ref oMissing, ref oMissing, ref oMissing);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }
        #endregion #region 表格处理
    }
}
