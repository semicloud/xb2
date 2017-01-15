using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Entity;
using Xb2.Entity.Business;
using Xb2.GUI.Catalog;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.GUI.M.Item
{
    public partial class FrmEditMItem : FrmBase
    {
        private DataRow _dataRow;
        private Operation _operation;

        public FrmEditMItem(Operation operation, XbUser user)
        {
            this.InitializeComponent();
            this.User = user;
            this._operation = operation;
            if (operation == Operation.Create)
            {
                this.Text = "新建测项";
            }
            if (operation == Operation.Edit)
            {
                this.Text = "编辑测项";
            }
        }

        public FrmEditMItem(Operation operation, DataRow dataRow, XbUser user)
        {
            this.InitializeComponent();
            this.User = user;
            this._operation = operation;
            this._dataRow = dataRow;
            if (operation == Operation.Create)
            {
                this.Text = "新建测项";
            }
            if (operation == Operation.Edit)
            {
                this.Text = "编辑测项";
            }
            label2.Text = _dataRow["编号"].ToString();
            textBox1.Text = _dataRow["观测单位"].ToString();
            textBox2.Text = _dataRow["地名"].ToString();
            textBox3.Text = _dataRow["方法名"].ToString();
            textBox4.Text = _dataRow["测项名"].ToString();
            textBox5.Text = _dataRow["经度"].ToString();
            textBox6.Text = _dataRow["纬度"].ToString();
            textBox7.Text = _dataRow["数据单位"].ToString();
            textBox8.Text = _dataRow["观测信度"].ToString();
            textBox9.Text = _dataRow["倾向"].ToString();
            textBox10.Text = _dataRow["倾角"].ToString();
            textBox11.Text = _dataRow["测线与断层交角"].ToString();
            textBox12.Text = _dataRow["点位图名"].ToString();
            textBox13.Text = _dataRow["备注1"].ToString();
            textBox14.Text = _dataRow["备注2"].ToString();
            textBox15.Text = _dataRow["备注3"].ToString();
        }

        /// <summary>
        /// 确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, System.EventArgs e)
        {
            if (this._operation == Operation.Create)
            {
                var isSuccess = this.CreateMItem();
                MessageBox.Show(isSuccess ? "创建成功！" : "创建失败！");
            }
            if (this._operation == Operation.Edit)
            {
                var isSuccess = this.EditMItem();
                MessageBox.Show(isSuccess ? "编辑成功！" : "编辑失败！");
            }
        }

        /// <summary>
        /// 创建测项
        /// </summary>
        /// <returns></returns>
        private bool CreateMItem()
        {
            var sql = "select * from " + DbHelper.TnMItem();
            var adapter = new MySqlDataAdapter(sql, DbHelper.ConnectionString);
            var builder = new MySqlCommandBuilder(adapter);
            var dt = new DataTable();
            adapter.Fill(dt);
            var dr = dt.NewRow();
            dr["观测单位"] = textBox1.Text.Trim().GetStringOrDBNull();
            dr["地名"] = textBox2.Text.Trim().GetStringOrDBNull();
            dr["方法名"] = textBox3.Text.Trim().GetStringOrDBNull();
            dr["测项名"] = textBox4.Text.Trim().GetStringOrDBNull();
            dr["经度"] = textBox5.Text.Trim().GetDoubleOrDBNull();
            dr["纬度"] = textBox6.Text.Trim().GetDoubleOrDBNull();
            dr["数据单位"] = textBox7.Text.Trim().GetStringOrDBNull();
            dr["观测信度"] = textBox8.Text.Trim().GetDoubleOrDBNull();
            dr["倾向"] = textBox9.Text.Trim().GetStringOrDBNull();
            dr["倾角"] = textBox10.Text.Trim().GetStringOrDBNull();
            dr["测线与断层交角"] = textBox11.Text.Trim().GetStringOrDBNull();
            dr["点位图名"] = textBox12.Text.Trim().GetStringOrDBNull();
            dr["备注1"] = textBox13.Text.Trim().GetStringOrDBNull();
            dr["备注2"] = textBox14.Text.Trim().GetStringOrDBNull();
            dr["备注3"] = textBox15.Text.Trim().GetStringOrDBNull();
            dt.Rows.Add(dr);
            var n = adapter.Update(dt);
            return n > 0;
        }

        /// <summary>
        /// 保存编辑的测项
        /// </summary>
        /// <returns></returns>
        private bool EditMItem()
        {
            var sql = "select * from {0} where 编号={1}";
            var id = Convert.ToInt32(label2.Text.Trim());
            sql = string.Format(sql, DbHelper.TnMItem(), id);
            Debug.Print(sql);
            //生成adapter对象，省的写那么长的update语句了
            var adapter = new MySqlDataAdapter(sql, DbHelper.ConnectionString);
            var builder = new MySqlCommandBuilder(adapter);
            var dt = new DataTable();
            adapter.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("没找到要编辑的数据！保存失败！");
                return false;
            }
            var dr = dt.Rows[0];
            dr["观测单位"] = textBox1.Text.Trim().GetStringOrDBNull();
            dr["地名"] = textBox2.Text.Trim().GetStringOrDBNull();
            dr["方法名"] = textBox3.Text.Trim().GetStringOrDBNull();
            dr["测项名"] = textBox4.Text.Trim().GetStringOrDBNull();
            dr["经度"] = textBox5.Text.Trim().GetDoubleOrDBNull();
            dr["纬度"] = textBox6.Text.Trim().GetDoubleOrDBNull();
            dr["数据单位"] = textBox7.Text.Trim().GetStringOrDBNull();
            dr["观测信度"] = textBox8.Text.Trim().GetDoubleOrDBNull();
            dr["倾向"] = textBox9.Text.Trim().GetStringOrDBNull();
            dr["倾角"] = textBox10.Text.Trim().GetStringOrDBNull();
            dr["测线与断层交角"] = textBox11.Text.Trim().GetStringOrDBNull();
            dr["点位图名"] = textBox12.Text.Trim().GetStringOrDBNull();
            dr["备注1"] = textBox13.Text.Trim().GetStringOrDBNull();
            dr["备注2"] = textBox14.Text.Trim().GetStringOrDBNull();
            dr["备注3"] = textBox15.Text.Trim().GetStringOrDBNull();
            var n = adapter.Update(dt);
            return n > 0;
        }


        /// <summary>
        /// 取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Png文件(*.png)|*.png";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox12.Text = openFileDialog.FileName;
            }
        }
    }
}
