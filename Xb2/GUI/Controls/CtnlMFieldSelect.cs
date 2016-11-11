using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.GUI.M.Item;
using Xb2.Utils;
using Xb2.Utils.Database;
using XbApp.View.M.Item;

namespace Xb2.GUI.Controls
{
    public partial class CtnlMFieldSelect : UserControl
    {
        //控件距离FlowLayoutPanel的距离，妈的FlowLayout的布局太难弄了
        //直接编码控制吧
        private readonly int WIDTH_TO_FLP = 4;
        //字段名
        public string FieldName { get; set; }
        //视图名称
        public string ViewName { get; set; }
        //查询测项的SQL
        private string _itemSQL;

        public CtnlMFieldSelect(string viewName)
        {
            this.InitializeComponent();
            this.ViewName = viewName;
            this._itemSQL = string.Format("select * from {0} where ", this.ViewName);
        }

        /// <summary>
        /// 重新设置textbox和checkboxlist的宽度
        /// 以适应flowlayoutPanel的宽度
        /// </summary>
        private void ResizeControlWidth()
        {
            this.textBox1.Width = this.flowLayoutPanel1.Width - this.WIDTH_TO_FLP;
            this.checkedListBox1.Width = this.textBox1.Width;
        }

        private FrmSelectMItem GetParentForm()
        {
            FrmSelectMItem form = null;
            if (this.ParentForm != null)
            {
                form = (FrmSelectMItem) this.ParentForm;
            }
            return form;
        }

        /// <summary>
        /// 利用字段sql更新checkboxlist中的字段内容
        /// </summary>
        /// <param name="fsql"></param>
        public void RefreshCheckBoxList(string fsql)
        {
            //先把selectedIndex事件注销，否则在绑定数据源的时候该事件会被触发两次
            //造成性能瓶颈
            this.checkedListBox1.SelectedIndexChanged -= this.checkedListBox1_SelectedIndexChanged;
            Debug.Print("fsql:" + fsql);
            var dataTable = MySqlHelper.ExecuteDataset(Db.CStr(), fsql).Tables[0];
            this.checkedListBox1.DataSource = null;
            this.checkedListBox1.DataSource = dataTable.GetColumnOfString(this.FieldName);
            this.checkedListBox1.SelectedIndexChanged += this.checkedListBox1_SelectedIndexChanged;
        }

        private void MItemFieldQuery_Load(object sender, EventArgs e)
        {
            this.groupBox1.Text = this.FieldName;
            this.groupBox1.Dock = DockStyle.Fill;
            this.ResizeControlWidth();
            var fsql = "select distinct {0} as {0} from {1}";
            fsql = string.Format(fsql, this.FieldName, this.ViewName);
            this.RefreshCheckBoxList(fsql);
        }
        
        private void MItemFieldSelect_Resize(object sender, EventArgs e)
        {
            this.ResizeControlWidth();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var clauses = this.GetParentForm().GetFieldSelectClauses();
            //用and操作符组合查询子句
            if (clauses.Count > 0)
            {
                var builder = new StringBuilder();
                foreach (var clo in clauses)
                {
                    builder.Append(clo + " and ");
                }
                //形成总子句，并删除该子句的最后5个字符，即'and'加两个空格字符
                var clause = builder.ToString().Remove(builder.ToString().Length - 5, 5);
                //与拼接查询sql 语句
                var sql = _itemSQL + clause;
                Debug.Print("sql:" + sql);
                //利用拼接好的语句刷新查询窗口中的数据
                this.GetParentForm().RefreshData(sql);
            }
            else
            {
                //var sql = string.Format("select * from {0}", Db.TnMItem());
                //var dataTable = MySqlHelper.ExecuteDataset(Db.CStr(), sql).Tables[0];
                //无查询字段，拿空表填充查询窗体的dgv
                this.GetParentForm().RefreshData(string.Empty);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (this.checkedListBox1.DataSource != null)
                {
                    if (!this.textBox1.Text.Trim().Equals(string.Empty))
                    {
                        var allItems = (List<string>) this.checkedListBox1.DataSource;
                        var selectedItems = this.checkedListBox1.CheckedItems.Cast<string>().ToList();
                        var foundedItems = allItems.FindAll(t => t.Contains(this.textBox1.Text));
                        if (foundedItems.Count > 0)
                        {
                            //把找到的项放到前面，没选的放到后面
                            //之前选中的项还是
                            var dataSouce = new List<string>();
                            dataSouce.AddRange(foundedItems);
                            foundedItems.ForEach(s => allItems.Remove(s));
                            dataSouce.AddRange(allItems);
                            this.checkedListBox1.SelectedIndexChanged -= this.checkedListBox1_SelectedIndexChanged;
                            this.checkedListBox1.DataSource = null;
                            this.checkedListBox1.ClearSelected();
                            this.checkedListBox1.DataSource = dataSouce;
                            for (int i = 0; i < selectedItems.Count; i++)
                            {
                                var selectedItem = selectedItems[i];
                                var index = this.checkedListBox1.Items.IndexOf(selectedItem);
                                this.checkedListBox1.SetItemChecked(index,true);
                            }
                            this.checkedListBox1.SelectedIndexChanged += this.checkedListBox1_SelectedIndexChanged;
                        }
                    }
                }
            }
        }

    }
}