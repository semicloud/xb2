using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using NLog;
using Xb2.GUI.M.Item;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.GUI.Controls
{
    /// <summary>
    /// 测项字段查询自定义控件
    /// </summary>
    public partial class CtnlMFieldSelect : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //控件距离FlowLayoutPanel的距离，妈的FlowLayout的布局太难弄了
        //直接编码控制吧
        private readonly int WIDTH_TO_FLP = 4;
        
        private string m_fieldName;

        /// <summary>
        /// 查询字段名
        /// </summary>
        public string FieldName
        {
            get { return m_fieldName; }
            set { this.m_fieldName = value; }
        }

        /// <summary>
        /// 视图名称
        /// ViewName的来源可能有两种：
        /// 一种是测项总表
        /// 一种是保存的测项区域查询视图
        /// </summary>
        private string m_viewName;
        
        /// <summary>
        /// 查询测项的SQL
        /// </summary>
        private string m_baseCommandText;

        public CtnlMFieldSelect(string fieldName, string viewName)
        {
            this.InitializeComponent();
            this.m_viewName = viewName;
            this.m_fieldName = fieldName;
            this.m_baseCommandText = string.Format("select * from {0} where ", this.m_viewName);
            Logger.Info("创建测项字段查询控件，字段名：{0}，视图名：{1}", m_fieldName, m_viewName);
            Logger.Info("字段查询基语句：" + m_baseCommandText);
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

        private FrmSelectMItem GetFrmSelectMItem()
        {
            FrmSelectMItem form = null;
            if (this.ParentForm != null)
            {
                form = (FrmSelectMItem) this.ParentForm;
            }
            return form;
        }

        /// <summary>
        /// 利用字段fieldCommandText更新checkboxlist中的字段内容
        /// 该函数在两个地方被调用
        /// 1. 该查询空间被加载时
        /// 2. 主窗体查询条件改变时
        /// </summary>
        /// <param name="fieldCommandText"></param>
        public void RefreshCheckBoxList(string fieldCommandText)
        {
            // 注意：先把selectedIndex事件注销，否则在绑定数据源的时候该事件会被触发两次，造成性能瓶颈
            this.checkedListBox1.SelectedIndexChanged -= this.checkedListBox1_SelectedIndexChanged;
            var dataTable = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, fieldCommandText).Tables[0];
            this.checkedListBox1.DataSource = null;
            this.checkedListBox1.DataSource = dataTable.GetColumnOfString(m_fieldName);
            this.checkedListBox1.SelectedIndexChanged += this.checkedListBox1_SelectedIndexChanged;
            Logger.Debug("更新CheckedBoxList:" + fieldCommandText);
        }

        private void MItemFieldQuery_Load(object sender, EventArgs e)
        {
            this.groupBox1.Text = this.FieldName;
            this.groupBox1.Dock = DockStyle.Fill;
            this.ResizeControlWidth();

            var fieldCommandText = "select distinct {0} as {0} from {1}";
            fieldCommandText = string.Format(fieldCommandText, this.FieldName, this.m_viewName);
            RefreshCheckBoxList(fieldCommandText);
        }
        
        private void MItemFieldSelect_Resize(object sender, EventArgs e)
        {
            this.ResizeControlWidth();
        }

        /// <summary>
        /// 随着用户点击CheckBoxList中的项
        /// 主窗体中的测项数据随之刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var clauses = this.GetFrmSelectMItem().GetFieldSelectClauses();
            Logger.Info("从主窗体中获取的查询子句：");
            foreach (string clause in clauses)
            {
                Logger.Info("  " + clause);
            }
            //用and操作符组合查询子句
            if (clauses.Count > 0)
            {
                var builder = new StringBuilder();
                foreach (var clo in clauses)
                {
                    builder.Append(clo + " and ");
                }
                //形成总子句，并删除该子句的最后5个字符，即'and'加两个空格字符
                var mainClause = builder.ToString().Remove(builder.ToString().Length - 5, 5);
                //与拼接查询sql 语句
                var commandText = m_baseCommandText + mainClause;
                Logger.Info("拼接成的总查询子句：" + commandText);
                //利用拼接好的语句刷新查询窗口中的数据
                this.GetFrmSelectMItem().RefreshDataGridViewAndCheckedBoxList(commandText);
            }
            else
            {
                //无查询字段，拿空表填充查询窗体的dgv
                this.GetFrmSelectMItem().RefreshDataGridViewAndCheckedBoxList(string.Empty);
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