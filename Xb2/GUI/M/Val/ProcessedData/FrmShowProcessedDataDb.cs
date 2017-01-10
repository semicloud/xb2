using System;
using System.Diagnostics;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Entity.Business;
using Xb2.GUI.Main;
using Xb2.Utils.Database;

namespace Xb2.GUI.M.Val.ProcessedData
{
    /// <summary>
    /// 展示用户的基础数据库列表
    /// </summary>
    public partial class FrmShowProcessedDataDb : FrmBase
    {
        /// <summary>
        /// 选择的基础数据库Id
        /// </summary>
        public int DbId { get; private set; }
        /// <summary>
        /// 基础数据库对应的测项Id
        /// </summary>
        public int ItemId { get; private set; }

        public FrmShowProcessedDataDb(XbUser user)
        {
            InitializeComponent();
            this.User = user;
            this.DbId = this.ItemId = -1;
        }

        private void RefreshDataGridView()
        {
            var sql = "select {1}.编号,{0}.编号 as 测项编号,{0}.观测单位,{0}.地名,{0}.测项名,{0}.方法名,{1}.库名 as 基础数据库名"
                      + " from {0} inner join {1} on {0}.编号={1}.测项编号 where {1}.用户编号={2}";
            sql = string.Format(sql, DbHelper.TnMItem(), DbHelper.TnProcessedDb(), User.ID);
            Debug.Print(sql);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, sql).Tables[0];
            dataGridView1.DataSource = dt;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToOrderColumns = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Columns["编号"].FillWeight = 2;
            dataGridView1.Columns["测项编号"].FillWeight = 4;
            dataGridView1.Columns["观测单位"].FillWeight = 4;
            dataGridView1.Columns["地名"].FillWeight = 4;
            dataGridView1.Columns["测项名"].FillWeight = 4;
            dataGridView1.Columns["方法名"].FillWeight = 4;
            dataGridView1.Columns["基础数据库名"].FillWeight = 20;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void FrmShowProcessedDataDb_Load(object sender, System.EventArgs e)
        {
            RefreshDataGridView();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //双击标题列不返回
            if (e.RowIndex > -1)
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    this.DbId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["编号"].Value);
                    this.ItemId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["测项编号"].Value);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

    }
}
