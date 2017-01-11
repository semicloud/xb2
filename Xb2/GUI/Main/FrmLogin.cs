using System;
using System.Windows.Forms;
using Xb2.Entity.Business;
using Xb2.Utils.Database;

namespace Xb2.GUI.Main
{
    public partial class FrmLogin : FrmBase
    {
        public FrmLogin()
        {
            InitializeComponent();
            this.User = new XbUser();
        }

        private void FrmLogin_Activated(object sender, EventArgs e)
        {
            UserNameTextBox.Focus();
        }

        //如果未选中 管理员 按钮
        //有此 用户名即可登录
        private void LoginButton_Click(object sender, EventArgs e)
        {
            if (UserNameTextBox.Text.Trim().Equals(""))
            {
                ShowMsgLabel.Text = "请输入用户名！";
                return;
            }
            if (PasswordTextBox.Text.Trim().Equals("") && this.AdminRadioButton.Checked)
            {
                ShowMsgLabel.Text = "请输入密码！";
                return;
            }
            this.User.Name = UserNameTextBox.Text;
            this.User.Password = PasswordTextBox.Text;
            this.User.IsAdmin = AdminRadioButton.Checked;

            var userId = DaoObject.GetUserId(this.User.Name, this.User.Password, this.User.IsAdmin);
            if (userId != -1 && userId!=-9999)
            {
                this.User.ID = Convert.ToInt32(userId);
                this.DialogResult = DialogResult.OK;
            }
            else if (userId == -9999)
            {
                MessageBox.Show("连接数据库失败！");
            }
            else
            {
                MessageBox.Show("密码错误或不存在的用户！");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
