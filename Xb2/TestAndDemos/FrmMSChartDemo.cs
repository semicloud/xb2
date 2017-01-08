using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Accord.Math;
using MySql.Data.MySqlClient;
using Xb2.Algorithms.Numberical;
using Xb2.Utils;
using Xb2.Utils.Control;
using Xb2.Utils.Database;

namespace Xb2.TestAndDemos
{
    public partial class FrmMSChartDemo : Form
    {
        public FrmMSChartDemo()
        {
            InitializeComponent();
            chart1.MouseWheel += Chart1OnMouseWheel;
            chart1.MouseClick += Chart1OnMouseClick;
        }

        private int redPointCount;

        private void Chart1OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (chart1.HitTest(e.X, e.Y).ChartElementType == ChartElementType.DataPoint)
            {
                var pointIndex = chart1.HitTest(e.X, e.Y).PointIndex;
                var point = chart1.Series[0].Points[pointIndex];
                if (point.MarkerColor == Color.Blue)
                {
                    redPointCount++;
                    if (redPointCount <= 2)
                    {
                        point.MarkerColor = Color.Red;
                    }
                    if (redPointCount == 2)
                    {
                        var dialogResult = MessageBox.Show("确定使用这两个数据点进行等间距插值？", "提问",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (dialogResult == DialogResult.OK)
                        {
                            var targetPoints = chart1.Series[0].Points.ToList().FindAll(p => p.MarkerColor == Color.Red);
                            if (targetPoints.Count == 2)
                            {
                                targetPoints.Sort(
                                    (p1, p2) => DateTime.FromOADate(p1.XValue).CompareTo(DateTime.FromOADate(p2.XValue)));
                                var point1 = targetPoints[0];
                                var point2 = targetPoints[1];
                                Debug.Print("point1:{0},{1}, point2:{2},{3}", DateTime.FromOADate(point1.XValue),
                                    point1.YValues[0], DateTime.FromOADate(point2.XValue), point2.YValues[0]);
                                var interpolation = Processmethod.EqualDistanceInterpolation(targetPoints, 1);
                                var dt = chart1.GetTable();
                                interpolation.ForEach(d => dt.Rows.Add(d.Date, d.Value));
                                dt.AcceptChanges();
                                var dv = dt.DefaultView;
                                dv.Sort = "观测日期";
                                ChartHelper.BindChartWithData(chart1, dv.ToTable());
                                chart1.Series[0].Points.Apply(p => p.MarkerColor = Color.Blue);
                                redPointCount = 0;
                            }
                            else
                            {
                                MessageBox.Show("选取点错误！");
                            }
                        }
                    }
                }
                else if (point.MarkerColor == Color.Red)
                {
                    redPointCount --;
                    point.MarkerColor = Color.Blue;
                }
            }
        }

        private void Chart1OnMouseWheel(object sender, MouseEventArgs e)
        {
            chart1.Focus();
            //MessageBox.Show(e.Delta.ToString());
            if (e.Delta < 0)
            {
                chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
            }
            if (e.Delta > 0)
            {
                double xMin = chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                double xMax = chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum;
                double posXStart = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) - (xMax - xMin)/4;
                double posXFinish = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + (xMax - xMin)/4;
                chart1.ChartAreas[0].AxisX.ScaleView.Zoom(posXStart, posXFinish);
            }
        }

        private void FrmMSChartDemo_Load(object sender, System.EventArgs e)
        {
            var sql = "select 观测日期,观测值 from 系统_原始数据 where 测项编号=16 order by 观测日期";
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString(), sql).Tables[0];
            chart1.DataSource = dt;
            chart1.ChartAreas[0].AxisY.Minimum = (double) dt.Compute("min(观测值)", "") - 1;
            chart1.ChartAreas[0].AxisY.Maximum = (double) dt.Compute("max(观测值)", "") + 1;
            chart1.ChartAreas[0].AxisX.Minimum = Convert.ToDateTime(dt.Compute("min(观测日期)", "")).AddMonths(-1).ToOADate();
            chart1.ChartAreas[0].AxisX.Maximum = Convert.ToDateTime(dt.Compute("max(观测日期)", "")).AddMonths(1).ToOADate();
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy-MM-dd";
            chart1.ChartAreas[0].AxisY.LabelStyle.Format = "#0.00";
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.Series[0].Points.DataBind(dt.AsEnumerable(), "观测日期", "观测值","");
            chart1.Series[0].Points.Apply(p => p.ToolTip = DateTime.FromOADate(p.XValue).SStr() + "," + p.YValues[0]);
            chart1.Series[0].MarkerSize = 8;
            chart1.Series[0].MarkerStyle = MarkerStyle.Circle;
            chart1.Series[0].MarkerColor = Color.Blue;
        }
    }
}
