using System;
using System.Windows.Forms;

namespace Xb2.GUI.Computing
{
    public partial class FrmResizeCharts : Form
    {
        public FrmResizeCharts()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var width = Convert.ToInt32(textBox1.Text);
            var height = Convert.ToInt32(textBox2.Text);
            var form = (FrmDisplayCharts) this.Owner;
            form.ResizeCharts(width, height);
            this.Close();
        }
    }
}
