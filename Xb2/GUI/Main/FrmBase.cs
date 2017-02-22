using System.Data;
using System.Windows.Forms;
using Xb2.Entity.Business;
using Xb2.GUI.M.Item;

namespace Xb2.GUI.Main
{
    public partial class FrmBase : Form
    {
        protected XbUser _user;

        /// <summary>
        /// 当前登录的用户
        /// </summary>
        public XbUser User
        {
            get { return _user; }
            set { _user = value; }
        }

        protected readonly int DIST_TO_MOUSE = 10;

        public FrmBase()
        {
            InitializeComponent();
        }

        public FrmFirst GetMainForm()
        {
            return (FrmFirst) this.MdiParent;
        }

        /// <summary>
        /// 选测项
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        protected static DataTable GetSelectedItemDataTable(XbUser user)
        {
            var frmSelectMItem = new FrmSelectMItem(user)
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            if (frmSelectMItem.ShowDialog() == DialogResult.OK)
            {
                return frmSelectMItem.Result;
            }
            MessageBox.Show("选测项出现问题！");
            return null;
        }
    }
}
