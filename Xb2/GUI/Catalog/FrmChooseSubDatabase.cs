using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Entity.Business;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.GUI.Catalog
{
    public partial class FrmChooseSubDatabase : FrmBase
    {
        /// <summary>
        /// 选中的数据 的 名称和类型
        /// </summary>
        public string DbNameAndType { get; set; }
        public FrmChooseSubDatabase(XbUser user)
        {
            InitializeComponent();
            this.CUser = user;
        }

        private void FrmDelSubDatabase_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + "-[" + this.CUser.Name + "]";
            this.RefreshDataGridView();
        }

        private void RefreshDataGridView()
        {
            var sql = "select 子库名称 from 系统_地震目录子库 where 用户编号=" + this.CUser.ID;
            var dtZK = MySqlHelper.ExecuteDataset(Db.CStr(), sql).Tables[0];
            dtZK.Columns.Add("类别", typeof(string));
            dtZK.FillColumn("类别", "子库");
            //应甲方要求，将标注库也放在标注库里
            sql = "select 标注库名称 from 系统_地震目录标注库 where 用户编号=" + this.CUser.ID;
            var dtBZK = MySqlHelper.ExecuteDataset(Db.CStr(), sql).Tables[0];
            for (int i = 0; i < dtBZK.Rows.Count; i++)
            {
                var row = dtZK.NewRow();
                row["子库名称"] = dtBZK.Rows[i]["标注库名称"].ToString();
                row["类别"] = "标注库";
                dtZK.Rows.Add(row);
            }
            //地震目录也放在子库里，即用户可以选择从直接从地震目录中生成子库
            var dataRow = dtZK.NewRow();
            dataRow["子库名称"] = Db.TnCategory();
            dataRow["类别"] = "地震目录";
            dtZK.Rows.InsertAt(dataRow, 0);
            //增加序号列
            dtZK = DataHelper.IdentifyDataTable(dtZK);
            this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = dtZK;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.dataGridView1.Columns[0].Width = 40;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var dgv = this.dataGridView1;
            if (dgv.DataSource != null)
            {
                if (dgv.Rows.Count > 0)
                {
                    if (dgv.SelectedRows.Count > 0)
                    {
                        var dbName = dgv.SelectedRows[0].Cells["子库名称"].Value.ToString();
                        var type = dgv.SelectedRows[0].Cells["类别"].Value.ToString();
                        this.DbNameAndType = dbName + "," + type;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
        }

    }
}
