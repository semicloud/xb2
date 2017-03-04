using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Xb2.Entity.Business;
using Xb2.GUI.Input;
using Xb2.GUI.Main;

namespace Xb2
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //XbUser user = new XbUser();
            //user.ID = 1;
            //user.Name = "admin";
            //user.IsAdmin = true;
            //user.Password = "admin";
            //Application.Run(new FrmBeatifulChart2());

            //Application.Run(new FrmInterface4192(user, new List<int>() {15,166,32},
            //    new DateTime(1989, 1, 1),
            //    new DateTime(2004, 2, 29)));

            FrmLogin frmLogin = new FrmLogin();
            frmLogin.ShowDialog();
            if (frmLogin.DialogResult == DialogResult.OK)
            {
                frmLogin.Close();
                var frmFirst = new FrmFirst(frmLogin.User);
                Application.Run(frmFirst);
            }
        }
    }
}
