using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Utils.Control;
using Xb2.Utils.Database;

namespace Xb2.TestAndDemos
{
    public partial class FrmLagrangeInterpolationDemo : Form
    {
        public FrmLagrangeInterpolationDemo()
        {
            InitializeComponent();
        }

        private void FrmLagrangeInterpolationDemo_Load(object sender, System.EventArgs e)
        {
            var sql = "select 观测日期,观测值 from 系统_原始数据 where 测项编号=16 order by 观测日期";
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString(), sql).Tables[0];
            ChartHelper.BindChartWithData(chart1, dt);
            //chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
        }
    }
}
