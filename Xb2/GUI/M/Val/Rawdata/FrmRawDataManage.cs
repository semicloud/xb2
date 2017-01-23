using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using NLog;
using Xb2.Entity;
using Xb2.Entity.Business;
using Xb2.GUI.M.Item;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.GUI.M.Val.Rawdata
{
    public partial class FrmRawDataManage : FrmBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public FrmRawDataManage(XbUser user)
        {
            InitializeComponent();
            User = user;
        }

        #region 选测项菜单、清空测项菜单

        /// <summary>
        /// 选测项菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var form = new FrmSelectMItem(User);
            form.Owner = this;
            //选完项后加载选项结果
            if (form.ShowDialog() == DialogResult.OK)
            {
                var colNames = new[] {"编号", "观测单位", "地名", "方法名", "测项名"};
                listBox1.DataSource = null;
                listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
                listBox1.DataSource = form.Result.GetDataLine(",", colNames);
            }
        }

        /// <summary>
        /// 清空测项菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            listBox1.SelectedIndexChanged -= listBox1_SelectedIndexChanged;
            listBox1.DataSource = null;
        }

        #endregion

        #region 原始数据 新增、编辑、删除、导入菜单
        /// <summary>
        /// 新建原始数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedValue != null)
            {
                int itemId = GetCurrentMItemId();
                Logger.Info("用户 {0} 新建测项 {1} 的原始数据", this.User.ID, itemId);
                var form = new FrmEditRawData(Operation.Create, itemId);
                form.Owner = this;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        /// <summary>
        /// 编辑原始数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedValue != null)
            {
                if (dataGridView1.SelectedRows[0] != null)
                {
                    var index = dataGridView1.SelectedRows[0].Index;
                    var dataRow = ((DataTable) dataGridView1.DataSource).Rows[index];
                    var form = new FrmEditRawData(dataRow,Operation.Edit);
                    form.Owner = this;
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.ShowDialog();
                }
                else
                {
                    MessageBox.Show("请先选中原始数据");
                }
            }
        }

        /// <summary>
        /// 删除菜单，可多项删除原始数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            var itemId = GetCurrentMItemId();
            if (itemId != -1)
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    var rows = dataGridView1.SelectedRows.OfType<DataGridViewRow>();
                    var ids = rows.Select(r => Convert.ToInt32(r.Cells["编号"].Value)).ToList();
                    var answ = MessageBox.Show("确定删除" + ids.Count + "条数据吗？", "提问",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question);
                    if (answ == DialogResult.OK)
                    {
                        var sql = "select * from {0} where 测项编号={1}";
                        sql = string.Format(sql, DbHelper.TnRData(), itemId);
                        var adapter = new MySqlDataAdapter(sql, DbHelper.ConnectionString);
                        var builder = new MySqlCommandBuilder(adapter);
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        dt.PrimaryKey = new[] {dt.Columns["编号"]};
                        foreach (var id in ids)
                        {
                            dt.Rows.Find(id).Delete();
                        }
                        var n = adapter.Update(dt);
                        if (n > 0)
                        {
                            MessageBox.Show("数据已删除！");
                            RefreshRawData(itemId);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            var itemId = GetCurrentMItemId();
            if (itemId != -1)
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "txt文件(*.txt)|*.txt";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //打开数据查看窗口
                    var form = new FrmDisplayRawData(dialog.FileName) {Owner = this};
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        //获取数据查看窗口的数据，去掉“序号”列
                        var dataTable = DataTableHelper.UnIdentifyDataTable(form.DataTable);
                        //加入“测项编号”列，并填入测项编号
                        dataTable.Columns.Add("测项编号", typeof(int));
                        dataTable.FillColumn("测项编号", itemId);
                        //从数据库中查询到这个测项的数据
                        var dt = new DataTable();
                        var sql = "select * from {0} where 测项编号={1}";
                        sql = string.Format(sql, DbHelper.TnRData(), itemId);
                        var adapter = new MySqlDataAdapter(sql, DbHelper.ConnectionString);
                        var builder=  new MySqlCommandBuilder(adapter);
                        adapter.Fill(dt);
                        //数据表合并
                        dt.Merge(dataTable, true, MissingSchemaAction.Error);
                        //更新数据
                        var n = adapter.Update(dt);
                        if (n > 0)
                        {
                            MessageBox.Show("导入成功！");
                            RefreshRawData(itemId);
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 获得当前选中的测项的编号
        /// </summary>
        /// <returns></returns>
        private int GetCurrentMItemId()
        {
            if (listBox1.DataSource == null) return -1;
            var line = listBox1.SelectedValue.ToString();
            var id = Convert.ToInt32(line.Split(',')[0]);
            return id;
        }

        #region 绑定原始数据的DataGridView
        /// <summary>
        /// 使用测项编号刷新原始数据表
        /// </summary>
        /// <param name="itemId"></param>
        public void RefreshRawData(int itemId)
        {
            var dt = DaoObject.GetRawData(itemId);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = dt;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            if (dataGridView1.Columns["观测日期"] != null)
            {
                dataGridView1.Columns["观测日期"].DefaultCellStyle.Format = "yyyy/MM/dd";
            }
            //编号、测项编号列隐藏
            if (dataGridView1.Columns["编号"] != null && dataGridView1.Columns["测项编号"] != null)
            {
                dataGridView1.Columns["编号"].Visible = false;
                dataGridView1.Columns["测项编号"].Visible = false;
            }
            foreach (DataGridViewColumn dataGridViewCoumn in dataGridView1.Columns)
            {
                dataGridViewCoumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        #endregion

        #region 选中的要编辑的原始数据多于一行，则不允许编辑

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //选中的数据多于一行的话，则编辑菜单不可用
                toolStripMenuItem4.Enabled = !(dataGridView1.SelectedRows.Count > 1);
                contextMenuStrip2.Show(Cursor.Position);
            }
        }

        #endregion

        #region 单击列表菜单更换选定的测项

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var itemId = GetCurrentMItemId();
            if (itemId != -1)
            {
                RefreshRawData(itemId);
            }
            else
            {
                MessageBox.Show("未获取测项!");
            }
        }

        #endregion
    }
}
