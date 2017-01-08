using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Entity.Business;
using Xb2.GUI.Catalog;
using Xb2.GUI.Controls;
using Xb2.GUI.M.Item.ToolWindow;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;
using XbApp.View.M.Item.ToolWindow;

namespace Xb2.GUI.M.Item
{
    public partial class FrmSelectMItem : FrmBase
    {
        /// <summary>
        /// 查询SQL
        /// </summary>
        public string SQL { get; set; }

        /// <summary>
        /// 字段查询控件(FlowlayoutPanel)距离FlowLayout的距离
        /// </summary>
        private readonly int _distToFlowLayoutPanel = 10;

        /// <summary>
        /// 视图名称，对测项的区域查询需要用到视图
        /// </summary>
        private string _viewName;

        /// <summary>
        /// 选到的测项
        /// </summary>
        public DataTable Result { get; private set; }

        public FrmSelectMItem(XbUser user)
        {
            this.InitializeComponent();
            this.SQL = string.Empty;
            this.CUser = user;
            //默认视图为查询测项总表
            this._viewName = DbHelper.TnMItem();
        }

        private void FrmSelectMItem_Load(object sender, System.EventArgs e)
        {
        }

        /// <summary>
        /// 用该SQL刷新窗体中的数据
        /// </summary>
        /// <param name="sql"></param>
        public void RefreshData(string sql)
        {
            //什么也没查
            if (sql.Equals(string.Empty))
            {
                //用空测项表填充datagridview
                this.SQL = String.Empty;
                //从数据视图中查询数据
                sql = "select * from " + this._viewName;
                var emptyDt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString(), sql).Tables[0].Clone();
                this.RefreshDataGridView(emptyDt);
                return;
            }
            var fieldNames = new[] {"观测单位", "地名", "测项名", "方法名", "断层走向"};
            //这里重新生成一个带排序的SQL语句，因为在下面需要使用不带排序的sql
            var sortSql = sql + " order by 观测单位,地名,方法名";
            //用sortSql更新DataGridView
            var dataTable = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString(), sortSql).Tables[0];
            this.SQL = sql;
            this.RefreshDataGridView(dataTable);
            //SQL语句中已经查询的字段集合，该字段的Checkboxlist就不再更新了
            var queriedFieldNames = fieldNames.ToList().FindAll(fn => sql.Contains(fn));
            if (queriedFieldNames.Count > 0)
            {
                //这些已经有得查询字段名就不再更新checkboxlist了
                Debug.Print("already has field:" + string.Join(",", queriedFieldNames));
                if (this.flowLayoutPanel1.Controls.Count > 0)
                {
                    for (int i = 0; i < this.flowLayoutPanel1.Controls.Count; i++)
                    {
                        if (this.flowLayoutPanel1.Controls[i] is CtnlMFieldSelect)
                        {
                            var control = (CtnlMFieldSelect) this.flowLayoutPanel1.Controls[i];
                            //根据SQL语句，更新那些还未查询的字段的值
                            if (!queriedFieldNames.Contains(control.FieldName))
                            {
                                //在该SQL的基础上，生成该字段内容的SQL语句
                                //将select * from...中的*用distinct 字段名代替
                                var fsql = sql.Replace("*",
                                    "distinct " + control.FieldName + " as " + control.FieldName);
                                //将查询内容绑定到CheckBoxList上
                                control.RefreshCheckBoxList(fsql);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 用dataTable刷新DataGridView
        /// </summary>
        /// <param name="dataTable"></param>
        public void RefreshDataGridView(DataTable dataTable)
        {
            this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = DataHelper.BuildChooseColumn(dataTable);
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.MultiSelect = true;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AllowUserToOrderColumns = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            //按照单元格的内容决定列的宽度，最后一列填充DataGridView
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.Columns[this.dataGridView1.ColumnCount - 1].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.Fill;
            for (int i = 1; i < this.dataGridView1.ColumnCount; i++)
            {
                this.dataGridView1.Columns[i].ReadOnly = true;
            }
        }

        /// <summary>
        /// 向窗体中添加查询字段
        /// </summary>
        /// <param name="fieldName">字段名</param>
        public void AddSelectField(string fieldName)
        {
            var control = new CtnlMFieldSelect(this._viewName);
            control.FieldName = fieldName;
            control.Width = flowLayoutPanel1.Width - _distToFlowLayoutPanel;
            this.flowLayoutPanel1.Controls.Add(control);
        }

        /// <summary>
        /// 删除窗体中的查询字段
        /// </summary>
        /// <param name="fieldName"></param>
        public void RemoveSelectField(string fieldName)
        {
            for (int i = 0; i < flowLayoutPanel1.Controls.Count; i++)
            {
                if (flowLayoutPanel1.Controls[i] is CtnlMFieldSelect)
                {
                    var fieldSelect = (CtnlMFieldSelect) flowLayoutPanel1.Controls[i];
                    if (fieldSelect.FieldName.Equals(fieldName))
                    {
                        flowLayoutPanel1.Controls.Remove(flowLayoutPanel1.Controls[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 获取该窗体目前已经有的查询字段
        /// </summary>
        /// <returns></returns>
        public List<string> GetSelectFields()
        {
            var fields = new List<string>();
            if (this.flowLayoutPanel1.Controls.Count > 0)
            {
                for (int i = 0; i < this.flowLayoutPanel1.Controls.Count; i++)
                {
                    if (this.flowLayoutPanel1.Controls[i] is CtnlMFieldSelect)
                    {
                        fields.Add(((CtnlMFieldSelect) this.flowLayoutPanel1.Controls[i]).FieldName);
                    }
                }
            }
            return fields;
        }

        /// <summary>
        /// 获取字段的查询子句
        /// 即 形如 字段名 in ('v1','v2')这样的查询语句
        /// 例如 观测单位 in ('搜救中心','二测')
        /// </summary>
        /// <returns></returns>
        public List<string> GetFieldSelectClauses()
        {
            var clauses = new List<string>();
            if (this.flowLayoutPanel1.Controls.Count > 0)
            {
                //遍历界面中的检索控件，将控件中已经Check的项连同字段名取出来
                for (int i = 0; i < this.flowLayoutPanel1.Controls.Count; i++)
                {
                    if (this.flowLayoutPanel1.Controls[i] is CtnlMFieldSelect)
                    {
                        var fieldControl = (CtnlMFieldSelect) this.flowLayoutPanel1.Controls[i];
                        //如果该查询字段有选中的项
                        var chkBoxList = fieldControl.checkedListBox1;
                        if (chkBoxList.CheckedItems.Count > 0)
                        {
                            var builder = new StringBuilder();
                            for (int j = 0; j < chkBoxList.CheckedItems.Count; j++)
                            {
                                builder.Append("'" + chkBoxList.CheckedItems[j] + "',");
                            }
                            //去掉最后一个逗号
                            var expression = builder.ToString()
                                .Remove(builder.ToString().Length - 1);
                            expression = "(" + expression + ")";
                            clauses.Add("(" + fieldControl.FieldName + " in " + expression + ")");
                        }
                    }
                }
            }
            return clauses;
        }

        private void flowLayoutPanel1_Resize(object sender, System.EventArgs e)
        {
            if (this.flowLayoutPanel1.Controls.Count > 0)
            {
                //调整子控件的宽度
                for (int i = 0; i < this.flowLayoutPanel1.Controls.Count; i++)
                {
                    var control = this.flowLayoutPanel1.Controls[i];
                    if (control is CtnlMFieldSelect)
                    {
                        control.Width = this.flowLayoutPanel1.Width - _distToFlowLayoutPanel;
                    }
                }
            }
        }

        //加字段
        private void button1_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanel1.Controls.Count > 0)
            {
                var ans = MessageBox.Show("当前正在进行查询，重新选择字段将清空本次查询内容！是否重新选择字段？"
                    , "提问", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ans == DialogResult.Yes)
                {
                    flowLayoutPanel1.Controls.Clear();
                }
                else
                {
                    return;
                }
            }
            var frmShowSelectFields = new FrmShowSelectFields();
            frmShowSelectFields.StartPosition = FormStartPosition.Manual;
            int x = Cursor.Position.X + this.DIST_TO_MOUSE;
            int y = Cursor.Position.Y + this.DIST_TO_MOUSE;
            frmShowSelectFields.Location = new Point(x,y);
            frmShowSelectFields.Owner = this;
            frmShowSelectFields.Show();
        }

        private void flowLayoutPanel1_ControlAdded(object sender, ControlEventArgs e)
        {
            if (e.Control is CtnlMFieldSelect)
            {
                e.Control.Width = this.flowLayoutPanel1.Width - 30;
            }
        }

        /// <summary>
        /// 区域按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            var posx = Cursor.Position.X + DIST_TO_MOUSE;
            var posy = Cursor.Position.Y + DIST_TO_MOUSE;
            var form = new FrmRegionSelectMItem(this.CUser);
            form.Owner = this;
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(posx, posy);
            var r = form.ShowDialog();
            //设置视图名称，字段查询皆从该视图中查询
            if (r == DialogResult.OK)
            {
                this._viewName = form.ViewName;
            }
        }

        /// <summary>
        /// 重查按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            this.flowLayoutPanel1.Controls.Clear();
            this.dataGridView1.DataSource = null;
            this.SQL = string.Empty;
        }

        /// <summary>
        /// 快查按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            var frmQueryCmd = new FrmQueryMItemCmd(this.CUser, QueryCmdAction.Use);
            frmQueryCmd.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            frmQueryCmd.Owner = this;
            var dialogResult = frmQueryCmd.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString(), frmQueryCmd.Command).Tables[0];
                this.RefreshDataGridView(dt);
            }
        }

        /// <summary>
        /// 保存查询条件按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (this.SQL.Equals(""))
            {
                MessageBox.Show("请先进行查询再保存查询条件");
                return;
            }
            if (flowLayoutPanel1.Controls.Count == 0)
            {
                MessageBox.Show("已经保存的查询条件!");
                return;
            }
            var frmQueryCmd = new FrmQueryMItemCmd(this.CUser, QueryCmdAction.Save);
            frmQueryCmd.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            frmQueryCmd.Owner = this;
            frmQueryCmd.ShowDialog();
        }

        /// <summary>
        /// 导出按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            var form = new FrmExportFields();
            List<string> unSelectedFields = new List<string>();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                unSelectedFields = form.UnExportedFields;
                Debug.Print("un exported fields:" + string.Join(",", unSelectedFields));
            }
            if (dataGridView1.DataSource != null)
            {
                DataTable dt = ((DataTable) dataGridView1.DataSource).Copy();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dt.Rows[i]["选择"]) == 0)
                    {
                        dt.Rows[i].Delete();
                    }
                }
                dt.AcceptChanges();
                //将不需要导出的列删除，默认删除选择列和编号列
                dt.Columns.Remove("选择");
                dt.Columns.Remove("编号");
                unSelectedFields.ForEach(field => dt.Columns.Remove(field));
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "文本文件(*.txt)|*.txt";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var extension = Path.GetExtension(saveFileDialog.FileName);
                    if (extension != null && extension.Equals(".txt"))
                    {
                        DataHelper.Export(saveFileDialog.FileName, dt);
                    }
                }
            }
        }

        /// <summary>
        /// 确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource != null)
            {
                this.Result = null;
                //去除未选择的测项
                var copiedTable = ((DataTable) dataGridView1.DataSource).Copy();
                this.Result = copiedTable.RemoveUnCheckedRow();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("还未选中测项哩~");
                this.Result = null;
            }
        }

        #region ContextMenuStrip

        /// <summary>
        /// 全选记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count > 0)
            {
                for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                {
                    this.dataGridView1.Rows[i].Cells[0].Value = true;
                }
                this.dataGridView1.RefreshEdit();
            }
        }

        /// <summary>
        /// 全不选记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count > 0)
            {
                for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                {
                    this.dataGridView1.Rows[i].Cells[0].Value = false;
                }
                this.dataGridView1.RefreshEdit();
            }
        }

        /// <summary>
        /// 反选记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count > 0)
            {
                if (this.dataGridView1.Columns.Contains("选择"))
                {
                    for (int i = 0; i < this.dataGridView1.RowCount; i++)
                    {
                        var cell = this.dataGridView1.Rows[i].Cells[0];
                        cell.Value = !Convert.ToBoolean(cell.Value);
                    }
                }
                this.dataGridView1.RefreshEdit();
            }
        }

        /// <summary>
        /// 全选高亮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                for (int i = 0; i < this.dataGridView1.SelectedRows.Count; i++)
                {
                    this.dataGridView1.SelectedRows[i].Cells[0].Value = true;
                }
                this.dataGridView1.RefreshEdit();
            }
        }

        /// <summary>
        /// 全不选高亮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                for (int i = 0; i < this.dataGridView1.SelectedRows.Count; i++)
                {
                    this.dataGridView1.SelectedRows[i].Cells[0].Value = false;
                }
                this.dataGridView1.RefreshEdit();
            }
        }

        /// <summary>
        /// 反选高亮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                if (this.dataGridView1.Columns.Contains("选择"))
                {
                    for (int i = 0; i < this.dataGridView1.SelectedRows.Count; i++)
                    {
                        var cell = this.dataGridView1.SelectedRows[i].Cells[0];
                        cell.Value = !Convert.ToBoolean(cell.Value);
                    }
                    this.dataGridView1.RefreshEdit();
                }
            }
        }

        /// <summary>
        /// 编辑测项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var index = dataGridView1.SelectedRows[0].Index;
                var dataRow = ((DataTable) (dataGridView1.DataSource)).Rows[index];
                var form = new FrmEditMItem(Operation.Edit, dataRow, this.CUser);
                form.StartPosition = FormStartPosition.CenterParent;
                form.Show();
            }
        }

        /// <summary>
        /// 删除测项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {

        }

        #endregion

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //点到标题行返回，且为第1列
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                //选中的单元格状态取反
                var cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                cell.Value = !Convert.ToBoolean(cell.Value);
                dataGridView1.RefreshEdit();
            }
        }

        /// <summary>
        /// 处理datagridview的右键菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(dataGridView1.DataSource == null) return;
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dataGridView1.Rows[e.RowIndex].Selected = true;
                //如果高亮了多行测项，则编辑菜单不可用
                toolStripMenuItem8.Enabled = !(dataGridView1.SelectedRows.Count > 1);
                contextMenuStrip1.Show(Cursor.Position);
            }
        }
    }
}