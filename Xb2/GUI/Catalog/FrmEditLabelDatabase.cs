using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Entity.Business;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.GUI.Catalog
{
    public partial class FrmEditLabelDatabase : FrmBase
    {
        //SQL语句太长了，放在这里当个模板吧，这样对各个列的处理也方便
        /// <summary>
        /// base sql
        /// </summary>
        private readonly string bsql =
            "select 编号, date(发震日期) as 发震日期,time(发震时间) as 发震时间,经度,纬度,震级单位,round(震级值,1) as 震级值,定位参数,参考地点 from {0}";

        public FrmEditLabelDatabase(XbUser user)
        {
            InitializeComponent();
            this.User = user;
        }

        private void FrmEditLabelDatabase_Load(object sender, EventArgs e)
        {
            var dataTable = GetLabelDbInfo();
            RefreshDataGridView1(DataTableHelper.IdentifyDataTable(dataTable));
        }

        /// <summary>
        /// 更新主表
        /// </summary>
        /// <param name="dataTable"></param>
        private void RefreshDataGridView1(DataTable dataTable)
        {
            this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = dataTable;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.Columns[0].Width = 40;
        }

        /// <summary>
        /// 更新从表
        /// </summary>
        /// <param name="dataTable"></param>
        private void RefreshDataGridView2(DataTable dataTable)
        {
            this.dataGridView2.DataSource = null;
            this.dataGridView2.DataSource = dataTable;
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.AllowUserToResizeRows = false;
            this.dataGridView2.AllowUserToResizeColumns = false;
            this.dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView2.MultiSelect = false;
            this.dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView2.Columns[0].Width = 40;
            //隐藏编号列
            dataGridView2.Columns["编号"].Visible = false;
            //震级值格式化显示
            this.dataGridView2.Columns["震级值"].DefaultCellStyle.Format = "0.0";
        }

        /// <summary>
        /// 根据标注库名和用户编号获取标注库数据
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        private DataTable GetLabelDbData(string dbName)
        {
            var labelDbId = DaoObject.GetLabelDbId(dbName, this.User.ID);
            var sql = string.Format(bsql, DbHelper.TnLabelDbData()) + " where 标注库编号=" + labelDbId;
            Debug.Print(sql);
            return MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, sql).Tables[0];
        }

        /// <summary>
        /// 根据用户名获取标注库名
        /// </summary>
        /// <returns></returns>
        private DataTable GetLabelDbInfo()
        {
            this.Text = this.Text + "-[" + this.User.Name + "]";
            var sql = "select 标注库名称 from {0} " + "where 用户编号={1}";
            sql = string.Format(sql, DbHelper.TnLabelDb(), User.ID);
            Debug.Print(sql);
            return MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, sql).Tables[0];
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView1.DataSource != null)
            {
                //点击行标题不触发该事件
                if (this.dataGridView1.Rows.Count > 0 && e.RowIndex >= 0)
                {
                    var dgv = this.dataGridView1;
                    var labelDbName = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    var labelDbId = DaoObject.GetLabelDbId(labelDbName, User.ID);
                    var sql = string.Format(bsql, DbHelper.TnLabelDbData()) + " where 标注库编号=" + labelDbId;
                    Debug.Print(sql);
                    var dataTable = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, sql).Tables[0];
                    RefreshDataGridView2(dataTable);
                }
            }
        }

        /// <summary>
        /// 选定进行标地震的地震目录
        /// </summary>
        public DataTable ConfirmedDataTable { get; private set; }

        private void 选定该标注库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView2.DataSource != null)
            {
                this.ConfirmedDataTable = null;
                this.ConfirmedDataTable = (DataTable) dataGridView2.DataSource;
                this.Close();
            }
        }

        /// <summary>
        /// 删除标注库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.DataSource == null || this.dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择一个标注库再删除！");
                return;
            }
            var dbname = dataGridView1.SelectedRows[0].Cells["标注库名称"].Value.ToString();
            var ans = MessageBox.Show("确定删除标注库【" + dbname + "】吗？", "提问",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (ans == DialogResult.OK)
            {
                var sql = "delete from {0} where 用户编号={1} and 标注库名称='{2}'";
                sql = string.Format(sql, DbHelper.TnLabelDb(), User.ID, dbname);
                var n = MySqlHelper.ExecuteNonQuery(DbHelper.ConnectionString, sql);
                if (n > 0)
                {
                    MessageBox.Show("删除成功！");
                    //删除成功后需要重新绑定数据，以表明数据已删除
                    var dataTable = GetLabelDbInfo();
                    RefreshDataGridView1(DataTableHelper.IdentifyDataTable(dataTable));
                    //从表数据源设为空，
                    RefreshDataGridView2(new DataTable());
                }
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            #region 输入验证
            if (this.dataGridView1.DataSource == null || this.dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择一个标注库！");
                return;
            }
            if (this.dataGridView2.DataSource == null)
            {
                MessageBox.Show("无地震目录！");
                return;
            }
            #endregion
            var dataTable = ((DataTable) this.dataGridView2.DataSource).Copy();
            dataTable.Columns.Add(new DataColumn("选择", typeof(Int32)));
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                dataTable.Rows[i]["选择"] = 1;
            }
            var frmShowLabelChart = new FrmShowLabelChart(dataTable, ChartType.ArrowChart);
            frmShowLabelChart.ShowDialog();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.DataSource == null || this.dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择一个标注库！");
                return;
            }
            if (this.dataGridView2.DataSource == null)
            {
                MessageBox.Show("无地震目录！");
                return;
            }
            var dataTable = ((DataTable) this.dataGridView2.DataSource).Copy();
            dataTable.Columns.Add(new DataColumn("选择", typeof(Int32)));
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                dataTable.Rows[i]["选择"] = 1;
            }
            var frmShowLabelChart = new FrmShowLabelChart(dataTable, ChartType.SequenceChart);
            frmShowLabelChart.ShowDialog();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.DataSource == null)
            {
                MessageBox.Show("请先选定一个标注库！");
                return;
            }
            if (this.dataGridView1.DataSource == null)
            {
                MessageBox.Show("请先选定一个标注库！");
                return;
            }
            if (this.dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选定一个标注库！");
                return;
            }
            var dataTable = (DataTable) this.dataGridView2.DataSource;
            var dbName = dataGridView1.SelectedRows[0].Cells["标注库名称"].Value.ToString();
            var form = new FrmCreateEditRecord(dataTable.NewRow(), dbName, Operation.Create, User);
            form.StartPosition = FormStartPosition.CenterScreen;
            var dialogResult = form.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                var dt = GetLabelDbData(dbName);
                RefreshDataGridView2(dt);
            }
        }

        //改地震目录
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.DataSource == null)
            {
                MessageBox.Show("请先选定一个标注库！");
                return;
            }
            if (this.dataGridView1.DataSource == null)
            {
                MessageBox.Show("请先选定一个标注库！");
                return;
            }
            if (this.dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选定一个标注库！");
                return;
            }
            if (this.dataGridView2.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选中一条地震目录再修改！");
                return;
            }
            var dataTable = (DataTable) this.dataGridView2.DataSource;
            var dbname =
                this.dataGridView1.SelectedRows[0].Cells["标注库名称"].Value.ToString();
            var selectedRow = dataTable.Rows[this.dataGridView2.SelectedRows[0].Index];
            if (selectedRow != null)
            {
                var form = new FrmCreateEditRecord(selectedRow, dbname, Operation.Edit, User);
                form.StartPosition = FormStartPosition.CenterScreen;
                var dialogResult = form.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    var dt = GetLabelDbData(dbname);
                    RefreshDataGridView2(dt);
                }
            }
            else
            {
                MessageBox.Show("未选择地震目录！", "错误");
            }
        }

        //删地震目录
        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            #region 删除验证
            if (this.dataGridView2.DataSource == null)
            {
                MessageBox.Show("请先选定一个标注库！");
                return;
            }
            if (this.dataGridView1.DataSource == null)
            {
                MessageBox.Show("请先选定一个标注库！");
                return;
            }
            if (this.dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选定一个标注库！");
                return;
            }
            if (this.dataGridView2.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选中一条地震目录再删除！");
                return;
            }
            #endregion
            
            var dbname = dataGridView1.SelectedRows[0].Cells["标注库名称"].Value.ToString();
            var ans = MessageBox.Show("确定删除本条地震目录吗？", "提问", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);
            if (ans == DialogResult.OK)
            {
                var id = Convert.ToInt32(this.dataGridView2.SelectedRows[0].Cells["编号"].Value);
                var sql = "delete from {0} where 编号={1}";
                sql = string.Format(sql,DbHelper.TnLabelDbData(), id);
                Debug.Print(sql);
                if (MySqlHelper.ExecuteNonQuery(DbHelper.ConnectionString, sql) > 0)
                {
                    var dt = GetLabelDbData(dbname);
                    RefreshDataGridView2(dt);
                }
            }
        }
    }
}
