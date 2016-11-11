using System;
using System.Windows.Forms;
using Xb2.Utils;

namespace Xb2.GUI.M.Val.ProcessedData
{
    public partial class FrmInterpolation : Form
    {
        public FrmInterpolation()
        {
            InitializeComponent();
            dateTimePicker1.Value = new DateTime(2002, 8, 15);
        }

        private void FrmInterpolation_Load(object sender, System.EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim().Equals(""))
            {
                MessageBox.Show("正在进行常数差值，请务必输入一个数值！");
                return;
            }
            double t;
            if (!double.TryParse(textBox1.Text.Trim(), out t))
            {
                MessageBox.Show("请输入一个数字！");
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public DateTime GetDate()
        {
            return dateTimePicker1.Value;
        }

        public double GetValue()
        {
            return textBox1.Text.Trim().ToDouble();
        }
    }
}
