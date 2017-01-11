using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using NLog;
using Xb2.Entity.Business;
using Xb2.GUI.Controls;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.GUI.Catalog
{
    public partial class FrmGenSubDatabase : FrmBase
    {
        // 查询命令模板
        private string m_commandTemplate = "select date(发震日期) as 发震日期,time(发震时间) as 发震时间,"
                                           + "经度,纬度,震级单位,round(震级值,1) as 震级值,定位参数,参考地点 from 系统_地震目录 where ";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //用于构建查询命令的StringBuilder
        private StringBuilder m_stringBuilder;

        public StringBuilder SqlBuilder
        {
            get { return this.m_stringBuilder; }
        }

        public FrmGenSubDatabase(XbUser user)
        {
            InitializeComponent();
            this.User = user;
            this.m_stringBuilder = new StringBuilder();
        }

        private void FrmGenSubDatabase_Load(object sender, EventArgs e)
        {
            var dateRange = DaoObject.GetEarthquakeDateRange(DbHelper.TnCategory());
            var magnitudeRange = DaoObject.GetEarthquakeMagnitudeRange(DbHelper.TnCategory());
            dateTimePicker1.Value = dateRange.Start;
            dateTimePicker2.Value = dateRange.End;
            textBox1.Text = magnitudeRange[0].ToString();
            textBox2.Text = magnitudeRange[1].ToString();
            this.Text = this.Text + "[" + this.User.Name + "]";
        }

        #region 查询按钮操作、拼接查询语句、生成子库按钮操作

        /// <summary>
        /// 查询按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            DateTime startDate = this.dateTimePicker1.Value;
            DateTime endDate = this.dateTimePicker2.Value;
            double lowerMagnitude = Convert.ToDouble(this.textBox1.Text.Trim());
            double upperMagnitude = Convert.ToDouble(this.textBox2.Text.Trim());

            #region 参考地点输入条件的验证

            if (flowLayoutPanel1.GetTextBoxs().Any(t => t.Text.Trim().Equals("")))
            {
                MessageBox.Show("参考地点输入不完整！");
                return;
            }

            #endregion

            #region 圆形区域输入条件的验证

            var circleQueries =
                flowLayoutPanel2.Controls.Cast<Control>().ToList().FindAll(c => c.GetType() == typeof(CircleQuery));
            foreach (var control in circleQueries)
            {
                var circleQuery = (CircleQuery) control;
                if (!circleQuery.IsLegalInput())
                {
                    MessageBox.Show("圆形查询条件输入不完整！");
                    return;
                }
            }

            #endregion

            #region 矩形区域输入验证

            var rectQueries =
                flowLayoutPanel3.Controls.Cast<Control>().ToList().FindAll(c => c.GetType() == typeof(RectQuery));
            foreach (var control in rectQueries)
            {
                var rectQuery = (RectQuery) control;
                if (!rectQuery.IsLegalInput())
                {
                    MessageBox.Show("矩形查询条件输入不完整！");
                    return;
                }
            }

            #endregion

            #region 按照查询条件拼接Select语句

            m_stringBuilder = new StringBuilder(this.m_commandTemplate);
            m_stringBuilder.AppendFormat("(发震日期 between \'{0}\' and \'{1}\' ) ", startDate.SStr(), endDate.SStr());
            //加入发震日期的约束条件
            m_stringBuilder.AppendFormat("and (震级值 between {0} and {1}) ", lowerMagnitude, upperMagnitude); //加入震级值约束条件
            m_stringBuilder.Append(GetLocationNameClause()); //加入参考地点约束条件
            m_stringBuilder.Append(GetCircleQueryClause()); //加入圆域查询约束条件
            m_stringBuilder.Append(GetRectQueryClause()); //加入矩形区域查询约束条件
            m_stringBuilder.AppendLine("order by 发震日期, 参考地点");
            var commandText = m_stringBuilder.ToString();
            Logger.Debug(commandText);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
            RefreshDataGridView(dt);

            #endregion

        }

        /// <summary>
        /// 获得矩形查询表达式
        /// </summary>
        /// <returns></returns>
        private string GetRectQueryClause()
        {
            var rectQueryControls = flowLayoutPanel3.Controls.Cast<Control>()
                .ToList().FindAll(p => p.GetType() == typeof(RectQuery))
                .Cast<RectQuery>().ToList();
            Logger.Debug("共获取到 {0} 个矩形域查询空间", rectQueryControls.Count);
            var commandTextBuilder = new StringBuilder();
            if (rectQueryControls.Count > 0)
            {
                if (rectQueryControls.Count == 1)
                {
                    //如果还进行圆形查询，则位置之间的逻辑运算符应为or
                    if (flowLayoutPanel2.Controls.Count > 0)
                    {
                        return string.Format("or (in_rect({0},{1},{2},{3},经度,纬度)) ",
                            rectQueryControls[0].Lng1, rectQueryControls[0].Lat1, rectQueryControls[0].Lng2,
                            rectQueryControls[0].Lat2);
                    }
                    return string.Format("and (in_rect({0},{1},{2},{3},经度,纬度)) ",
                        rectQueryControls[0].Lng1, rectQueryControls[0].Lat1, rectQueryControls[0].Lng2,
                        rectQueryControls[0].Lat2);
                }
                for (int i = 0; i < rectQueryControls.Count; i++)
                {
                    double lng1 = rectQueryControls[i].Lng1, lat1 = rectQueryControls[0].Lat1;
                    double lng2 = rectQueryControls[i].Lng2, lat2 = rectQueryControls[0].Lat2;
                    if (i == 0)
                    {
                        if (flowLayoutPanel2.Controls.Count > 0)
                        {
                            commandTextBuilder.AppendFormat("or (in_rect({0},{1},{2},{3},经度,纬度) ", lng1, lat1, lng2,
                                lat2);
                            continue;
                        }
                        commandTextBuilder.AppendFormat("and (in_rect({0},{1},{2},{3},经度,纬度) ", lng1, lat1, lng2, lat2);
                        continue;
                    }
                    if (i == rectQueryControls.Count - 1)
                    {
                        commandTextBuilder.AppendFormat("or in_rect({0},{1},{2},{3},经度,纬度)) ", lng1, lat1, lng2, lat2);
                        break;
                    }
                    commandTextBuilder.AppendFormat("or in_rect({0},{1},{2},{3},经度,纬度) ", lng1, lat1, lng2, lat2);
                }
            }
            Logger.Debug("矩形域查询表达式：{0}", commandTextBuilder.ToString());
            return commandTextBuilder.ToString();
        }

        /// <summary>
        /// 获取圆查询条件表达式
        /// </summary>
        /// <returns></returns>
        private string GetCircleQueryClause()
        {
            var circleQueryControls = flowLayoutPanel2.Controls.Cast<Control>().ToList()
                .FindAll(c => c.GetType() == typeof(CircleQuery)).Cast<CircleQuery>().ToList();
            Logger.Debug("共获取到 {0} 个圆域查询空间", circleQueryControls.Count);
            var commandTextBuilder = new StringBuilder();
            if (circleQueryControls.Count > 0)
            {
                if (circleQueryControls.Count == 1)
                {
                    return string.Format("and (in_circle({0},{1},经度,纬度,{2})) ", circleQueryControls[0].Lng,
                        circleQueryControls[0].Lat,
                        circleQueryControls[0].Dist);
                }
                for (int i = 0; i < circleQueryControls.Count; i++)
                {
                    double lng = circleQueryControls[i].Lng;
                    double lat = circleQueryControls[i].Lat;
                    double dist = circleQueryControls[i].Dist;
                    if (i == 0)
                    {
                        commandTextBuilder.AppendFormat("and (in_circle({0},{1},经度,纬度,{2}) ", lng, lat, dist);
                        continue;
                    }
                    if (i == circleQueryControls.Count - 1)
                    {
                        commandTextBuilder.AppendFormat("or in_circle({0},{1},经度,纬度,{2})) ", lng, lat, dist);
                        break;
                    }
                    commandTextBuilder.AppendFormat("or in_circle({0},{1},经度,纬度,{2}) ", lng, lat, dist);
                }
            }
            Logger.Debug("圆域查询表达式：{0}", commandTextBuilder.ToString());
            return commandTextBuilder.ToString();
        }

        /// <summary>
        /// 获得所有的查询地名
        /// </summary>
        /// <returns></returns>
        private string GetLocationNameClause()
        {
            var locations = new List<string>();
            var controls = flowLayoutPanel1.Controls.Cast<Control>().ToList();
            var textBoxs = controls.FindAll(c => c.GetType() == typeof(TextBox));
            textBoxs.ForEach(t => locations.Add(t.Text.Trim()));
            var builder = new StringBuilder();
            if (locations.Count > 0)
            {
                if (locations.Count == 1)
                {
                    return string.Format("and (参考地点 like \'%{0}%\') ", locations[0]);
                }
                for (int i = 0; i < locations.Count; i++)
                {
                    if (i == 0)
                    {
                        builder.AppendFormat("and ((参考地点 like \'%{0}%\') ", locations[i]);
                        continue;
                    }
                    if (i == locations.Count - 1)
                    {
                        builder.AppendFormat("or (参考地点 like \'%{0}%\')) ", locations[i]);
                        break;
                    }
                    builder.AppendFormat("or (参考地点 like \'%{0}%\')", locations[i]);
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 生成子库 按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            var databaseName = textBox3.Text.Trim(); //子库名
            var userId = this.User.ID;
            #region 输入合法性验证

            //数据源不能为空
            if (this.dataGridView1.DataSource == null)
            {
                MessageBox.Show("无查询结果，无法生成子库！");
                return;
            }
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("无查询结果，无法生成子库！");
                return;
            }
            //必须输入子库名
            if (databaseName.Equals(""))
            {
                MessageBox.Show("请先输入子库名称再生成！");
                return;
            }
            //至少需要选中一条数据
            if (dataGridView1.Rows.Count > 0)
            {
                int number = 0;
                DataTable dataTable = (DataTable) dataGridView1.DataSource;
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dataTable.Rows[i]["选择"]) == 1)
                    {
                        number = number + 1;
                    }
                }
                if (number == 0)
                {
                    MessageBox.Show("你没有选中任何地震目录，无法生成子库！");
                    return;
                }
            }
            //不能有重名的子库
            var sql = "select count(*) from {0} where 用户编号={1} and 子库名称='{2}'";
            sql = string.Format(sql, DbHelper.TnSubDb(), this.User.ID, databaseName);
            if (Convert.ToInt32(MySqlHelper.ExecuteScalar(DbHelper.ConnectionString, sql)) > 0)
            {
                MessageBox.Show("已经存在名为【" + databaseName + "】的子库，请换个名字");
                return;
            }

            #endregion
            var dataSource = (DataTable) dataGridView1.DataSource;
            var ans = DaoObject.SaveSubDatabase(userId, databaseName, dataSource);
            if (ans == 0)
            {
                MessageBox.Show("保存成功！");
            }
            else
            {
                MessageBox.Show("保存失败！错误代码：" + ans);
            }
        }

        #endregion

        #region 加、减查询条件的按钮操作

        /// <summary>
        /// 加参考地点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            TextBox textBox = new TextBox();
            textBox.Width = (int) (flowLayoutPanel1.Width * .8);
            textBox.Tag = Convert.ToInt32(flowLayoutPanel1.Tag) + 1;
            flowLayoutPanel1.Controls.Add(textBox);
            flowLayoutPanel1.Tag = textBox.Tag;
        }

        /// <summary>
        /// 减参考地点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanel1.Controls.Count > 0)
            {
                var controls = flowLayoutPanel1.Controls.Cast<Control>().ToList();
                var textBoxs = controls.FindAll(c => c.GetType() == typeof(TextBox));
                var textBox = textBoxs.Find(t => Convert.ToInt32(t.Tag) == Convert.ToInt32(flowLayoutPanel1.Tag));
                flowLayoutPanel1.Controls.Remove(textBox);
                flowLayoutPanel1.Tag = Convert.ToInt32(flowLayoutPanel1.Tag) - 1;
            }
        }

        /// <summary>
        /// 加圆域查询条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            CircleQuery circleQuery = new CircleQuery();
            circleQuery.Tag = Convert.ToInt32(flowLayoutPanel2.Tag) + 1;
            flowLayoutPanel2.Controls.Add(circleQuery);
            flowLayoutPanel2.Tag = circleQuery.Tag;
        }

        /// <summary>
        /// 减圆域查询条件
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
                    circleQueries.Find(cq => Convert.ToInt32(cq.Tag) == Convert.ToInt32(flowLayoutPanel2.Tag));
                flowLayoutPanel2.Controls.Remove(circleQuery);
                flowLayoutPanel2.Tag = Convert.ToInt32(flowLayoutPanel2.Tag) - 1;
            }
        }

        /// <summary>
        /// 加矩形域查询条件
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
        /// 减矩形域查询条件
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
                    rectQueries.Find(rq => Convert.ToInt32(rq.Tag) == Convert.ToInt32(flowLayoutPanel3.Tag));
                flowLayoutPanel3.Controls.Remove(circleQuery);
                flowLayoutPanel3.Tag = Convert.ToInt32(flowLayoutPanel3.Tag) - 1;
            }
        }

        #endregion

        #region 快查 、保存查询条件操作

        /// <summary>
        /// 快查按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            FrmQueryQuakeCmd frmQueryCmd = new FrmQueryQuakeCmd(this.User, QueryCmdAction.Use);
            frmQueryCmd.Owner = this;
            var dialogResult = frmQueryCmd.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                RefreshDataGridView(MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, frmQueryCmd.Command).Tables[0]);
            }
        }

        /// <summary>
        /// 保存查询条件 按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (this.SqlBuilder.ToString().Equals(""))
            {
                MessageBox.Show("请先进行查询再保存查询条件");
                return;
            }
            FrmQueryQuakeCmd frmQueryCmd = new FrmQueryQuakeCmd(this.User, QueryCmdAction.Save);
            frmQueryCmd.Owner = this;
            frmQueryCmd.ShowDialog();
        }

        #endregion

        #region DataGridView相关事件和右键菜单

        /// <summary>
        /// 使用新加入的数据刷新并绑定DataGridView
        /// </summary>
        /// <param name="dataTable"></param>
        private void RefreshDataGridView(DataTable dataTable)
        {
            if (!dataTable.Columns.Contains("选择"))
            {
                dataTable.Columns.Add(new DataColumn("选择", typeof(Boolean)));
                dataTable.Columns["选择"].SetOrdinal(0);
            }
            this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = dataTable;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            //调整列宽
            this.dataGridView1.Columns[0].FillWeight = 7; //选择列
            this.dataGridView1.Columns[1].FillWeight = 7; //发震日期
            //发震日期列的格式化字符串
            this.dataGridView1.Columns[1].DefaultCellStyle.Format = "yyyy/MM/dd";
            this.dataGridView1.Columns[2].FillWeight = 7; //发震时间
            this.dataGridView1.Columns[3].FillWeight = 4; //经度
            this.dataGridView1.Columns[4].FillWeight = 4; //纬度
            this.dataGridView1.Columns[5].FillWeight = 4; //震级单位
            this.dataGridView1.Columns[6].FillWeight = 4; //震级值
            this.dataGridView1.Columns[6].DefaultCellStyle.Format = "#0.0";
            this.dataGridView1.Columns[7].FillWeight = 7; //定位参数
            this.dataGridView1.Columns[8].FillWeight = 21; //参考地点

            foreach (DataGridViewColumn dataGridViewColumn in this.dataGridView1.Columns)
            {
                //除了选择列允许编辑外，其他列不允许编辑
                dataGridViewColumn.ReadOnly = !dataGridViewColumn.Name.Equals("选择");
            }
            //将所有数据选中
            for (var i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells["选择"].Value = true;
            }
        }

        /// <summary>
        /// 让单击到单元格内就可以选中或不选“选择”复选框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView1.DataSource == null || this.dataGridView1.Rows.Count == 0)
            {
                return;
            }
            //如果单击了“选择”列，且不是标题行，则将翻转选中状态
            //甲方嫌复选框太小了，让单击到单元格内就可以选中或不选复选框
            if (e.ColumnIndex == 0 && e.RowIndex > -1)
            {
                var cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                cell.Value = !Convert.ToBoolean(cell.Value);
                this.dataGridView1.RefreshEdit();
            }
        }

        /// <summary>
        /// 数据源不为null，则显示右键菜单，否则不显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //数据源不为null，则显示右键菜单，否则不显示
                if (this.dataGridView1.DataSource != null)
                {
                    this.contextMenuStrip1.Show(this.dataGridView1, e.X, e.Y);
                }
            }
        }

        /// <summary>
        /// 全选记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count > 0)
            {
                for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                {
                    this.dataGridView1.Rows[i].Cells[0].Value = true;
                }
                this.dataGridView1.RefreshEdit();
            }
        }

        /// <summary>
        /// 全不选记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count > 0)
            {
                for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                {
                    this.dataGridView1.Rows[i].Cells[0].Value = false;
                }
                this.dataGridView1.RefreshEdit();
            }
        }

        /// <summary>
        /// 反选记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count > 0)
            {
                if (this.dataGridView1.Columns.Contains("选择"))
                {
                    for (int i = 0; i < this.dataGridView1.RowCount; i++)
                    {
                        var cell = this.dataGridView1.Rows[i].Cells[0];
                        cell.Value = !Convert.ToBoolean(cell.Value);
                    }
                }
                this.dataGridView1.RefreshEdit();
            }
        }

        /// <summary>
        /// 全选高亮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                for (int i = 0; i < this.dataGridView1.SelectedRows.Count; i++)
                {
                    this.dataGridView1.SelectedRows[i].Cells[0].Value = true;
                }
                this.dataGridView1.RefreshEdit();
            }
        }

        /// <summary>
        /// 全不选高亮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                for (int i = 0; i < this.dataGridView1.SelectedRows.Count; i++)
                {
                    this.dataGridView1.SelectedRows[i].Cells[0].Value = false;
                }
                this.dataGridView1.RefreshEdit();
            }
        }

        /// <summary>
        /// 反选高亮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                if (this.dataGridView1.Columns.Contains("选择"))
                {
                    for (int i = 0; i < this.dataGridView1.SelectedRows.Count; i++)
                    {
                        var cell = this.dataGridView1.SelectedRows[i].Cells[0];
                        cell.Value = !Convert.ToBoolean(cell.Value);
                    }
                    this.dataGridView1.RefreshEdit();
                }
            }
        }

        #endregion
    }
}
