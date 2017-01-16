using System;
using System.Drawing;
using System.Windows.Forms;

namespace Xb2.TestAndDemos
{
    public partial class FrmInput3 : Form
    {
        public FrmInput3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmInputItemDetail frmInputItemDetail = new FrmInputItemDetail();
            frmInputItemDetail.StartPosition = FormStartPosition.Manual;
            frmInputItemDetail.Location = new Point(Cursor.Position.X + 20, Cursor.Position.Y + 20);
            frmInputItemDetail.ShowDialog();
        }
    }
}
