using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Entity.Business;
using Xb2.GUI.Main;
using Xb2.Utils.Database;

namespace Xb2.GUI.Catalog
{
    public partial class FrmCreateEditRecord : FrmBase
    {
        private string _labelDatabaseName;
        private Operation _operation;
        private DataRow _dataRow;
        private Int32 _editedId; //要修改的地震目录的Id

        //构造函数，需要传入
        public FrmCreateEditRecord(DataRow dataRow, string dbname, Operation operation,XbUser user)
        {
            InitializeComponent();
            this._operation = operation;
            this._dataRow = dataRow;
            this._labelDatabaseName = dbname;
            this.CUser = user;
            if (this._operation == Operation.Create)
            {
                this.Text = "新建地震目录";
            }
            if (this._operation == Operation.Edit)
            {
                this.Text = "编辑地震目录";
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
                this._editedId = Convert.ToInt32(dataRow["编号"]);
                Debug.Print("要求改的地震目录ID：" + this._editedId);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

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

        /// <summary>
        /// 更新或者创建地震目录
        /// </summary>
        /// <returns></returns>
        private bool Process()
        {
            var datetime1 = this.dateTimePicker1.Value.Date;
            var datetime2= new DateTime();
            if (_operation == Operation.Create)
            {
                datetime2 = this.dateTimePicker2.Value;
            }
            if (_operation == Operation.Edit)
            {
                var timespan = TimeSpan.Parse(_dataRow["发震时间"].ToString());
                datetime2 = new DateTime(datetime1.Year, datetime1.Month, datetime1.Day,
                    timespan.Hours, timespan.Minutes, timespan.Seconds);
            }
            var latitude = Convert.ToInt32(this.textBox1.Text.Trim());
            var longitude = Convert.ToInt32(this.textBox2.Text.Trim());
            var magnitude = Math.Round(Convert.ToSingle(this.textBox3.Text.Trim()), 2);
            var magnitudeUnit = this.textBox4.Text.Trim();
            var locationParameter = this.textBox5.Text.Trim();
            var location = this.textBox6.Text.Trim();

            var labelDbId = DaoObject.GetLabelDbId(_labelDatabaseName, CUser.ID);
            var sql = "select * from {0} where 标注库编号={1}";
            sql = string.Format(sql, DbHelper.TnLabelDbData(), labelDbId);
            var adapter = new MySqlDataAdapter(sql, DbHelper.ConnectionString());
            var builder = new MySqlCommandBuilder(adapter);
            builder.ConflictOption = ConflictOption.OverwriteChanges;
            var dataTable = new DataTable();
            adapter.Fill(dataTable);
            //创建地震目录
            if (this._operation == Operation.Create)
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
                dataRow["标注库编号"] = labelDbId;
                dataTable.Rows.Add(dataRow);
                return adapter.Update(dataTable) > 0;
            }
            //更新地震目录
            if (this._operation == Operation.Edit)
            {
                var dataRow = dataTable.Rows.Cast<DataRow>().ToList().Find(r => Convert.ToInt32(r["编号"]) == this._editedId);
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
                    Debug.Print("ERROR!DataRow is null!");
                }
            }
            return false;
        }
        
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            this.dateTimePicker2.Value = this.dateTimePicker1.Value;
        }
    }

    public enum Operation
    {
        Create,
        Edit,
    }
}
