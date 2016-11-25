using System.Windows.Forms;
using Xb2.Entity.Business;

namespace Xb2.GUI.Main
{
    public partial class FrmBase : Form
    {
        /// <summary>
        /// 当前登录的用户CurrentUser
        /// </summary>
        public XbUser CUser { get; set; }

        protected readonly int DIST_TO_MOUSE = 10;

        public FrmBase()
        {
            InitializeComponent();
        }

        public FrmFirst GetMainForm()
        {
            return (FrmFirst) this.MdiParent;
        }
    }
}
