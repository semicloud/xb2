using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using NLog;
using Xb2.Entity.Business;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.GUI.Catalog
{
    public partial class FrmGenLabelDatabase : FrmBase
    {
        private string m_selectedDatabaseName; //选中的地震目录标注库
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public FrmGenLabelDatabase(XbUser user)
        {
            InitializeComponent();
            this.User = user;
            this.m_selectedDatabaseName = string.Empty;
        }

        private void FrmGenSubDatabase_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + "-[" + this.User.Name + "]";
        }

        #region 筛选按钮事件及相关操作
        /// <summary>
        /// 筛选按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource == null || dataGridView1.Rows.Count <= 0)
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

        #endregion

        #region 生成标注库按钮

        /// <summary>
        /// 生成地震目录标注库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            var databaseName = textBox3.Text.Trim();
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
                    MessageBox.Show("没有选中任何地震目录，无法生成标注库！");
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
            if (DaoObject.GetLabelDbId(databaseName,User.ID) != -1)
            {
                MessageBox.Show("已经存在名称为【" + databaseName + "】的标注库了！");
                return;
            }

            #endregion
            var ans = DaoObject.SaveLabelDatabase(this.User.ID, databaseName, (DataTable) dataGridView1.DataSource);
            if (ans == 0)
            {
                MessageBox.Show("保存成功！");
            }
            else
            {
                MessageBox.Show("保存地震目录标注库失败！");
            }
        }

        #endregion

        #region 工具按钮区：加子库、标地震、反选、序列图

        /// <summary>
        /// 根据子库名称和子库类型获取用于生成标注库的地震目录
        /// 子库类型包括：
        /// 1. 地震目录总库
        /// 2. 地震目录子库
        /// 3. 地震目录标注库
        /// </summary>
        /// <param name="databaseName">子库名</param>
        /// <param name="databaseType">子库类型</param>
        /// <returns></returns>
        private DataTable GetCategories(string databaseName, string databaseType)
        {
            var commandText = string.Empty;
            //使用内连接查询子库数据
            if (databaseType.Equals("子库"))
            {
                commandText = "select date(发震日期) as 发震日期,time(发震时间) as 发震时间,"
                              + "经度,纬度,震级单位,round(震级值,1) as 震级值,定位参数,参考地点"
                              + " from {0} inner join {1} on {0}.编号={1}.子库编号"
                              + " where 子库名称='{2}' and 用户编号={3}";
                commandText = string.Format(commandText, DbHelper.TnSubDb(), DbHelper.TnSubDbData(), databaseName, User.ID);
            }
            //使用内连接查询标注库数据
            if (databaseType.Equals("标注库"))
            {
                commandText = "select date(发震日期) as 发震日期,time(发震时间) as 发震时间,"
                              + "经度,纬度,震级单位,round(震级值,1) as 震级值,定位参数,参考地点"
                              + " from {0} inner join {1} on {0}.编号={1}.标注库编号"
                              + " where 标注库名称='{2}' and 用户编号={3}";
                commandText = string.Format(commandText, DbHelper.TnLabelDb(), DbHelper.TnLabelDbData(), databaseName, User.ID);
            }
            if (databaseType.Equals("地震目录"))
            {
                //直接从地震目录中查询出记录
                commandText = "select date(发震日期) as 发震日期,time(发震时间) as 发震时间,"
                              + "经度,纬度,震级单位,round(震级值,1) as 震级值,定位参数,参考地点 from {0}";
                commandText = string.Format(commandText, DbHelper.TnCategory());
            }
            Logger.Debug(commandText);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
            Logger.Info("从 {0} 中查询 地震目录 {1}，返回 {2} 条记录", databaseType, databaseName, dt.Rows.Count);
            return dt;
        }

        /// <summary>
        /// 显示标地震图窗口，有两种类型的标地震图：箭头图和序列图
        /// </summary>
        /// <param name="chartType"></param>
        private void ShowChartForm(ChartType chartType)
        {
            #region 输入验证

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
            var dataTable = (DataTable)this.dataGridView1.DataSource;
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

            #endregion
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

        /// <summary>
        /// 加子库按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            // 打开选择子库界面
            var frmChooseSubDatabase = new FrmChooseSubDatabase(this.User) {Owner = this};
            var dialogResult = frmChooseSubDatabase.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                this.label7.Text = frmChooseSubDatabase.DbNameAndType;
                Logger.Info("DbNameAndType:" + frmChooseSubDatabase.DbNameAndType);
                var databaseName = frmChooseSubDatabase.DbNameAndType.Split(',')[0];
                var databaseType = frmChooseSubDatabase.DbNameAndType.Split(',')[1];
                Logger.Info("选定的数据库名：{0}，类型：{1}", databaseName, databaseType);
                var dt = GetCategories(databaseName, databaseType);
                //查询出来的记录默认选中
                dt = DataTableHelper.BuildChooseColumn(dt);
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

        /// <summary>
        /// 标地震（箭头图）按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            ShowChartForm(ChartType.ArrowChart);
        }

        /// <summary>
        /// 标地震（序列图）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            ShowChartForm(ChartType.SequenceChart);
        }

        /// <summary>
        /// 反选按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource == null)
            {
                MessageBox.Show("请先加载子库！");
                return;
            }
            if (dataGridView1.Rows.Count <= 0)
            {
                MessageBox.Show("请先加载子库！");
                return;
            }
            var dataTable = (DataTable) this.dataGridView1.DataSource;
            RefreshDataGridView(this.InverseChooseState(dataTable));
        }

        #endregion

        #region DataGridView相关事件

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
        /// 刷新datagrdiview
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
            // DataGridView的列禁止排序
            foreach (DataGridViewColumn dataGridViewColumn in dataGridView1.Columns)
            {
                dataGridViewColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        #endregion
    }
}
