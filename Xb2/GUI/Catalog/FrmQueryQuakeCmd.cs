using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Entity.Business;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;
using ExtendMethodDataTable = Xb2.Utils.ExtendMethod.ExtendMethodDataTable;

namespace Xb2.GUI.Catalog
{
    public partial class FrmQueryQuakeCmd : FrmBase
    {
        private QueryCmdAction _action;
        public string Command;

        public FrmQueryQuakeCmd(XbUser user, QueryCmdAction action)
        {
            InitializeComponent();
            this.User = user;
            this._action = action;
        }

        private void FrmQueryCmd_Load(object sender, System.EventArgs e)
        {
            this.Text = this.Text + "[" + this.User.Name + "]";
            //如果是用来查询，则给GridView挂上鼠标双击的事件，直接双击则使用已保存的查询条件
            if (this._action == QueryCmdAction.Use)
            {
                this.dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            }

            RefreshDataGridView();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            if (this.textBox1.Text.Trim().Equals(""))
            {
                MessageBox.Show("请输入查询名后再保存！");
                return;
            }
            if (this.Owner is FrmGenSubDatabase)
            {
                var frmGenSubDatabase = (FrmGenSubDatabase) this.Owner;
                var cmd = frmGenSubDatabase.SqlBuilder.ToString();
                var cmdName = this.textBox1.Text.Trim();
                var userId = this.User.ID;
                if (SaveCmd(userId, cmdName, cmd))
                {
                    MessageBox.Show("保存成功！");
                    RefreshDataGridView();
                }
                else
                {
                    MessageBox.Show("保存失败！");
                }
            }
        }

        private void RefreshDataGridView()
        {
            var sql = "select 命令名称,命令文本 from {0} where 用户编号={1}";
            sql = string.Format(sql, DaoObject.TnQCategory(), this.User.ID);
            var dt = MySqlHelper.ExecuteDataset(DaoObject.ConnectionString, sql).Tables[0];
            var identifiedTable = ExtendMethodDataTable.IdentifyDataTable(dt);
            this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = identifiedTable;
            this.dataGridView1.Columns["命令文本"].Visible = false;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.dataGridView1.Columns[0].Width = 35;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        }

        //保存查询条件至数据库
        private bool SaveCmd(int userId, string cmdName, string cmd)
        {
            var sql = "select * from " + DaoObject.TnQCategory();
            var dataTable = MySqlHelper.ExecuteDataset(DaoObject.ConnectionString,sql).Tables[0];
            var dataRow = dataTable.NewRow();
            dataRow["用户编号"] = userId;
            dataRow["命令名称"] = cmdName;
            dataRow["命令文本"] = cmd;
            dataTable.Rows.Add(dataRow);
            var adapter = new MySqlDataAdapter(sql, DaoObject.ConnectionString);
            var commandBuilder = new MySqlCommandBuilder(adapter);
            return adapter.Update(dataTable) > 0;
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                this.Command = dataGridView1.Rows[e.RowIndex].Cells["命令文本"].Value.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }

    public enum QueryCmdAction
    {
        Save,
        Use,
    }
}
