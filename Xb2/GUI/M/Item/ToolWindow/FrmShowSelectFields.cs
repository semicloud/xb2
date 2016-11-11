using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Xb2.GUI.M.Item;

namespace XbApp.View.M.Item.ToolWindow
{
    public partial class FrmShowSelectFields : System.Windows.Forms.Form
    {
        private readonly string[] SELECT_FIELDS = {"观测单位", "地名", "方法名", "测项名", "断层走向"};

        public FrmShowSelectFields()
        {
            this.InitializeComponent();
        }

        private void FrmShowSelectFields_Load(object sender, EventArgs e)
        {
            this.RefreshDataGridView();
            this.DisableAlreadySelectedFields();
            this.Height = this.dataGridView1.Height;
        }

        /// <summary>
        /// 绑定DataGridView
        /// </summary>
        private void RefreshDataGridView()
        {
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = this.GetDataSource();
            this.dataGridView1.Columns[0].Width = 60;
            this.dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView1.Columns[1].ReadOnly = true;
        }

        /// <summary>
        /// 将索引为rowIndex的DataGridView中的数据行禁用
        /// </summary>
        /// <param name="rowIndex"></param>
        private void DisableDataGridViewRow(int rowIndex)
        {
            this.dataGridView1.Rows[rowIndex].Frozen = true;
            this.dataGridView1.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
        }

        /// <summary>
        /// 禁用已经加入查询界面的字段名
        /// 就是将该字段的背景色打灰
        /// </summary>
        private void DisableAlreadySelectedFields()
        {
            //获取父窗体中已经存在的查询字段
            var fs = this.GetOwner().GetSelectFields();
            var rows = this.dataGridView1.Rows.Cast<DataGridViewRow>();
            if (fs.Count > 0)
            {
                for (int i = 0; i < fs.Count; i++)
                {
                    var row = rows.First(r => r.Cells["字段名"].Value.ToString().Equals(fs[i]));
                    if (row != null)
                    {
                        this.DisableDataGridViewRow(row.Index);
                    }
                }
            }
        }

        /// <summary>
        /// 获取拥有者窗体
        /// </summary>
        /// <returns></returns>
        private FrmSelectMItem GetOwner()
        {
            FrmSelectMItem frmSelectMItem = null;
            if (this.Owner != null)
            {
                frmSelectMItem = (FrmSelectMItem) this.Owner;
            }
            return frmSelectMItem;
        }

        /// <summary>
        /// 获取DataGridView的数据源
        /// </summary>
        /// <returns></returns>
        private DataTable GetDataSource()
        {
            var dt = new DataTable();
            dt.Columns.Add("选择", typeof(bool));
            dt.Columns.Add("字段名", typeof(string));
            for (int i = 0; i < this.SELECT_FIELDS.Length; i++)
            {
                var dataRow = dt.NewRow();
                dataRow["选择"] = false;
                dataRow["字段名"] = this.SELECT_FIELDS[i];
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex == 0)
            {
                //点到单元格内就反选，而不是点到复选框上
                this.dataGridView1.Rows[e.RowIndex].Cells["选择"].Value
                    = !Convert.ToBoolean(this.dataGridView1.Rows[e.RowIndex].Cells["选择"].Value);
                this.dataGridView1.RefreshEdit();
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var row = this.dataGridView1.Rows[e.RowIndex];
            var isChecked = Convert.ToBoolean(row.Cells["选择"].Value);
            var fieldName = Convert.ToString(row.Cells["字段名"].Value);
            if (isChecked)
            {
                this.GetOwner().AddSelectField(fieldName);
            }
            else
            {
                this.GetOwner().RemoveSelectField(fieldName);
            }
        }
    }
}
