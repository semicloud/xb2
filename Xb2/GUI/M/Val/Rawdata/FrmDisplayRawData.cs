using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Xb2.Utils;
using ExtendMethodDataTable = Xb2.Utils.ExtendMethod.ExtendMethodDataTable;

namespace Xb2.GUI.M.Val.Rawdata
{
    public partial class FrmDisplayRawData : Form
    {
        private string _fileName;
        private DataTable _dataTable;

        public DataTable DataTable
        {
            get { return _dataTable; }
            set { this._dataTable = value; }
        }

        public FrmDisplayRawData(string fileName)
        {
            InitializeComponent();
            this._fileName = fileName;
            this._dataTable = new DataTable();
            this._dataTable.Columns.Add("观测日期", typeof(DateTime));
            this._dataTable.Columns.Add("观测值", typeof(double));

            this.Load += FrmDisplayRawData_Load; 
        }

        void FrmDisplayRawData_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            string[] lines = System.IO.File.ReadAllLines(_fileName);
            foreach (string line in lines)
            {
                var date = DateTime.ParseExact(ParseValueFromString(line, 0, 8), "yyyyMMdd",
                    CultureInfo.CurrentCulture);
                var value = Convert.ToDouble(ParseValueFromString(line, 8, -1));
                var dataRow = this._dataTable.NewRow();
                dataRow["观测日期"] = date;
                dataRow["观测值"] = value;
                _dataTable.Rows.Add(dataRow);
            }
            var dt = ExtendMethodDataTable.IdentifyDataTable(_dataTable);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = dt;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Columns[0].FillWeight = 30;
            dataGridView1.Columns[1].FillWeight = 50;
            dataGridView1.Columns[2].FillWeight = 50;
            dataGridView1.Columns[1].DefaultCellStyle.Format = "yyyy/MM/dd";
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
        }

        /// <summary>
        /// 解析字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private static string ParseValueFromString(string str, int start, int len)
        {
            string scalar;
            if (len == -1)
                scalar = str.Substring(start).Trim();
            else
                scalar = str.Substring(start, len).Trim();
            return scalar.Equals("") ? "null" : scalar;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource != null)
            {
                this._dataTable = (DataTable) dataGridView1.DataSource;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        
    }
}
