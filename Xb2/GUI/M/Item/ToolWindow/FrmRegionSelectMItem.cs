using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using NLog;
using Xb2.Entity.Business;
using Xb2.GUI.Controls;
using Xb2.GUI.Controls.User;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;
using ExtendMethodDataTable = Xb2.Utils.ExtendMethod.ExtendMethodDataTable;

namespace Xb2.GUI.M.Item.ToolWindow
{
    /// <summary>
    /// 区域查询的处理逻辑：
    /// 用户输入区域查询条件
    /// 点击保存按钮
    /// 后台开始按照用户输入的查询条件拼接查询语句
    /// 将查询语句保存为视图，并把视图取一个名字存在数据库里（名字不可重复）
    /// 用户在 选测项 界面上选择 区域查询时
    /// 其实就是在不同的 区域查询视图上 进行查询
    /// </summary>
    public partial class FrmRegionSelectMItem : FrmBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public FrmRegionSelectMItem(XbUser user)
        {
            this.InitializeComponent();
            this.User = user;
        }

        /// <summary>
        /// 返回的数据视图名
        /// </summary>
        public string ViewName { get; private set; }

        #region 加减圆域查询，加减矩形域查询

        /// <summary>
        /// 圆域查询+
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, System.EventArgs e)
        {
            QueryCircleControl queryCircleControl = new QueryCircleControl();
            queryCircleControl.Tag = Convert.ToInt32(flowLayoutPanel2.Tag) + 1;
            flowLayoutPanel2.Controls.Add(queryCircleControl);
            flowLayoutPanel2.Tag = queryCircleControl.Tag;
        }

        /// <summary>
        /// 圆域查询-
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanel2.Controls.Count > 0)
            {
                var controls = flowLayoutPanel2.Controls.Cast<System.Windows.Forms.Control>().ToList();
                var circleQueries = controls.FindAll(c => c.GetType() == typeof(QueryCircleControl));
                var circleQuery =
                    circleQueries.Find(
                        cq => Convert.ToInt32(cq.Tag) == Convert.ToInt32(flowLayoutPanel2.Tag));
                flowLayoutPanel2.Controls.Remove(circleQuery);
                flowLayoutPanel2.Tag = Convert.ToInt32(flowLayoutPanel2.Tag) - 1;
            }
        }

        /// <summary>
        /// 矩形域查询+
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            QueryRectangleControl queryRectangleControl = new QueryRectangleControl();
            queryRectangleControl.Tag = Convert.ToInt32(flowLayoutPanel3.Tag) + 1;
            flowLayoutPanel3.Controls.Add(queryRectangleControl);
            flowLayoutPanel3.Tag = queryRectangleControl.Tag;
        }

        /// <summary>
        /// 矩形域查询-
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanel3.Controls.Count > 0)
            {
                var controls = flowLayoutPanel3.Controls.Cast<System.Windows.Forms.Control>().ToList();
                var rectQueries = controls.FindAll(c => c.GetType() == typeof(QueryRectangleControl));
                var circleQuery =
                    rectQueries.Find(
                        rq => Convert.ToInt32(rq.Tag) == Convert.ToInt32(flowLayoutPanel3.Tag));
                flowLayoutPanel3.Controls.Remove(circleQuery);
                flowLayoutPanel3.Tag = Convert.ToInt32(flowLayoutPanel3.Tag) - 1;
            }
        }

        #endregion

        #region 保存按钮

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            var viewDisplayName = textBox1.Text.Trim();
            Logger.Info("View Disply Name:{0}", viewDisplayName);

            #region 输入检查

            //查询控件的个数不能都为0
            if (this.GetCircleQueries().Count == 0 && this.GetRectQueries().Count == 0)
            {
                MessageBox.Show("查询条件为空！");
                return;
            }
            //圆域查询条件检查
            foreach (var cquery in this.GetCircleQueries())
            {
                if (!cquery.IsLegalInput())
                {
                    MessageBox.Show("圆形查询条件输入不完整！");
                    return;
                }
            }
            //矩形域查询条件检查
            foreach (var rquery in this.GetRectQueries())
            {
                if (!rquery.IsLegalInput())
                {
                    MessageBox.Show("矩形查询条件输入不完整！");
                    return;
                }
            }
            //查询条件名不能为空
            if (textBox1.Text.Trim().Equals(string.Empty))
            {
                MessageBox.Show("请输入查询条件名！");
                return;
            }

            #endregion

            // 是否有重名的区域查询条件？
            var commandText = "select count(*) from {0} where 用户编号={1} and 视图名称='{2}'";
            commandText = string.Format(commandText, DaoObject.TnRQMItem(), this.User.ID, viewDisplayName);
            Logger.Debug(commandText);
            var n = Convert.ToInt32(MySqlHelper.ExecuteScalar(DaoObject.ConnectionString, commandText));
            Logger.Info("查询是否存在名为 {0} 的视图 ？ {1}", viewDisplayName, n > 0);
            if (n > 0)
            {
                MessageBox.Show("已经存在名称【" + viewDisplayName + "】，未保存！");
                return;
            }

            // 生成视图创建语句，创建视图
            // 视图名不允许有-，所以替换为_，视图名为用户名_GUID
            var viewName = this.User.Name + "_" + Guid.NewGuid().ToString().Replace('-', '_');
            Logger.Info("视图名：" + viewName);
            var viewCommandText = string.Format("create view {0} as " + this.GetCommandText(), viewName);
            MySqlHelper.ExecuteNonQuery(DaoObject.ConnectionString, viewCommandText);
            Logger.Debug("视图创建语句：" + viewCommandText);

            // 视图如果创建成功了，向数据库中写入记录
            if (DaoObject.HasView(viewName))
            {
                commandText = "insert into {0}(用户编号,视图名称,视图体) values ({1},'{2}','{3}')";
                commandText = string.Format(commandText, DaoObject.TnRQMItem(), this.User.ID, viewDisplayName, viewName);
                Logger.Debug(commandText);
                n = MySqlHelper.ExecuteNonQuery(DaoObject.ConnectionString, commandText);
                Logger.Info("保存视图记录 " + (n > 0));
                if (n > 0)
                {
                    MessageBox.Show("保存成功！");
                    RefreshDataGridView();
                }
            }
            else
            {
                MessageBox.Show("视图生成失败！");
            }
        }

        #endregion

        #region 获取界面上的圆域查询控件 和 矩形域查询控件

        /// <summary>
        /// 获取圆域查询控件
        /// </summary>
        /// <returns></returns>
        private List<QueryCircleControl> GetCircleQueries()
        {
            var f2Controls = flowLayoutPanel2.Controls.Cast<System.Windows.Forms.Control>().ToList();
            var circleQueries = f2Controls.FindAll(c => c.GetType() == typeof(QueryCircleControl));
            return circleQueries.Cast<QueryCircleControl>().ToList();
        }

        /// <summary>
        /// 获取矩形域查询控件
        /// </summary>
        /// <returns></returns>
        private List<QueryRectangleControl> GetRectQueries()
        {
            var f2Controls = flowLayoutPanel3.Controls.Cast<System.Windows.Forms.Control>().ToList();
            var circleQueries = f2Controls.FindAll(c => c.GetType() == typeof(QueryRectangleControl));
            return circleQueries.Cast<QueryRectangleControl>().ToList();
        }

        #endregion

        #region 解析查询控件上的查询表达式，和生成查询命令

        /// <summary>
        /// 获得矩形域查询表达式
        /// </summary>
        /// <returns></returns>
        private string GetRectQueryClause()
        {
            var rqueries = this.GetRectQueries();
            var builder = new StringBuilder();
            if (rqueries.Count > 0)
            {
                if (rqueries.Count == 1)
                {
                    //如果还进行圆形查询，则位置之间的逻辑运算符应为or
                    if (flowLayoutPanel2.Controls.Count > 0)
                    {
                        return string.Format("or (in_rect({0},{1},{2},{3},经度,纬度)) ",
                            rqueries[0].Lng1, rqueries[0].Lat1, rqueries[0].Lng2, rqueries[0].Lat2);
                    }
                    return string.Format("and (in_rect({0},{1},{2},{3},经度,纬度)) ",
                        rqueries[0].Lng1, rqueries[0].Lat1, rqueries[0].Lng2, rqueries[0].Lat2);
                }
                for (int i = 0; i < rqueries.Count; i++)
                {
                    double lng1 = rqueries[i].Lng1, lat1 = rqueries[0].Lat1;
                    double lng2 = rqueries[i].Lng2, lat2 = rqueries[0].Lat2;
                    if (i == 0)
                    {
                        if (flowLayoutPanel2.Controls.Count > 0)
                        {
                            builder.AppendFormat("or (in_rect({0},{1},{2},{3},经度,纬度) ", lng1, lat1,
                                lng2, lat2);
                            continue;
                        }
                        builder.AppendFormat("and (in_rect({0},{1},{2},{3},经度,纬度) ", lng1, lat1,
                            lng2, lat2);
                        continue;
                    }
                    if (i == rqueries.Count - 1)
                    {
                        builder.AppendFormat("or in_rect({0},{1},{2},{3},经度,纬度)) ", lng1, lat1, lng2,
                            lat2);
                        break;
                    }
                    builder.AppendFormat("or in_rect({0},{1},{2},{3},经度,纬度) ", lng1, lat1, lng2,
                        lat2);
                }
            }
            Logger.Debug("矩形域查询表达式：" + builder);
            return builder.ToString();
        }

        /// <summary>
        /// 获取圆查询条件表达式
        /// </summary>
        /// <returns></returns>
        private string GetCircleQueryClause()
        {
            var cqueries = this.GetCircleQueries();
            var builder = new StringBuilder();
            if (cqueries.Count > 0)
            {
                if (cqueries.Count == 1)
                {
                    return string.Format("and (in_circle({0},{1},经度,纬度,{2})) ", cqueries[0].Lng,
                        cqueries[0].Lat,
                        cqueries[0].Dist);
                }
                for (int i = 0; i < cqueries.Count; i++)
                {
                    double lng = cqueries[i].Lng;
                    double lat = cqueries[i].Lat;
                    double dist = cqueries[i].Dist;
                    if (i == 0)
                    {
                        builder.AppendFormat("and (in_circle({0},{1},经度,纬度,{2}) ", lng, lat, dist);
                        continue;
                    }
                    if (i == cqueries.Count - 1)
                    {
                        builder.AppendFormat("or in_circle({0},{1},经度,纬度,{2})) ", lng, lat, dist);
                        break;
                    }
                    builder.AppendFormat("or in_circle({0},{1},经度,纬度,{2}) ", lng, lat, dist);
                }
            }
            Logger.Debug("圆域查询表达式：" + builder);
            return builder.ToString();
        }

        /// <summary>
        /// 获得用于创建（定义）视图的sql语句
        /// </summary>
        /// <returns></returns>
        private string GetCommandText()
        {
            var stringBuilder = new StringBuilder(string.Format("select * from {0} where ", DaoObject.TnMItem()));
            Logger.Debug("拼接区域查询SQL语句，基本查询语句：" + stringBuilder);
            stringBuilder.Append(GetCircleQueryClause());
            stringBuilder.Append(GetRectQueryClause());
            Logger.Debug("拼接完成：{0}", stringBuilder);
            var commandText = stringBuilder.ToString();
            commandText = commandText.Replace("and", "");
            // 由于和地震目录使用了同一个距离查询参数
            // 所以必须和地震目录的经纬度格式统一
            commandText = commandText.Replace("纬度", "lnglat_to_int(纬度)");
            commandText = commandText.Replace("经度", "lnglat_to_int(经度)");
            Logger.Debug("稍作字符串处理，得到最终的查询语句：" + commandText);
            return commandText;
        }

        #endregion

        private void FrmRegionSelectMItem_Load(object sender, EventArgs e)
        {
            this.RefreshDataGridView();
        }

        private void RefreshDataGridView()
        {
            var commandText = "select 编号,视图名称 as 名称,视图体 from {0} where 用户编号={1}";
            commandText = string.Format(commandText, DaoObject.TnRQMItem(), this.User.ID);
            Logger.Debug(commandText);
            var dt = MySqlHelper.ExecuteDataset(DaoObject.ConnectionString, commandText).Tables[0];
            Logger.Info("查询用户 {0} 的区域查询视图信息， 共返回 {1} 条", this.User.ID, dt.Rows.Count);
            
            //加入测项表，即不使用区域查询
            var dr = dt.NewRow();
            dr["名称"] = "测项";
            dr["视图体"] = DaoObject.TnMItem();
            dt.Rows.InsertAt(dr, 0);
            dt = ExtendMethodDataTable.IdentifyDataTable(dt);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = dt;
            dataGridView1.MultiSelect = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            if (dataGridView1.Columns["序号"] != null)
            {
                dataGridView1.Columns["序号"].FillWeight = 10;
            }
            if (dataGridView1.Columns["名称"] != null)
            {
                dataGridView1.Columns["名称"].FillWeight = 50;
            }
            //隐藏视图体列、编号列
            if (dataGridView1.Columns["视图体"] != null)
            {
                dataGridView1.Columns["视图体"].Visible = false;
            }
            if (dataGridView1.Columns["编号"] != null)
            {
                dataGridView1.Columns["编号"].Visible = false;
            }
            // 设置DataGridView不可排序
            foreach (DataGridViewColumn datagridviewcolumn in dataGridView1.Columns)
            {
                datagridviewcolumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        
        #region 双击选中的数据行，则选中当前的区域查询视图并关闭窗体
        
        private void dataGridView1_CellMouseDoubleClick(object sender,
            DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.Button == MouseButtons.Left)
            {
                //获取视图名，关闭
                var viewName = dataGridView1.Rows[e.RowIndex].Cells["视图体"].Value.ToString();
                this.ViewName = viewName;
                Logger.Info("用户当前选中区域查询视图：" + this.ViewName);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        #endregion

        #region 右击选中的数据行，显示删除区域查询视图的菜单和事件

        /// <summary>
        /// 右键单击DataGridView，显示删除菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.Button == MouseButtons.Right)
            {
                dataGridView1.Rows[e.RowIndex].Selected = true;
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        /// <summary>
        /// 删除查询条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var row = dataGridView1.SelectedRows[0];
                var viewName = row.Cells["视图体"].Value.ToString();
                //测项表不能删除
                if (viewName.Equals(DaoObject.TnMItem()))
                {
                    MessageBox.Show("测项表不可删除！");
                    return;
                }
                var id = Convert.ToInt32(row.Cells["编号"].Value);
                Logger.Info("要删除的区域查询视图：{0}，视图体:{1}", id, viewName);
                var commandText = "drop view " + viewName;
                Logger.Debug(commandText);
                MySqlHelper.ExecuteNonQuery(DaoObject.ConnectionString, commandText);
                if (!DaoObject.HasView(viewName))
                {
                    commandText = "delete from {0} where 编号={1}";
                    commandText = string.Format(commandText, DaoObject.TnRQMItem(), id);
                    Logger.Debug(commandText);
                    var n = MySqlHelper.ExecuteNonQuery(DaoObject.ConnectionString, commandText);
                    Logger.Info("删除视图记录，编号=" + id + "，" + (n > 0));
                    if (n > 0)
                    {
                        MessageBox.Show("已删除！");
                        this.RefreshDataGridView();
                    }
                }
            }

        }

        #endregion
    }
}
