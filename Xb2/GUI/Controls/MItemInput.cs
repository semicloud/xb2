using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Entity.Business;
using Xb2.Utils.Database;

namespace Xb2.GUI.Controls
{
    public partial class MItemInput : UserControl
    {
        private int m_mitemId;
        private XbUser m_user;

        public MItemInput(XbUser user, Int32 mitemId)
        {
            InitializeComponent();
            this.m_user = user;
            this.m_mitemId = mitemId;
        }

        private void MItemInput_Load(object sender, EventArgs e)
        {
            var dtMitem = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString,
                string.Format("select * from {0} where 编号=" + m_mitemId, DbHelper.TnMItem())).Tables[0];
            if (dtMitem.Rows.Count == 0)
            {
                MessageBox.Show("不存在的测项！编号：" + m_mitemId);
                return;
            }
            label3.Text = string.Format("测项编号:{0}，{1}-{2}-{3}-{4}", m_mitemId,
                dtMitem.Rows[0]["观测单位"], dtMitem.Rows[0]["地名"], dtMitem.Rows[0]["方法名"], dtMitem.Rows[0]["测项名"]);
            Debug.Print("测项编号：" + m_mitemId);
            //根据测项编号和用户编号查询基础数据库信息
            var sql = string.Format("select 编号,库名,是否默认 from {0} where 用户编号={1} and 测项编号={2}", DbHelper.TnProcessedDb(),
                this.m_user.ID, m_mitemId);
            Debug.Print("根据测项编号和用户编号查询基础数据库信息：\n" + sql);
            var dtEssDb = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, sql).Tables[0];
            bool hasDefaultDb = dtEssDb.AsEnumerable().Any(r => r.Field<bool>("是否默认"));
            Debug.Print("用户{0}，测项{1}是否有默认基础数据库？{2}", this.m_user, m_mitemId, hasDefaultDb);
            //将原始数据加到基础数据里
            var dr = dtEssDb.NewRow();
            dr["库名"] = "原始数据";
            dr["编号"] = -1; //-1代表取原始数据
            dr["是否默认"] = !hasDefaultDb;
            dtEssDb.Rows.InsertAt(dr, 0);
            //对每一个基础数据库生成一个RadioButton
            for (int i = 0; i < dtEssDb.Rows.Count; i++)
            {
                RadioButton rb = new RadioButton
                {
                    Text = dtEssDb.Rows[i]["编号"] + "," + dtEssDb.Rows[i]["库名"],
                    AutoCheck = true,
                    AutoSize = true
                };
                // 将默认的基础数据库设为选中状态
                rb.Checked = Convert.ToBoolean(dtEssDb.Rows[i]["是否默认"]);
                flowLayoutPanel1.Controls.Add(rb);
            }
        }


    }
}
