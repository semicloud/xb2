using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NLog;
using Xb2.Algorithms.Core.Entity;
using Xb2.Entity.Business;
using Xb2.GUI.Main;
using Xb2.Utils;

namespace Xb2.TestAndDemos
{
    public partial class FrmInputInterface1 : FrmBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public FrmInputInterface1(XbUser user)
        {
            InitializeComponent();
            dataGridView1.SelectionChanged += DataGridView1OnSelectionChanged;
            this.User = user;
        }

        public List<DateValue> DateValueList { get; set; } //观测数据
        public DateTime StartDate { get; set; } //开始日期
        public DateTime EndDate { get; set; } //结束日日期
        public int Period { get; set; } //观测周期
        public List<DateValue> ProcessedDateValueList { get; set; } //处理后的观测数据


        private void DataGridView1OnSelectionChanged(object sender, EventArgs eventArgs)
        {
            dataGridView1.ClearSelection();
        }

        private void FrmSingleInput_Load(object sender, EventArgs e)
        {
            var kIndexes = DateValueHelper.GetKIndexes(this.DateValueList, this.StartDate, this.EndDate);
            dataGridView1.Rows.Clear();
            dataGridView1.Rows.Add();
            Enumerable.Range(0, kIndexes.Length)
                .ToList()
                .ForEach(i => dataGridView1.Rows[0].Cells[i].Value = kIndexes[i]);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("请给定观测周期！");
                return;
            }
            this.Period = Convert.ToInt32(textBox1.Text);
            this.ProcessedDateValueList = new List<DateValue>(); //TODO 尚未完成
            Logger.Info("确定观测周期：{0}", this.Period);
            Logger.Info("处理后的基础数据，共 {0} 条", this.ProcessedDateValueList.Count);
        }
    }
}
