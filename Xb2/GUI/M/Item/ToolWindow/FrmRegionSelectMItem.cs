using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Entity.Business;
using Xb2.GUI.Controls;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.GUI.M.Item.ToolWindow
{
    public partial class FrmRegionSelectMItem : FrmBase
    {
        public FrmRegionSelectMItem(XbUser user)
        {
            this.InitializeComponent();
            this.CUser = user;
        }

        /// <summary>
        /// 返回的数据视图名
        /// </summary>
        public string ViewName { get; private set; }

        /// <summary>
        /// 圆域查询+
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, System.EventArgs e)
        {
            CircleQuery circleQuery = new CircleQuery();
            circleQuery.Tag = Convert.ToInt32(flowLayoutPanel2.Tag) + 1;
            flowLayoutPanel2.Controls.Add(circleQuery);
            flowLayoutPanel2.Tag = circleQuery.Tag;
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
                var controls = flowLayoutPanel2.Controls.Cast<Control>().ToList();
                var circleQueries = controls.FindAll(c => c.GetType() == typeof(CircleQuery));
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
            RectQuery rectQuery = new RectQuery();
            rectQuery.Tag = Convert.ToInt32(flowLayoutPanel3.Tag) + 1;
            flowLayoutPanel3.Controls.Add(rectQuery);
            flowLayoutPanel3.Tag = rectQuery.Tag;
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
                var controls = flowLayoutPanel3.Controls.Cast<Control>().ToList();
                var rectQueries = controls.FindAll(c => c.GetType() == typeof(RectQuery));
                var circleQuery =
                    rectQueries.Find(
                        rq => Convert.ToInt32(rq.Tag) == Convert.ToInt32(flowLayoutPanel3.Tag));
                flowLayoutPanel3.Controls.Remove(circleQuery);
                flowLayoutPanel3.Tag = Convert.ToInt32(flowLayoutPanel3.Tag) - 1;
            }
        }

        /// <summary>
        /// 关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            var viewDisplayName = textBox1.Text.Trim();

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

            //是否有重名的区域查询条件？
            var sql = "select count(*) from {0} where 用户编号={1} and 视图名称='{2}'";
            sql = string.Format(sql, Db.TnRQMItem(), this.CUser.ID, viewDisplayName);
            Debug.Print(sql);
            var n = Convert.ToInt32(MySqlHelper.ExecuteScalar(Db.CStr(), sql));
            if (n > 0)
            {
                MessageBox.Show("已经存在名称【" + viewDisplayName + "】，未保存！");
                return;
            }
            //生成视图创建语句，创建视图
            //视图名不允许有-，所以替换为_
            var viewName = this.CUser.Name + "_" + Guid.NewGuid().ToString().Replace('-', '_');
            var viewSQL = string.Format("create view {0} as " + this.GetSql(), viewName);
            Debug.Print(viewSQL);
            MySqlHelper.ExecuteNonQuery(Db.CStr(), viewSQL);
            Debug.Print(viewSQL);
            //视图如果创建成功了，向数据库中写入记录
            if (Db.HasView(viewName))
            {
                sql = "insert into {0}(用户编号,视图名称,视图体) values ({1},'{2}','{3}')";
                sql = string.Format(sql, Db.TnRQMItem(), this.CUser.ID, viewDisplayName, viewName);
                Debug.Print(sql);
                n = MySqlHelper.ExecuteNonQuery(Db.CStr(), sql);
                if (n > 0)
                {
                    MessageBox.Show("保存成功！");
                    this.RefreshDataGridView();
                }
            }
            else
            {
                MessageBox.Show("视图生成失败！");
            }
        }

        /// <summary>
        /// 确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
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

            #endregion

        }

        /// <summary>
        /// 获取圆域查询控件
        /// </summary>
        /// <returns></returns>
        private List<CircleQuery> GetCircleQueries()
        {
            var f2Controls = flowLayoutPanel2.Controls.Cast<Control>().ToList();
            var circleQueries = f2Controls.FindAll(c => c.GetType() == typeof(CircleQuery));
            return circleQueries.Cast<CircleQuery>().ToList();
        }

        /// <summary>
        /// 获取矩形域查询控件
        /// </summary>
        /// <returns></returns>
        private List<RectQuery> GetRectQueries()
        {
            var f2Controls = flowLayoutPanel3.Controls.Cast<Control>().ToList();
            var circleQueries = f2Controls.FindAll(c => c.GetType() == typeof(RectQuery));
            return circleQueries.Cast<RectQuery>().ToList();
        }

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
            return builder.ToString();
        }

        /// <summary>
        /// 获得用于创建（定义）视图的sql语句
        /// </summary>
        /// <returns></returns>
        private string GetSql()
        {
            var sb = new StringBuilder(string.Format("select * from {0} where ", Db.TnMItem()));
            sb.Append(this.GetCircleQueryClause());
            sb.Append(this.GetRectQueryClause());
            var sql = sb.ToString();
            sql = sql.Replace("and", "");
            //由于和地震目录使用了同一个距离查询参数
            //所以必须和地震目录的经纬度格式统一
            sql = sql.Replace("纬度", "lnglat_to_int(纬度)");
            sql = sql.Replace("经度", "lnglat_to_int(经度)");
            Debug.Print(sql);
            return sql;
        }

        private void FrmRegionSelectMItem_Load(object sender, EventArgs e)
        {
            this.RefreshDataGridView();
        }

        private void RefreshDataGridView()
        {
            var sql = "select 编号,视图名称 as 名称,视图体 from {0} where 用户编号={1}";
            sql = string.Format(sql, Db.TnRQMItem(), this.CUser.ID);
            Debug.Print(sql);
            var dt = MySqlHelper.ExecuteDataset(Db.CStr(), sql).Tables[0];
            //加入测项表，即不使用区域查询
            var dr = dt.NewRow();
            dr["名称"] = "测项";
            dr["视图体"] = Db.TnMItem();
            dt.Rows.InsertAt(dr, 0);
            dt = DataHelper.IdentifyDataTable(dt);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = dt;
            dataGridView1.MultiSelect = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.Columns["序号"].FillWeight = 10;
            dataGridView1.Columns["名称"].FillWeight = 50;
            //隐藏视图体列、编号列
            dataGridView1.Columns["视图体"].Visible = false;
            dataGridView1.Columns["编号"].Visible = false;
        }

        private void dataGridView1_CellMouseDoubleClick(object sender,
            DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.Button == MouseButtons.Left)
            {
                //获取视图名，关闭
                var viewName = dataGridView1.Rows[e.RowIndex].Cells["视图体"].Value.ToString();
                this.ViewName = viewName;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

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
                var vn = row.Cells["视图体"].Value.ToString();
                //测项表不能删除
                if (vn.Equals(Db.TnMItem()))
                {
                    MessageBox.Show("测项表不可删除！");
                    return;
                }
                var id = Convert.ToInt32(row.Cells["编号"].Value);
                Debug.Print("编号：{0}，视图体:{1}", id, vn);
                var sql = "drop view " + vn;
                Debug.Print(sql);
                MySqlHelper.ExecuteNonQuery(Db.CStr(), sql);
                if (!Db.HasView(vn))
                {
                    sql = "delete from {0} where 编号={1}";
                    sql = string.Format(sql, Db.TnRQMItem(), id);
                    Debug.Print(sql);
                    var n = MySqlHelper.ExecuteNonQuery(Db.CStr(), sql);
                    if (n > 0)
                    {
                        MessageBox.Show("已删除！");
                        this.RefreshDataGridView();
                    }
                }
            }

        }


    }
}
