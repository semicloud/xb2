using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using NLog;
using Xb2.Entity;
using Xb2.Entity.Business;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;
using ExtendMethodDataTable = Xb2.Utils.ExtendMethod.ExtendMethodDataTable;

namespace Xb2.GUI.Catalog
{
    public partial class FrmEditLabelDatabase : FrmBase
    {
        /// <summary>
        /// 选定进行标地震的地震目录
        /// </summary>
        public DataTable SelectedCategories { get; private set; }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public FrmEditLabelDatabase(XbUser user)
        {
            InitializeComponent();
            this.User = user;
        }

        private void FrmEditLabelDatabase_Load(object sender, EventArgs e)
        {
            // 根据用户编号查询地震标注库信息
            var dt = DaoObject.GetLabelDatabasesInfo(this.User.ID);
            RefreshDataGridView1(ExtendMethodDataTable.IdentifyDataTable(dt));
        }

        #region DataGridView相关

        /// <summary>
        /// 选中主表的行同时显示从表的数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView1.DataSource != null)
            {
                Logger.Info("RowIndex:" + e.RowIndex);
                Logger.Info("Column Count:" + dataGridView1.Columns.Count);
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    Logger.Info("Column:{0}, Value:{1}", dataGridView1.Columns[i].HeaderText
                        , dataGridView1.Rows[e.RowIndex].Cells[i].Value);
                }
                //点击行标题不触发该事件
                if (this.dataGridView1.Rows.Count > 0 && e.RowIndex >= 0)
                {
                    var databaseName = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                    var databaseId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value);
                    Logger.Info("选定的标注库编号：{0}，标注库名称：{1}", databaseId, databaseName);
                    var dt = DaoObject.GetCategoriesFromLabelDatabase(this.User.ID, databaseId, databaseName);
                    RefreshDataGridView2(dt);
                }
            }
        }

        /// <summary>
        /// 更新主表
        /// </summary>
        /// <param name="dataTable"></param>
        private void RefreshDataGridView1(DataTable dataTable)
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = dataTable;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AllowUserToOrderColumns = false;
            if (dataGridView1.Columns["编号"] != null)
            {
                dataGridView1.Columns["编号"].Visible = false;
            }
            if (dataGridView1.Columns["用户编号"] != null)
            {
                dataGridView1.Columns["用户编号"].Visible = false;
            }
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dataGridView1.Columns[0].Width = 40;

        }

        /// <summary>
        /// 更新从表
        /// </summary>
        /// <param name="dataTable"></param>
        private void RefreshDataGridView2(DataTable dataTable)
        {
            dataGridView2.DataSource = null;
            dataGridView2.DataSource = dataTable;
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.AllowUserToResizeRows = false;
            dataGridView2.AllowUserToResizeColumns = false;
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView2.MultiSelect = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.Columns[0].Width = 40;
            dataGridView2.RowsDefaultCellStyle.SelectionBackColor = Color.Green;
            //隐藏编号列
            if (dataGridView2.Columns["编号"] != null)
            {
                dataGridView2.Columns["编号"].Visible = false;
            }
            //震级值格式化显示
            if (dataGridView2.Columns["震级值"] != null)
            {
                this.dataGridView2.Columns["震级值"].DefaultCellStyle.Format = "0.0";
            }
            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        #endregion

        #region 主表的ContextMenuStrip，包括删除标注库、标地震、选定该标注库等

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
                sql = string.Format(sql, DaoObject.TnLabelDb(), User.ID, dbname);
                var n = MySqlHelper.ExecuteNonQuery(DaoObject.ConnectionString, sql);
                if (n > 0)
                {
                    MessageBox.Show("删除成功！");
                    // 删除成功后需要重新绑定数据，以表明数据已删除
                    var dt = DaoObject.GetLabelDatabasesInfo(this.User.ID);
                    RefreshDataGridView1(ExtendMethodDataTable.IdentifyDataTable(dt));
                    // 从表数据源设为空
                    RefreshDataGridView2(new DataTable());
                }
            }
        }

        /// <summary>
        /// 标地震菜单项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 序列图菜单项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 选定该标注库菜单项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 选定该标注库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView2.DataSource != null)
            {
                this.SelectedCategories = null;
                this.SelectedCategories = (DataTable) dataGridView2.DataSource;
                this.Close();
            }
        }

        #endregion

        #region 从表的ContextMenuStrip，包括加记录，改记录，删记录3个功能

        /// <summary>
        /// 加记录菜单项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            #region 输入验证

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

            #endregion
            var dataTable = (DataTable) this.dataGridView2.DataSource;
            var databaseName = dataGridView1.SelectedRows[0].Cells["标注库名称"].Value.ToString();
            var databaseId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["编号"].Value);
            var frmCreateEditRecord = new FrmCreateEditRecord(dataTable.NewRow(), databaseName, Operation.Create, User)
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            var dialogResult = frmCreateEditRecord.ShowDialog();
            // 加记录完成后更新从表
            if (dialogResult == DialogResult.OK)
            {
                var dt = DaoObject.GetCategoriesFromLabelDatabase(this.User.ID, databaseId, databaseName);
                RefreshDataGridView2(dt);
            }
        }

        /// <summary>
        /// 改记录菜单项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            #region 输入验证

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

            #endregion
            var dataTable = (DataTable) this.dataGridView2.DataSource;
            var databseName = dataGridView1.SelectedRows[0].Cells["标注库名称"].Value.ToString();
            var databaseId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["编号"].Value);
            var selectedRow = dataTable.Rows[this.dataGridView2.SelectedRows[0].Index];
            if (selectedRow != null)
            {
                var frmCreateEditRecord = new FrmCreateEditRecord(selectedRow, databseName, Operation.Edit, User)
                {
                    StartPosition = FormStartPosition.CenterScreen
                };
                var dialogResult = frmCreateEditRecord.ShowDialog();
                // 改记录完成后更新从表
                if (dialogResult == DialogResult.OK)
                {
                    var dt = DaoObject.GetCategoriesFromLabelDatabase(this.User.ID, databaseId, databseName);
                    RefreshDataGridView2(dt);
                }
            }
            else
            {
                MessageBox.Show("未选择地震目录！", "错误");
            }
        }

        /// <summary>
        /// 删记录菜单项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            #region 输入验证
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

            var databaseName = dataGridView1.SelectedRows[0].Cells["标注库名称"].Value.ToString();
            var databaseId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["编号"].Value);
            var ans = MessageBox.Show("确定删除本条地震目录吗？", "提问", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);
            if (ans == DialogResult.OK)
            {
                var recordId = Convert.ToInt32(this.dataGridView2.SelectedRows[0].Cells["编号"].Value);
                if (DaoObject.DeleteLabelDatabaseRecord(recordId))
                {
                    var dt = DaoObject.GetCategoriesFromLabelDatabase(this.User.ID, databaseId, databaseName);
                    RefreshDataGridView2(dt);
                }
            }
        }

        #endregion
    }
}
