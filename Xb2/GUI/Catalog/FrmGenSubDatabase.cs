using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Config;
using Xb2.Entity.Business;
using Xb2.GUI.Controls;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.GUI.Catalog
{
    public partial class FrmGenSubDatabase : FrmBase
    {
        private string baseSQL = "select date(发震日期) as 发震日期,time(发震时间) as 发震时间,"
                                 + "经度,纬度,震级单位,round(震级值,1) as 震级值,定位参数,参考地点 from 系统_地震目录 where ";
        private StringBuilder _sb;

        public StringBuilder SqlBuilder
        {
            get { return this._sb; }
        }

        public FrmGenSubDatabase(XbUser user)
        {
            InitializeComponent();
            this.CUser = user;
            this._sb = new StringBuilder();
        }

        //查询按钮
        private void button1_Click(object sender, EventArgs e)
        {
            DateTime start = this.dateTimePicker1.Value;
            DateTime end = this.dateTimePicker2.Value;
            double mlow = Convert.ToDouble(this.textBox1.Text.Trim());
            double mhigh = Convert.ToDouble(this.textBox2.Text.Trim());

            if (flowLayoutPanel1.GetTextBoxs().Any(t => t.Text.Trim().Equals("")))
            {
                MessageBox.Show("参考地点输入不完整！");
                return;
            }

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
            _sb = new StringBuilder(this.baseSQL);
            _sb.AppendFormat("(发震日期 between \'{0}\' and \'{1}\' ) ", start.SStr(), end.SStr());
            _sb.AppendFormat("and (震级值 between {0} and {1}) ", mlow, mhigh);
            _sb.Append(GetLocationNameClause());
            _sb.Append(GetCircleQueryClause());
            _sb.Append(GetRectQueryClause());
            _sb.AppendLine("order by 发震日期, 参考地点");
            var qsql = _sb.ToString();
            var dt = MySqlHelper.ExecuteDataset(Xb2Config.GetConnStr(), qsql).Tables[0];
            RefreshDataGridView(dt);
        }

        //获取矩形查询表达式
        private string GetRectQueryClause()
        {
            var rqueries = flowLayoutPanel3.Controls.Cast<Control>()
                .ToList()
                .FindAll(c => c.GetType() == typeof(RectQuery))
                .Cast<RectQuery>()
                .ToList();
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
                            builder.AppendFormat("or (in_rect({0},{1},{2},{3},经度,纬度) ", lng1, lat1, lng2, lat2);
                            continue;
                        }
                        builder.AppendFormat("and (in_rect({0},{1},{2},{3},经度,纬度) ", lng1, lat1, lng2, lat2);
                        continue;
                    }
                    if (i == rqueries.Count - 1)
                    {
                        builder.AppendFormat("or in_rect({0},{1},{2},{3},经度,纬度)) ", lng1, lat1, lng2, lat2);
                        break;
                    }
                    builder.AppendFormat("or in_rect({0},{1},{2},{3},经度,纬度) ", lng1, lat1, lng2, lat2);
                }
            }
            return builder.ToString();
        }

        //获取圆查询条件表达式
        private string GetCircleQueryClause()
        {
            var cqueries =
                flowLayoutPanel2.Controls.Cast<Control>()
                    .ToList()
                    .FindAll(c => c.GetType() == typeof(CircleQuery))
                    .Cast<CircleQuery>()
                    .ToList();
            var builder = new StringBuilder();
            if (cqueries.Count > 0)
            {
                if (cqueries.Count == 1)
                {
                    return string.Format("and (in_circle({0},{1},经度,纬度,{2})) ", cqueries[0].Lng, cqueries[0].Lat,
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

        //获得所有的查询地名
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

        private void FrmGenSubDatabase_Load(object sender, EventArgs e)
        {
            var sql =
                "select min(发震日期) as mindate ,max(发震日期) as maxdate,min(震级值) as minm,max(震级值) as maxm from 系统_地震目录";
            var dataTable = MySqlHelper.ExecuteDataset(Xb2Config.GetConnStr(), sql).Tables[0];
            if (dataTable.Rows.Count > 0)
            {
                this.dateTimePicker1.Value = Convert.ToDateTime(dataTable.Rows[0]["mindate"]);
                this.dateTimePicker2.Value = Convert.ToDateTime(dataTable.Rows[0]["maxdate"]);
                this.textBox1.Text = dataTable.Rows[0]["minm"].ToString();
                this.textBox2.Text = dataTable.Rows[0]["maxm"].ToString();
            }
            this.Text = this.Text + "[" + this.CUser.Name + "]";
        }

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
            this.dataGridView1.Columns[0].FillWeight = 7;//选择列
            this.dataGridView1.Columns[1].FillWeight = 7;//发震日期
            //发震日期列的格式化字符串
            this.dataGridView1.Columns[1].DefaultCellStyle.Format = "yyyy/MM/dd";
            this.dataGridView1.Columns[2].FillWeight = 7;//发震时间
            this.dataGridView1.Columns[3].FillWeight = 4;//经度
            this.dataGridView1.Columns[4].FillWeight = 4;//纬度
            this.dataGridView1.Columns[5].FillWeight = 4;//震级单位
            this.dataGridView1.Columns[6].FillWeight = 4;//震级值
            this.dataGridView1.Columns[6].DefaultCellStyle.Format = "#0.0";
            this.dataGridView1.Columns[7].FillWeight = 7;//定位参数
            this.dataGridView1.Columns[8].FillWeight = 21;//参考地点

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
        /// 将查询到的且用户选中的记录添加入架构表内，以支持adapter的操作
        /// </summary>
        /// <param name="subDbId">子库编号</param>
        /// <param name="dt">子库内容</param>
        /// <returns></returns>
        private DataTable LoadSelectedRecords(int subDbId,DataTable dt)
        {
            DataTable dataTable = (DataTable) dataGridView1.DataSource;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                //找到选中的数据行
                if (Convert.ToInt32(dataTable.Rows[i]["选择"]) == 1)
                {
                    DataRow dr = dt.NewRow();
                    //由于从数据库中截取了发震时间的时间部分，所有在这里得把
                    //日期补上，才能重新填入数据库中
                    var date = Convert.ToDateTime(dataTable.Rows[i]["发震日期"]);
                    var time = DateTime.ParseExact(dataTable.Rows[i]["发震时间"].ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);
                    dr["子库编号"] = subDbId; //important!
                    dr["发震日期"] = Convert.ToDateTime(dataTable.Rows[i]["发震日期"]);
                    dr["发震时间"] = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
                    dr["纬度"] = Convert.ToInt32(dataTable.Rows[i]["纬度"]);
                    dr["经度"] = Convert.ToInt32(dataTable.Rows[i]["经度"]);
                    dr["震级单位"] = Convert.ToString(dataTable.Rows[i]["震级单位"]);
                    dr["震级值"] = Convert.ToSingle(dataTable.Rows[i]["震级值"]);
                    dr["定位参数"] = Convert.ToInt32(dataTable.Rows[i]["定位参数"]);
                    dr["参考地点"] = Convert.ToString(dataTable.Rows[i]["参考地点"]);
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            TextBox textBox = new TextBox();
            textBox.Width = (int) (flowLayoutPanel1.Width*.8);
            textBox.Tag = Convert.ToInt32(flowLayoutPanel1.Tag) + 1;
            flowLayoutPanel1.Controls.Add(textBox);
            flowLayoutPanel1.Tag = textBox.Tag;
        }

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

        private void button8_Click(object sender, EventArgs e)
        {
            CircleQuery circleQuery = new CircleQuery();
            circleQuery.Tag = Convert.ToInt32(flowLayoutPanel2.Tag) + 1;
            flowLayoutPanel2.Controls.Add(circleQuery);
            flowLayoutPanel2.Tag = circleQuery.Tag;
        }

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

        private void button10_Click(object sender, EventArgs e)
        {
            RectQuery rectQuery = new RectQuery();
            rectQuery.Tag = Convert.ToInt32(flowLayoutPanel3.Tag) + 1;
            flowLayoutPanel3.Controls.Add(rectQuery);
            flowLayoutPanel3.Tag = rectQuery.Tag;
        }

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

        /// <summary>
        /// 保存子库信息
        /// </summary>
        /// <param name="subDbName">标注库名</param>
        /// <returns></returns>
        private bool SaveSubDbInfo(string subDbName)
        {
            var sql = "insert into {0}(用户编号,子库名称) values ({1},'{2}')";
            sql = string.Format(sql, DbHelper.TnSubDb(), this.CUser.ID, subDbName);
            return MySqlHelper.ExecuteNonQuery(DbHelper.ConnectionString(), sql) > 0;
        }

        /// <summary>
        /// 获取新建的子库编号
        /// </summary>
        /// <param name="subDbName">子库名</param>
        /// <returns></returns>
        private int GetSubDbID(string subDbName)
        {
            //表中可以有重名的子库，只要用户Id不一样就行了
            var sql = "select 编号 from {0} where 用户编号={1} and 子库名称='{2}'";
            sql = string.Format(sql, DbHelper.TnSubDb(), this.CUser.ID, subDbName);
            var ret = MySqlHelper.ExecuteScalar(DbHelper.ConnectionString(), sql);
            if (ret != null) return Convert.ToInt32(ret);
            return -1;
        }

        /// <summary>
        /// 保存子库数据
        /// </summary>
        /// <param name="subDbId">子库编号</param>
        /// <returns></returns>
        private bool SaveSubDbData(int subDbId)
        {
            var dt = new DataTable();
            var sql = "select * from {0} where 子库编号={1}";
            sql = string.Format(sql, DbHelper.TnSubDbData(), subDbId);
            var adapter = new MySqlDataAdapter(sql, DbHelper.ConnectionString());
            var builder = new MySqlCommandBuilder(adapter);
            adapter.Fill(dt);
            dt = LoadSelectedRecords(subDbId, dt);
            var n = adapter.Update(dt);
            return n > 0;
        }

        //生成子库 按钮
        private void button3_Click(object sender, EventArgs e)
        {
            var subDbName = textBox3.Text.Trim();
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
            if (subDbName.Equals(""))
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
            sql = string.Format(sql, DbHelper.TnSubDb(), this.CUser.ID, subDbName);
            if (Convert.ToInt32(MySqlHelper.ExecuteScalar(DbHelper.ConnectionString(), sql)) > 0)
            {
                MessageBox.Show("已经存在名为【" + subDbName + "】的子库，请换个名字");
                return;
            }
            #endregion
            var isSaveDbInfo = SaveSubDbInfo(subDbName);
            if (isSaveDbInfo)
            {
                var subDbId = GetSubDbID(subDbName);
                if (subDbId > 0)
                {
                    var isDataSaved = SaveSubDbData(subDbId);
                    if (isDataSaved)
                    {
                        MessageBox.Show("保存成功！");
                    }
                }
            }
        }

        //保存查询条件 按钮
        private void button4_Click(object sender, EventArgs e)
        {
            if (this.SqlBuilder.ToString().Equals(""))
            {
                MessageBox.Show("请先进行查询再保存查询条件");
                return;
            }
            FrmQueryQuakeCmd frmQueryCmd = new FrmQueryQuakeCmd(this.CUser, QueryCmdAction.Save);
            frmQueryCmd.Owner = this;
            frmQueryCmd.ShowDialog();
        }

        //快查按钮
        private void button2_Click(object sender, EventArgs e)
        {
            FrmQueryQuakeCmd frmQueryCmd = new FrmQueryQuakeCmd(this.CUser, QueryCmdAction.Use);
            frmQueryCmd.Owner = this;
            var dialogResult = frmQueryCmd.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                RefreshDataGridView(MySqlHelper.ExecuteDataset(DbHelper.ConnectionString(), frmQueryCmd.Command).Tables[0]);
            }
        }

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

    }
}
