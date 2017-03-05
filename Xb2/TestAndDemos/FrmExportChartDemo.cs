using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Xb2.Computing.CoreAlgorithms.Entities;
using Xb2.Computing.CoreAlgorithms.Entities.Number;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.TestAndDemos
{
    public partial class FrmExportChartDemo : Form
    {
        public FrmExportChartDemo()
        {
            InitializeComponent();
        }

        private void FrmExportChartDemo_Load(object sender, EventArgs e)
        {
            var calcResult = new CalcResult();
            calcResult.NumericalTable = DaoObject.GetRawData(17);
            calcResult.Title = "石景山区玉泉西街1号院";
            var chart = ChartHelper.BuildChart(calcResult);
            chart.Size = new Size(500, 260);
            chart.ChartAreas[0].AxisX.Title = "观测日期";
            chart.ChartAreas[0].AxisY.Title = "观测值";
//            chart.AntiAliasing = AntiAliasingStyles.All;
//            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
            this.Controls.Add(chart);

            var oldChartSize = chart.Size;
            var oldTitleFont = chart.Titles[0].Font;
            var oldAxisTitleFont = chart.ChartAreas[0].AxisX.TitleFont;
            var oldAxisLabelFont = chart.ChartAreas[0].AxisX.LabelStyle.Font;
            var oldAxisLineWidth = chart.ChartAreas[0].AxisX.LineWidth;
            var oldAxisTickMarkLineWidth = chart.ChartAreas[0].AxisX.MajorTickMark.LineWidth;
            var oldAxisYIntervals = chart.ChartAreas[0].AxisY.Interval;
            var oldAxisXIntervals = chart.ChartAreas[0].AxisX.Interval;
            var oldSeriesBorderWidth = chart.Series[0].BorderWidth;

            var times = 10;

            chart.Size = new Size(chart.Width*times, chart.Height*times);
            chart.Titles[0].Font = new Font(oldTitleFont.Name, oldTitleFont.Size*times);
            chart.ChartAreas[0].AxisX.TitleFont = new Font(oldAxisTitleFont.Name, oldAxisTitleFont.Size*times);
            chart.ChartAreas[0].AxisY.TitleFont = new Font(oldAxisTitleFont.Name, oldAxisTitleFont.Size*times);
            chart.ChartAreas[0].AxisX.LabelStyle.Font = new Font(oldAxisLabelFont.Name, oldAxisLabelFont.Size*times);
            chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font(oldAxisLabelFont.Name, oldAxisLabelFont.Size*times);
            chart.ChartAreas[0].AxisX.LineWidth = oldAxisLineWidth*times;
            chart.ChartAreas[0].AxisY.LineWidth = oldAxisLineWidth*times;
            chart.ChartAreas[0].AxisX.MajorTickMark.LineWidth = oldAxisTickMarkLineWidth*times;
            chart.ChartAreas[0].AxisY.MajorTickMark.LineWidth = oldAxisTickMarkLineWidth*times;
            //坐标轴上的Label间距先不调整了
            //chart.ChartAreas[0].AxisX.Interval = oldAxisXIntervals/Math.Round(times/3.0);
            //chart.ChartAreas[0].AxisY.Interval = oldAxisYIntervals/Math.Round(times/3.0);
            chart.Series[0].BorderWidth = oldSeriesBorderWidth*times;
            chart.Invalidate();
            chart.SaveImage(@"C:\1.png", ChartImageFormat.Png);

            chart.Size = oldChartSize;
            chart.Titles[0].Font = oldTitleFont;
            chart.ChartAreas[0].AxisX.TitleFont = oldTitleFont;
            chart.ChartAreas[0].AxisY.TitleFont = oldTitleFont;
            chart.ChartAreas[0].AxisX.LabelStyle.Font = oldAxisLabelFont;
            chart.ChartAreas[0].AxisY.LabelStyle.Font = oldAxisLabelFont;
            chart.ChartAreas[0].AxisX.LineWidth = oldAxisLineWidth;
            chart.ChartAreas[0].AxisY.LineWidth = oldAxisLineWidth;
            chart.ChartAreas[0].AxisX.MajorTickMark.LineWidth = oldAxisTickMarkLineWidth;
            chart.ChartAreas[0].AxisY.MajorTickMark.LineWidth = oldAxisTickMarkLineWidth;
            chart.ChartAreas[0].AxisX.Interval = oldAxisXIntervals;
            chart.ChartAreas[0].AxisY.Interval = oldAxisYIntervals;
            chart.Series[0].BorderWidth = oldSeriesBorderWidth;

            chart.Invalidate();
        }
    }
}
