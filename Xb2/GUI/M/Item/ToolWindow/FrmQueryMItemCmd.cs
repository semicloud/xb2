using System;
using System.Diagnostics;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Entity.Business;
using Xb2.GUI.Catalog;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.GUI.M.Item.ToolWindow
{
    public partial class FrmQueryMItemCmd : FrmBase
    {
        private QueryCmdAction _action;
        public string Command;

        public FrmQueryMItemCmd(XbUser user, QueryCmdAction action)
        {
            this.InitializeComponent();
            this.User = user;
            this._action = action;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
        }

        private void FrmQueryCmd_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + "[" + this.User.Name + "]";
            //如果是用来查询，则给GridView挂上鼠标双击的事件，直接双击则使用已保存的查询条件
            if (this._action == QueryCmdAction.Use)
            {
                this.dataGridView1.CellDoubleClick += this.dataGridView1_CellDoubleClick;
            }
            this.RefreshDataGridView();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            if (this.textBox1.Text.Trim().Equals(""))
            {
                MessageBox.Show("请输入查询名后再保存！");
                return;
            }
            if (this.Owner is FrmSelectMItem)
            {
                //获取查测项界面，获取当前界面的SQL命令，保存之
                var form = (FrmSelectMItem)this.Owner;
                var cmd = form.CommandText;
                var cmdName = this.textBox1.Text.Trim();
                var userId = this.User.ID;
                //命令名是否重复查询
                var sql = "select count(*) from {0} where 用户编号={1} and 命令名称='{2}'";
                sql = string.Format(sql, DbHelper.TnQMItem(), this.User.ID, cmdName);
                var n = Convert.ToInt32(MySqlHelper.ExecuteScalar(DbHelper.ConnectionString, sql));
                if (n > 0)
                {
                    MessageBox.Show("已存在名为【" + cmdName + "】的查询！");
                    return;
                }
                //保存查询条件
                if (this.SaveCmd(userId, cmdName, cmd))
                {
                    MessageBox.Show("保存成功！");
                    this.RefreshDataGridView();
                }
                else
                {
                    MessageBox.Show("保存失败！");
                }
            }
        }

        /// <summary>
        /// 更新DataGridView
        /// </summary>
        private void RefreshDataGridView()
        {
            var sql = "select 编号,命令名称,命令文本 from {0} where 用户编号={1}";
            sql = string.Format(sql, DbHelper.TnQMItem(), this.User.ID);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, sql).Tables[0];
            var identifiedTable = DataTableHelper.IdentifyDataTable(dt);
            this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = identifiedTable;
            this.dataGridView1.Columns["编号"].Visible = false;
            this.dataGridView1.Columns["命令文本"].Visible = false;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.dataGridView1.Columns[0].Width = 35;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        }

        //保存查询条件至数据库
        private bool SaveCmd(int userId, string cmdName, string cmd)
        {
            var sql = "select * from " + DbHelper.TnQMItem();
            var dataTable = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString,sql).Tables[0];
            var dataRow = dataTable.NewRow();
            dataRow["用户编号"] = userId;
            dataRow["命令名称"] = cmdName;
            dataRow["命令文本"] = cmd;
            dataTable.Rows.Add(dataRow);
            var adapter = new MySqlDataAdapter(sql, DbHelper.ConnectionString);
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

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    var id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["编号"].Value);
                    var confirm = MessageBox.Show("确定删除该查询吗？" + id, "提问", 
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (confirm == DialogResult.OK)
                    {
                        var sql = "delete from {0} where 编号={1} and 用户编号={2}";
                        sql = string.Format(sql, DbHelper.TnQMItem(), id, User.ID);
                        var n = MySqlHelper.ExecuteNonQuery(DbHelper.ConnectionString, sql);
                        Debug.Print("sql:{0},returns:{1}", sql, n);
                        RefreshDataGridView();
                    }
                }
            }
        }
    }
}
