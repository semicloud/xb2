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
        /// ����Ŀ¼��ע������
        /// </summary>
        private string m_labelDatabaseName;

        /// <summary>
        /// �������
        /// </summary>
        private Operation m_operation;

        /// <summary>
        /// �����������
        /// </summary>
        private DataRow m_dataRow;

        /// <summary>
        /// Ҫ�޸ĵĵ���Ŀ¼��Id
        /// </summary>
        private Int32 m_editedId;

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="dataRow">1������Ŀ¼��DataRow</param>
        /// <param name="dbname">��ע������</param>
        /// <param name="operation">��������</param>
        /// <param name="user">�û�����</param>
        public FrmCreateEditRecord(DataRow dataRow, string dbname, Operation operation,XbUser user)
        {
            InitializeComponent();
            this.m_operation = operation;
            this.m_dataRow = dataRow;
            this.m_labelDatabaseName = dbname;
            this.User = user;
            //�������ں�ʱ�䲻�������
            this.dateTimePicker1.Value = Convert.ToDateTime(dataRow["��������"]);
            dateTimePicker1.Enabled = false;
            this.dateTimePicker2.Value = Convert.ToDateTime(dataRow["��������"]);
            dateTimePicker2.Enabled = false;
            this.textBox1.Text = dataRow["γ��"].ToString();
            this.textBox2.Text = dataRow["����"].ToString();
            this.textBox3.Text = dataRow["��ֵ"].ToString();
            this.textBox4.Text = dataRow["�𼶵�λ"].ToString();
            this.textBox5.Text = dataRow["��λ����"].ToString();
            this.textBox6.Text = dataRow["�ο��ص�"].ToString();
            this.m_editedId = Convert.ToInt32(dataRow["���"]);
            Logger.Info("Ҫ��ĵĵ���Ŀ¼ID��" + this.m_editedId);
        }

        #region ȷ����ȡ����ť

        private void button1_Click(object sender, EventArgs e)
        {
            #region ������֤
            double t;
            //γ����֤
            if (this.textBox1.Text.Trim() == string.Empty)
            {
                MessageBox.Show("������γ�ȣ�");
                this.textBox1.Focus();
                return;
            }
            if (!double.TryParse(this.textBox1.Text.Trim(), out t))
            {
                MessageBox.Show("γ�ȱ���Ϊһ�����֣�");
                this.textBox1.Focus();
                return;
            }
            //������֤
            if (this.textBox2.Text.Trim() == string.Empty)
            {
                MessageBox.Show("�����뾭�ȣ�");
                this.textBox2.Focus();
                return;
            }
            if (!double.TryParse(this.textBox2.Text.Trim(), out t))
            {
                MessageBox.Show("���ȱ���Ϊһ�����֣�");
                this.textBox2.Focus();
                return;
            }
            //��ֵ��֤
            if (this.textBox3.Text.Trim() == string.Empty)
            {
                MessageBox.Show("��������ֵ��");
                this.textBox3.Focus();
                return;
            }
            if (!double.TryParse(this.textBox3.Text.Trim(), out t))
            {
                MessageBox.Show("��ֵ����Ϊһ�����֣�");
                this.textBox3.Focus();
                return;
            }
            //�ο��ص���֤
            if (this.textBox6.Text.Trim() == string.Empty)
            {
                MessageBox.Show("������ο��ص㣡");
                this.textBox6.Focus();
                return;
            }
            #endregion

            if (this.Process())
            {
                MessageBox.Show("�����ɹ���");
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
        /// ���»��ߴ�������Ŀ¼
        /// </summary>
        /// <returns></returns>
        private bool Process()
        {
            #region ��ȡ�������

            var datetime1 = this.dateTimePicker1.Value.Date;
            var datetime2 = new DateTime();
            if (m_operation == Operation.Create)
            {
                datetime2 = this.dateTimePicker2.Value;
            }
            if (m_operation == Operation.Edit)
            {
                var timespan = TimeSpan.Parse(m_dataRow["����ʱ��"].ToString());
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
            var commandText = "select * from {0} where ��ע����={1}";
            commandText = string.Format(commandText, DbHelper.TnLabelDbData(), labelDatabaseId);
            var adapter = new MySqlDataAdapter(commandText, DbHelper.ConnectionString);
            var builder = new MySqlCommandBuilder(adapter);
            builder.ConflictOption = ConflictOption.OverwriteChanges;
            var dataTable = new DataTable();
            adapter.Fill(dataTable);
            //��������Ŀ¼
            if (this.m_operation == Operation.Create)
            {
                DataRow dataRow = dataTable.NewRow();
                dataRow["��������"] = datetime1;
                dataRow["����ʱ��"] = datetime2;
                dataRow["γ��"] = latitude;
                dataRow["����"] = longitude;
                dataRow["��ֵ"] = magnitude;
                dataRow["�𼶵�λ"] = magnitudeUnit;
                dataRow["��λ����"] = locationParameter;
                dataRow["�ο��ص�"] = location;
                dataRow["��ע����"] = labelDatabaseId;
                dataTable.Rows.Add(dataRow);
                return adapter.Update(dataTable) > 0;
            }
            //���µ���Ŀ¼
            if (this.m_operation == Operation.Edit)
            {
                var dataRow =
                    dataTable.Rows.Cast<DataRow>().ToList().Find(r => Convert.ToInt32(r["���"]) == this.m_editedId);
                if (dataRow != null)
                {
                    dataRow["��������"] = datetime1;
                    dataRow["����ʱ��"] = datetime2;
                    dataRow["γ��"] = latitude;
                    dataRow["����"] = longitude;
                    dataRow["��ֵ"] = magnitude;
                    dataRow["�𼶵�λ"] = magnitudeUnit;
                    dataRow["��λ����"] = locationParameter;
                    dataRow["�ο��ص�"] = location;
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
                this.Text = "�½�����Ŀ¼";
            }
            if (this.m_operation == Operation.Edit)
            {
                this.Text = "�༭����Ŀ¼";
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            this.dateTimePicker2.Value = this.dateTimePicker1.Value;
        }
    }
}
