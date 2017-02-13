﻿using System.Windows.Forms;
using Xb2.Entity.Business;

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
    }
}
