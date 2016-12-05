using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Entity.Business;
using Xb2.GUI.M.Item;
using Xb2.GUI.Main;
using Xb2.Utils.Database;

namespace Xb2.GUI.Computing.Input
{
    public partial class FrmSingleInput : FrmBase
    {
        public FrmSingleInput(XbUser user)
        {
            InitializeComponent();
            this.CUser = user;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            FrmSelectMItem frmSelectMItem = new FrmSelectMItem(this.CUser)
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            var confirm = frmSelectMItem.ShowDialog();
            if (confirm == DialogResult.OK)
            {
                var dt = frmSelectMItem.Result;
                if (dt.Rows.Count > 1)
                {
                    MessageBox.Show("只能选择一个测项！");
                    return;
                }
                var mItemId = dt.Rows[0]["编号"];
                Debug.Print("测项编号：" + mItemId);
                var sql = string.Format("select 编号,库名,是否默认 from {0} where 用户编号={1} and 测项编号={2}", Db.TnProcessedDb(),
                    this.CUser.ID, mItemId);
                Debug.Print("SQL:" + sql);
                dt = MySqlHelper.ExecuteDataset(Db.CStr(), sql).Tables[0];
                bool hasDefaultDb = dt.AsEnumerable().Any(r => r.Field<bool>("是否默认"));
                Debug.Print("用户{0}，测项{1}是否有默认基础数据库？{2}", this.CUser.ID, mItemId, hasDefaultDb);
                var dr = dt.NewRow();
                dr["库名"] = "原始数据";
                dr["编号"] = -1; //-1代表取原始数据
                dr["是否默认"] = !hasDefaultDb;
                dt.Rows.InsertAt(dr, 0);
                checkedListBox1.DataSource = dt;
                checkedListBox1.DisplayMember = "库名";
                checkedListBox1.ValueMember = "编号";
                var dbname = dt.AsEnumerable().First(r => r.Field<bool>("是否默认"))["库名"];
                Debug.Print("默认数据：" + dbname);
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    var drv = (DataRowView) checkedListBox1.Items[i];
                    checkedListBox1.SetItemChecked(i, Convert.ToBoolean(drv.Row["是否默认"]));
                }

                //实现checkboxlist的单选功能
            }
        }
    }
}
