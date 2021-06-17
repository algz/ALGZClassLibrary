using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace XCommon
{
    public class GridViewClass
    {
        /// <summary>
        /// GridView 增加序号。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void dataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, grid.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }


        /// <summary>
        /// 指定的行中的单元格变换颜色
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="row"></param>
        /// <param name="txt">单元格中的文本.(默认为不合格)</param>
        public static void Grid_RowPrePaint(DataGridView grid, int row, string txt = "不合格")
        {
            if (grid.Rows.Count > 0)
            {
                for (int col = 0; col < grid.Columns.Count; col++)
                {
                    DataGridViewCell cell = grid.Rows[row].Cells[col];
                    if ((cell.FormattedValue + "").Contains("不合格"))
                    {
                        cell.Style.ForeColor = Color.Red;  //整行颜色
                    }
                }
            }

            //string status = grid.Rows[e.RowIndex].Cells["cl_checkstatus"].Value.ToString();
            //string uploadstate = grid.Rows[e.RowIndex].Cells["cl_upload_state"].Value.ToString();
            //switch (status)
            //{
            //    case "已审核":
            //        grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;  //整行颜色
            //        break;

            //}
            //switch (uploadstate)
            //{
            //    case "已上传":
            //        grid.Rows[e.RowIndex].Cells["cl_upload_state"].Style.ForeColor = Color.Green;  //某个单元格颜色
            //        break;
            //}
        }

        /// <summary>
        /// 通过指定的ID列,获得当前Grid行,DataSorce数据源（必须为DataTable类型）的行数据.
        /// 如果找不到选定行,则返回null.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="grid_col_index">GridView 控件的列索引值</param>
        /// <param name="dataTable_colname">GridView.DataSource (DataTable)的列名</param>
        /// <returns></returns>
        public static DataRow GetCurrentDataRow(DataGridView grid, int grid_col_index = 0, string dataTable_colname = "id")
        {
            DataTable dt = grid.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0 || grid.CurrentRow == null)
            {
                return null;
            }
            string id = grid.CurrentRow.Cells[grid_col_index].FormattedValue + "";
            DataRow[] drs = dt.Select(dataTable_colname + "='" + id + "'");
            if (drs.Length > 0)
            {
                DataRow curDr = dt.Select(dataTable_colname + "='" + id + "'")[0];
                return curDr;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 获得DataGridView.DataSource数据源的当前对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grid"></param>
        /// <param name="grid_col_index"></param>
        /// <param name="propertyName"></param>
        /// <returns>当前对象不存在，返回null</returns>
        public static T GetCurrentObject<T>(DataGridView grid, int grid_col_index = 0, string propertyName = "id")
        {
            if (typeof(DataRow) == typeof(T))
            {
                object s = GetCurrentDataRow(grid, grid_col_index, propertyName);
                return (T)s;
            }
            List<T> list = null;
            if (grid.DataSource is BindingSource)
            {
                BindingSource bs = grid.DataSource as BindingSource;
                list = bs.DataSource as List<T>;
            }
            else if (grid.DataSource is BindingList<T>)
            {
                BindingList<T> bindList = grid.DataSource as BindingList<T>;
                list = bindList.ToList();
            }
            else
            {
                list = (List<T>)grid.DataSource;
            }
            //List<T> list = (List<T>)grid.DataSource;
            if (list != null && list.Count() > 0)
            {
                if (grid.CurrentRow == null)
                {
                    return default(T);
                }
                string id = grid.CurrentRow.Cells[grid_col_index].FormattedValue + "";
                foreach (T s in list)
                {
                    PropertyInfo pi = s.GetType().GetProperties().First(x => x.Name.ToUpper() == propertyName.ToUpper());
                    string val = pi.GetValue(s, null)?.ToString();
                    if (val == id)
                    {
                        return s;
                    }
                }
                //T curDr = dt.Select(dataTable_colname + "='" + id + "'")[0];
                //return curDr;
                return default(T);
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Grid.RowStateChanged.
        /// 每行的HeaderCell赋值行号。
        /// </summary>
        /// <param name="e"></param>
        public static void Grid_RowStateChanged(DataGridViewRowStateChangedEventArgs e)
        {
            e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);
        }

        /// <summary>
        /// 每行的HeaderCell赋值行号。
        /// 可直接设置到GridView 中。
        /// 示例:
        /// this.jdGrid.RowStateChanged += new DataGridViewRowStateChangedEventHandler(GridConfig.Grid_RowStateChanged);
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Grid_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            try
            {
                //行号没有完全显示出来的解决办法是将DataGridView的RowHeadersWidthSizeMode属性设置为AutoSizeToFirstHeader。
                // e.Row.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                DataGridView grid = sender as DataGridView;
                grid.RowHeadersWidth = 50;
                if (!grid.RowHeadersWidthSizeMode.Equals(DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader))
                {
                    //grid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;
                }
                e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);
            }
            catch
            {

            }


        }

        /// <summary>
        /// 适用于CheckBox选择列的Grid.
        /// column[0]：ID；column[1]:checkbox.
        /// 示例:
        /// this.airModelGrid.CellClick +=new DataGridViewCellEventHandler(CheckBoxGrid_CellClick);
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void CheckBoxGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            if (e.RowIndex != -1)
            {
                bool flag = Convert.ToBoolean(grid.Rows[e.RowIndex].Cells[1].FormattedValue);
                grid.Rows[e.RowIndex].Cells[1].Value = !flag;
            }
        }

        /// <summary>
        /// 获取选择的IDS.适用于CheckBox选择列的Grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static List<string> GetSelectedIdsOfCheckBoxGrid(DataGridView grid)
        {
            List<string> _selectedGKIds = new List<string>();
            if (grid.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (Convert.ToBoolean(row.Cells[1].FormattedValue))
                    {
                        _selectedGKIds.Add(row.Cells[0].FormattedValue + "");
                    }
                }
            }

            return _selectedGKIds;
        }

        /// <summary>
        /// 刷新数据,适用于CheckBox选择列的Grid和普通Grid.
        /// column[0]：ID；column[1]:checkbox.
        /// </summary>
        /// <param name="grid">Grid</param>
        /// <param name="obj">DataSource</param>
        /// <param name="_selectedIds"></param>
        public static void RefreshDataGrid(DataGridView grid, object obj, List<string> _selectedIds = null)
        {
            grid.DataSource = obj;
            if (grid.Columns.Count > 2 && _selectedIds != null && grid.Columns[1] is DataGridViewCheckBoxColumn)
            {
                foreach (DataGridViewRow dr in grid.Rows)
                {
                    foreach (string id in _selectedIds)
                    {
                        if (dr.Cells[0].FormattedValue + "" == id)
                        {
                            dr.Cells[1].Value = true;
                        }
                    }

                }
            }

        }

        /// <summary>
        /// 绑定Grid.ComboxColumn数据源
        /// 示例:
        /// Dictionary<string, string> dic = new Dictionary<string, string> { { "ZH", "载荷" }, { "ZT", "姿态" }, { "DW", "档位" }, { "SD", "深度" } };
        /// GridConfig.SetGridComboxColumnDataSource(sgGKZHGrid, "sgZHAttr1Col", dic);
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="columnName"></param>
        /// <param name="dataSource"></param>
        /// <param name="displayMember"></param>
        /// <param name="valueMember"></param>
        public static void SetGridComboxColumnDataSource(DataGridView grid, string columnName, object dataSource, string displayMember = "Value", string valueMember = "Key")
        {
            DataGridViewComboBoxColumn box = (DataGridViewComboBoxColumn)grid.Columns[columnName];
            BindingSource bs = new BindingSource();
            bs.DataSource = dataSource;// new Dictionary<string, string> { { "S", "时序" }, { "L", "极限" }, { "G", "极数" } };
            box.DataSource = bs;
            box.DisplayMember = displayMember;
            box.ValueMember = valueMember;
        }

        //public static void RefreshGridData(DataGridView grid)
        //{
        //    IList obj = grid.DataSource as IList;
        //    var obj1 = obj;
        //    //obj.CopyTo(obj1,0);


        //    obj.Clear();
        //    grid.DataSource = obj;
        //}
    }
}
