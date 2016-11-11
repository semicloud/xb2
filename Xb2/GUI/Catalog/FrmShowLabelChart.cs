using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using Xb2.GUI.Main;
using Xb2.Utils;

namespace Xb2.GUI.Catalog
{
    /// <summary>
    /// 标地震的两种图
    /// </summary>
    public enum ChartType
    {
        /// <summary>
        /// 箭头图
        /// </summary>
        ArrowChart,

        /// <summary>
        /// 序列图
        /// </summary>
        SequenceChart
    }

    public partial class FrmShowLabelChart : FrmBase
    {
        //图类型
        private ChartType _chartType;
        //包含地震目录的数据表
        private DataTable _dataTable;

        /// <summary>
        /// 展示标地震图，该界面需要使用MSChart重写
        /// </summary>
        /// <param name="dataTable">包含地震目录的dataTable</param>
        /// <param name="chartType">图类型：标地震图或序列图</param>
        public FrmShowLabelChart(DataTable dataTable, ChartType chartType)
        {
            InitializeComponent();
            this._dataTable = RemoveUnSelectedDataRows(dataTable);
            this._chartType = chartType;
            if (this._chartType == ChartType.ArrowChart)
            {
                this.Text = "标地震";
            }
            if (this._chartType == ChartType.SequenceChart)
            {
                this.Text = "序列图";
            }
        }

        /// <summary>
        /// 删掉未选择的数据行
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private DataTable RemoveUnSelectedDataRows(DataTable dataTable)
        {
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dataTable.Rows[i]["选择"]) == 0)
                    {
                        dataTable.Rows[i].Delete();
                    }
                }
            }
            return dataTable;
        }

        private void FrmShowLabelChart_Load(object sender, EventArgs e)
        {
            //排序，顺带去除一下重复
            var dv = _dataTable.DefaultView;
            dv.Sort = "发震日期";
            _dataTable = dv.ToTable(true);
            Debug.Print("datatable count:" + _dataTable.Rows.Count);

            var minx = (DateTime) _dataTable.Compute("min(发震日期)", "");
            var maxx = (DateTime) _dataTable.Compute("max(发震日期)", "");
            var miny = (double) _dataTable.Compute("min(震级值)", "") - 1;
            var maxy = (double) _dataTable.Compute("max(震级值)", "") + 1;
            Debug.Print("min date:{0}, max date:{1}", minx.SStr(), maxx.ToShortDateString());
            Debug.Print("min magnitude:{0}, max magnitude:{1}", miny, maxy);

            chart1.Legends.Clear();
            chart1.BackColor = Color.Transparent;
            chart1.ChartAreas[0].BackColor = Color.Transparent;
            chart1.ChartAreas[0].AxisX.Title = "发震日期";
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy-MM-dd";
            chart1.ChartAreas[0].AxisX.Minimum = minx.AddMonths(-3).ToOADate();
            chart1.ChartAreas[0].AxisX.Maximum = maxx.AddMonths(3).ToOADate();
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisX.ArrowStyle = AxisArrowStyle.SharpTriangle;
            chart1.ChartAreas[0].AxisY.ArrowStyle = AxisArrowStyle.SharpTriangle;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = maxy;
            //下面的代码可以对ChartArea的位置进行设置，供参考
            //chart1.ChartAreas[0].Position.Auto = false;
            //chart1.ChartAreas[0].Position.X = 0;
            //chart1.ChartAreas[0].Position.Y = 5;
            //chart1.ChartAreas[0].Position.Height = 90;
            //chart1.ChartAreas[0].Position.Width = 90;

            chart1.Series[0].ChartType = SeriesChartType.Point;
            chart1.Series[0].ChartArea = chart1.ChartAreas[0].Name;
            chart1.Series[0].Points.DataBind(_dataTable.AsEnumerable(), "发震日期", "震级值", "");

            if (_chartType == ChartType.ArrowChart)
            {
                for (int i = 0; i < _dataTable.Rows.Count; i++)
                {
                    var arrowAnnotation = new LineAnnotation
                    {
                        ClipToChartArea = chart1.ChartAreas[0].Name,
                        EndCap = LineAnchorCapStyle.Arrow,
                        AxisX = chart1.ChartAreas[0].AxisX,
                        AxisY = chart1.ChartAreas[0].AxisY,
                        X = chart1.Series[0].Points[i].XValue,
                        Y = maxy,
                        LineColor = Color.Red,
                        Height = 6,
                        Width = 0
                    };
                    var textAnnotation = new TextAnnotation
                    {
                        ClipToChartArea = chart1.ChartAreas[0].Name,
                        AxisX = chart1.ChartAreas[0].AxisX,
                        AxisY = chart1.ChartAreas[0].AxisY,
                        X = chart1.Series[0].Points[i].XValue + 1,
                        Y = maxy,
                        Alignment = ContentAlignment.BottomLeft,
                        Text = _dataTable.Rows[i]["参考地点"] + "\n"
                               + Convert.ToDouble(_dataTable.Rows[i]["震级值"]).ToString("#0.0")
                    };
                    chart1.Annotations.Add(arrowAnnotation);
                    chart1.Annotations.Add(textAnnotation);
                }
            }
            chart1.Invalidate();
        }

        private void chart1_PostPaint(object sender, ChartPaintEventArgs e)
        {
            //唉，直接拿graphics撸一个吧
            if (_chartType == ChartType.SequenceChart)
            {
                chart1.Annotations.Clear();
                var y1 =  chart1.ChartAreas[0].AxisY.ValueToPixelPosition(0);
                for (int i = 0; i < chart1.Series[0].Points.Count;i++)
                {
                    var x1 = chart1.ChartAreas[0].AxisX.ValueToPixelPosition(chart1.Series[0].Points[i].XValue);
                    var y2 = y1 - chart1.Series[0].Points[i].YValues[0]*6;
                    e.ChartGraphics.Graphics.DrawLine(new Pen(Color.Red, 1.0f), (float) x1, (float) y1, (float) x1,
                        (float) y2);
                }
                chart1.Invalidate();
            }
        }
    }
}
