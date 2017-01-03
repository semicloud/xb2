using System;
using System.Windows.Forms;
using Xb2.Entity.Business;
using Xb2.GUI.Controls;
using Xb2.GUI.M.Item;
using Xb2.GUI.Main;

namespace Xb2.TestAndDemos
{
    public partial class FrmInput : FrmBase
    {
        public FrmInput(XbUser user)
        {
            InitializeComponent();
            this.CUser = user;
        }

        private void FrmInput_Load(object sender, EventArgs e)
        {
            //MItemInput mItemInput = new MItemInput(this.CUser, 15);
            //this.flowLayoutPanel1.Controls.Add(mItemInput);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.flowLayoutPanel1.Controls.Clear();
            FrmSelectMItem frmSelectMItem = new FrmSelectMItem(this.CUser)
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            var confirm = frmSelectMItem.ShowDialog();
            if (confirm == DialogResult.OK)
            {
                var dt = frmSelectMItem.Result;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var mItemId = Convert.ToInt32(dt.Rows[i]["编号"]);
                    this.flowLayoutPanel1.Controls.Add(new MItemInput(this.CUser,mItemId));
                }
            }
        }
    }
}
