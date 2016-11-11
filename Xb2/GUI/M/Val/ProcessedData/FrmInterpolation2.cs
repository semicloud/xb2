using System;
using System.Windows.Forms;

namespace XbApp.View.M.Value.ProcessedData
{
    public partial class FrmInterpolation2 : Form
    {
        private readonly string _title;
        
        /// <summary>
        /// 用于拉格朗日差值
        /// </summary>
        /// <param name="title"></param>
        public FrmInterpolation2(string title)
        {
            InitializeComponent();
            this._title = title;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public DateTime GetDate()
        {
            return dateTimePicker1.Value;
        }

        private void FrmInterpolation2_Load(object sender, EventArgs e)
        {
            this.Text = _title;
            this.StartPosition = FormStartPosition.CenterScreen;
        }
    }
}
