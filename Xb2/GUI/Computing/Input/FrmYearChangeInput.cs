using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Algorithms.Core.Entity;
using Xb2.Algorithms.Core.Methods.Regression;
using Xb2.Algorithms.Core.Methods.YearChange;
using Xb2.Entity.Business;
using Xb2.GUI.M.Item;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.GUI.Computing.Input
{
    public partial class FrmYearChangeInput : FrmBase
    {
        public FrmYearChangeInput(XbUser user)
        {
            InitializeComponent();
            this.CUser = user;
            if (this.YearChangeInput == null)
            {
                this.YearChangeInput = new Xb2YearChangeInput();
            }
        }

        public Xb2YearChangeInput YearChangeInput { get; private set; }

        /// <summary>
        /// 选测项按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();//清理
            this.YearChangeInput.MItemId = 0;
            this.YearChangeInput.DatabaseId = 0;
            this.YearChangeInput.DatabaseName = string.Empty;

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
                //获取测项编号
                this.YearChangeInput.MItemId = Convert.ToInt32(dt.Rows[0]["编号"]);
                //注意下面这个字符串中的全角逗号，在算法中要使用这个全角逗号来获取测项字符串
                label3.Text = string.Format("测项编号:{0}，{1}-{2}-{3}-{4}", YearChangeInput.MItemId,
                    dt.Rows[0]["观测单位"], dt.Rows[0]["地名"], dt.Rows[0]["方法名"], dt.Rows[0]["测项名"]);
                Debug.Print("测项编号：" + YearChangeInput.MItemId);
                //根据测项编号和用户编号查询基础数据库信息
                var sql = string.Format("select 编号,库名,是否默认 from {0} where 用户编号={1} and 测项编号={2}", DbHelper.TnProcessedDb(),
                    this.CUser.ID, this.YearChangeInput.MItemId);
                Debug.Print("根据测项编号和用户编号查询基础数据库信息：\n" + sql);
                dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString(), sql).Tables[0];
                bool hasDefaultDb = dt.AsEnumerable().Any(r => r.Field<bool>("是否默认"));
                Debug.Print("用户{0}，测项{1}是否有默认基础数据库？{2}", this.CUser.ID, this.YearChangeInput.MItemId, hasDefaultDb);
                //将原始数据加到基础数据里
                var dr = dt.NewRow();
                dr["库名"] = "原始数据";
                dr["编号"] = -1; //-1代表取原始数据
                dr["是否默认"] = !hasDefaultDb;
                dt.Rows.InsertAt(dr, 0);
                //对每一个基础数据库生成一个RadioButton
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    RadioButton rb = new RadioButton
                    {
                        Text = dt.Rows[i]["编号"] + "," + dt.Rows[i]["库名"],
                        AutoCheck = true,
                        AutoSize = true
                    };
                    rb.CheckedChanged += RbOnCheckedChanged;
                    //将默认的基础数据库设为选中状态
                    rb.Checked = Convert.ToBoolean(dt.Rows[i]["是否默认"]);
                    flowLayoutPanel1.Controls.Add(rb);
                }
            }
        }

        private void RbOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            RadioButton rb = (RadioButton) sender;
            //选中单选按钮，则修改相应信息
            if (rb.Checked)
            {
                String[] strs = rb.Text.Split(',');
                //设置基础数据库ID，基础数据库名称和测项字符串
                this.YearChangeInput.DatabaseId = Convert.ToInt32(strs[0]);
                this.YearChangeInput.DatabaseName = strs[1];
                this.YearChangeInput.MItemStr = label3.Text;
                Debug.Print("选定的编号：{0}，基础数据库名称：{1}", this.YearChangeInput.DatabaseId, this.YearChangeInput.DatabaseName);
                var sql = "select min(观测日期) as s,max(观测日期) as t from {0} where {1}={2}";
                if (this.YearChangeInput.DatabaseId == -1)
                {
                    sql = string.Format(sql, DbHelper.TnRData(), "测项编号", this.YearChangeInput.MItemId);
                    Debug.Print("从原始数据中查询日期：" + sql);
                }
                else
                {
                    sql = string.Format(sql, DbHelper.TnProcessedDbData(), "库编号", this.YearChangeInput.DatabaseId);
                    Debug.Print("从基础数据库中查询日期：" + sql);
                }
                DetermineDateTime(sql);
            }
        }

        /// <summary>
        /// 确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            var sql = "select 观测日期,观测值 from {0} where {1}={2} order by 观测日期";
            if (this.YearChangeInput.DatabaseId == -1)
            {
                sql = string.Format(sql, DbHelper.TnRData(), "测项编号", this.YearChangeInput.MItemId);
                Debug.Print("从原始数据中查询数据：" + sql);
            }
            else
            {
                sql = string.Format(sql, DbHelper.TnProcessedDbData(), "库编号", this.YearChangeInput.DatabaseId);
                Debug.Print("从基础数据库中查询数据：" + sql);
            }
            this.YearChangeInput.Start = dateTimePicker1.Value;
            this.YearChangeInput.End = dateTimePicker2.Value;
            //按照开始日期和结束日期截取数据
            var dateValueList = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString(), sql).Tables[0].RetrieveDateValues();
            var dateRange = new DateRange(this.YearChangeInput.Start, this.YearChangeInput.End);
            this.YearChangeInput.DateValueList = dateValueList.Between(dateRange);
            //下一步就是调用FrmDisplayChart中的方法来绘制图形
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// 关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DetermineDateTime(string sql)
        {
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString(), sql).Tables[0];
            if (dt != null)
            {
                Debug.Print("开始日期：{0}，结束日期：{1}",
                    Convert.ToDateTime(dt.Rows[0]["s"]).ToShortDateString(),
                    Convert.ToDateTime(dt.Rows[0]["t"]).ToShortDateString());
                dateTimePicker1.Value = Convert.ToDateTime(dt.Rows[0]["s"]);
                dateTimePicker2.Value = Convert.ToDateTime(dt.Rows[0]["t"]);
            }
            else
            {
                Debug.Print("警告！未查到数据！");
                MessageBox.Show("无数据！");
            }
        }
    }
}
