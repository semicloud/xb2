using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using NLog;
using Xb2.Entity.Business;
using Xb2.GUI.Main;
using Xb2.Utils.Database;

namespace Xb2.GUI.Catalog
{
    public partial class FrmDelSubDatabase : FrmBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public FrmDelSubDatabase(XbUser user)
        {
            InitializeComponent();
            this.User = user;
        }

        private void FrmDelSubDatabase_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + "-[" + this.User.Name + "]";
            this.RefreshDataGridView();
        }

        private void RefreshDataGridView()
        {
            //var sql = "select 子库名称 from {0} where 用户编号={1}";
            //sql = string.Format(sql, DbHelper.TnSubDb(), User.ID);
            //var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, sql).Tables[0];
            //dt = DataTableHelper.IdentifyDataTable(dt);

            this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = DaoObject.GetSubDatabaseInfos(this.User.ID);
            if (this.dataGridView1.Columns["编号"]!=null)
            {
                this.dataGridView1.Columns["编号"].Visible = false;
            }
            if (this.dataGridView1.Columns["用户编号"] != null)
            {
                this.dataGridView1.Columns["用户编号"].Visible = false;
            }
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.dataGridView1.Columns[0].Width = 40;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.MultiSelect = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count <= 0) return;
            if (dataGridView1.SelectedRows.Count == 0) return;
            var ans = MessageBox.Show("确定删除吗？", "提问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (ans == DialogResult.OK)
            {
                var databaseName = dataGridView1.SelectedRows[0].Cells["子库名称"].Value.ToString();
                var databaseId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["编号"].Value);
                Logger.Info("用户要删除的地震目录子库名称：{0}，编号：{1}", databaseName, databaseId);
                var result = DaoObject.DeleteSubDatabase(this.User.ID, databaseId, databaseName);
                if (result)
                {
                    MessageBox.Show("删除成功！");
                    RefreshDataGridView();
                }
            }
        }
    }
}
