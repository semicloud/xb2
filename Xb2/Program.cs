using System;
using System.Windows.Forms;
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
            //Application.Run(new FrmLagrangeInterpolationDemo());

            FrmLogin frmLogin = new FrmLogin();
            frmLogin.ShowDialog();
            if (frmLogin.DialogResult == DialogResult.OK)
            {
                frmLogin.Close();
                var frmFirst = new FrmFirst(frmLogin.CUser);
                Application.Run(frmFirst);
            }
        }
    }
}
