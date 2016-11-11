using System.Collections.Generic;
using System.Windows.Forms;

namespace Xb2.GUI.M.Val.ProcessedData
{
    public partial class FrmShowLog : Form
    {
        public FrmShowLog()
        {
            InitializeComponent();
        }

        public void SetLogs(List<string> list)
        {
            listBox1.DataSource = list;
        }
    }
}
