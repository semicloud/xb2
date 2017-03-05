using System;
using System.Data;
using System.Windows.Forms;
using Xb2.Entity;
using Xb2.Utils;
using Xb2.Utils.Database;
using Xb2.Utils.ExtendMethod;

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

        #region 确定按钮、保存或新建原始数据
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
            var memo1 = textBox2.Text.GetStringOrDbNull();
            var memo2 = textBox3.Text.GetStringOrDbNull();
            if (this._operation == Operation.Create)
            {
                var isCreated = DaoObject.CreateRawData(_itemId, date, value, memo1, memo2);
                if (isCreated)
                {
                    MessageBox.Show("保存成功！");
                    GetOwner().RefreshRawData(_itemId);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("新建测项原始数据失败！");
                }
            }
            if (this._operation == Operation.Edit)
            {
                var dataId = Convert.ToInt32(label2.Text);
                var isEditSaved = DaoObject.EditSaveRawData(_itemId, dataId, date, value, memo1, memo2);
                if (isEditSaved)
                {
                    MessageBox.Show("保存成功！");
                    GetOwner().RefreshRawData(_itemId);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("保存测项原始数据失败！");
                }
            }
        }
        #endregion

        #region 关闭按钮
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        private FrmRawDataManage GetOwner()
        {
            return (FrmRawDataManage) this.Owner;
        }
    }
}
