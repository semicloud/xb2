using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NLog;
using Xb2.Algorithms.Core.Entity;
using Xb2.Algorithms.Core.Input;
using Xb2.Entity.Business;
using Xb2.GUI.M.Item;
using Xb2.GUI.Main;
using Xb2.TestAndDemos;

namespace Xb2.GUI.Input
{
    public partial class FrmNZBInput : FrmBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private List<NZBInput> _nzbInputs;

        /// <summary>
        /// 获取消趋势输入
        /// </summary>
        /// <returns></returns>
        public List<XQSInput> GetNzbInputs()
        {
            throw new NotImplementedException();
        }

        public int ItemId { get; set; }

        public FrmNZBInput(XbUser user)
        {
            InitializeComponent();
            this.User = user;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmSelectMItem frmSelectMItem = new FrmSelectMItem(this.User)
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            var dialogResult = frmSelectMItem.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                var dt = frmSelectMItem.Result;
                this.ItemId = Convert.ToInt32(dt.Rows[0]["编号"]);
                Logger.Info("用户选择了测项 {0} ", this.ItemId);
                button1.Text = "测项编号：" + ItemId;
            }
        }

        private void dateTimePicker2_Leave(object sender, EventArgs e)
        {
            if (this.ItemId == 0)
            {
                MessageBox.Show("没有选择测项！");
                return;
            }
            Form1 form1 = new Form1(this.User, this.ItemId, 
                dateTimePicker1.Value, dateTimePicker2.Value);
            form1.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
        

    }
}
