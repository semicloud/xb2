using System;
using System.Windows.Forms;

namespace Xb2.GUI.M.Val.ProcessedData
{
    public partial class FrmInputPeriod : Form
    {
        public FrmInputPeriod()
        {
            InitializeComponent();
        }

        public int Period { get; private set; }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Period = Convert.ToInt32(textBox1.Text.Trim());
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
