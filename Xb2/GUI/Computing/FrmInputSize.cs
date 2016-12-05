using System;
using System.Windows.Forms;

namespace Xb2.GUI.Computing
{
    public partial class FrmInputSize : Form
    {
        public FrmInputSize()
        {
            InitializeComponent();
        }

        private void FrmInputSth_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var frmConfigChart = (FrmConfigChart) Owner;
            frmConfigChart.ChangeXYTitle(textBox1.Text, textBox2.Text);
            this.Close();
        }
    }
}
