using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Xb2.GUI.M.Item.ToolWindow
{
    public partial class FrmShowSelectFields : Form
    {
        /// <summary>
        /// 主查询字段
        /// </summary>
        private string[] m_fields = {"观测单位", "地名", "方法名", "测项名", "断层走向"};

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

        #region DataGridView相关事件

        /// <summary>
        /// 获取DataGridView的数据源
        /// </summary>
        /// <returns></returns>
        private DataTable GetFieldTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("选择", typeof(bool));
            dt.Columns.Add("字段名", typeof(string));
            for (int i = 0; i < this.m_fields.Length; i++)
            {
                var dataRow = dt.NewRow();
                dataRow["选择"] = false;
                dataRow["字段名"] = this.m_fields[i];
                dt.Rows.Add(dataRow);
            }
            return dt;
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
            this.dataGridView1.DataSource = this.GetFieldTable();
            this.dataGridView1.Columns[0].Width = 60;
            this.dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView1.Columns[1].ReadOnly = true;
            foreach (DataGridViewColumn dataGridViewColumn in dataGridView1.Columns)
            {
                dataGridViewColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        /// <summary>
        /// 获取拥有者窗体，即FrmSelectMItem窗体
        /// </summary>
        /// <returns></returns>
        private FrmSelectMItem GetFrmSelectMItem()
        {
            FrmSelectMItem frmSelectMItem = null;
            if (this.Owner != null)
            {
                frmSelectMItem = (FrmSelectMItem) this.Owner;
            }
            return frmSelectMItem;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var row = this.dataGridView1.Rows[e.RowIndex];
            var isChecked = Convert.ToBoolean(row.Cells["选择"].Value);
            var fieldName = Convert.ToString(row.Cells["字段名"].Value);
            if (isChecked)
            {
                this.GetFrmSelectMItem().AddSelectField(fieldName);
            }
            else
            {
                this.GetFrmSelectMItem().RemoveSelectField(fieldName);
            }
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

        #endregion

        /// <summary>
        /// 禁用已经加入查询界面的字段名，即不可进行再次查询
        /// 就是将该字段的背景色打灰
        /// </summary>
        private void DisableAlreadySelectedFields()
        {
            //获取父窗体中已经存在的查询字段
            var existFields = this.GetFrmSelectMItem().GetExistFields();
            if (existFields.Count > 0)
            {
                for (int i = 0; i < existFields.Count; i++)
                {
                    foreach (DataGridViewRow dataGridViewRow in dataGridView1.Rows)
                    {
                        var fieldName = dataGridViewRow.Cells["字段名"].Value.ToString();
                        if (fieldName.Equals(existFields[i]))
                        {
                            dataGridViewRow.Frozen = true;
                            dataGridViewRow.DefaultCellStyle.BackColor = Color.LightGray;
                        }
                    }
                }
            }
        }
    }
}
