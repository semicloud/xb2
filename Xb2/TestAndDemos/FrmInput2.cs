using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using NLog;
using Xb2.Algorithms.Core.Entity;
using Xb2.Entity.Business;
using Xb2.GUI.M.Item;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.TestAndDemos
{
    public partial class FrmInput2 : FrmBase
    {
        public int ItemId { get; set; } //测项编号
        public DateTime StartDate { get; set; } //开始日期
        public DateTime EndDate { get; set; } //结束日期
        public int Period { get; set; } // 观测周期
        public List<DateValue> FinalDateValueList { get; set; } //最终参与计算的观测数据
        public int WLen { get; set; } //窗长
        public int SLen { get; set; } //步长
        public int Delta { get; set; } //时间间隔
        public int ProcessedDatabaseId { get; set; } //基础数据库编号
        public string ProcessedDatabaseName { get; set; } //基础数据库名称

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        public FrmInput2(XbUser user)
        {
            InitializeComponent();
            this.User = user;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FrmInputInterface1 interface1 = new FrmInputInterface1(this.User);
            if (this.ProcessedDatabaseId == -1)
            {
                interface1.DateValueList = DaoObject.GetRawData(this.ItemId).RetrieveDateValues();
            }
            else
            {
                interface1.DateValueList = DaoObject.GetProcessedData(this.ProcessedDatabaseId).RetrieveDateValues();
            }
            interface1.StartDate = dateTimePicker1.Value;
            interface1.EndDate = dateTimePicker2.Value;
            interface1.ShowDialog();
        }

        private void FrmInput2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmSelectMItem frmSelectMItem = new FrmSelectMItem(this.User)
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            var confirm = frmSelectMItem.ShowDialog();

            if (confirm == DialogResult.OK)
            {
                flowLayoutPanel1.Controls.Clear();
                var dt = frmSelectMItem.Result;
                if (dt.Rows.Count > 1)
                {
                    MessageBox.Show("只能选择一个测项，请重新选择！");
                    return;
                }
                this.ItemId = Convert.ToInt32(dt.Rows[0]["编号"]);
                Logger.Info("用户选择了测项 {0} ", this.ItemId);
                label6.Text = ItemId.ToString();
                var databaseInfoTable = DaoObject.GetUserProcessedDatabaseInfos(this.User.ID,
                    this.ItemId, "编号", "库名", "是否默认");
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
            }
        }

        private void RbOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            var rb = (RadioButton) sender;
            if (rb.Checked)
            {
                var array = rb.Text.Split(',');
                this.ProcessedDatabaseId = Convert.ToInt32(array[0]);
                this.ProcessedDatabaseName = array[1];
                Logger.Info("用户选择了编号为 {0} 的基础数据库 {1}",
                    this.ProcessedDatabaseId, this.ProcessedDatabaseName);
            }
        }
    }
}
