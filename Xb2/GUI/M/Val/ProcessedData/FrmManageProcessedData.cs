using System;
using System.Diagnostics;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Entity.Business;
using Xb2.GUI.M.Item;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.GUI.M.Val.ProcessedData
{
    public partial class FrmManageProcessedData : FrmBase
    {
        public FrmManageProcessedData(XbUser user)
        {
            InitializeComponent();
            CUser = user;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var form = new FrmSelectMItem(this.CUser);
            form.Owner = this;
            //选完项后加载选项结果
            if (form.ShowDialog() == DialogResult.OK)
            {
                var colNames = new[] { "编号", "观测单位", "地名", "方法名", "测项名" };
                listBox1.DataSource = null;
                listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
                listBox1.DataSource = form.Result.GetDataLine(",", colNames);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var itemID = GetCurrentMItemID();
            if (itemID != -1)
            {
                RefreshDataGridView1(itemID);
            }
            else
            {
                MessageBox.Show("未获取测项!");
            }
        }

        private void RefreshDataGridView1(int itemId)
        {
            var sql = "select {1}.编号,{0}.编号 as 测项编号,{0}.观测单位,{0}.地名,{0}.测项名,{0}.方法名,{1}.库名 as 基础数据库名,{1}.是否默认"
                      + " from {0} inner join {1} on {0}.编号={1}.测项编号 where {1}.用户编号={2} and 测项编号={3}";
            sql = string.Format(sql, Db.TnMItem(), Db.TnProcessedDb(), CUser.ID, itemId);
            Debug.Print(sql);
            var dt = MySqlHelper.ExecuteDataset(Db.CStr(), sql).Tables[0];
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
            dataGridView1.Columns["是否默认"].FillWeight = 4;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (column.Name != "是否默认")
                {
                    column.ReadOnly = true;
                }
            }
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        /// <summary>
        /// 获得当前选中的测项的编号
        /// </summary>
        /// <returns></returns>
        private int GetCurrentMItemID()
        {
            if (listBox1.DataSource == null) return -1;
            var line = listBox1.SelectedValue.ToString();
            var id = Convert.ToInt32(line.Split(',')[0]);
            return id;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            listBox1.SelectedIndexChanged -= listBox1_SelectedIndexChanged;
            listBox1.DataSource = null;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //如果单击了是否默认列，则更改并保存默认列
            if (e.RowIndex > -1 && dataGridView1.Columns[e.ColumnIndex].HeaderText == "是否默认")
            {
                dataGridView1.Rows[e.RowIndex].Selected = true;
                var dbId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["编号"].Value);
                var isChanged = ChangeDefaultDatabase(dbId);
                if (isChanged)
                {
                    MessageBox.Show("更改默认基础数据成功！已保存！");
                    RefreshDataGridView1(GetCurrentMItemID());
                }
                else
                {
                    MessageBox.Show("更改默认基础数据出现问题！");
                }
            }
            if (e.RowIndex > -1 && dataGridView1.Columns[e.ColumnIndex].HeaderText != "是否默认")
            {
                dataGridView1.Rows[e.RowIndex].Selected = true;
                //MessageBox.Show("选中了！");
                //TODO Start at Here!
                Debug.Print("Run Here!");
            }
        }

        private bool ChangeDefaultDatabase(int dbId)
        {
            var itemId = GetCurrentMItemID();
            var userId = CUser.ID;
            Debug.Print("更改基础数据库{0}为默认基础数据库，测项编号{1}，用户编号{2}", dbId, itemId, userId);
            var sql = "update {0} set 是否默认=0 where 用户编号={1} and 测项编号={2}";
            sql = string.Format(sql, Db.TnProcessedDb(), userId, itemId);
            var isRemoveDefualt = MySqlHelper.ExecuteNonQuery(Db.CStr(), sql) > 0;
            Debug.Print("清除默认信息：" + isRemoveDefualt);
            sql = string.Format("update {0} set 是否默认=1 where 编号={1}", Db.TnProcessedDb(), dbId);
            var isUpdated = MySqlHelper.ExecuteNonQuery(Db.CStr(), sql) > 0;
            Debug.Print("更新默认基础数据：" + isUpdated);
            return isUpdated;
        }
    }
}
