using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using NLog;
using Xb2.Algorithms.Core.Entity;
using Xb2.Entity.Business;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.TestAndDemos
{
    public partial class Form1 : FrmBase
    {
        private int m_itemId; // 测项编号
        private DateTime m_startDate; // 开始日期
        private DateTime m_endDate; // 结束日期
        public int ProcessedDatabaseId { get; set; }
        public string ProcessedDatabaseName { get; set; }
        public List<DateValue> FinalDateValueList { get; set; }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Form1(XbUser user, int itemId, DateTime startDate, DateTime endDate)
        {
            InitializeComponent();
            this.User = user;
            this.m_itemId = itemId;
            this.m_startDate = startDate;
            this.m_endDate = endDate;
            Logger.Info("用户编号：{0}，测项编号：{1}，开始日期：{2}，结束日期：{3}",
                user.ID, m_itemId, startDate.ToShortDateString(), m_endDate.ToShortDateString());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 加载基础数据库
            var databaseInfoTable = DaoObject.GetUserProcessedDatabaseInfos(this.User.ID,
                m_itemId, "编号", "库名", "是否默认");
            var dr = databaseInfoTable.NewRow();
            dr["库名"] = "原始数据";
            dr["编号"] = -1; //-1代表取原始数据
            dr["是否默认"] = !databaseInfoTable.AsEnumerable().Any(row => row.Field<bool>("是否默认"));
            databaseInfoTable.Rows.InsertAt(dr, 0);
            for (int i = 0; i < databaseInfoTable.Rows.Count; i++)
            {
                RadioButton rb = new RadioButton
                {
                    Text = databaseInfoTable.Rows[i]["编号"] + "," + databaseInfoTable.Rows[i]["库名"],
                    AutoCheck = true,
                    AutoSize = true
                };
                rb.CheckedChanged += RbOnCheckedChanged;
                // 将默认的基础数据库设为选中状态
                rb.Checked = Convert.ToBoolean(databaseInfoTable.Rows[i]["是否默认"]);
                flowLayoutPanel1.Controls.Add(rb);
            }

            // 填入K指数
            if (this.ProcessedDatabaseId == 0 || m_itemId == 0)
            {
                MessageBox.Show("没有选定基础数据，无法计算K指数！");
                return;
            }
            this.dataGridView1.Rows.Clear();
            this.dataGridView1.Rows.Add();
            var kIndexs = DateValueHelper.GetKIndexes(this.FinalDateValueList, m_startDate, m_endDate);
            for (int i = 0; i < kIndexs.Length; i++)
            {
                dataGridView1.Rows[0].Cells[i].Value = kIndexs[i];
            }
        }

        private void RbOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            var rb = (RadioButton)sender;
            if (rb.Checked)
            {
                var array = rb.Text.Split(',');
                this.ProcessedDatabaseId = Convert.ToInt32(array[0]);
                this.ProcessedDatabaseName = array[1];
                if (this.ProcessedDatabaseId == -1)
                {
                    this.FinalDateValueList = DaoObject.GetRawData(m_itemId).RetrieveDateValues();
                }
                else
                {
                    this.FinalDateValueList =
                        DaoObject.GetProcessedData(this.ProcessedDatabaseId).RetrieveDateValues();
                }
                Logger.Info("用户选择了编号为 {0} 的基础数据库 {1},共 {2} 条观测数据",
                    this.ProcessedDatabaseId, this.ProcessedDatabaseName, this.FinalDateValueList.Count);
            }
        }
    }
}
