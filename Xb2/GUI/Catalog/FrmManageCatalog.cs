using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Config;
using Xb2.Entity.Business;
using Xb2.Entity.Business.Catalog;
using Xb2.GUI.Main;
using Xb2.Utils.Database;

namespace Xb2.GUI.Catalog
{
    public partial class FrmManageCatalog : FrmBase
    {
        private static List<Q01File> _Q01Files = new List<Q01File>();

        public FrmManageCatalog(XbUser user)
        {
            InitializeComponent();
            this.CUser = user;
        }

        private void RefreshDataGridView()
        {
            String sql = "select 编号,开始日期,结束日期,文件名,记录数,加入时间,用户,已导入数据库 as 导入 from q01文件";
            DataTable dataTable = MySqlHelper.ExecuteDataset(Db.CStr(), sql).Tables[0];
            this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = dataTable;
            this.dataGridView1.ContextMenuStrip = contextMenuStrip1;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            this.dataGridView1.Columns["编号"].FillWeight = 3;
            this.dataGridView1.Columns["开始日期"].FillWeight = 5;
            this.dataGridView1.Columns["结束日期"].FillWeight = 5;
            this.dataGridView1.Columns["文件名"].FillWeight = 15;
            this.dataGridView1.Columns["记录数"].FillWeight = 3;
            this.dataGridView1.Columns["加入时间"].FillWeight = 5;
            this.dataGridView1.Columns["用户"].FillWeight = 3;
            this.dataGridView1.Columns["导入"].FillWeight = 3;


            this.dataGridView1.Columns["编号"].ReadOnly = true;
            this.dataGridView1.Columns["开始日期"].ReadOnly = true;
            this.dataGridView1.Columns["结束日期"].ReadOnly = true;
            this.dataGridView1.Columns["记录数"].ReadOnly = true;
            this.dataGridView1.Columns["加入时间"].ReadOnly = true;
            this.dataGridView1.Columns["用户"].ReadOnly = true;
            this.dataGridView1.Columns["导入"].ReadOnly = true;
        }

        private void FrmQ01Managing_Load(object sender, EventArgs e)
        {
            RefreshDataGridView();
        }

        public void CopyQ01FileToApp()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "*.q01";
            openFileDialog.Filter = "q01文件(*.q01)|*.q01";
            openFileDialog.InitialDirectory = "E:\\";
            openFileDialog.Title = "选择q01文件";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.CheckFileExists)
                {
                    var fileNames = openFileDialog.FileNames.ToList();
                    var copied = fileNames.TrueForAll(s => Q01File.AddQ01File(s, this.CUser));
                    if (copied)
                    {
                        StringBuilder sb = new StringBuilder("以下q01文件：\n");
                        fileNames.ForEach(fn => sb.AppendLine("\t-" + fn));
                        sb.AppendLine("已加入系统...");
                        MessageBox.Show(sb.ToString());
                        RefreshDataGridView();
                    }
                }
            }
        }

        private void 添加Q01文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyQ01FileToApp();
        }

        //改名
        public void EditQ01FileName()
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                this.dataGridView1.CurrentCell = this.dataGridView1.SelectedRows[0].Cells["文件名"];
                this.dataGridView1.BeginEdit(true);
            }
            else
            {
                MessageBox.Show("请先选中一行再改名！");
            }
        }

        //Q01文件导入到数据库
        public void ImportQ01FileToDb()
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                if (Convert.ToBoolean(this.dataGridView1.SelectedRows[0].Cells["已导入数据库"].Value))
                {
                    MessageBox.Show("该文件已导入数据库，无法再次导入！");
                    return;
                }
                var fileName = this.dataGridView1.SelectedRows[0].Cells["文件名"].Value.ToString();
                var dialogResult = MessageBox.Show("确定将【" + fileName + "】导入数据库中吗？", "提问", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question);
                if (dialogResult != DialogResult.Cancel)
                {
                    var isImported = Q01File.ImportToDatabase(fileName);
                    if (isImported)
                    {
                        MessageBox.Show("导入成功！");
                        RefreshDataGridView();
                    }
                }
            }
            else
            {
                MessageBox.Show("请先选中一行再导入！");
            }
        }

        //删除Q01文件
        internal void DeleteQ01File()
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                String fileName = this.dataGridView1.SelectedRows[0].Cells["文件名"].Value.ToString();
                var dialogResult = MessageBox.Show("确定要在系统中删除【" + fileName + "】文件吗？", "提问",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.OK)
                {
                    bool isOK = Q01File.DeleteQ01File(fileName);
                    if (isOK)
                    {
                        MessageBox.Show("已删除文件【" + fileName + "】！");
                        this.RefreshDataGridView();
                    }
                }
            }
            else
            {
                MessageBox.Show("请先选中一行再删除！");
            }
        }

        //从数据库中卸载Q01文件（的记录）
        public void UnimportQ01FileFromDb()
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                if (Convert.ToBoolean(this.dataGridView1.SelectedRows[0].Cells["已导入数据库"].Value))
                {
                    String fileName = this.dataGridView1.SelectedRows[0].Cells["文件名"].Value.ToString();
                    var dialogResult = MessageBox.Show("确定将【" + fileName + "】卸载吗？",
                        "提问", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        var isOK = Q01File.UnimportQ01RecordFromDb(fileName);
                        if (isOK)
                        {
                            MessageBox.Show("已成功从数据库中卸载！");
                            this.RefreshDataGridView();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("该Q01文件尚未导入数据库，无法卸载！");
                }
            }
            else
            {
                MessageBox.Show("请先选中一行再删除！");
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        private string _editingFileName;
        private string _editedFileName;

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            _editingFileName = this.dataGridView1.CurrentCell.Value.ToString();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            _editedFileName = this.dataGridView1.CurrentCell.Value.ToString();
            if (_editedFileName != _editingFileName)
            {
                var dialogResult = MessageBox.Show("确定将【" + _editingFileName + "】改名为【" + _editedFileName + "】吗？",
                    "提问", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.No)
                {
                    this.dataGridView1.CurrentCell.Value = _editingFileName;
                    return;
                }
                if (dialogResult == DialogResult.Yes)
                {
                    Int32 id = Int32.Parse(this.dataGridView1.CurrentRow.Cells[0].Value.ToString());
                    bool isChanged = Q01File.ChangeQ01FileName(id, _editingFileName, _editedFileName);
                    if (isChanged)
                    {
                        MessageBox.Show("完成！");
                    }
                }
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
                return;
            //GetMainForm().toolStripButton7.Enabled = true;
        }

       
       
    }
}
