using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.GUI.Catalog;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.GUI.M.Val.Rawdata
{
    public partial class FrmEditRawData : Form
    {
        private Operation _operation;
        private DataRow _dataRow;
        private int _itemId;

        /// <summary>
        /// 新建原始数据
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="itemId"></param>
        public FrmEditRawData(Operation operation, int itemId)
        {
            InitializeComponent();
            this._operation = operation;
            if (operation == Operation.Create)
            {
                this.Text = "新建原始数据";
                this._itemId = itemId;
                this.label4.Text = itemId.ToString();
            }
        }

        /// <summary>
        /// 编辑原始数据
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="operation"></param>
        public FrmEditRawData(DataRow dataRow, Operation operation)
        {
            InitializeComponent();
            this._operation = operation;
            this._dataRow = dataRow;
            this._itemId = Convert.ToInt32(dataRow["测项编号"]);
            if (operation == Operation.Edit)
            {
                this.Text = "编辑原始数据";
            }
            this.label4.Text = this._itemId.ToString();
            this.label2.Text = dataRow["编号"].ToString();
            this.dateTimePicker1.Value = Convert.ToDateTime(dataRow["观测日期"]);
            this.textBox1.Text = dataRow["观测值"].ToString();
            this.textBox2.Text = dataRow["备注1"].ToString();
            this.textBox3.Text = dataRow["备注2"].ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            #region 输入验证

            if (textBox1.Text.Trim().Equals(""))
            {
                MessageBox.Show("请输入观测值！");
                return;
            }

            #endregion
            var date = dateTimePicker1.Value.Date;
            var value = Convert.ToDouble(textBox1.Text.Trim());
            var memo1 = textBox2.Text.GetStringOrDBNull();
            var memo2 = textBox3.Text.GetStringOrDBNull();
            if (this._operation == Operation.Create)
            {
                var sql = "select * from {0} where 测项编号={1}";
                sql = string.Format(sql, DbHelper.TnRData(), _itemId);
                var dt = new DataTable();
                var adapter = new MySqlDataAdapter(sql, DbHelper.ConnectionString);
                var builder = new MySqlCommandBuilder(adapter);
                adapter.Fill(dt);
                var row = dt.NewRow();
                row["测项编号"] = _itemId;
                row["观测日期"] = date;
                row["观测值"] = value;
                row["备注1"] = memo1;
                row["备注2"] = memo2;
                dt.Rows.Add(row);
                var n = adapter.Update(dt);
                if (n > 0)
                {
                    MessageBox.Show("保存成功！");
                    GetOwner().RefreshRawData(_itemId);
                    this.Close();
                }
            }
            if (this._operation == Operation.Edit)
            {
                var sql = "select * from {0} where 编号={1}";
                var id = Convert.ToInt32(label2.Text);
                sql = string.Format(sql, DbHelper.TnRData(), id);
                Debug.Print(sql);
                var dt = new DataTable();
                var adapter = new MySqlDataAdapter(sql, DbHelper.ConnectionString);
                var builder = new MySqlCommandBuilder(adapter);
                adapter.Fill(dt);
                var row = dt.Rows[0];
                row["观测日期"] = date;
                row["观测值"] = value;
                row["备注1"] = memo1;
                row["备注2"] = memo2;
                var n = adapter.Update(dt);
                if (n > 0)
                {
                    MessageBox.Show("保存成功！");
                    GetOwner().RefreshRawData(_itemId);
                    this.Close();
                }
            }
        }

        private FrmRawDataManage GetOwner()
        {
            return (FrmRawDataManage) this.Owner;
        }
    }
}
