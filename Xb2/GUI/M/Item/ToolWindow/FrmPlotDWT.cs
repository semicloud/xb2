using System;

namespace Xb2.GUI.M.Item.ToolWindow
{
    /// <summary>
    /// 在折线图中创建点位图的示例，需要使用MSChart控件重做
    /// </summary>
    public partial class FrmPlotDWT : System.Windows.Forms.Form
    {
        public FrmPlotDWT()
        {
            this.InitializeComponent();
        }

        private void FrmPlotDWT_Load(object sender, EventArgs e)
        {
            //var sql = "select 观测日期,观测值 from 测值_测试 order by 观测日期";
            //var dataTable = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString(), sql).Tables[0];
            //var dateValues = DtUtils.RetrieveDateValues(dataTable);
            //var xz = this.GetXAxis(dateValues.Select(d => d.Date).Min(), dateValues.Select(d => d.Date).Max());
            //var yz = this.GetYAxis();
            //var line = new LineSeries {DataFieldX = "Date", DataFieldY = "Value", ItemsSource = dateValues, StrokeThickness = 1, Color = Color.Black.ToOxyColor()};
            //var image = Image.FromFile(@"Images\avatar-04.png");
            //ImageAnnotation annotation = new ImageAnnotation();
            //OxyImage oxyImage = null;
            //using (var stream = new MemoryStream())
            //{
            //    image.Save(stream, ImageFormat.Png);
            //    oxyImage = new OxyImage(stream.ToArray());
            //}
            //annotation.ImageSource = oxyImage;
            //annotation.Opacity = .8;
            //annotation.Interpolate = false;
            //annotation.Width = new PlotLength(.1, PlotLengthUnit.RelativeToPlotArea);
            //annotation.HorizontalAlignment = HorizontalAlignment.Center;
            //annotation.VerticalAlignment = VerticalAlignment.Middle;
            //annotation.X = new PlotLength(0.1, PlotLengthUnit.RelativeToPlotArea);
            //annotation.Y = new PlotLength(0.1, PlotLengthUnit.RelativeToPlotArea);
            //annotation.MouseDown += (obj, args) =>
            //{
            //    if (args.ChangedButton == OxyMouseButton.Left)
            //    {
            //        annotation.PlotModel.InvalidatePlot(false);
            //        args.Handled = true;
            //    }
            //};
            //annotation.MouseMove += (obj, args) =>
            //{
            //    if (args.ChangedButton == OxyMouseButton.Left)
            //    {
            //        annotation.X = new PlotLength(annotation.InverseTransform(args.Position).X, PlotLengthUnit.Data);
            //        annotation.Y = new PlotLength(annotation.InverseTransform(args.Position).Y, PlotLengthUnit.Data);
            //        annotation.PlotModel.InvalidatePlot(false);
            //        args.Handled = true;
            //    }
            //};
            //annotation.MouseUp += (obj, args) =>
            //{
            //    if (args.ChangedButton == OxyMouseButton.Left)
            //    {
            //        args.Handled = true;
            //    }
            //};

            //var model = this.GetPlotModel();
            //model.Axes.Add(xz);
            //model.Axes.Add(yz);
            //model.Series.Add(line);
            //model.Annotations.Add(annotation);
            //var plot = new Plot {Dock = DockStyle.Fill, Model = model};
            //this.Controls.Add(plot);

            //PngExporter.Export(model, "c://x.png", plot.Width, plot.Height, Brushes.Transparent);
        }



        //private PlotModel GetPlotModel()
        //{
        //    var model = new PlotModel()
        //    {
        //        //绘图区边框粗细设为0，即无边框
        //        //同时需设置Axis的LineThickness为1，且AxisLineStyle为Solid
        //        //这样就去掉了上面和右面的线
        //        PlotAreaBorderThickness = 0,
        //        PlotMargins = new OxyThickness(20, 20, 20, 20),
        //    };
        //    return model;
        //}

        ////获取X轴
        //private DateTimeAxis GetXAxis(DateTime minDate, DateTime maxDate)
        //{
        //    var xAxis = new DateTimeAxis()
        //    {
        //        Title = "观测日期",
        //        Position = AxisPosition.Bottom,
        //        AxislineThickness = 1,
        //        AxislineStyle = LineStyle.Solid,
        //        Minimum = DateTimeAxis.ToDouble(minDate),
        //        Maximum = DateTimeAxis.ToDouble(maxDate),
        //        IntervalLength = 40,
        //        IsPanEnabled = false,
        //        IsZoomEnabled = false
        //    };
        //    return xAxis;
        //}

        ////获取Y轴
        //private LinearAxis GetYAxis()
        //{
        //    var yAxis = new LinearAxis()
        //    {
        //        Position = AxisPosition.Left,
        //        AxislineThickness = 1,
        //        AxislineStyle = LineStyle.Solid,
        //        IsPanEnabled = false,
        //        IsZoomEnabled = false,
        //        StringFormat = "#0.00"
        //        //Minimum = 0,
        //        //Maximum = 1
        //    };
        //    return yAxis;
        //}

    }
}
