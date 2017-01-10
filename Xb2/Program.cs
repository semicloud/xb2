using System;
using System.Windows.Forms;
using Xb2.Entity.Business;
using Xb2.GUI.Main;
using Xb2.TestAndDemos;

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
            //Application.Run(new FrmInput2(user));

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
