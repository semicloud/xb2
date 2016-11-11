using System.Collections.Generic;
using System.Windows.Forms;

namespace Xb2.GUI.M.Item.ToolWindow
{
    /// <summary>
    /// 导出测项时选择导出列的界面
    /// </summary>
    public partial class FrmExportFields : System.Windows.Forms.Form
    {
        public List<string> UnExportedFields { get; private set; }

        public FrmExportFields()
        {
            this.InitializeComponent();
            this.UnExportedFields = new List<string>();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (!checkedListBox1.GetItemChecked(i))
                {
                    this.UnExportedFields.Add(checkedListBox1.Items[i].ToString());
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
