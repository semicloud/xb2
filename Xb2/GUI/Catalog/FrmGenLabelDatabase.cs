using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Entity.Business;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.GUI.Catalog
{
    public partial class FrmGenLabelDatabase : FrmBase
    {
        private string _selectedDbName;

        public FrmGenLabelDatabase(XbUser user)
        {
            InitializeComponent();
            this.CUser = user;
            this._selectedDbName = string.Empty;
        }

        /// <summary>
        /// 将DataTable的选择列反选
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private DataTable InverseChooseState(DataTable dataTable)
        {
            if (!dataTable.Columns.Contains("选择"))
            {
                throw new Exception("无有选择列呀~");
            }
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                var isSelected = Convert.ToInt32(dataTable.Rows[i]["选择"]);
                if (isSelected == 1)
                {
                    dataTable.Rows[i]["选择"] = 0;
                }
                else if (isSelected == 0)
                {
                    dataTable.Rows[i]["选择"] = 1;
                }
            }
            return dataTable;
        }

        //筛选按钮
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.DataSource == null || this.dataGridView1.Rows.Count <= 0)
            {
                MessageBox.Show("请先加子库，然后进行筛选！");
                return;
            }
            var dateTimeLower = this.dateTimePicker1.Value;
            var dateTimeUpper = this.dateTimePicker2.Value;
            var magnitudeLower = Convert.ToSingle(this.textBox1.Text.Trim());
            var magnitudeUpper = Convert.ToSingle(this.textBox2.Text.Trim());
            var dataTable = (DataTable) this.dataGridView1.DataSource;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                var date = Convert.ToDateTime(dataTable.Rows[i]["发震日期"]);
                var magnitude = Convert.ToSingle(dataTable.Rows[i]["震级值"]);
                if (date >= dateTimeLower && date <= dateTimeUpper)
                {
                    if (magnitude >= magnitudeLower && magnitude <= magnitudeUpper)
                    {
                        dataTable.Rows[i]["选择"] = 1;
                    }
                    else
                    {
                        dataTable.Rows[i]["选择"] = 0;
                    }
                }
                else
                {
                    dataTable.Rows[i]["选择"] = 0;
                }
            }
            RefreshDataGridView(dataTable);
        }

        //反选按钮
        private void button8_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.DataSource == null)
            {
                MessageBox.Show("请先加载子库！");
                return;
            }
            if (this.dataGridView1.Rows.Count <= 0)
            {
                MessageBox.Show("请先加载子库！");
                return;
            }
            var dataTable = (DataTable) this.dataGridView1.DataSource;
            RefreshDataGridView(this.InverseChooseState(dataTable));
        }

        private void FrmGenSubDatabase_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + "-[" + this.CUser.Name + "]";
        }

        //刷新datagrdiview
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
            this.dataGridView1.MultiSelect = false;
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
        }

        //将查询到的且用户选中的记录添加入架构表内，以支持adapter的操作
        private DataTable ImportSelectedRecords(DataTable schema)
        {
            DataTable dataTable = (DataTable) dataGridView1.DataSource;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                //找到选中的数据行
                if (Convert.ToInt32(dataTable.Rows[i]["选择"]) == 1)
                {
                    DataRow dataRow = schema.NewRow();
                    var date = Convert.ToDateTime(dataTable.Rows[i]["发震日期"]);
                    var time = DateTime.ParseExact(dataTable.Rows[i]["发震时间"].ToString(), "HH:mm:ss",
                        CultureInfo.InvariantCulture);
                    dataRow["发震日期"] = Convert.ToDateTime(dataTable.Rows[i]["发震日期"]);
                    dataRow["发震时间"] = new DateTime(date.Year, date.Month, date.Day, time.Hour,
                        time.Minute, time.Second);
                    dataRow["纬度"] = Convert.ToInt32(dataTable.Rows[i]["纬度"]);
                    dataRow["经度"] = Convert.ToInt32(dataTable.Rows[i]["经度"]);
                    dataRow["震级单位"] = Convert.ToString(dataTable.Rows[i]["震级单位"]);
                    dataRow["震级值"] = Convert.ToSingle(dataTable.Rows[i]["震级值"]);
                    dataRow["定位参数"] = Convert.ToInt32(dataTable.Rows[i]["定位参数"]);
                    dataRow["参考地点"] = Convert.ToString(dataTable.Rows[i]["参考地点"]);
                    schema.Rows.Add(dataRow);
                }
            }
            return schema;
        }

        //生成按钮
        private void button3_Click(object sender, EventArgs e)
        {
            var labelDbName = textBox3.Text.Trim();

            #region 输入合法性检查

            if (this.dataGridView1.DataSource == null)
            {
                MessageBox.Show("请先加载子库！");
                return;
            }
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("请先加载子库！");
                return;
            }
            //需要至少选中一条地震目录
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
                    MessageBox.Show("你没有选中任何地震目录，无法生成标注库！");
                    return;
                }
            }
            //需要输入标注库名
            if (textBox3.Text.Trim().Equals(""))
            {
                MessageBox.Show("请先输入标注库名称再生成！");
                return;
            }
            //不能创建同名的标注库
            if (DaoObject.GetLabelDbId(labelDbName,CUser.ID) != -1)
            {
                MessageBox.Show("已经存在名称为【" + labelDbName + "】的标注库了！");
                return;
            }

            #endregion
            //新建标注库记录
            var sql = "insert into {0}(用户编号,标注库名称) values ({1},'{2}')";
            sql = string.Format(sql, DbHelper.TnLabelDb(), CUser.ID, labelDbName);
            var ans = MySqlHelper.ExecuteNonQuery(DbHelper.ConnectionString(), sql);
            //标注库记录新建成功之后，获取标注库编号，然后向标注库数据表中插入地震目录，标注库创建完成
            if (ans > 0)
            {
                var id = DaoObject.GetLabelDbId(labelDbName, CUser.ID);
                sql = "select * from {0} where 标注库编号={1}";
                sql = string.Format(sql, DbHelper.TnLabelDbData(), id);
                var dt = new DataTable();
                var adapter = new MySqlDataAdapter(sql, DbHelper.ConnectionString());
                adapter.Fill(dt);
                var builder = new MySqlCommandBuilder(adapter);
                dt = LoadSelectedRecords(id, dt);
                int n = adapter.Update(dt);
                if (n > 0)
                {
                    MessageBox.Show("保存成功！");
                }
            }
        }

        /// <summary>
        /// 将查询到的且用户选中的记录添加入架构表内，以支持adapter的操作
        /// </summary>
        /// <param name="labelDbId">标注库编号</param>
        /// <param name="dt">标注库内容</param>
        /// <returns></returns>
        private DataTable LoadSelectedRecords(int labelDbId, DataTable dt)
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
                    var timeStr = dataTable.Rows[i]["发震时间"].ToString();
                    var time = DateTime.ParseExact(timeStr, "HH:mm:ss", CultureInfo.InvariantCulture);
                    dr["标注库编号"] = labelDbId; //important!
                    dr["发震日期"] = Convert.ToDateTime(dataTable.Rows[i]["发震日期"]);
                    dr["发震时间"] = MergeDateTime(date, time);
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

        /// <summary>
        /// 分别取两个日期类型的日期和时间
        /// 凑成一个新的日期
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private DateTime MergeDateTime(DateTime date, DateTime time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hour,
                time.Minute, time.Second);
        }

        /// <summary>
        /// 根据子库名称和子库类型获取用于生成标注库的地震目录
        /// </summary>
        /// <param name="dbName">子库名</param>
        /// <param name="type">子库类型</param>
        /// <returns></returns>
        private DataTable GetData(string dbName, string type)
        {
            var dt = new DataTable();
            var sql = string.Empty;
            //使用内连接查询子库数据
            if (type.Equals("子库"))
            {
                sql = "select date(发震日期) as 发震日期,time(发震时间) as 发震时间,"
                      + "经度,纬度,震级单位,round(震级值,1) as 震级值,定位参数,参考地点"
                      + " from {0} inner join {1} on {0}.编号={1}.子库编号"
                      + " where 子库名称='{2}' and 用户编号={3}";
                sql = string.Format(sql, DbHelper.TnSubDb(), DbHelper.TnSubDbData(), dbName, CUser.ID);
                Debug.Print(sql);
            }
            //使用内连接查询标注库数据
            if (type.Equals("标注库"))
            {
                sql = "select date(发震日期) as 发震日期,time(发震时间) as 发震时间,"
                      + "经度,纬度,震级单位,round(震级值,1) as 震级值,定位参数,参考地点"
                      + " from {0} inner join {1} on {0}.编号={1}.标注库编号"
                      + " where 标注库名称='{2}' and 用户编号={3}";
                sql = string.Format(sql, DbHelper.TnLabelDb(), DbHelper.TnLabelDbData(), dbName, CUser.ID);
                Debug.Print(sql);
            }
            if (type.Equals("地震目录"))
            {
                //直接从地震目录中查询出记录
                sql = "select date(发震日期) as 发震日期,time(发震时间) as 发震时间,"
                      + "经度,纬度,震级单位,round(震级值,1) as 震级值,定位参数,参考地点 from {0}";
                sql = string.Format(sql, DbHelper.TnCategory());
            }
            Debug.Print(sql);
            dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString(), sql).Tables[0];
            return dt;
        }

        //加子库
        private void button5_Click(object sender, EventArgs e)
        {
            var form = new FrmChooseSubDatabase(this.CUser);
            form.Owner = this;
            var dialogResult = form.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                this.label7.Text = form.DbNameAndType;
                //Name and Type
                var dbName = form.DbNameAndType.Split(',')[0];
                var dbType = form.DbNameAndType.Split(',')[1];
                var dt = GetData(dbName, dbType);
                //查询出来的记录默认选中
                dt = DataHelper.BuildChooseColumn(dt);
                RefreshDataGridView(dt);
                //从查询出的数据中找到日期、震级的最大最小值
                var minDate = Convert.ToDateTime(dt.AsEnumerable().Min(r => r["发震日期"]));
                var maxDate = Convert.ToDateTime(dt.AsEnumerable().Max(r => r["发震日期"]));
                var minMag = Convert.ToSingle(dt.AsEnumerable().Min(r => r["震级值"]));
                var maxMag = Convert.ToSingle(dt.AsEnumerable().Max(r => r["震级值"]));
                this.dateTimePicker1.Value = minDate;
                this.dateTimePicker2.Value = maxDate;
                this.textBox1.Text = minMag.ToString();
                this.textBox2.Text = maxMag.ToString();
            }
        }

        //标地震（箭头图）
        private void button6_Click(object sender, EventArgs e)
        {
            ShowChartForm(ChartType.ArrowChart);
        }

        //标地震（序列图）
        private void button7_Click(object sender, EventArgs e)
        {
            ShowChartForm(ChartType.SequenceChart);
        }

        /// <summary>
        /// 显示标地震图窗口，有两种类型的标地震图：箭头图和序列图
        /// </summary>
        /// <param name="chartType"></param>
        private void ShowChartForm(ChartType chartType)
        {
            if (this.dataGridView1.DataSource == null)
            {
                MessageBox.Show("没有查询结果，无法绘制！");
                return;
            }
            if (this.dataGridView1.Rows.Count <= 0)
            {
                MessageBox.Show("没有查询结果，无法绘制！");
                return;
            }
            var dataTable = (DataTable) this.dataGridView1.DataSource;
            if (this.dataGridView1.Rows.Count > 0)
            {
                int selectedRowsCount = 0;
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dataTable.Rows[i]["选择"]) == 1)
                    {
                        selectedRowsCount++;
                    }
                }
                if (selectedRowsCount == 0)
                {
                    MessageBox.Show("没有选中的地震目录，无法绘制！");
                    return;
                }
            }
            //上面都是输入验证
            
            FrmShowLabelChart frmShowLabelChart = null;
            if (chartType == ChartType.ArrowChart)
            {
                frmShowLabelChart = new FrmShowLabelChart(dataTable.Copy(), ChartType.ArrowChart);
            }
            else if (chartType == ChartType.SequenceChart)
            {
                frmShowLabelChart = new FrmShowLabelChart(dataTable.Copy(), ChartType.SequenceChart);
            }
            if (frmShowLabelChart != null)
            {
                frmShowLabelChart.Owner = this;
                frmShowLabelChart.ShowDialog();
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


    }
}
