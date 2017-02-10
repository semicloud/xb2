using System;
using System.Windows.Forms;
using NLog;
using Xb2.Entity.Business;
using Xb2.GUI.M.Item;
using Xb2.GUI.Main;

namespace Xb2.TestAndDemos
{
    public partial class FrmXQSInput : FrmBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public int ItemId { get; set; }
        public FrmXQSInput(XbUser user)
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
                if (dt.Rows.Count > 1)
                {
                    MessageBox.Show("只能选择一个测项，请重新选择！");
                    return;
                }
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
