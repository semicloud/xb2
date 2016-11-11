using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Entity.Business;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.GUI.M.Val.ProcessedData
{
    public partial class FrmSaveProcessedData : FrmBase
    {
        /**
         * 还得他娘的分成两类讨论
         * 如果是没有生成的基础数据，则创建基础数据库，保存基础数据库
         * 保存数据
         * 如果是已经生成的基础数据，则修改基础数据库，
         * 保存数据
         */
        /// <summary>
        /// 测项Id
        /// </summary>
        public int ItemId { get; set; }
        /// <summary>
        /// 基础数据库Id
        /// </summary>
        public int ProcessedDataId { get; set; }
        /// <summary>
        /// K指数
        /// </summary>
        public string KIndex { get; set; }
        /// <summary>
        /// 编辑的数据
        /// </summary>
        public DataTable DataTable { get; set; }
        /// <summary>
        /// 基础数据的操作记录
        /// </summary>
        public List<String> Logger { get; set; }

        
        public FrmSaveProcessedData(XbUser user)
        {
            InitializeComponent();
            this.CUser = user;
        }

        private void FrmSaveProcessedData_Load(object sender, EventArgs e)
        {
            if (this.ItemId != 0)
            {
                //测项编号
                this.textBox1.Text = this.ItemId.ToString();
                //用户名
                this.textBox2.Text = this.CUser.Name;
                //K指数
                this.textBox4.Text = this.KIndex;
                //基础数据库编号
                this.textBox5.Text = this.ProcessedDataId.ToString();
                this.dataGridView1.DataSource = this.DataTable;

                //如果保存的是原始数据，就没有覆盖这个选项了
                //如果保存的是基础数据，则默认覆盖原基础数据
                if (this.ProcessedDataId == 0)
                    checkBox1.Enabled = false;
                else
                    checkBox1.Checked = true;

                //自动生成基础数据库名，注意日期的格式化
                var sql = "select 观测单位,地名,方法名,测项名 from {0} where 编号={1}";
                sql = string.Format(sql, Db.TnMItem(), this.ItemId);
                var dr = MySqlHelper.ExecuteDataRow(Db.CStr(), sql);
                Debug.Print("sql:{0}, returns:{1}", sql, dr);
                if (dr != null)
                {
                    var gcdw = dr["观测单位"].ToString();
                    var dm = dr["地名"].ToString();
                    var cxm = dr["测项名"].ToString();
                    var ffm = dr["方法名"].ToString();
                    var dbname = string.Format("{0}_{1}_{2}_{3}_{4}_基础数据_{5}",
                        this.CUser.Name, gcdw, dm, ffm, cxm, DateTime.Now.Date.ToString("yyyyMMdd"));
                    this.textBox3.Text = dbname;
                }
                else
                {
                    MessageBox.Show("不存在的测项，编号" + this.ItemId, "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.dataGridView1.Columns["观测日期"].DefaultCellStyle.Format = "yyyy-MM-dd";
                this.dataGridView1.Columns["观测值"].DefaultCellStyle.Format = "#0.00";
            }
        }

        /// <summary>
        /// 是否已存在某基础数据库
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="itemId"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        private static bool HasProcessedDataDb(int userId, int itemId, string dbName)
        {
            var sql = string.Format("select 编号 from {0} where 用户编号={1} and 测项编号={2} and 库名='{3}'",
                Db.TnProcessedDb(), userId, itemId, dbName);
            var dt = MySqlHelper.ExecuteDataset(Db.CStr(), sql).Tables[0];
            return dt.Rows.Count > 0;
        }

        private static bool SaveProcessedDataDbInfo(int userId, int itemId, string dbName, object k, object period,
            string logger)
        {
            var sql = string.Format("select * from {0} where 用户编号={1} and 测项编号={2}",
                Db.TnProcessedDb(), userId, itemId);
            var dt = new DataTable();
            var adapter = new MySqlDataAdapter(sql, Db.CStr());
            var builder = new MySqlCommandBuilder(adapter);
            adapter.Fill(dt);
            var dr = dt.NewRow();
            dr["用户编号"] = userId;
            dr["测项编号"] = itemId;
            dr["库名"] = dbName;
            dr["K指数"] = k;
            dr["观测周期"] = period;
            dr["操作记录"] = logger;
            dr["是否默认"] = false;
            dt.Rows.Add(dr);
            return adapter.Update(dt) > 0;
        }

        private static bool SaveProcessedDataDbData(int userId, int itemId, string dbName, DataTable dataTable)
        {
            var sql = string.Format("select 编号 from {0} where 用户编号={1} and 测项编号={2} and 库名='{3}'",
                Db.TnProcessedDb(), userId, itemId, dbName);
            var dbId = MySqlHelper.ExecuteScalar(Db.CStr(), sql);
            Debug.Print("基础数据库编号：" + dbId);
            sql = string.Format("select * from {0} where 库编号={1}", Db.TnProcessedDbData(), dbId);
            var dt = new DataTable();
            var adapter = new MySqlDataAdapter(sql, Db.CStr());
            var builder = new MySqlCommandBuilder(adapter);
            adapter.Fill(dt);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                var dr2 = dt.NewRow();
                dr2["库编号"] = dbId;
                dr2["观测日期"] = dataTable.Rows[i]["观测日期"];
                dr2["观测值"] = dataTable.Rows[i]["观测值"];
                dt.Rows.Add(dr2);
            }
            return adapter.Update(dt) > 0;
        }

        private void SaveButton_Click(object sender, System.EventArgs e)
        {
            //基础数据库名
            var dbName = this.textBox3.Text.Trim();
            //K指数
            var k = this.textBox4.Text.Trim().GetStringOrDBNull();
            //观测周期
            var period = this.textBox6.Text.Trim().GetInt32OrDbNull();
            //操作步骤
            var logger = string.Join("|", this.Logger);
            if (this.ItemId != 0)
            {
                //原始数据保存为基础数据库
                //先保存基础数据库的信息，如库名，K指数等
                //再保存基础数据库的数据
                //检查是否存在同名基础数据库，有则提示用户修改
                //HERE！
                if (HasProcessedDataDb(this.CUser.ID, this.ItemId, dbName))
                {
                    MessageBox.Show("已经存在名为【" + dbName + "】的基础数据库，请换个名字。");
                    return;
                }
                var isDbInfoSaved = SaveProcessedDataDbInfo(this.CUser.ID, this.ItemId, dbName, k, period, logger);
                Debug.Print("保存用户基础数据库，用户编号{0}，测项编号{1}，库名{2}，返回{3}",
                    this.CUser.ID, this.ItemId, dbName, isDbInfoSaved);
                if (isDbInfoSaved)
                {
                    //保存基础数据库数据
                    var isDbDataSaved = SaveProcessedDataDbData(this.CUser.ID, this.ItemId, dbName, this.DataTable);
                    MessageBox.Show(isDbDataSaved ? "保存成功！" : "保存失败！");
                }
                else
                {
                    MessageBox.Show("保存用户基础数据库失败！");
                }
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}