using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using NLog;
using Xb2.Entity.Business;
using Xb2.GUI.Main;
using Xb2.Utils.Database;

namespace Xb2.Entity
{
    public partial class FrmCreateEditRecord : FrmBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 地震目录标注库名称
        /// </summary>
        private string m_labelDatabaseName;

        /// <summary>
        /// 操作类别
        /// </summary>
        private Operation m_operation;

        /// <summary>
        /// 输入的数据行
        /// </summary>
        private DataRow m_dataRow;

        /// <summary>
        /// 要修改的地震目录的Id
        /// </summary>
        private Int32 m_editedId;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataRow">1条地震目录的DataRow</param>
        /// <param name="dbname">标注库名称</param>
        /// <param name="operation">操作类型</param>
        /// <param name="user">用户对象</param>
        public FrmCreateEditRecord(DataRow dataRow, string dbname, Operation operation,XbUser user)
        {
            InitializeComponent();
            this.m_operation = operation;
            this.m_dataRow = dataRow;
            this.m_labelDatabaseName = dbname;
            this.User = user;
            //发震日期和时间不允许更改
            this.dateTimePicker1.Value = Convert.ToDateTime(dataRow["发震日期"]);
            dateTimePicker1.Enabled = false;
            this.dateTimePicker2.Value = Convert.ToDateTime(dataRow["发震日期"]);
            dateTimePicker2.Enabled = false;
            this.textBox1.Text = dataRow["纬度"].ToString();
            this.textBox2.Text = dataRow["经度"].ToString();
            this.textBox3.Text = dataRow["震级值"].ToString();
            this.textBox4.Text = dataRow["震级单位"].ToString();
            this.textBox5.Text = dataRow["定位参数"].ToString();
            this.textBox6.Text = dataRow["参考地点"].ToString();
            this.m_editedId = Convert.ToInt32(dataRow["编号"]);
            Logger.Info("要求改的地震目录ID：" + this.m_editedId);
        }

        #region 确定和取消按钮

        private void button1_Click(object sender, EventArgs e)
        {
            #region 输入验证
            double t;
            //纬度验证
            if (this.textBox1.Text.Trim() == string.Empty)
            {
                MessageBox.Show("请输入纬度！");
                this.textBox1.Focus();
                return;
            }
            if (!double.TryParse(this.textBox1.Text.Trim(), out t))
            {
                MessageBox.Show("纬度必须为一个数字！");
                this.textBox1.Focus();
                return;
            }
            //经度验证
            if (this.textBox2.Text.Trim() == string.Empty)
            {
                MessageBox.Show("请输入经度！");
                this.textBox2.Focus();
                return;
            }
            if (!double.TryParse(this.textBox2.Text.Trim(), out t))
            {
                MessageBox.Show("经度必须为一个数字！");
                this.textBox2.Focus();
                return;
            }
            //震级值验证
            if (this.textBox3.Text.Trim() == string.Empty)
            {
                MessageBox.Show("请输入震级值！");
                this.textBox3.Focus();
                return;
            }
            if (!double.TryParse(this.textBox3.Text.Trim(), out t))
            {
                MessageBox.Show("震级值必须为一个数字！");
                this.textBox3.Focus();
                return;
            }
            //参考地点验证
            if (this.textBox6.Text.Trim() == string.Empty)
            {
                MessageBox.Show("请输入参考地点！");
                this.textBox6.Focus();
                return;
            }
            #endregion

            if (this.Process())
            {
                MessageBox.Show("操作成功！");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// 更新或者创建地震目录
        /// </summary>
        /// <returns></returns>
        private bool Process()
        {
            #region 获取输入变量

            var datetime1 = this.dateTimePicker1.Value.Date;
            var datetime2 = new DateTime();
            if (m_operation == Operation.Create)
            {
                datetime2 = this.dateTimePicker2.Value;
            }
            if (m_operation == Operation.Edit)
            {
                var timespan = TimeSpan.Parse(m_dataRow["发震时间"].ToString());
                datetime2 = new DateTime(datetime1.Year, datetime1.Month, datetime1.Day,
                    timespan.Hours, timespan.Minutes, timespan.Seconds);
            }
            var latitude = Convert.ToInt32(this.textBox1.Text.Trim());
            var longitude = Convert.ToInt32(this.textBox2.Text.Trim());
            var magnitude = Math.Round(Convert.ToSingle(this.textBox3.Text.Trim()), 2);
            var magnitudeUnit = this.textBox4.Text.Trim();
            var locationParameter = this.textBox5.Text.Trim();
            var location = this.textBox6.Text.Trim();

            #endregion


            var labelDatabaseId = DaoObject.GetLabelDbId(m_labelDatabaseName, User.ID);
            var commandText = "select * from {0} where 标注库编号={1}";
            commandText = string.Format(commandText, DbHelper.TnLabelDbData(), labelDatabaseId);
            var adapter = new MySqlDataAdapter(commandText, DbHelper.ConnectionString);
            var builder = new MySqlCommandBuilder(adapter);
            builder.ConflictOption = ConflictOption.OverwriteChanges;
            var dataTable = new DataTable();
            adapter.Fill(dataTable);
            //创建地震目录
            if (this.m_operation == Operation.Create)
            {
                DataRow dataRow = dataTable.NewRow();
                dataRow["发震日期"] = datetime1;
                dataRow["发震时间"] = datetime2;
                dataRow["纬度"] = latitude;
                dataRow["经度"] = longitude;
                dataRow["震级值"] = magnitude;
                dataRow["震级单位"] = magnitudeUnit;
                dataRow["定位参数"] = locationParameter;
                dataRow["参考地点"] = location;
                dataRow["标注库编号"] = labelDatabaseId;
                dataTable.Rows.Add(dataRow);
                return adapter.Update(dataTable) > 0;
            }
            //更新地震目录
            if (this.m_operation == Operation.Edit)
            {
                var dataRow =
                    dataTable.Rows.Cast<DataRow>().ToList().Find(r => Convert.ToInt32(r["编号"]) == this.m_editedId);
                if (dataRow != null)
                {
                    dataRow["发震日期"] = datetime1;
                    dataRow["发震时间"] = datetime2;
                    dataRow["纬度"] = latitude;
                    dataRow["经度"] = longitude;
                    dataRow["震级值"] = magnitude;
                    dataRow["震级单位"] = magnitudeUnit;
                    dataRow["定位参数"] = locationParameter;
                    dataRow["参考地点"] = location;
                    return adapter.Update(dataTable) > 0;
                }
                else
                {
                    Logger.Error("DataRow is NULL");
                }
            }
            return false;
        }

        #endregion

        private void FrmCreateEditRecord_Load(object sender, EventArgs e)
        {
            if (this.m_operation == Operation.Create)
            {
                this.Text = "新建地震目录";
            }
            if (this.m_operation == Operation.Edit)
            {
                this.Text = "编辑地震目录";
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            this.dateTimePicker2.Value = this.dateTimePicker1.Value;
        }
    }
}
