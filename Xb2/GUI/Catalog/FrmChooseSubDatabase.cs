using System;
using System.Linq;
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
            this.User = user;
        }

        private void FrmDelSubDatabase_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + "-[" + this.User.Name + "]";
            this.RefreshDataGridView();
        }

        private void RefreshDataGridView()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = DaoObject.GetLabelDatabaseInfosAll(this.User.ID);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns[0].Width = 40;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToOrderColumns = false;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        /// <summary>
        /// 双击选中的单元格表示选定当前标注库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.DataSource != null)
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    if (dataGridView1.SelectedRows.Count > 0)
                    {
                        var dbName = dataGridView1.SelectedRows[0].Cells["子库名称"].Value.ToString();
                        var type = dataGridView1.SelectedRows[0].Cells["类别"].Value.ToString();
                        this.DbNameAndType = dbName + "," + type;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
        }

    }
}
