using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Entity.Business;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;
using MBtns = System.Windows.Forms.MessageBoxButtons;
using MIcons = System.Windows.Forms.MessageBoxIcon;

namespace Xb2.GUI.Catalog
{
    public partial class FrmDelSubDatabase : FrmBase
    {
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
            var sql = "select 子库名称 from {0} where 用户编号={1}";
            sql = string.Format(sql, DbHelper.TnSubDb(), User.ID);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, sql).Tables[0];
            dt = DataTableHelper.IdentifyDataTable(dt);
            this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = dt;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.dataGridView1.Columns[0].Width = 40;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.MultiSelect = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dgv = this.dataGridView1;
            if (dgv.Rows.Count <= 0) return;
            if (dgv.SelectedRows.Count == 0) return;
            var ans = MessageBox.Show("确定删除吗？", "提问", MBtns.OKCancel, MIcons.Question);
            if (ans == DialogResult.OK)
            {
                var subDbName = dgv.SelectedRows[0].Cells["子库名称"].Value.ToString();
                var sql = "delete from {0} where 用户编号={1} and 子库名称='{2}'";
                sql = string.Format(sql,DbHelper.TnSubDb(), User.ID, subDbName);
                var n = MySqlHelper.ExecuteNonQuery(DbHelper.ConnectionString, sql) ;
                if (n >= 0)
                {
                    MessageBox.Show("删除成功！");
                    RefreshDataGridView();
                }
            }
        }
    }
}
