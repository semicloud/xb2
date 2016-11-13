using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Accord.Math;
using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using Xb2.Algorithms.Core;
using Xb2.Algorithms.Core.Entity;
using Xb2.Algorithms.Numberical;
using Xb2.Config;
using Xb2.GUI.M.Val.ProcessedData;
using XbApp.View.M.Value.ProcessedData;

namespace Xb2.Utils.Control
{
    /// <summary>
    /// Chart空间帮助类
    /// </summary>
    public static class ChartHelper
    {
        #region Chart控件的相关设置

        /// <summary>
        /// Chart控件纵坐标的扩充
        /// 如果没有扩充的话数据整好填充满Y轴
        /// </summary>
        private static readonly double Y_EXPAND = 2;

        /// <summary>
        /// Chart空间横坐标的扩充，单位是月
        /// </summary>
        private static readonly int X_EXPAND = 2;

        /// <summary>
        /// X轴刻度的格式化字符串
        /// </summary>
        private static readonly string X_FORMAT = "yyyy-MM-dd";

        /// <summary>
        /// Y轴刻度的格式化字符串
        /// </summary>
        private static readonly string Y_FORMAT = "0.00";

        /// <summary>
        /// 坐标轴箭头的样式
        /// </summary>
        private static readonly AxisArrowStyle ARROW_STYLE = AxisArrowStyle.None;

        /// <summary>
        /// 点标注的大小
        /// </summary>
        private static readonly int MARKER_SIZE =7;

        /// <summary>
        /// 点标注的颜色
        /// </summary>
        private static readonly Color MARKER_COLOR = Color.Blue;

        /// <summary>
        /// 点标注的样式
        /// </summary>
        private static readonly MarkerStyle MARKER_STYLE = MarkerStyle.Circle;

        /// <summary>
        /// 目标点（例如突跳点，即要处理的点）的样式
        /// </summary>
        private static readonly MarkerStyle TARGET_MARKER_STYLE = MarkerStyle.Triangle;

        /// <summary>
        /// 目标点的颜色
        /// </summary>
        private static readonly Color TARGET_MARKER_COLOR = Color.Red;

        /// <summary>
        /// X列的标题，需要与数据源dt中的列名相同，都是观测日期
        /// </summary>
        private static readonly string X_TITLE = "观测日期";

        /// <summary>
        /// Y列的标题，需要与数据源dt中的列名相同，都是观测值
        /// </summary>
        private static readonly string Y_TITLE = "观测值";

        /// <summary>
        /// Chart中绘制矩形的线的宽度（粗细）
        /// </summary>
        private static readonly int RECT_FITNESS = 2;

        /// <summary>
        /// Chart中绘制矩形的颜色
        /// </summary>
        private static readonly Color RECT_COLOR = Color.Green;

        #endregion

        #region 基本Chart控件和方法

        /// <summary>
        /// 获得一个模板Chart
        /// Tag属性被赋值为一个Stack of DataTable
        /// 用于Chart的撤销操作
        /// 该Chart中的ChartArea[0]的Tag被设置为一个
        /// List of String
        /// 用于记录用户的基础数据操作记录
        /// 该方法：
        /// 设置的Chart的堆栈对象，存储在Chart的Tag属性中
        /// 设置了Chart的各种基本属性，X轴，Y轴等
        /// </summary>
        /// <returns></returns>
        private static Chart GetTemplateChart()
        {
            //Chart对象设置
            var chart = new Chart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                //绑定用于撤销的数据源堆栈
                Tag = new Stack<DataTable>()
            };
            //ChartArea对象的设置
            {
                if (!chart.ChartAreas.Any()) chart.ChartAreas.Add(new ChartArea());
                chart.ChartAreas[0].BackColor = Color.Transparent;
                chart.ChartAreas[0].AxisX.LabelStyle.Format = X_FORMAT;
                chart.ChartAreas[0].AxisY.LabelStyle.Format = Y_FORMAT;
                chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                //chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Green;
                //chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Green;
                //chart.ChartAreas[0].AxisX.MinorGrid.Enabled = true;
                //chart.ChartAreas[0].AxisY.MinorGrid.Enabled = true;

                //chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                //chart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                //chart.ChartAreas[0].AxisX.MajorGrid.IntervalType = DateTimeIntervalType.Years;
                //chart.ChartAreas[0].AxisX.MajorGrid.Interval = 1;


                chart.ChartAreas[0].AxisX.ArrowStyle = ARROW_STYLE;
                chart.ChartAreas[0].AxisY.ArrowStyle = ARROW_STYLE;

                //绑定用于日志记录的List
                chart.ChartAreas[0].Tag = new List<string>();

                //chart.ChartAreas[0].BackColor = Color.LightGreen;
                //chart.BackColor = Color.LightBlue;
            }
            return chart;
        }

        public static Chart BindChartWithYAxisValue(Chart chart, DataTable dt, double[] yAxisMinMax)
        {
            //这个方法用于使用给定的最大最小值调整图件的坐标
            //根据用户的需求来处理吧
            return null;
        }

        /// <summary>
        /// 将Chart控件与数据源绑定
        /// 然后返回Chart控件
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Chart BindChartWithData(Chart chart, DataTable dt)
        {
            if (chart == null) throw new NullReferenceException("Chart not be null!");
            var maxy = (double) dt.Compute(string.Format("MAX({0})", Y_TITLE), "");
            var miny = (double) dt.Compute(string.Format("MIN({0})", Y_TITLE), "");
            var maxx = (DateTime) dt.Compute(string.Format("MAX({0})", X_TITLE), "");
            var minx = (DateTime) dt.Compute(string.Format("MIN({0})", X_TITLE), "");
            var sd = dt.GetColumnOfDouble("观测值").StandardDeviation();
            Debug.Print("观测值标准差：" + sd);
            chart.DataSource = dt;
            //Y_Expand的范围为上下一个标准差，因为有的时候数据中
            //有异常值，Y_Expand设置的太小就点不到数据点上 (⊙﹏⊙)b
            chart.ChartAreas[0].AxisY.Maximum = Convert.ToDouble(maxy) + sd;
            chart.ChartAreas[0].AxisY.Minimum = Convert.ToDouble(miny) - sd;
            chart.ChartAreas[0].AxisX.Maximum = Convert.ToDateTime(maxx).AddMonths(X_EXPAND).ToOADate();
            chart.ChartAreas[0].AxisX.Minimum = Convert.ToDateTime(minx).AddMonths(-X_EXPAND).ToOADate();
            if (!chart.Series.Any()) chart.Series.Add(new Series());
            chart.Series[0].ChartType = SeriesChartType.Line;
            chart.Series[0].XValueType = ChartValueType.Date;
            chart.Series[0].YValueType = ChartValueType.Double;
            chart.Series[0].XValueMember = X_TITLE;
            chart.Series[0].YValueMembers = Y_TITLE;
            chart.Series[0].Color = Color.Black;
            chart.Series[0].MarkerStyle = MARKER_STYLE;
            chart.Series[0].MarkerColor = MARKER_COLOR;
            chart.Series[0].MarkerSize = MARKER_SIZE;
            chart.DataBind();
            chart.Series[0].Points.Apply(p => p.ToolTip = DateTime.FromOADate(p.XValue).SStr() + p.YValues[0]);
            return chart;
        }

        /// <summary>
        /// 获取一个默认的Chart，使用数据源dt进行数据绑定
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <returns></returns>
        private static Chart GetDefaultChart(DataTable dt)
        {
            return BindChartWithData(GetTemplateChart(), dt);
        }

        /// <summary>
        /// 突跳处理→拟合
        /// 务必注意该函数的参数，是一个回归方法的委托
        /// 突跳处理的回归拟合都要使用这个函数
        /// 提高了复用性，如果需要修改的话也只该这一个函数
        /// 就好了
        /// </summary>
        /// <param name="regressionMethod">
        /// 回归方法的委托，见Processmethod类
        /// </param>
        /// <returns></returns>
        private static Chart GetRegressionVirtualChart(Func<List<DataPoint>, string> regressionMethod,string processName)
        {
            var baseChart = GetTemplateChart();
            //目标点
            DataPoint targetPoint = null;
            //绘制的矩形框
            var rectanges = new List<Rectangle>();
            //鼠标起止点，用于控制绘制矩形
            Point startPos = new Point();
            Point currentPos = new Point();
            //正在绘制矩形的标识
            bool isDrawing = false;
            //矩形选中的数据点，用于拟合
            var rectPoints = new List<DataPoint>();
            //用于控制Y移动的最大坐标，超过此值则会引发异常
            double maxYPos = 0;
            double minYPos = 0;
            //用于记录日志操作的数据点，初始数据点
            DataPoint initPoint = null;
            baseChart.MouseDown += delegate(object sender, MouseEventArgs e)
            {
                //左键才可以画矩形
                if (e.Button != MouseButtons.Left) return;
                var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                //如果点击到绘图区，并且矩形数量少于2个，则可以开始绘制矩形
                if (ht.ChartElementType == ChartElementType.PlottingArea)
                {
                    if (rectanges.Count >= 2) return;
                    currentPos = startPos = e.Location;
                    isDrawing = true;
                }
                if (targetPoint != null) targetPoint = null;
                //点击类型是数据点，且已经绘制了2个矩形，则将该点设为目标点
                if (ht.ChartElementType == ChartElementType.DataPoint && rectanges.Count == 2)
                {
                    targetPoint = baseChart.Series[0].Points[ht.PointIndex];
                    initPoint = baseChart.Series[0].Points[ht.PointIndex];
                    //当前数据源入栈，供撤销
                    var stack = baseChart.GetStack();
                    stack.Push(baseChart.GetTable().Copy());
                    LogPushStack(stack);
                }
            };

            baseChart.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                if (e.Button != MouseButtons.Left) return;
                if (e.Y >= minYPos && e.Y <= maxYPos)
                {
                    //画矩形
                    currentPos = e.Location;
                    if (isDrawing) baseChart.Invalidate();
                    //绘制数据点标签
                    if (targetPoint != null && e.Button == MouseButtons.Left)
                    {
                        targetPoint.YValues[0] = baseChart.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);
                        targetPoint.YValues[0] = Math.Round(targetPoint.YValues[0], Xb2Config.GetPrecision());
                        if (rectPoints.Count > 0)
                        {
                            //目标点加入回归数据点集合，然后对这些数据进行排序和去重
                            rectPoints.Add(targetPoint);
                            rectPoints.Sort((p1, p2) => p1.XValue.CompareTo(p2.XValue));
                            rectPoints = rectPoints.Distinct().ToList();
                            //调用数据处理方法进行回归
                            targetPoint.Label = regressionMethod(rectPoints);
                            //强制重绘Chart控件
                            baseChart.Invalidate();
                        }
                    }
                }
            };

            baseChart.MouseUp += delegate(object sender, MouseEventArgs e)
            {
                if (isDrawing)
                {
                    isDrawing = false;
                    var rc = GetRect(startPos, currentPos);
                    if (rc.Width > 0 && rc.Height > 0) rectanges.Add(rc);
                    if (rectanges.Count == 2)
                    {
                        var dialogResult = MessageBox.Show("确定使用这些数据进行操作吗？", "提问", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes)
                        {
                            rectanges.Clear();
                            baseChart.Invalidate();
                            return;
                        }
                        var points1 = baseChart.Series[0].Points.ContainsBy(rectanges[0], baseChart.ChartAreas[0]);
                        var points2 = baseChart.Series[0].Points.ContainsBy(rectanges[1], baseChart.ChartAreas[0]);
                        if (points1.Count == 0 || points2.Count == 0)
                        {
                            MessageBox.Show("选取数据不完整，无法进行线性拟合！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            rectanges.Clear();
                            baseChart.Invalidate();
                            return;
                        }
                        rectPoints.AddRange(points1);
                        rectPoints.AddRange(points2);
                        Debug.Print("selectedDataPointCount:" + rectPoints.Count);
                        rectPoints.ForEach(p => Debug.Print("x:{0}, y:{1}", p.XValue, p.YValues[0]));
                    }
                }

                if (targetPoint != null)
                {
                    //更新数据员
                    var index = baseChart.Series[0].Points.IndexOf(targetPoint);
                    baseChart.GetTable().Rows[index][Y_TITLE] = targetPoint.YValues[0];
                    baseChart.GetTable().AcceptChanges();
                    baseChart.DataBind();
                    //记录日志
                    var logger = baseChart.GetLogger();
                    logger.Add(processName + DateValue.Desc(initPoint.ToDateValue(), targetPoint.ToDateValue()));

                    //数据点置空
                    targetPoint.Label = null;
                    targetPoint = null;
                    initPoint = null;
                }
            };

            baseChart.Paint += delegate(object sender, PaintEventArgs e)
            {
                //绘制已有的矩形
                if (rectanges.Count > 0)
                {
                    e.Graphics.DrawRectangles(new Pen(RECT_COLOR, RECT_FITNESS), rectanges.ToArray());
                }
                //绘制正在拖动，正在画的矩形
                if (isDrawing)
                {
                    e.Graphics.DrawRectangle(new Pen(RECT_COLOR, RECT_FITNESS), GetRect(startPos, currentPos));
                }
            };

            baseChart.PostPaint += (sender, e) =>
            {
                maxYPos = baseChart.ChartAreas[0].AxisY.ValueToPixelPosition(baseChart.ChartAreas[0].AxisY.Minimum);
                minYPos = baseChart.ChartAreas[0].AxisY.ValueToPixelPosition(baseChart.ChartAreas[0].AxisY.Maximum);
            };
            return baseChart;
        }

        /// <summary>
        /// 台阶处理→多点平均趋势处理
        /// 务必注意该函数的参数，是一个回归方法的委托
        /// 台阶处理的回归拟合都要使用这个函数
        /// 提高了复用性，如果需要修改的话也只该这一个函数
        /// 就好了
        /// </summary>
        /// <param name="regressionMethod"></param>
        /// <returns></returns>
        private static Chart GetMultiPointRegressionVirtualChart(Func<List<DataPoint>, string> regressionMethod)
        {
            var baseChart = GetTemplateChart();
            //文本标记，用于显示平均值和差值
            var annotationName = "Result Annotation";
            var annotation = new TextAnnotation
            {
                Name = annotationName,
                Font = new Font("宋体", 10, FontStyle.Bold),
                IsMultiline = true,
                AllowMoving = true,
                Alignment = ContentAlignment.MiddleLeft
            };
            baseChart.Annotations.Add(annotation);
            //鼠标位置记录变量
            var startPos = new Point();
            var currentPos = new Point();
            //Y轴最大最小值设置，防止鼠标移动超过此值从而触发异常
            double minYPos = 0, maxYPos = 0;
            var drawing = false;
            //绘制的矩形
            var rectangles = new List<Rectangle>();
            //矩形中的数据点，索引为0表示第1次绘制的矩形包括的数据点，1代表第2次
            var rectangleDataPoints = new Dictionary<int, List<DataPoint>>();
            //当前正在移动的矩形的索引值
            var movingRectangleIndex = -1;
            //绘制矩形的画笔
            var rectPen = new Pen(Color.Red, 2);

            /* Mouse Click方法的主要作用结束多点平均法的处理
             * 在绘图区已经有2个矩形的情况下（表明用户已经进行了台阶处理）
             * 用户在非矩形绘制区双击鼠标，提示是否结束台阶处理
             * 如果用户决定结束台阶处理，则将用户编辑的数据点加入数据源，
             * 并更新数据源，然后清空绘制的矩形和文本标记以及重置相关变量
             * 最后重绘Chart控件
             */
            baseChart.MouseClick += (sender, e) =>
            {
                if (rectangles.Count == 2)
                {
                    var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                    //不是绘图区就返回
                    if (ht.ChartElementType != ChartElementType.PlottingArea) return;
                    //在非矩形区域单击才可以结束台阶处理
                    if (rectangles[0].Contains(e.Location) || rectangles[1].Contains(e.Location)) return;
                    var message = MessageBox.Show("完成本次台阶处理吗？", "提问", MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question);
                    if (message != DialogResult.OK) return;
                    // 将用户编辑后的数据更新至数据源
                    var dt = baseChart.GetTable();
                    var points1 = rectangleDataPoints[0];
                    var points2 = rectangleDataPoints[1];
                    // 矩形1的更新
                    foreach (var dataPoint in points1)
                    {
                        var index = baseChart.Series[0].Points.IndexOf(dataPoint);
                        var value = Math.Round(dataPoint.YValues[0], Xb2Config.GetPrecision());
                        dt.Rows[index][Y_TITLE] = value;
                        Debug.Print("update datatable, data point index:{0}, value:{1}", index, value);
                    }
                    // 矩形2的更新
                    foreach (var dataPoint in points2)
                    {
                        var index = baseChart.Series[0].Points.IndexOf(dataPoint);
                        var value = Math.Round(dataPoint.YValues[0], Xb2Config.GetPrecision());
                        dt.Rows[index][Y_TITLE] = value;
                        Debug.Print("update datatable, data point index:{0}, value:{1}", index, value);
                    }
                    // 数据源接受数据
                    dt.AcceptChanges();
                    // 相关变量的重置
                    rectangles.Clear();
                    rectangleDataPoints.Clear();
                    annotation.Visible = false;
                    movingRectangleIndex = -1;
                    baseChart.Invalidate();
                }
            };

            //该事件负责计算Y坐标的最大最小值，防止鼠标移动超过最大范围而引发异常
            baseChart.PostPaint += delegate(object sender, ChartPaintEventArgs e)
            {
                maxYPos = baseChart.ChartAreas[0].AxisY.ValueToPixelPosition(baseChart.ChartAreas[0].AxisY.Minimum);
                minYPos = baseChart.ChartAreas[0].AxisY.ValueToPixelPosition(baseChart.ChartAreas[0].AxisY.Maximum);
            };

            baseChart.Paint += (sender, e) =>
            {
                //鼠标拖动时的处理
                if (drawing)
                {
                    e.Graphics.DrawRectangle(rectPen, GetRect(startPos, currentPos));
                }
                if (rectangles.Count > 0)
                    e.Graphics.DrawRectangles(rectPen, rectangles.ToArray());
            };

            /**
             * Mouse Down 事件主要负责相关变量的初始化和数据源的入栈
             */
            baseChart.MouseDown += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    //更新当前坐标
                    currentPos = startPos = e.Location;
                    if (rectangles.Count == 2)
                    {
                        //在已经绘制好两个矩形的情况下，如果鼠标落下在任意一个矩形区域内
                        //则表明用户开始进行台阶处理
                        //这时将数据源入栈，供撤销操作
                        if (rectangles[0].Contains(e.Location) || rectangles[1].Contains(e.Location))
                        {
                            var rectangle = rectangles.Find(r => r.Contains(e.Location));
                            //选中矩形 重要！
                            movingRectangleIndex = rectangles.IndexOf(rectangle);
                            var stack = baseChart.GetStack();
                            stack.Push(baseChart.GetTable().Copy());
                            LogPushStack(stack);
                        }
                    }
                    else
                    {
                        //如果两个矩形还没有绘制好，则继续绘制矩形
                        var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                        if (ht.ChartElementType == ChartElementType.PlottingArea)
                        {
                            drawing = true;
                        }
                    }
                }
            };

            /**
             * 这个Mouse Move事件可是太重要了
             */
            baseChart.MouseMove += (sender, e) =>
            {
                if (e.Button != MouseButtons.Left) return;
                if (e.Y >= minYPos && e.Y <= maxYPos)
                {
                    currentPos = e.Location;
                    //绘制矩形
                    if (drawing)
                    {
                        baseChart.Invalidate();
                        return;
                    }
                    //矩形绘制完毕，并且选中了1个矩形，则开始计算相关数据点的值
                    if (rectangles.Count == 2 && movingRectangleIndex != -1)
                    {
                        //确定移动的矩形
                        var movingRectangle = rectangles[movingRectangleIndex];
                        //如果鼠标位于移动的矩形内
                        if (movingRectangle.Contains(e.Location))
                        {
                            //矩形的移动量
                            var pixelOffset = currentPos.Y - startPos.Y;
                            //数据点的移动量
                            var dataOffset = baseChart.ChartAreas[0].AxisY.PixelPositionToValue(currentPos.Y)
                                             - baseChart.ChartAreas[0].AxisY.PixelPositionToValue(startPos.Y);
                            //设置矩形和数据点的新位置
                            movingRectangle.Y = movingRectangle.Y + pixelOffset;
                            //由于Rectangle是结构体，为值传递，需要将更改的元素直接赋值回去，又是一个按值传递的实例呀~
                            rectangles[movingRectangleIndex] = movingRectangle;
                            //更新移动矩形中的数据点的值
                            var movingRectanglePoints = rectangleDataPoints[movingRectangleIndex];
                            movingRectanglePoints.ForEach(pt => pt.YValues[0] += dataOffset);
                            //找到另一个没有移动的矩形，获取其中的数据点，并计算平均值
                            var standRectangleIndex = movingRectangleIndex == 0 ? 1 : 0;
                            var standRectanglePoints = rectangleDataPoints[standRectangleIndex];
                            //! 这里可以进行下一步操作，线性回归等
                            var pointsToLinearRegression = new List<DataPoint>();
                            pointsToLinearRegression.AddRange(standRectanglePoints);
                            pointsToLinearRegression.AddRange(movingRectanglePoints);
                            pointsToLinearRegression.Sort((p1, p2) => p1.XValue.CompareTo(p2.XValue));
                            // 显示文本注解
                            var anno = (TextAnnotation) baseChart.Annotations.FindByName(annotationName);
                            if (anno != null)
                            {
                                if (!anno.Visible) anno.Visible = true;
                                anno.AnchorDataPoint = movingRectanglePoints.Last();
                                //! 在这里调用了回归方法
                                anno.Text = regressionMethod(pointsToLinearRegression);
                            }
                            startPos = e.Location;
                        }
                        baseChart.Invalidate();
                    }
                }
            };

            baseChart.MouseUp += (sender, e) =>
            {
                if (drawing)
                {
                    drawing = false;
                    //获得当前绘制的矩形
                    var rc = GetRect(startPos, currentPos);
                    //在矩形数小于2的情况下加入矩形列表
                    if (rc.Width > 0 && rc.Height > 0 && rectangles.Count < 2)
                    {
                        rectangles.Add(rc);
                    }
                    //两个矩形绘制完成后，提取各自包括的数据点
                    if (rectangles.Count == 2)
                    {
                        var rectangle1 = rectangles[0];
                        var rectangle2 = rectangles[1];
                        var points1 = baseChart.Series[0].Points.ContainsBy(rectangle1, baseChart.ChartAreas[0]);
                        var points2 = baseChart.Series[0].Points.ContainsBy(rectangle2, baseChart.ChartAreas[0]);
                        // 非法情况的处理
                        if (points1.Count == 0 || points2.Count == 0 || rectangle1.IntersectsWith(rectangle2))
                        {
                            MessageBox.Show("没有选到数据，且矩形不许相交！请重新绘制！");
                            rectangles.Clear();
                            rectangleDataPoints.Clear();
                            annotation.Visible = false;
                            movingRectangleIndex = -1;
                            baseChart.Invalidate();
                            return;
                        }
                        rectangleDataPoints.Clear();
                        rectangleDataPoints.Add(0, points1);
                        rectangleDataPoints.Add(1, points2);
                    }
                    movingRectangleIndex = -1;
                    baseChart.Invalidate();
                }
            };

            return baseChart;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 根据两点坐标获取矩形
        /// </summary>
        /// <param name="startPos">起始点</param>
        /// <param name="currentPos">当前点</param>
        /// <returns></returns>
        private static Rectangle GetRect(Point startPos, Point currentPos)
        {
            var rect = new Rectangle(
                Math.Min(startPos.X, currentPos.X),
                Math.Min(startPos.Y, currentPos.Y),
                Math.Abs(startPos.X - currentPos.X),
                Math.Abs(startPos.Y - currentPos.Y));
            return rect;
        }

        /// <summary>
        /// 记录入栈情况
        /// </summary>
        /// <param name="stack"></param>
        private static void LogPushStack(Stack<DataTable> stack)
        {
            Debug.Print("push datatable to stack, current stack size {0}", stack.Count);
        }

        /// <summary>
        /// 获取基础数据处理的日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        private static string GetLog(String log)
        {
            return DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + ":" + log;
        }

        #endregion

        #region 用于基础数据处理的Chart控件

        /// <summary>
        /// 获取一个没有事件，没有数据的普通Chart
        /// </summary>
        /// <returns></returns>
        public static Chart GetOrdinaryChart()
        {
            return GetTemplateChart();
        }

        #region 突跳处理

        /// <summary>
        /// 突跳处理->图解法
        /// 对应的Chart控件
        /// 注意：此处处理方法的Chart控件都没有绑定数据
        /// 进行的处理是
        /// 1.获取一个模板Chart
        /// 2.为该模板Chart添加鼠标事件，以支持基础数据处理
        /// 绑定数据的操作是在用于实际处理数据时进行的
        /// 详见FrmProcessData类
        /// </summary>
        /// <returns></returns>
        public static Chart GetPointMovingChart()
        {
            //获取模板chart，但并没有绑定数据，你看，该方法也没有数据源参数传进来
            var baseChart = GetTemplateChart();
            //选中的数据点
            DataPoint testPoint = null;
            //用于记录操作日志的点
            DateValue initDateValue = null;
            DateValue editedDateValue = null;
            //控制点
            double maxYPos = 0;
            double minYPos = 0;
            //检测鼠标按下的区域是否是数据点，如果是数据点，则将该点赋值给testpoint
            //并设置testpoint的自定义属性i为数据点的索引值
            //然后，将当前chart的数据源（DataTable）入栈，用于撤销操作
            baseChart.MouseDown += delegate(object sender, MouseEventArgs e)
            {
                if (e.Button != MouseButtons.Left) return;
                var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                var stack = (Stack<DataTable>) baseChart.Tag;
                if (ht.ChartElementType == ChartElementType.DataPoint)
                {
                    //找到点击的数据点
                    testPoint = baseChart.Series[0].Points[ht.PointIndex];
                    initDateValue = new DateValue(DateTime.FromOADate(testPoint.XValue), testPoint.YValues[0]);
                    //将当前的数据源入栈，注意Copy哦，这可是活生生的引用传递和值传递的例子啊
                    stack.Push(baseChart.GetTable().Copy());
                    Debug.Print("push datatable to stack, current stack size {0}", stack.Count);
                }
            };

            //鼠标移动时，根据鼠标的像素位置计算testpoint数据点的Y值
            //同时获取testpoint的Lable，并刷新Chart控件
            baseChart.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                if (testPoint == null) return;
                if (e.Button != MouseButtons.Left) return;
                //鼠标必须在最大范围内移动才行
                if (e.Y >= minYPos && e.Y <= maxYPos)
                {
                    testPoint.YValues[0] = baseChart.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);
                    testPoint.Label = testPoint.GetLabel();
                    baseChart.Invalidate();
                }
            };

            baseChart.PostPaint += (sender, e) =>
            {
                maxYPos = baseChart.ChartAreas[0].AxisY.ValueToPixelPosition(baseChart.ChartAreas[0].AxisY.Minimum);
                minYPos = baseChart.ChartAreas[0].AxisY.ValueToPixelPosition(baseChart.ChartAreas[0].AxisY.Maximum);
            };

            //鼠标松开时，获取testpoint的自定义属性，即编辑点的索引值
            //根据该索引值找到Chart数据源中对应的值，并更改这个值
            //这样做的原因是Chart和DataTable之间，我没有发现双向更新机制机制
            //如果不这样做的话，编辑后的数据点的值是无法保存回数据源中的
            baseChart.MouseUp += delegate(object sender, MouseEventArgs e)
            {
                if (testPoint != null)
                {
                    //获取修改点的索引和编辑后的值
                    //修改datatable中相应的值
                    //否则datatable中的值不会更新
                    var index = baseChart.Series[0].Points.IndexOf(testPoint);
                    var editedValue = Math.Round(testPoint.YValues[0], Xb2Config.GetPrecision());
                    baseChart.GetTable().Rows[index][Y_TITLE] = editedValue;
                    baseChart.GetTable().AcceptChanges();
                    baseChart.DataBind();
                    editedDateValue = new DateValue(DateTime.FromOADate(testPoint.XValue), editedValue);
                    baseChart.GetLogger().Add(GetLog("突跳处理（图解法），" + initDateValue + "→" + editedDateValue));
                    //去掉label，不加这一句label会始终显示
                    initDateValue = null;
                    editedDateValue = null;
                    testPoint.Label = null;
                    testPoint = null;
                }
            };

            return baseChart;
        }

        /// <summary>
        /// 突跳处理->删除法
        /// 对应的Chart控件
        /// </summary>
        /// <returns></returns>
        public static Chart GetPointDeleteChart()
        {
            var baseChart = GetTemplateChart();
            //选中的数据点
            DataPoint testPoint;

            baseChart.MouseClick += delegate(object sender, MouseEventArgs e)
            {
                if (e.Button != MouseButtons.Left) return;
                var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                var stack = (Stack<DataTable>) baseChart.Tag;
                if (ht.ChartElementType == ChartElementType.DataPoint)
                {
                    //找到点击的数据点
                    testPoint = baseChart.Series[0].Points[ht.PointIndex];
                    //用户确认是否删除该数据点
                    var question = string.Format("确定删除点{0},{1}吗？", testPoint.GetDateStr(), testPoint.GetValue());
                    var dialog = MessageBox.Show(question, "提问", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialog == DialogResult.Yes)
                    {
                        //用户确认删除后，在删除该数据之前，先将数据源推入堆栈，用于撤销
                        //将当前的数据源入栈，注意Copy哦，这可是活生生的引用传递和值传递的例子啊
                        stack.Push(baseChart.GetTable().Copy());
                        Debug.Print("push datatable to stack, current stack size {0}", stack.Count);
                        //获取要删除点的索引
                        var index = baseChart.Series[0].Points.IndexOf(testPoint);
                        Debug.Print("Index:{0}", index);
                        //在数据源中删除数据，并且要AcceptChanges，否则你可以试试
                        baseChart.GetTable().Rows[index].Delete();
                        baseChart.GetTable().AcceptChanges();
                        //重新绑定数据源
                        baseChart.DataBind();
                        //加入日志
                        baseChart.GetLogger().Add("突跳处理（删除法），删除点：" + testPoint.ToDateValue());
                    }
                    testPoint = null;
                }
            };

            return baseChart;
        }

        /// <summary>
        /// 突跳处理->平均值法
        /// 对应的Chart控件
        /// 所谓平均值法，我认为可以分为4步
        /// 1.先选择一个突跳点，称为目标点
        /// 2.在突跳点的前后（或其他位置），各绘制一个矩形区域
        /// 3.计算这两个矩形区域内的数据的平均值
        /// 4.将目标点的数据值设置为该平均值
        /// </summary>
        /// <returns></returns>
        public static Chart GetAvgMethodChart()
        {
            var baseChart = GetTemplateChart();
            //要处理的突跳点，即目标点
            DataPoint targetPoint = null;
            DataPoint initPoint = null;
            //绘制的矩形框
            var rectanges = new List<Rectangle>();
            //鼠标起止点，用于控制绘制矩形
            Point startPos = new Point();
            Point currentPos = new Point();
            //正在绘制矩形的标识
            bool isDrawing = false;
            //是否可以绘制矩形的标识
            bool canDrawRect = false;

            baseChart.MouseClick += delegate(object sender, MouseEventArgs e)
            {
                if (e.Button != MouseButtons.Left) return;
                //如果已经有目标点，则不再设置目标点
                if (targetPoint != null) return;
                var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                if (ht.ChartElementType == ChartElementType.DataPoint)
                {
                    //找到目标点
                    targetPoint = baseChart.Series[0].Points[ht.PointIndex];
                    initPoint = baseChart.Series[0].Points[ht.PointIndex];
                    //用户确认是否使用平均法处理该数据点，如果用户取消，需要将testPoint重新置空，用于用户再次选择
                    var question = string.Format("确定使用平均法处理点{0},{1}吗？", targetPoint.GetDateStr(), targetPoint.GetValue());
                    var dialog = MessageBox.Show(question, "提问", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialog == DialogResult.Yes)
                    {
                        targetPoint.MarkerStyle = TARGET_MARKER_STYLE;
                        targetPoint.MarkerColor = TARGET_MARKER_COLOR;
                        canDrawRect = true;
                    }
                    else
                    {
                        targetPoint = null;
                    }

                }
            };

            baseChart.MouseDown += delegate(object sender, MouseEventArgs e)
            {
                //最多画两个矩形
                if (rectanges.Count >= 2) return;
                //左键才可以画矩形
                if (e.Button != MouseButtons.Left) return;
                if (!canDrawRect) return;
                var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                if (ht.ChartElementType == ChartElementType.PlottingArea)
                {
                    currentPos = startPos = e.Location;
                    isDrawing = true;
                }
            };

            baseChart.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                //画矩形
                currentPos = e.Location;
                if (isDrawing) baseChart.Invalidate();
            };

            baseChart.MouseUp += delegate(object sender, MouseEventArgs e)
            {
                if (isDrawing)
                {
                    isDrawing = false;
                    var rc = GetRect(startPos, currentPos);
                    if (rc.Width > 0 && rc.Height > 0) rectanges.Add(rc);
                    //矩形绘制完毕，开始计算平均值
                    if (rectanges.Count == 2)
                    {
                        var dialogResult = MessageBox.Show("计算平均值？", "提问", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                        //提问用户是否使用绘制的矩形计算平均值，如果用户选择“否”
                        //就把矩形清空，由用户重新绘制
                        if (dialogResult != DialogResult.Yes)
                        {
                            rectanges.Clear();
                            baseChart.Invalidate();
                            baseChart.Cursor = Cursors.Cross;
                            return;
                        }
                        //用户选择“是”，则判断两个矩形中是否选取到了点，只要一个矩形选取不到点，就提示出错
                        //矩形清空，然后由用户重新绘制
                        var points1 = baseChart.Series[0].Points.ContainsBy(rectanges[0], baseChart.ChartAreas[0]);
                        var points2 = baseChart.Series[0].Points.ContainsBy(rectanges[1], baseChart.ChartAreas[0]);
                        //以下4个变量用于记录操作日志
                        var date1 = points1.First().ToDateValue().Date;
                        var date2 = points1.Last().ToDateValue().Date;
                        var date3 = points2.First().ToDateValue().Date;
                        var date4 = points2.Last().ToDateValue().Date;
                        points1.AddRange(points2);
                        Debug.Print("rectange 1,2 contains {0},{1} points, respectively", points1.Count, points2.Count);
                        if (points1.Count == 0 || points2.Count == 0)
                        {
                            MessageBox.Show("选取数据不完整，无法计算平均值！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            rectanges.Clear();
                            baseChart.Invalidate();
                            baseChart.Cursor = Cursors.Cross;
                            return;
                        }
                        //当前数据源入栈
                        var stack = baseChart.GetStack();
                        stack.Push(baseChart.GetTable().Copy());
                        Debug.Print("push datatable to stack, current stack size {0}", stack.Count);
                        canDrawRect = false;
                        var avg = Math.Round(points1.Average(p => p.YValues[0]), Xb2Config.GetPrecision());
                        Debug.Print("total point count:{0}, mean:{1}", points1.Count, avg);
                        var index = baseChart.Series[0].Points.IndexOf(targetPoint);
                        Debug.Print("target point index:" + index);
                        baseChart.GetTable().Rows[index][Y_TITLE] = avg;
                        Debug.Print("change value in data table to {0}", baseChart.GetTable().Rows[index][Y_TITLE]);
                        baseChart.GetTable().AcceptChanges();
                        baseChart.DataBind();
                        //加入操作记录
                        var log = string.Format("突跳处理→平均值法，使用{0}~{1},{2}~{3}取平均值，{4}", date1.SStr(), date2.SStr(),
                            date3.SStr(), date4.SStr(), initPoint.ToDateValue() + "→" + avg);
                        baseChart.GetLogger().Add(log);

                        //目标点置空
                        targetPoint = null;
                        initPoint = null;
                        //矩形清空
                        rectanges.Clear();
                    }
                }
            };

            baseChart.Paint += delegate(object sender, PaintEventArgs e)
            {
                //绘制已有的矩形
                if (rectanges.Count > 0)
                {
                    e.Graphics.DrawRectangles(new Pen(RECT_COLOR, RECT_FITNESS), rectanges.ToArray());
                }
                //绘制正在拖动，正在画的矩形
                if (isDrawing)
                {
                    e.Graphics.DrawRectangle(new Pen(RECT_COLOR, RECT_FITNESS), GetRect(startPos, currentPos));
                }
            };

            return baseChart;
        }

        /// <summary>
        /// 突跳处理→活动平均法
        /// 活动平均法与平均值法的区别在于
        /// 平均值法需要前后绘制两个矩形
        /// But，活动平均法只需要绘制一个矩形就可以了
        /// </summary>
        /// <returns></returns>
        public static Chart GetFlexAvgMethodChart()
        {
            var baseChart = GetTemplateChart();
            //要处理的突跳点，即目标点
            DataPoint targetPoint = null;
            DataPoint initPoint = null;
            //绘制的矩形框
            var rectanges = new List<Rectangle>();
            //鼠标起止点，用于控制绘制矩形
            Point startPos = new Point();
            Point currentPos = new Point();
            //正在绘制矩形的标识
            bool isDrawing = false;
            //是否可以绘制矩形的标识
            bool canDrawRect = false;
            baseChart.MouseClick += delegate(object sender, MouseEventArgs e)
            {
                if (e.Button != MouseButtons.Left) return;
                //如果已经有目标点，则不再设置目标点
                if (targetPoint != null) return;
                var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                if (ht.ChartElementType == ChartElementType.DataPoint)
                {
                    //找到目标点
                    targetPoint = baseChart.Series[0].Points[ht.PointIndex];
                    initPoint = baseChart.Series[0].Points[ht.PointIndex];
                    //用户确认是否使用平均法处理该数据点
                    var question = string.Format("确定使用活动平均法处理点{0},{1}吗？", targetPoint.GetDateStr(),
                        targetPoint.GetValue());
                    var dialog = MessageBox.Show(question, "提问", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialog == DialogResult.Yes)
                    {
                        targetPoint.MarkerStyle = TARGET_MARKER_STYLE;
                        targetPoint.MarkerColor = TARGET_MARKER_COLOR;
                        canDrawRect = true;
                    }
                    //如果用户取消，需要将testPoint重新置空，否则用户将无法再次选择目标点
                    else
                    {
                        targetPoint = null;
                    }

                }
            };

            baseChart.MouseDown += delegate(object sender, MouseEventArgs e)
            {
                //最多画一个矩形
                if (rectanges.Count >= 1) return;
                //左键才可以画矩形
                if (e.Button != MouseButtons.Left) return;
                if (!canDrawRect) return;
                var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                if (ht.ChartElementType == ChartElementType.PlottingArea)
                {
                    currentPos = startPos = e.Location;
                    isDrawing = true;
                }
            };

            baseChart.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                //画矩形
                currentPos = e.Location;
                if (isDrawing) baseChart.Invalidate();
            };

            baseChart.MouseUp += delegate(object sender, MouseEventArgs e)
            {
                if (isDrawing)
                {
                    isDrawing = false;
                    var rc = GetRect(startPos, currentPos);
                    if (rc.Width > 0 && rc.Height > 0) rectanges.Add(rc);
                    //矩形绘制完毕，开始计算平均值
                    if (rectanges.Count == 1)
                    {
                        var dialogResult = MessageBox.Show("计算平均值？", "提问", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                        //提问用户是否使用绘制的矩形计算平均值，如果用户选择“否”
                        //就把矩形清空，由用户重新绘制
                        if (dialogResult != DialogResult.Yes)
                        {
                            rectanges.Clear();
                            baseChart.Invalidate();
                            baseChart.Cursor = Cursors.Cross;
                            return;
                        }
                        //用户选择“是”，则判断两个矩形中是否选取到了点，只要一个矩形选取不到点，就提示出错
                        //矩形清空，然后由用户重新绘制
                        var points1 = baseChart.Series[0].Points.ContainsBy(rectanges[0], baseChart.ChartAreas[0]);
                        Debug.Print("rectange contains {0} points, respectively", points1.Count);
                        if (points1.Count == 0)
                        {
                            MessageBox.Show("选取数据不完整，无法计算平均值！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            rectanges.Clear();
                            baseChart.Invalidate();
                            baseChart.Cursor = Cursors.Cross;
                            return;
                        }
                        //当前数据源入栈
                        var stack = baseChart.GetStack();
                        stack.Push(baseChart.GetTable().Copy());
                        Debug.Print("push datatable to stack, current stack size {0}", stack.Count);
                        canDrawRect = false;
                        //计算平均值
                        var avg = Math.Round(points1.Average(p => p.YValues[0]), Xb2Config.GetPrecision());
                        Debug.Print("total point count:{0}, mean:{1}", points1.Count, avg);
                        var index = baseChart.Series[0].Points.IndexOf(targetPoint);
                        Debug.Print("target point index:" + index);
                        //写回数据源
                        baseChart.GetTable().Rows[index][Y_TITLE] = avg;
                        Debug.Print("change value in data table to {0}", baseChart.GetTable().Rows[index][Y_TITLE]);
                        baseChart.GetTable().AcceptChanges();
                        baseChart.DataBind();
                        //操作日志处理
                        var log = string.Format("突跳处理→活动平均法，使用{0}~{1}的观测值取平均值，{2}",
                            points1.First().ToDateValue().Date.SStr(), points1.Last().ToDateValue().Date.SStr(),
                            initPoint.ToDateValue() + "→" + avg);
                        baseChart.GetLogger().Add(log);
                        //目标点置空
                        targetPoint = null;
                        initPoint = null;
                        //矩形清空
                        rectanges.Clear();
                    }
                }
            };

            baseChart.Paint += delegate(object sender, PaintEventArgs e)
            {
                //绘制已有的矩形
                if (rectanges.Count > 0)
                {
                    e.Graphics.DrawRectangles(new Pen(RECT_COLOR, RECT_FITNESS), rectanges.ToArray());
                }
                //绘制正在拖动，正在画的矩形
                if (isDrawing)
                {
                    e.Graphics.DrawRectangle(new Pen(RECT_COLOR, RECT_FITNESS), GetRect(startPos, currentPos));
                }
            };

            return baseChart;
        }

        /// <summary>
        /// 突跳处理→数学模型法
        /// 该方法其实就是选中一个突跳点
        /// 然后用突跳点周围的数据进行拉格朗日插值
        /// 用插值的值代替突跳点的值
        /// </summary>
        /// <returns></returns>
        public static Chart GetMathModelChart()
        {
            var baseChart = GetTemplateChart();
            //选中的数据点
            DataPoint testPoint;

            baseChart.MouseClick += delegate(object sender, MouseEventArgs e)
            {
                if (e.Button != MouseButtons.Left) return;
                var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                var stack = (Stack<DataTable>) baseChart.Tag;
                if (ht.ChartElementType == ChartElementType.DataPoint)
                {
                    //找到点击的数据点
                    testPoint = baseChart.Series[0].Points[ht.PointIndex];
                    //用户确认是否删除该数据点
                    var question = string.Format("确定使用数学模型法处理点{0},{1}吗？", testPoint.GetDateStr(), testPoint.GetValue());
                    var dialog = MessageBox.Show(question, "提问", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialog == DialogResult.Yes)
                    {
                        //用户确认删除后，在删除该数据之前，先将数据源推入堆栈，用于撤销
                        //将当前的数据源入栈，注意Copy哦，这可是活生生的引用传递和值传递的例子啊
                        stack.Push(baseChart.GetTable().Copy());
                        LogPushStack(stack);
                        var dataPoints = baseChart.Series[0].Points.ToList();
                        //获取要删除点的索引
                        var index = dataPoints.IndexOf(testPoint);
                        Debug.Print("Wanted Deleted Index:{0}", index);
                        var date = DateTime.FromOADate(testPoint.XValue);
                        var oldValue = testPoint.YValues[0];
                        var value = Processmethod.GetLagrangeInterpolationResult(dataPoints, date);
                        value = Math.Round(value, Xb2Config.GetPrecision());
                        //在数据源中删除数据，并且要AcceptChanges，否则你可以试试
                        baseChart.GetTable().Rows[index][Y_TITLE] = value;
                        baseChart.GetTable().AcceptChanges();
                        //重新绑定数据源
                        baseChart.DataBind();

                        //日志处理
                        var log = string.Format("突跳处理，数学模型法:{0}, {1}→{2}", date.SStr(), oldValue, value);
                        baseChart.GetLogger().Add(log);
                        MessageBox.Show("处理成功！\n " + oldValue + "->" + value);
                    }

                }
            };

            return baseChart;
        }



        /// <summary>
        /// 突跳处理→拟合→线性拟合
        /// </summary>
        /// <returns></returns>
        public static Chart GetLinearRegressionChart()
        {
            return GetRegressionVirtualChart(Processmethod.GetLinearRegressionExpression, "突跳处理→线性拟合");
        }

        /// <summary>
        /// 突跳处理→拟合→多项式拟合
        /// </summary>
        /// <returns></returns>
        public static Chart GetPolyRegressionChart()
        {
            return GetRegressionVirtualChart(Processmethod.GetPolyRegressionExpression, "突跳处理→多项式拟合");
        }

        /// <summary>
        /// 突跳处理→拟合→对数拟合
        /// </summary>
        /// <returns></returns>
        public static Chart GetLogitRegressionChart()
        {
            return GetRegressionVirtualChart(Processmethod.GetLogitRegressionExpression, "突跳处理→对数拟合");
        }

        /// <summary>
        /// 突跳处理→拟合→幂函数拟合
        /// </summary>
        /// <returns></returns>
        public static Chart GetPowerFuncRegressChart()
        {
            return GetRegressionVirtualChart(Processmethod.GetPowerRegresExpression, "突跳处理→幂函数拟合");
        }

        /// <summary>
        /// 突跳处理→拟合→指数e拟合
        /// </summary>
        /// <returns></returns>
        public static Chart GetExpRegressionChart()
        {
            return GetRegressionVirtualChart(Processmethod.GetExpRegressionExpression, "突跳处理→指数e拟合");
        }

        #endregion

        #region 缺数处理

        /// <summary>
        /// 平均值法
        /// 在绘图区域中绘制两个矩形
        /// 然后求这两个矩形中的观测值平均值
        /// 形成两个数据点
        /// 第1个数据点的x是第1个矩形中最后1个点的横坐标，y第1个矩形中观测值的平均值
        /// 第2个数据点的x是第2个矩形中第1个点的横坐标，y第2个矩形中观测值的平均值
        /// 使用这两个点做线性回归，然后将缺数点的x0作为自变量传入，算得y0，作为突跳点的观测值
        /// </summary>
        /// <returns></returns>
        public static Chart GetAvgMethodChart_LoseValue_Process()
        {
            var baseChart = GetTemplateChart();
            //要处理的突跳点，即目标点
            var rectanges = new List<Rectangle>();
            //鼠标起止点，用于控制绘制矩形
            Point startPos = new Point();
            Point currentPos = new Point();
            //正在绘制矩形的标识
            bool isDrawing = false;
            //是否可以绘制矩形的标识
            //bool canDrawRect = false;

            baseChart.MouseDown += delegate(object sender, MouseEventArgs e)
            {
                //最多画两个矩形
                if (rectanges.Count >= 2) return;
                //左键才可以画矩形
                if (e.Button != MouseButtons.Left) return;
                //if (!canDrawRect) return;
                var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                if (ht.ChartElementType == ChartElementType.PlottingArea)
                {
                    currentPos = startPos = e.Location;
                    isDrawing = true;
                }
            };

            baseChart.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                //画矩形
                currentPos = e.Location;
                if (isDrawing) baseChart.Invalidate();
            };

            baseChart.MouseUp += delegate(object sender, MouseEventArgs e)
            {
                if (isDrawing)
                {
                    isDrawing = false;
                    var rc = GetRect(startPos, currentPos);
                    if (rc.Width > 0 && rc.Height > 0) rectanges.Add(rc);
                    //矩形绘制完毕，开始计算平均值
                    if (rectanges.Count == 2)
                    {
                        var dialogResult = MessageBox.Show("确定使用绘制的矩形？", "提问", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                        //提问用户是否使用绘制的矩形计算平均值，如果用户选择“否”
                        //就把矩形清空，由用户重新绘制
                        if (dialogResult != DialogResult.Yes)
                        {
                            rectanges.Clear();
                            baseChart.Invalidate();
                            baseChart.Cursor = Cursors.Cross;
                            return;
                        }
                        //用户选择“是”，则判断两个矩形中是否选取到了点，只要有一个矩形选取不到点，就提示出错
                        //矩形清空，然后由用户重新绘制
                        var points0 = baseChart.Series[0].Points.ContainsBy(rectanges[0], baseChart.ChartAreas[0]);
                        var points1 = baseChart.Series[0].Points.ContainsBy(rectanges[1], baseChart.ChartAreas[0]);
                        Debug.Print("rectange 1,2 contains {0},{1} points, respectively", points0.Count, points1.Count);
                        if (points0.Count == 0 || points1.Count == 0)
                        {
                            MessageBox.Show("选取数据不完整，无法进行计算！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            rectanges.Clear();
                            baseChart.Invalidate();
                            return;
                        }
                        var frmInterpolation = new FrmInterpolation2("平均值法插值");
                        if (frmInterpolation.ShowDialog() == DialogResult.OK)
                        {
                            var date = frmInterpolation.GetDate();
                            var pt0 = new DataPoint(points0.Last().XValue, points0.Average(p => p.YValues[0]));
                            var pt1 = new DataPoint(points1.First().XValue, points1.Average(p => p.YValues[0]));
                            var xs = new List<double> {pt0.XValue, pt1.XValue}.ToArray();
                            var ys = new List<double> {pt0.YValues[0], pt1.YValues[0]}.ToArray();

                            var ps = Fit.Line(xs, ys);
                            var value = Math.Round(ps.Item1 + ps.Item2*date.ToOADate(), Xb2Config.GetPrecision());
                            Debug.Print("Liner Regression:\n x:{0} \n y:{1} \n x0:{2} \n estimated value:{3}",
                                string.Join(",", xs), string.Join(",", ys), date.ToOADate(), value);
                            if (baseChart.Interpolation(date, value,"缺数处理→平均值法插值"))
                            {
                                MessageBox.Show("插值成功！");
                                rectanges.Clear();
                                baseChart.Invalidate();
                            }
                        }
                        else
                        {
                            MessageBox.Show("平均值法插值已取消！");
                        }
                    }
                }
            };

            baseChart.Paint += delegate(object sender, PaintEventArgs e)
            {
                //绘制已有的矩形
                if (rectanges.Count > 0)
                {
                    e.Graphics.DrawRectangles(new Pen(RECT_COLOR, RECT_FITNESS), rectanges.ToArray());
                }
                //绘制正在拖动，正在画的矩形
                if (isDrawing)
                {
                    e.Graphics.DrawRectangle(new Pen(RECT_COLOR, RECT_FITNESS), GetRect(startPos, currentPos));
                }
            };

            return baseChart;
        }

        /// <summary>
        /// 缺数处理→线性法插值
        /// 在绘图区域绘制两个矩形
        /// 将这两个矩形中的点进行线性回归
        /// 使用回归后的模型计算缺数点x0的观测值y0
        /// </summary>
        /// <returns></returns>
        public static Chart GetLinearInterpolationChart()
        {
            var baseChart = GetTemplateChart();
            //要处理的突跳点，即目标点
            var rectanges = new List<Rectangle>();
            //鼠标起止点，用于控制绘制矩形
            Point startPos = new Point();
            Point currentPos = new Point();
            //正在绘制矩形的标识
            bool isDrawing = false;
            //是否可以绘制矩形的标识
            //bool canDrawRect = false;

            baseChart.MouseDown += delegate(object sender, MouseEventArgs e)
            {
                //最多画两个矩形
                if (rectanges.Count >= 2) return;
                //左键才可以画矩形
                if (e.Button != MouseButtons.Left) return;
                //if (!canDrawRect) return;
                var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                if (ht.ChartElementType == ChartElementType.PlottingArea)
                {
                    currentPos = startPos = e.Location;
                    isDrawing = true;
                }
            };

            baseChart.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                //画矩形
                currentPos = e.Location;
                if (isDrawing) baseChart.Invalidate();
            };

            baseChart.MouseUp += delegate(object sender, MouseEventArgs e)
            {
                if (isDrawing)
                {
                    isDrawing = false;
                    var rc = GetRect(startPos, currentPos);
                    if (rc.Width > 0 && rc.Height > 0) rectanges.Add(rc);
                    //矩形绘制完毕，开始计算平均值
                    if (rectanges.Count == 2)
                    {
                        var dialogResult = MessageBox.Show("确定使用绘制的矩形？", "提问", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                        //提问用户是否使用绘制的矩形计算平均值，如果用户选择“否”
                        //就把矩形清空，由用户重新绘制
                        if (dialogResult != DialogResult.Yes)
                        {
                            rectanges.Clear();
                            baseChart.Invalidate();
                            baseChart.Cursor = Cursors.Cross;
                            return;
                        }
                        //获取前后两个矩形选取到的数据点
                        var points0 = baseChart.Series[0].Points.ContainsBy(rectanges[0], baseChart.ChartAreas[0]);
                        var points1 = baseChart.Series[0].Points.ContainsBy(rectanges[1], baseChart.ChartAreas[0]);
                        Debug.Print("rectange 1,2 contains {0},{1} points, respectively", points0.Count, points1.Count);
                        //用户选择“是”，则判断两个矩形中是否选取到了点，只要有一个矩形选取不到点，就提示出错
                        //矩形清空，然后由用户重新绘制
                        if (points0.Count == 0 || points1.Count == 0)
                        {
                            MessageBox.Show("选取数据不完整，无法进行计算！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            rectanges.Clear();
                            baseChart.Invalidate();
                            return;
                        }
                        var frmInterpolation = new FrmInterpolation2("线性法插值");
                        if (frmInterpolation.ShowDialog() == DialogResult.OK)
                        {
                            Debug.Print("线性法插值");
                            var date = frmInterpolation.GetDate();
                            var xs = points0.Select(p => p.XValue).ToList();
                            xs.AddRange(points1.Select(p => p.XValue));
                            var ys = points0.Select(p => p.YValues[0]).ToList();
                            ys.AddRange(points1.Select(p => p.YValues[0]));

                            var ps = Fit.Line(xs.ToArray(), ys.ToArray());
                            var value = Math.Round(ps.Item1 + ps.Item2*date.ToOADate(), Xb2Config.GetPrecision());
                            Debug.Print("Liner Regression:\n x:{0} \n y:{1} \n x0:{2} \n estimated value:{3}",
                                string.Join(",", xs), string.Join(",", ys), date.ToOADate(), value);
                            if (baseChart.Interpolation(date, value, "缺数处理→线性法插值"))
                            {
                                MessageBox.Show("插值成功！");
                                rectanges.Clear();
                                baseChart.Invalidate();
                            }
                        }
                        else
                        {
                            MessageBox.Show("线性法插值已取消！");
                        }
                    }
                }
            };

            baseChart.Paint += delegate(object sender, PaintEventArgs e)
            {
                //绘制已有的矩形
                if (rectanges.Count > 0)
                {
                    e.Graphics.DrawRectangles(new Pen(RECT_COLOR, RECT_FITNESS), rectanges.ToArray());
                }
                //绘制正在拖动，正在画的矩形
                if (isDrawing)
                {
                    e.Graphics.DrawRectangle(new Pen(RECT_COLOR, RECT_FITNESS), GetRect(startPos, currentPos));
                }
            };

            return baseChart;
        }

        /// <summary>
        /// 缺数处理→多项式插值
        /// 在绘图区域绘制两个矩形
        /// 将这两个矩形中的点进行多项式回归
        /// 使用回归后的模型计算缺数点x0的观测值y0
        /// </summary>
        /// <returns></returns>
        public static Chart GetPolyInterpolationChart()
        {
            var baseChart = GetTemplateChart();
            //要处理的突跳点，即目标点
            var rectanges = new List<Rectangle>();
            //鼠标起止点，用于控制绘制矩形
            Point startPos = new Point();
            Point currentPos = new Point();
            //正在绘制矩形的标识
            bool isDrawing = false;
            //是否可以绘制矩形的标识
            //bool canDrawRect = false;

            baseChart.MouseDown += delegate(object sender, MouseEventArgs e)
            {
                //最多画两个矩形
                if (rectanges.Count >= 2) return;
                //左键才可以画矩形
                if (e.Button != MouseButtons.Left) return;
                //if (!canDrawRect) return;
                var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                if (ht.ChartElementType == ChartElementType.PlottingArea)
                {
                    currentPos = startPos = e.Location;
                    isDrawing = true;
                }
            };

            baseChart.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                //画矩形
                currentPos = e.Location;
                if (isDrawing) baseChart.Invalidate();
            };

            baseChart.MouseUp += delegate(object sender, MouseEventArgs e)
            {
                if (isDrawing)
                {
                    isDrawing = false;
                    var rc = GetRect(startPos, currentPos);
                    if (rc.Width > 0 && rc.Height > 0) rectanges.Add(rc);
                    //矩形绘制完毕，开始计算平均值
                    if (rectanges.Count == 2)
                    {
                        var dialogResult = MessageBox.Show("确定使用绘制的矩形？", "提问", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                        //提问用户是否使用绘制的矩形计算平均值，如果用户选择“否”
                        //就把矩形清空，由用户重新绘制
                        if (dialogResult != DialogResult.Yes)
                        {
                            rectanges.Clear();
                            baseChart.Invalidate();
                            baseChart.Cursor = Cursors.Cross;
                            return;
                        }
                        //获取前后两个矩形选取到的数据点
                        var points0 = baseChart.Series[0].Points.ContainsBy(rectanges[0], baseChart.ChartAreas[0]);
                        var points1 = baseChart.Series[0].Points.ContainsBy(rectanges[1], baseChart.ChartAreas[0]);
                        Debug.Print("rectange 1,2 contains {0},{1} points, respectively", points0.Count, points1.Count);
                        //用户选择“是”，则判断两个矩形中是否选取到了点，只要有一个矩形选取不到点，就提示出错
                        //矩形清空，然后由用户重新绘制
                        if (points0.Count == 0 || points1.Count == 0)
                        {
                            MessageBox.Show("选取数据不完整，无法进行计算！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            rectanges.Clear();
                            baseChart.Invalidate();
                            return;
                        }
                        var frmInterpolation = new FrmInterpolation2("多项式法插值");
                        if (frmInterpolation.ShowDialog() == DialogResult.OK)
                        {
                            Debug.Print("多项式法插值");
                            var date = frmInterpolation.GetDate();
                            var xs = points0.Select(p => p.XValue).ToList();
                            xs.AddRange(points1.Select(p => p.XValue));
                            var ys = points0.Select(p => p.YValues[0]).ToList();
                            ys.AddRange(points1.Select(p => p.YValues[0]));

                            var ps = Fit.Polynomial(xs.ToArray(), ys.ToArray(), 2);
                            var value = ps[0] + ps[1]*date.ToOADate() + ps[2]*date.ToOADate()*date.ToOADate();
                            value = Math.Round(value, Xb2Config.GetPrecision());
                            Debug.Print("Polynomial Regression:\n x:{0} \n y:{1} \n x0:{2} \n y0:{3}",
                                string.Join(",", xs), string.Join(",", ys), date.ToOADate(), value);
                            if (baseChart.Interpolation(date, value, "缺数处理→多项式插值"))
                            {
                                MessageBox.Show("插值成功！");
                                rectanges.Clear();
                                baseChart.Invalidate();
                            }
                        }
                        else
                        {
                            MessageBox.Show("多项式法插值已取消！");
                        }
                    }
                }
            };

            baseChart.Paint += delegate(object sender, PaintEventArgs e)
            {
                //绘制已有的矩形
                if (rectanges.Count > 0)
                {
                    e.Graphics.DrawRectangles(new Pen(RECT_COLOR, RECT_FITNESS), rectanges.ToArray());
                }
                //绘制正在拖动，正在画的矩形
                if (isDrawing)
                {
                    e.Graphics.DrawRectangle(new Pen(RECT_COLOR, RECT_FITNESS), GetRect(startPos, currentPos));
                }
            };

            return baseChart;
        }

        /// <summary>
        /// 缺数处理→等间距插值
        /// 用户在界面上选择其间缺数的2个数据点
        /// 用这两个点做线性回归，提示用于输入插值间隔，单位是月
        /// 然后利用回归系数进行等间隔插值
        /// 这个Chart允许鼠标滚轮放大，缩小操作
        /// </summary>
        /// <returns></returns>
        public static Chart GetEqualDistInterpolationChart()
        {
            var baseChart = GetTemplateChart();
            int redPointCount = 0;
            Debug.Print("redPointCount:" + redPointCount);

            #region 等间距插值需要进行图件局部放大操作，需要处理下面3个鼠标事件

            baseChart.MouseEnter += (sender, e) =>
            {
                baseChart.Focus();
            };

            baseChart.MouseLeave += (sender, e) =>
            {
                baseChart.Parent.Focus();
            };

            baseChart.MouseWheel += (sender, e) =>
            {
                baseChart.Focus();
                if (e.Delta < 0)
                {
                    baseChart.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    //baseChart.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                }
                if (e.Delta > 0)
                {
                    double xMin = baseChart.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                    double xMax = baseChart.ChartAreas[0].AxisX.ScaleView.ViewMaximum;
                    double posXStart = baseChart.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                    double posXFinish = baseChart.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;
                    baseChart.ChartAreas[0].AxisX.ScaleView.Zoom(posXStart, posXFinish);
                    baseChart.ChartAreas[0].AxisX.ScrollBar.ButtonColor = Color.LightGreen;
                }
            };

            #endregion

            //Mouse Click事件用于等间距插值
            baseChart.MouseClick += (sender, e) =>
            {
                if (e.Button != MouseButtons.Left) return;
                if (baseChart.HitTest(e.X, e.Y).ChartElementType == ChartElementType.DataPoint)
                {
                    var pointIndex = baseChart.HitTest(e.X, e.Y).PointIndex;
                    var point = baseChart.Series[0].Points[pointIndex];
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
                                var targetPoints = baseChart.Series[0].Points.ToList().FindAll(p => p.MarkerColor == Color.Red);
                                if (targetPoints.Count == 2)
                                {
                                    targetPoints.Sort(
                                        (p1, p2) => DateTime.FromOADate(p1.XValue).CompareTo(DateTime.FromOADate(p2.XValue)));
                                    var point1 = targetPoints[0];
                                    var point2 = targetPoints[1];
                                    Debug.Print("point1:{0},{1}, point2:{2},{3}", DateTime.FromOADate(point1.XValue),
                                        point1.YValues[0], DateTime.FromOADate(point2.XValue), point2.YValues[0]);
                                    // 注意，这里需要用户给一个观测周期，现在默认输的是1
                                    FrmInputPeriod frmInputPeriod = new FrmInputPeriod
                                    {
                                        StartPosition = FormStartPosition.CenterScreen
                                    };
                                    dialogResult = frmInputPeriod.ShowDialog();
                                    if (dialogResult != DialogResult.OK)
                                    {
                                        MessageBox.Show("取消等间距插值！");
                                        baseChart.Series[0].Points.Apply(p => p.MarkerColor = Color.Blue);
                                        redPointCount = 0;
                                        return;
                                    }
                                    //在进行等间距插值前将数据源入栈，供撤销
                                    var stack = baseChart.GetStack();
                                    stack.Push(baseChart.GetTable().Copy());
                                    LogPushStack(stack);
                                    //获取用户输入的管理周期，进行等间距插值
                                    var period = frmInputPeriod.Period;
                                    var interpolation = Processmethod.EqualDistanceInterpolation(targetPoints, period);
                                    //向当前数据源中加入插值点，然后按照日期排序，重新绑定Chart控件
                                    var dt = baseChart.GetTable();
                                    interpolation.ForEach(d => dt.Rows.Add(d.Date, d.Value.Round(2)));
                                    dt.AcceptChanges();
                                    var dv = dt.DefaultView;
                                    dv.Sort = "观测日期";
                                    BindChartWithData(baseChart, dv.ToTable());
                                    //数据点全部恢复蓝色，置红色点个数为0
                                    baseChart.Series[0].Points.Apply(p => p.MarkerColor = Color.Blue);
                                    redPointCount = 0;
                                    //日志记录
                                    var log = GetLog("缺数处理→等间距插值：" + string.Join(",", interpolation));
                                    baseChart.GetLogger().Add(log);
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
                        redPointCount--;
                        point.MarkerColor = Color.Blue;
                    }
                }
            };
            return baseChart;
        }

        #endregion

        #region 台阶处理

        /// <summary>
        /// 台阶处理→图解法
        /// </summary>
        /// <returns></returns>
        public static Chart GetStepMovingChart()
        {
            //获取模板chart，但并没有绑定数据，你看，该方法也没有数据源参数传进来
            var baseChart = GetTemplateChart();
            //鼠标位置记录变量
            var startPos = new Point();
            var currentPos = new Point();
            //Y轴最大最小值设置，防止鼠标移动超过此值从而触发异常
            double minYPos = 0, maxYPos = 0;
            var drawing = false;
            var rectangles = new List<Rectangle>();
            //选中的数据点
            var dataPoints = new Dictionary<int, List<DataPoint>>();
            var pen = new Pen(Color.Red, 2);

            Func<double, double> YValueToPixelPos = y => baseChart.ChartAreas[0].AxisY.ValueToPixelPosition(y);
            Func<double, double> YPixelPosToValue = p => baseChart.ChartAreas[0].AxisY.PixelPositionToValue(p);

            baseChart.PostPaint += delegate(object sender, ChartPaintEventArgs e)
            {
                maxYPos = YValueToPixelPos(baseChart.ChartAreas[0].AxisY.Minimum);
                minYPos = YValueToPixelPos(baseChart.ChartAreas[0].AxisY.Maximum);
            };

            baseChart.MouseDown += delegate(object sender, MouseEventArgs e)
            {
                if (e.Button != MouseButtons.Left) return;
                //初始化起始位置和当前位置
                currentPos = startPos = e.Location;
                //如果当前绘图区中已经存在矩形，且鼠标点位于矩形之内
                //则获取矩形内的数据点，并将当前数据源入栈，用于撤销
                //如果不存在则绘制矩形即可
                if (rectangles.Count > 0)
                {
                    if (rectangles[0].Contains(e.Location))
                    {
                        //当前数据源入栈，用于撤销
                        var stack = baseChart.GetStack();
                        var dt = baseChart.GetTable();
                        stack.Push(dt.Copy());
                        LogPushStack(stack);
                        //保存矩形中的数据点，用于拖动
                        var pts = baseChart.Series[0].Points.ContainsBy(rectangles[0], baseChart.ChartAreas[0]);
                        if (!dataPoints.ContainsKey(0))
                        {
                            dataPoints.Add(0, pts);
                        }
                    }
                }
                else
                {
                    //绘图区中不存在矩形，则绘制矩形
                    var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                    if (ht.ChartElementType == ChartElementType.PlottingArea)
                    {
                        drawing = true;
                    }
                }
            };

            baseChart.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                if (e.Button != MouseButtons.Left) return;
                //鼠标的移动范围不能超过绘图区， 否则会触发异常
                if (e.Y >= minYPos && e.Y <= maxYPos)
                {
                    //更新当前坐标
                    currentPos = e.Location;
                    if (drawing)
                    {
                        baseChart.Invalidate();
                        return;
                    }
                    //如果绘图区中有矩形，且鼠标按在矩形内
                    //则拖动矩形及矩形里的数据点
                    if (rectangles.Count > 0)
                    {
                        //鼠标按在矩形之外，直接返回
                        if (!rectangles[0].Contains(e.Location)) return;
                        //鼠标的像素位置变化量
                        var pixelOffset = currentPos.Y - startPos.Y;
                        //根据鼠标的像素变化量计算数据变化量
                        var dataOffset = YPixelPosToValue(currentPos.Y) - YPixelPosToValue(startPos.Y);
                        //更改矩形的Y值和数据点的Y值，注意这里rectangle是一个结构体，是值传递的
                        var rectangle = rectangles[0];
                        rectangle.Y += pixelOffset;
                        rectangles[0] = rectangle;
                        dataPoints[0].ForEach(p => p.YValues[0] += dataOffset);
                        //移动开始坐标，使得矩形可以拖动
                        startPos = e.Location;
                    }
                    //重绘Chart
                    baseChart.Invalidate();
                }
            };

            baseChart.MouseDoubleClick += delegate(object sender, MouseEventArgs e)
            {
                if (rectangles.Count > 0)
                {
                    var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                    if (ht.ChartElementType == ChartElementType.PlottingArea)
                    {
                        var message = MessageBox.Show("完成本次台阶处理吗？", "提问", MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Question);
                        if (message == DialogResult.OK)
                        {
                            //用户确定完成本次台阶处理后，更新数据源，写入日志
                            if (dataPoints.ContainsKey(0))
                            {
                                var dt = baseChart.GetTable();
                                if (dataPoints[0].Count > 0)
                                {
                                    foreach (var dataPoint in dataPoints[0])
                                    {
                                        var index = baseChart.Series[0].Points.IndexOf(dataPoint);
                                        var value = Math.Round(dataPoint.YValues[0], Xb2Config.GetPrecision());
                                        dt.Rows[index][Y_TITLE] = value;
                                        Debug.Print("更新数据源, 数据点:{0}, value:{1}", index, value);
                                    }
                                    dt.AcceptChanges();
                                    //日志处理
                                    var logger = baseChart.GetLogger();
                                    var point1 = dataPoints[0].First().ToDateValue();
                                    var point2 = dataPoints[0].Last().ToDateValue();
                                    var log = string.Format("台阶处理→图解法，从{0}到{1}", point1, point2);
                                    logger.Add(log);
                                }
                                else
                                {
                                    MessageBox.Show("没有选中数据点！");
                                }
                            }
                            rectangles.Clear();
                            dataPoints.Clear();
                            baseChart.Invalidate();
                        }
                    }
                }
            };

            baseChart.MouseUp += delegate(object sender, MouseEventArgs e)
            {
                //如果正在绘制矩形，则取消绘制
                if (e.Button == MouseButtons.Left)
                {
                    if (drawing)
                    {
                        drawing = false;
                    }
                    //获取当前正在绘制的矩形
                    var rectangle = GetRect(startPos, currentPos);
                    //矩形合法且矩形列表中没有矩形（因为只允许1次绘制1个矩形），则加入矩形列表，然后重绘Chart
                    if (rectangle.Width > 0 && rectangle.Height > 0)
                    {
                        //矩形没有选到数据，需重新绘制
                        if (!baseChart.Series[0].Points.ContainsBy(rectangle, baseChart.ChartAreas[0]).Any())
                        {
                            MessageBox.Show("没有选到数据，请重新绘制！");
                            rectangles.Clear();
                            baseChart.Invalidate();
                            return;
                        }
                        if (rectangles.Count == 0)
                        {
                            rectangles.Add(rectangle);
                        }
                    }
                    baseChart.Invalidate();
                }
            };

            baseChart.Paint += delegate(object sender, PaintEventArgs e)
            {
                if (drawing) e.Graphics.DrawRectangle(pen, GetRect(startPos, currentPos));
                if (rectangles.Count > 0) e.Graphics.DrawRectangles(pen, rectangles.ToArray());
            };

            return baseChart;
        }


        /// <summary>
        /// 台阶处理→多点平均平移法 
        /// </summary>
        /// <returns></returns>
        public static Chart GetMultiPointAvgSlideChart()
        {
            var baseChart = GetTemplateChart();
            //文本标记，用于显示平均值和差值
            TextAnnotation annotation = new TextAnnotation
            {
                Name = "Result Annotation",
                Font = new Font("宋体", 10, FontStyle.Bold),
                IsMultiline = true,
                AllowMoving = true,
                Alignment = ContentAlignment.MiddleLeft
            };
            baseChart.Annotations.Add(annotation);
            //鼠标位置记录变量
            var startPos = new Point();
            var currentPos = new Point();
            //Y轴最大最小值设置，防止鼠标移动超过此值从而触发异常
            double minYPos = 0, maxYPos = 0;
            var drawing = false;
            //绘制的矩形
            var rectangles = new List<Rectangle>();
            //矩形中的数据点，索引为0表示第1次绘制的矩形包括的数据点，1代表第2次
            var rectangleDataPoints = new Dictionary<int, List<DataPoint>>();
            //当前正在移动的矩形的索引值
            var movingRectangleIndex = -1;
            //绘制矩形的画笔
            var rectPen = new Pen(Color.Red, 2);

            /* Mouse Click方法的主要作用结束多点平均法的处理
             * 在绘图区已经有2个矩形的情况下（表明用户已经进行了台阶处理）
             * 用户在非矩形绘制区双击鼠标，提示是否结束台阶处理
             * 如果用户决定结束台阶处理，则将用户编辑的数据点加入数据源，
             * 并更新数据源，然后清空绘制的矩形和文本标记以及重置相关变量
             * 最后重绘Chart控件
             */
            baseChart.MouseClick += (sender, e) =>
            {
                if (rectangles.Count == 2)
                {
                    var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                    //不是绘图区就返回
                    if (ht.ChartElementType != ChartElementType.PlottingArea) return;
                    //在非矩形区域单击才可以结束台阶处理
                    if (rectangles[0].Contains(e.Location) || rectangles[1].Contains(e.Location)) return;
                    var message = MessageBox.Show("完成本次台阶处理吗？", "提问", MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question);
                    if (message != DialogResult.OK) return;
                    // 将用户编辑后的数据更新至数据源
                    var dt = baseChart.GetTable();
                    var points1 = rectangleDataPoints[0];
                    var points2 = rectangleDataPoints[1];
                    // 矩形1的更新
                    foreach (var dataPoint in points1)
                    {
                        var index = baseChart.Series[0].Points.IndexOf(dataPoint);
                        var value = Math.Round(dataPoint.YValues[0], Xb2Config.GetPrecision());
                        dt.Rows[index][Y_TITLE] = value;
                        Debug.Print("update datatable, data point index:{0}, value:{1}", index, value);
                    }
                    // 矩形2的更新
                    foreach (var dataPoint in points2)
                    {
                        var index = baseChart.Series[0].Points.IndexOf(dataPoint);
                        var value = Math.Round(dataPoint.YValues[0], Xb2Config.GetPrecision());
                        dt.Rows[index][Y_TITLE] = value;
                        Debug.Print("update datatable, data point index:{0}, value:{1}", index, value);
                    }
                    // 数据源接受数据
                    dt.AcceptChanges();
                    // 相关变量的重置
                    rectangles.Clear();
                    rectangleDataPoints.Clear();
                    annotation.Visible = false;
                    movingRectangleIndex = -1;
                    baseChart.Invalidate();
                }
            };

            //该事件负责计算Y坐标的最大最小值，防止鼠标移动超过最大范围而引发异常
            baseChart.PostPaint += delegate(object sender, ChartPaintEventArgs e)
            {
                maxYPos = baseChart.ChartAreas[0].AxisY.ValueToPixelPosition(baseChart.ChartAreas[0].AxisY.Minimum);
                minYPos = baseChart.ChartAreas[0].AxisY.ValueToPixelPosition(baseChart.ChartAreas[0].AxisY.Maximum);
            };

            baseChart.Paint += (sender, e) =>
            {
                //鼠标拖动时的处理
                if (drawing)
                {
                    e.Graphics.DrawRectangle(rectPen, GetRect(startPos, currentPos));
                }
                if (rectangles.Count > 0)
                    e.Graphics.DrawRectangles(rectPen, rectangles.ToArray());
            };

            /**
             * Mouse Down 事件主要负责相关变量的初始化和数据源的入栈
             */
            baseChart.MouseDown += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    //更新当前坐标
                    currentPos = startPos = e.Location;
                    if (rectangles.Count == 2)
                    {
                        //在已经绘制好两个矩形的情况下，如果鼠标落下在任意一个矩形区域内
                        //则表明用户开始进行台阶处理
                        //这时将数据源入栈，供撤销操作
                        if (rectangles[0].Contains(e.Location) || rectangles[1].Contains(e.Location))
                        {
                            var rectangle = rectangles.Find(r => r.Contains(e.Location));
                            //选中矩形 重要！
                            movingRectangleIndex = rectangles.IndexOf(rectangle);
                            var stack = baseChart.GetStack();
                            stack.Push(baseChart.GetTable().Copy());
                            LogPushStack(stack);
                        }
                    }
                    else
                    {
                        //如果两个矩形还没有绘制好，则继续绘制矩形
                        var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                        if (ht.ChartElementType == ChartElementType.PlottingArea)
                        {
                            drawing = true;
                        }
                    }
                }
            };

            /**
             * 这个Mouse Move事件可是太重要了
             */
            baseChart.MouseMove += (sender, e) =>
            {
                if (e.Button != MouseButtons.Left) return;
                if (e.Y >= minYPos && e.Y <= maxYPos)
                {
                    currentPos = e.Location;
                    //绘制矩形
                    if (drawing)
                    {
                        baseChart.Invalidate();
                        return;
                    }
                    //矩形绘制完毕，并且选中了1个矩形，则开始计算相关数据点的值
                    if (rectangles.Count == 2 && movingRectangleIndex != -1)
                    {
                        //确定移动的矩形
                        var movingRectangle = rectangles[movingRectangleIndex];
                        //如果鼠标位于移动的矩形内
                        if (movingRectangle.Contains(e.Location))
                        {
                            //矩形的移动量
                            var pixelOffset = currentPos.Y - startPos.Y;
                            //数据点的移动量
                            var dataOffset = baseChart.ChartAreas[0].AxisY.PixelPositionToValue(currentPos.Y)
                                             - baseChart.ChartAreas[0].AxisY.PixelPositionToValue(startPos.Y);
                            //设置矩形和数据点的新位置
                            movingRectangle.Y = movingRectangle.Y + pixelOffset;
                            //由于Rectangle是结构体，为值传递，需要将更改的元素直接赋值回去，又是一个按值传递的实例呀~
                            rectangles[movingRectangleIndex] = movingRectangle;
                            //更新移动矩形中的数据点的值
                            var movingRectanglePoints = rectangleDataPoints[movingRectangleIndex];
                            movingRectanglePoints.ForEach(pt => pt.YValues[0] += dataOffset);
                            //找到另一个没有移动的矩形，获取其中的数据点，并计算平均值
                            var standRectangleIndex = movingRectangleIndex == 0 ? 1 : 0;
                            var standRectanglePoints = rectangleDataPoints[standRectangleIndex];
                            //! 这里可以进行下一步操作，线性回归等
                            var movingPointsAverage = Math.Round(movingRectanglePoints.Average(p => p.YValues[0]),
                                Xb2Config.GetPrecision());
                            var standPointsAverage = Math.Round(standRectanglePoints.Average(p => p.YValues[0]),
                                Xb2Config.GetPrecision());
                            // 两个矩形平均值之差
                            var diff = Math.Round(movingPointsAverage - standPointsAverage, Xb2Config.GetPrecision());
                            // 显示文本注解
                            var anno = (TextAnnotation) baseChart.Annotations.FindByName("Result Annotation");
                            var sb = new StringBuilder();
                            sb.AppendLine("静止台阶平均值：" + standPointsAverage);
                            sb.AppendLine("移动台阶平均值：" + movingPointsAverage);
                            sb.AppendLine("差值：" + diff);
                            if (anno != null)
                            {
                                if (!anno.Visible) anno.Visible = true;
                                anno.AnchorDataPoint = movingRectanglePoints.Last();
                                anno.Text = sb.ToString();
                            }
                            startPos = e.Location;
                        }
                        baseChart.Invalidate();
                    }
                }
            };

            baseChart.MouseUp += (sender, e) =>
            {
                if (drawing)
                {
                    drawing = false;
                    //获得当前绘制的矩形
                    var rc = GetRect(startPos, currentPos);
                    //在矩形数小于2的情况下加入矩形列表
                    if (rc.Width > 0 && rc.Height > 0 && rectangles.Count < 2)
                    {
                        rectangles.Add(rc);
                    }
                    //两个矩形绘制完成后，提取各自包括的数据点
                    if (rectangles.Count == 2)
                    {
                        var rectangle1 = rectangles[0];
                        var rectangle2 = rectangles[1];
                        var points1 = baseChart.Series[0].Points.ContainsBy(rectangle1, baseChart.ChartAreas[0]);
                        var points2 = baseChart.Series[0].Points.ContainsBy(rectangle2, baseChart.ChartAreas[0]);
                        // 非法情况的处理
                        if (points1.Count == 0 || points2.Count == 0 || rectangle1.IntersectsWith(rectangle2))
                        {
                            MessageBox.Show("没有选到数据，且矩形不许相交！请重新绘制！");
                            rectangles.Clear();
                            rectangleDataPoints.Clear();
                            annotation.Visible = false;
                            movingRectangleIndex = -1;
                            baseChart.Invalidate();
                            return;
                        }
                        rectangleDataPoints.Clear();
                        rectangleDataPoints.Add(0, points1);
                        rectangleDataPoints.Add(1, points2);
                    }
                    movingRectangleIndex = -1;
                    baseChart.Invalidate();
                }
            };

            return baseChart;
        }

        /// <summary>
        /// 台阶处理→多点趋势平移法→线性拟合
        /// </summary>
        /// <returns></returns>
        public static Chart GetMultiPointLinearProcessChart()
        {
            return GetMultiPointRegressionVirtualChart(Processmethod.GetLinearRegressionExpression);
        }

        /// <summary>
        /// 台阶处理→多点趋势平移法→多项式拟合
        /// </summary>
        /// <returns></returns>
        public static Chart GetMultiPointPolyProcessChart()
        {
            return GetMultiPointRegressionVirtualChart(Processmethod.GetPolyRegressionExpression);
        }

        /// <summary>
        /// 台阶处理→多点趋势平移法→对数拟合
        /// </summary>
        /// <returns></returns>
        public static Chart GetMultiPointLogitProcessChart()
        {
            return GetMultiPointRegressionVirtualChart(Processmethod.GetLogitRegressionExpression);
        }

        /// <summary>
        /// 台阶处理→多点趋势平移法→幂函数拟合
        /// </summary>
        /// <returns></returns>
        public static Chart GetMultiPointPowerProcessChart()
        {
            return GetMultiPointRegressionVirtualChart(Processmethod.GetPowerRegresExpression);
        }

        /// <summary>
        /// 台阶处理→多点趋势平移法→指数e拟合
        /// </summary>
        /// <returns></returns>
        public static Chart GetMultiPointExpProcessChart()
        {
            return GetMultiPointRegressionVirtualChart(Processmethod.GetExpRegressionExpression);
        }

        #endregion

        #region 其他处理，添加趋势线

        /// <summary>
        /// 添加趋势线
        /// 用户在数据上绘制一个矩形
        /// 利用这个矩形中的数据做线性回归
        /// 在这个矩形框的宽度范围内给出回归直线
        /// </summary>
        /// <returns></returns>
        public static Chart GetRegressionLineChart()
        {
            var baseChart = GetTemplateChart();
            //绘制的矩形框
            var rectanges = new List<Rectangle>();
            //鼠标起止点，用于控制绘制矩形
            Point startPos = new Point();
            Point currentPos = new Point();
            //正在绘制矩形的标识
            bool isDrawing = false;
            baseChart.MouseDown += delegate(object sender, MouseEventArgs e)
            {
                //最多画一个矩形
                if (rectanges.Count >= 1) return;
                //左键才可以画矩形
                if (e.Button != MouseButtons.Left) return;
                var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                if (ht.ChartElementType == ChartElementType.PlottingArea)
                {
                    currentPos = startPos = e.Location;
                    isDrawing = true;
                }
            };

            baseChart.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                //画矩形
                currentPos = e.Location;
                if (isDrawing) baseChart.Invalidate();
            };

            baseChart.MouseUp += delegate(object sender, MouseEventArgs e)
            {
                if (isDrawing)
                {
                    isDrawing = false;
                    var rc = GetRect(startPos, currentPos);
                    if (rc.Width > 0 && rc.Height > 0) rectanges.Add(rc);
                    //矩形绘制完毕，开始计算平均值
                    if (rectanges.Count == 1)
                    {
                        var dialogResult = MessageBox.Show("绘制趋势线？", "提问", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                        //提问用户是否使用绘制的矩形计算平均值，如果用户选择“否”
                        //就把矩形清空，由用户重新绘制
                        if (dialogResult != DialogResult.Yes)
                        {
                            rectanges.Clear();
                            baseChart.Invalidate();
                            return;
                        }
                        //用户选择“是”，则判断矩形中是否选取到了点，只要一个矩形选取不到点，就提示出错
                        //矩形清空，然后由用户重新绘制
                        var points1 = baseChart.Series[0].Points.ContainsBy(rectanges[0], baseChart.ChartAreas[0]);
                        Debug.Print("rectange contains {0} points, respectively", points1.Count);
                        if (points1.Count == 0)
                        {
                            MessageBox.Show("选取数据不完整，无法绘制趋势线！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            rectanges.Clear();
                            baseChart.Invalidate();
                            return;
                        }
                        var x = points1.Select(p => p.XValue).ToArray();
                        var y = points1.Select(p => p.YValues[0]).ToArray();
                        var ans = Fit.Line(x, y);
                        if (baseChart.Series.FindByName("regline") != null)
                            baseChart.Series.Remove(baseChart.Series.FindByName("regline"));
                        var series = new Series("regline") {ChartType = SeriesChartType.Line, Color = Color.Red};
                        series.XValueType = ChartValueType.Double;
                        series.YValueType = ChartValueType.Double;
                        series.Points.AddXY(points1.First().XValue, points1.First().XValue * ans.Item2 + ans.Item1);
                        series.Points.AddXY(points1.Last().XValue, points1.Last().XValue*ans.Item2 + ans.Item1);
                        baseChart.Series.Add(series);
                        baseChart.Invalidate();
                        //矩形清空
                        rectanges.Clear();
                    }
                }
            };

            baseChart.Paint += delegate(object sender, PaintEventArgs e)
            {
                //绘制已有的矩形
                if (rectanges.Count > 0)
                {
                    e.Graphics.DrawRectangles(new Pen(RECT_COLOR, RECT_FITNESS), rectanges.ToArray());
                }
                //绘制正在拖动，正在画的矩形
                if (isDrawing)
                {
                    e.Graphics.DrawRectangle(new Pen(RECT_COLOR, RECT_FITNESS), GetRect(startPos, currentPos));
                }
            };
            return baseChart;
        }

        #endregion

        #endregion

        #region 用于分幅图管理的Chart控件

        /// <summary>
        /// 获得绘图区，dt用于设置绘图区的最大最小位置
        /// Index是用于给定ChartArea的Name
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static ChartArea GetChartArea(DataTable dt, int index)
        {
            ChartArea chartArea = new ChartArea();
            chartArea.Name = "area" + (index + 1);
            chartArea.BackColor = Color.Transparent;
            if (dt.Rows.Count > 0)
            {
                chartArea.AxisX.Minimum = ((DateTime) dt.Compute("min(观测日期)", "")).AddMonths(-X_EXPAND).ToOADate();
                chartArea.AxisX.Maximum = ((DateTime)dt.Compute("max(观测日期)", "")).AddMonths(X_EXPAND).ToOADate();
                chartArea.AxisY.Minimum = (double) dt.Compute("min(观测值)", "") - Y_EXPAND;
                chartArea.AxisY.Maximum = (double) dt.Compute("max(观测值)", "") + Y_EXPAND;
            }
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisX.LabelStyle.Format = "yyyy/MM/dd";
            chartArea.AxisY.LabelStyle.Format = "#0.00";
            return chartArea;
        }

        /// <summary>
        /// 获得绘图区，dt用于设置绘图区的最大最小位置
        /// name一般是绘图区的Title
        /// 该方法是上面方法的重载方法
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static ChartArea GetChartArea(DataTable dt, string name)
        {
            ChartArea chartArea = new ChartArea();
            chartArea.Name = name;
            chartArea.BackColor = Color.Transparent;
            if (dt.Rows.Count > 0)
            {
                chartArea.AxisX.Minimum = ((DateTime)dt.Compute("min(观测日期)", "")).AddMonths(-X_EXPAND).ToOADate();
                chartArea.AxisX.Maximum = ((DateTime)dt.Compute("max(观测日期)", "")).AddMonths(X_EXPAND).ToOADate();
                chartArea.AxisY.Minimum = (double)dt.Compute("min(观测值)", "") - Y_EXPAND;
                chartArea.AxisY.Maximum = (double)dt.Compute("max(观测值)", "") + Y_EXPAND;
            }
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisX.LabelStyle.Format = "yyyy/MM/dd";
            chartArea.AxisY.LabelStyle.Format = "#0.00";
            return chartArea;
        }

        /// <summary>
        /// 获得数据线对象
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static Series GetSeries(DataTable dt)
        {
            Series series = new Series();
            series.ChartType = SeriesChartType.Line;
            series.XValueType = ChartValueType.Date;
            series.YValueType = ChartValueType.Double;
            series.Color = Color.Black;
            series.Points.DataBind(dt.AsEnumerable(), "观测日期", "观测值", "");
            return series;
        }

        /// <summary>
        /// 上面方法的重载版本，需设置ChartAreaName
        /// 才能与ChartArea关联
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="chartAreaName"></param>
        /// <returns></returns>
        private static Series GetSeries(DataTable dt, string chartAreaName)
        {
            Series series = new Series();
            series.ChartType = SeriesChartType.Line;
            series.XValueType = ChartValueType.Date;
            series.YValueType = ChartValueType.Double;
            series.Color = Color.Black;
            series.Points.DataBind(dt.AsEnumerable(), "观测日期", "观测值", "");
            series.ChartArea = chartAreaName;
            return series;
        }

        /// <summary>
        /// 将算法的计算结果封装为Chart控件
        /// 该方法比较重要，包括分幅图管理中对于图件的重要操作
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static Chart BuildChart(List<CalcResult> results)
        {
            //chart控件的默认宽度，下面都是读取配置
            var width = Convert.ToInt32(ConfigurationManager.AppSettings["DISP_CHART_WIDTH"]);
            //每个chartArea的默认高度，使用这两个值计算chart控件的Size
            var heightPerArea = Convert.ToInt32(ConfigurationManager.AppSettings["DISP_CHART_AREA_HEIGHT"]);
            //第1个chartArea控件的起始Y坐标
            var startY = Convert.ToInt32(ConfigurationManager.AppSettings["DISP_CHART_AREA_Y_START"]);
            var startX = Convert.ToInt32(ConfigurationManager.AppSettings["DISP_CHART_AREA_X_START"]);
            //chartArea控件的宽度比例
            var chartAreaWidthPercent = Convert.ToInt32(ConfigurationManager.AppSettings["DISP_CHART_AREA_WIDTH_PERCENT"]);
            //chart控件的基本属性设置，边框等
            var chart = new Chart
            {
                BackColor = Color.Transparent,
                Size = new Size(width, results.Count*heightPerArea),
                BorderlineColor = Color.FromName(ConfigurationManager.AppSettings["DISP_CHART_BORDER_COLOR"]),
                BorderlineWidth = Convert.ToInt32(ConfigurationManager.AppSettings["DISP_CHART_BORDER_WIDTH"]),
                BorderDashStyle = ChartDashStyle.Solid,
                Titles = {results[0].Title}
            };

            //Chart控件中添加一个CheckBox，使得Chart可以选中
            //Chart控件选中时，边框样式改变
            CheckBox checkBox = new CheckBox {Location = new Point(5, 5), Size = new Size(20, 20)};
            checkBox.CheckedChanged += (sender, e) =>
            {
                var parent = (Chart) checkBox.Parent;
                parent.BorderlineWidth = checkBox.Checked ? 2 : 1;
                parent.BorderlineColor = checkBox.Checked ? Color.Blue : Color.Black;
                parent.BorderDashStyle = checkBox.Checked ? ChartDashStyle.Dash : ChartDashStyle.Solid;
            };
            chart.Controls.Add(checkBox);

            //Titles加到chart控件中
            //results.ForEach(rslt => chart.Titles.Add(rslt.Title));

            //startPos变量和MouseDown、MouseMove两个事件用于用鼠标在界面内拖动chart控件
            Point startPos = new Point();
            chart.MouseDown += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    startPos = e.Location;
                    //在拖动图件的时候将其带到最上层
                    chart.BringToFront();
                    //Alt键控制图件拖放，使分幅图叠加
                    if (System.Windows.Forms.Control.ModifierKeys == Keys.Alt)
                    {
                        chart.DoDragDrop(chart, DragDropEffects.Move);
                    }
                }
            };
            chart.MouseMove += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    chart.Left = e.X + chart.Left - startPos.X;
                    chart.Top = e.Y + chart.Top - startPos.Y;
                    chart.Invalidate();
                }
            };

            //-------------------------以下代码负责处理图件的拖放处理--------------------------------
            //下面DragOver和DragDrop方法用于控制Chart控件的拖放，即叠加
            //按住Alt键，同时拖动分幅图，拖动到另外一幅图上，可将两个分幅图叠加可将分幅图叠加
            chart.AllowDrop = true;
            chart.DragOver += (sender, e) =>
            {
                if (System.Windows.Forms.Control.ModifierKeys == Keys.Alt)
                {
                    e.Effect = DragDropEffects.Move;
                }
            };
            chart.DragDrop += (sender, e) =>
            {
                if (System.Windows.Forms.Control.ModifierKeys == Keys.Alt)
                {
                    System.Windows.Forms.Control c = e.Data.GetData(e.Data.GetFormats()[0]) as System.Windows.Forms.Control;
                    if (c != null)
                    {
                        Chart otherChart = (Chart) c;

                        //获取数据源和标题，生成ChartArea和Series对象插入到目标Chart控件中
                        var dt = otherChart.Series[0].Points.ToDataTable();
                        var chartArea = GetChartArea(dt, otherChart.Titles[0].Text);
                        var series = GetSeries(dt, otherChart.Titles[0].Text);

                        chart.ChartAreas.Add(chartArea);
                        chart.Series.Add(series);
                        chart.Titles.Add(chartArea.Name);
                        chart.Size = new Size(width, chart.ChartAreas.Count*(heightPerArea+2));
                        var chartAreaHeightPercent = (int) Math.Floor(100.0/chart.ChartAreas.Count);
                        for (int i = 0; i < chart.ChartAreas.Count; i++)
                        {
                            var area = chart.ChartAreas[i];
                            area.Position.Auto = false;
                            area.Position.Width = chartAreaWidthPercent;
                            area.Position.Height = chartAreaHeightPercent;
                            area.AxisX.Enabled = AxisEnabled.False;
                            //令绘图区域的Y轴对齐
                            area.Position.X = 5;
                            if (i == 0)
                            {
                                area.Position.Y = (chart.ChartAreas.Count - 1)*chartAreaHeightPercent + startY + 1;
                                area.AxisX.Enabled = AxisEnabled.True;
                            }
                            //除第1条和最后1条曲线外，其他曲线的处理
                            else if (i != chart.ChartAreas.Count - 1)
                            {
                                area.Position.Y = (chart.ChartAreas.Count - 1 - i)*chartAreaHeightPercent + startY + 1;
                            }
                            //最后一条曲线的处理
                            else
                            {
                                area.Position.Y = startY;
                                area.AxisY.ArrowStyle = AxisArrowStyle.SharpTriangle;
                            }
                            chart.Titles[i].DockedToChartArea = area.Name;
                        }
                    }
                }
            };
            //---------------------------------------------------------------------------------------

            /**
             * 根据计算结果生成chartArea，
             * 计算并设置chartArea的高度和Y坐标，
             * 从而形成分幅图，
             * 将图标题固定到chartArea
             */
            for (int i = 0; i < results.Count; i++)
            {
                var rslt = results[i];
                var chartArea = GetChartArea(rslt.NumericalTable.Copy(), i);
                var series = GetSeries(rslt.NumericalTable.Copy());
                series.ChartArea = chartArea.Name;
                //如果计算结果中只有一条曲线，那简单了
                var chartAreaHeightPercent = (int) Math.Floor(100.0/chart.ChartAreas.Count);
                if (results.Count == 1)
                {
                    chartArea.Position.Auto = false;
                    chartArea.Position.Width = chartAreaWidthPercent;
                    chartArea.Position.Height = 100;
                    chartArea.Position.Y = startY;
                    chartArea.Position.X = startX;
                    //chartArea.Name = chart.Titles[0].Text;
                    //设置该Tag属性是为了后面的分幅图拼图需要，Tag为0的ChartArea在最下面
                    //chartArea.Tag = 0; 
                    //chartArea.AxisX.ArrowStyle = AxisArrowStyle.SharpTriangle;
                    //chartArea.AxisY.ArrowStyle = AxisArrowStyle.SharpTriangle;
                    chart.Series.Add(series);
                    chart.ChartAreas.Add(chartArea);
                    chart.Titles[0].DockedToChartArea = chartArea.Name;
                    chart.ChartAreas[0].Name = chart.Titles[0].Text;
                    return chart;
                }
                //第1条曲线的处理
                if (i == 0)
                {
                    chartArea.Position.Auto = false;
                    chartArea.Position.Width = chartAreaWidthPercent;
                    chartArea.Position.Height = chartAreaHeightPercent;
                    chartArea.Position.Y = startY;
                    chartArea.AxisX.Enabled = AxisEnabled.False;
                    //chartArea.AxisY.ArrowStyle = AxisArrowStyle.SharpTriangle;
                }
                //除第1条和最后1条曲线外，其他曲线的处理
                else if (i != results.Count - 1)
                {
                    chartArea.Position.Auto = false;
                    chartArea.Position.Width = chartAreaWidthPercent;
                    chartArea.Position.Height = chartAreaHeightPercent;
                    chartArea.Position.Y = chart.ChartAreas[i - 1].Position.Bottom;
                    chartArea.AxisX.Enabled = AxisEnabled.False;
                }
                //最后一条曲线的处理
                else
                {
                    chartArea.Position.Auto = false;
                    chartArea.Position.Width = chartAreaWidthPercent;
                    chartArea.Position.Height = chartAreaHeightPercent;
                    chartArea.Position.Y = chart.ChartAreas[i - 1].Position.Bottom;
                    //chartArea.AxisX.ArrowStyle = AxisArrowStyle.SharpTriangle;
                }
                Debug.Print("{0},{1}", chartArea.Position.Y, chartArea.Position.Height);
                chart.Series.Add(series);
                chart.ChartAreas.Add(chartArea);
                //固定标题
                chart.Titles[i].DockedToChartArea = chartArea.Name;
            }
            return chart;
        }

        #endregion

        #region Chart扩展方法

        /// <summary>
        /// Chart撤销上一步操作
        /// 从栈顶弹出一个数据源
        /// 与Chart控件重新绑定
        /// BTW，修改了chart的操作日志
        /// 删除了最后1条操作日志
        /// </summary>
        /// <param name="chart"></param>
        public static void WithDraw(this Chart chart)
        {
            if (chart.GetStack().Count > 0)
            {
                BindChartWithData(chart, chart.GetStack().Pop());
                chart.GetLogger().Remove(chart.GetLogger().Last());
                Debug.Print("pop datatable from stack, current stack size:{0},logger size:{1}", chart.GetStack().Count,
                    chart.GetLogger().Count);
            }
        }

        /// <summary>
        /// 获取Chart控件的Stack&lt;DataTable&gt;，用于撤销操作
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static Stack<DataTable> GetStack(this Chart chart)
        {
            return (Stack<DataTable>) chart.Tag;
        }

        /// <summary>
        /// 设置Chart控件的Stack&lt;DataTable&gt;，用于撤销操作
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="stack"></param>
        public static void SetStack(this Chart chart, Stack<DataTable> stack)
        {
            chart.Tag = stack;
        }

        /// <summary>
        /// 获取Chart控件的List&lt;String&gt;,用于记录用户对基础数据的操作
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static List<string> GetLogger(this Chart chart)
        {
            if (chart.ChartAreas[0] == null)
                throw new NullReferenceException("Can not get the logger, chartArea is null!");
            return (List<string>) chart.ChartAreas[0].Tag;
        }

        /// <summary>
        /// 设置Chart控件的Logger，用于记录用户的操作
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="list"></param>
        public static void SetLogger(this Chart chart, List<String> list)
        {
            if (chart.ChartAreas[0] == null)
                throw new NullReferenceException("Can not set the logger, chartArea is null!");
            chart.ChartAreas[0].Tag = list;
        }

        /// <summary>
        /// 获得Chart控件中的CheckBox
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static CheckBox GetCheckBox(this Chart chart)
        {
            CheckBox checkBox = null;
            foreach (var control in chart.Controls)
            {
                if (control is CheckBox)
                {
                    checkBox = (CheckBox)control;
                    return checkBox;
                }
            }
            throw new Exception("Seems no checkbox in chart!");
        }

        /// <summary>
        /// 该（分幅图管理）中的Chart控件是否被选中
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static bool Checked(this Chart chart)
        {
            return chart.GetCheckBox().Checked;
        }

        /// <summary>
        /// 获取Chart控件的数据源，
        /// 即一个DataTable
        /// 该方法必须要在设置chart控件
        /// 的DataSource属性后才能使用
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static DataTable GetTable(this Chart chart)
        {
            return (DataTable) chart.DataSource;
        }

        /// <summary>
        /// 向Chart控件中插值
        /// 一般用于缺数处理
        /// 差值成功返回True，否则返回False
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="date"></param>
        /// <param name="value"></param>
        public static bool Interpolation(this Chart chart, DateTime date, double value, string processName)
        {
            var dt = chart.GetTable();
            //当前数据源入栈，用于撤销
            chart.GetStack().Push(dt.Copy());
            LogPushStack(chart.GetStack());
            //将得到的插值界面加入数据源
            var row = dt.NewRow();
            row[X_TITLE] = date;
            row[Y_TITLE] = value;
            dt.Rows.Add(row);
            dt.AcceptChanges();
            dt.DefaultView.Sort = X_TITLE + " ASC";
            BindChartWithData(chart, dt.DefaultView.ToTable());

            chart.GetLogger().Add(string.Format("{0}, {1},{2}", processName, date.SStr(), value));

            return true;
        }

        /// <summary>
        /// 减最小值处理
        /// </summary>
        /// <param name="chart"></param>
        public static void SubtractMinValue(this Chart chart)
        {
            var stack = chart.GetStack();
            stack.Push(chart.GetTable().Copy());
            LogPushStack(stack);

            var dt = chart.GetTable().Copy();
            var min = (double) dt.Compute("min(观测值)", "");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["观测值"] = Convert.ToDouble(dt.Rows[i]["观测值"]) - min;
            }
            dt.AcceptChanges();
            BindChartWithData(chart, dt);
        }

        /// <summary>
        /// 减最大值处理
        /// </summary>
        /// <param name="chart"></param>
        public static void SubtractMaxValue(this Chart chart)
        {
            var stack = chart.GetStack();
            stack.Push(chart.GetTable().Copy());
            LogPushStack(stack);

            var dt = chart.GetTable().Copy();
            var max = (double) dt.Compute("max(观测值)", "");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["观测值"] = Convert.ToDouble(dt.Rows[i]["观测值"]) - max;
            }
            dt.AcceptChanges();
            BindChartWithData(chart, dt);
        }

        /// <summary>
        /// 变符号处理
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static void InverseYValues(this Chart chart)
        {
            var stack = chart.GetStack();
            stack.Push(chart.GetTable().Copy());
            LogPushStack(stack);

            var dt = chart.GetTable().Copy();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["观测值"] = 0 - Convert.ToDouble(dt.Rows[i]["观测值"]);
            }
            dt.AcceptChanges();
            BindChartWithData(chart, dt);
        }

        #endregion

        #region ChartArea的扩展方法

#endregion

        #region DataPoint等其他扩展方法

        /// <summary>
        /// 根据datapoint的值获取标签
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        private static string GetLabel(this DataPoint dp)
        {
            return dp.GetDateStr() + "\n" + dp.GetValue();
        }

        /// <summary>
        /// 以日期形式获取数据点的X的值
        /// </summary>
        /// <param name="dataPoint"></param>
        /// <returns></returns>
        public static DateTime GetDate(this DataPoint dataPoint)
        {
            return DateTime.FromOADate(dataPoint.XValue);
        }

        /// <summary>
        /// 以日期形式获取数据点的X的值，并转换为短日期字符串
        /// </summary>
        /// <param name="dataPoint"></param>
        /// <returns></returns>
        public static string GetDateStr(this DataPoint dataPoint)
        {
            return dataPoint.GetDate().ToShortDateString();
        }

        /// <summary>
        /// 获取数据点的Y值
        /// </summary>
        /// <param name="dataPoint"></param>
        /// <returns></returns>
        public static double GetValue(this DataPoint dataPoint)
        {
            return Math.Round(dataPoint.YValues[0], Xb2Config.GetPrecision());
        }

        /// <summary>
        /// 在DataPoints中找到被矩形包括的点
        /// 需要传入chartArea对象，因为需要该对象
        /// 对数据点的横纵坐标进行映射，从数据空间
        /// 映射到界面空间
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="rectangle"></param>
        /// <param name="chartArea"></param>
        /// <returns></returns>
        public static List<DataPoint> ContainsBy(this DataPointCollection collection, Rectangle rectangle,
            ChartArea chartArea)
        {
            var ans = new List<DataPoint>();
            foreach (DataPoint dataPoint in collection)
            {
                var xpos = (float) chartArea.AxisX.ValueToPixelPosition(dataPoint.XValue);
                var ypos = (float) chartArea.AxisY.ValueToPixelPosition(dataPoint.YValues[0]);
                var pixelPoint = Point.Round(new PointF(xpos, ypos));
                if (rectangle.Contains(pixelPoint))
                {
                    ans.Add(dataPoint);
                }
            }
            return ans;
        }

        /// <summary>
        /// 将数据点转换为DateValue
        /// </summary>
        /// <param name="dataPoint"></param>
        /// <returns></returns>
        public static DateValue ToDateValue(this DataPoint dataPoint)
        {
            var date = DateTime.FromOADate(dataPoint.XValue);
            var value = dataPoint.YValues[0];
            return new DateValue(date,value);
        }

        /// <summary>
        /// DataPoint集合转换为DateValue集合
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <returns></returns>
        public static List<DateValue> ToDateValues(this IEnumerable<DataPoint> dataPoints)
        {
            var dateValues = new List<DateValue>();
            foreach (DataPoint dataPoint in dataPoints)
            {
                var date = DateTime.FromOADate(dataPoint.XValue);
                var value = dataPoint.YValues[0];
                dateValues.Add(new DateValue(date, value));
            }
            return dateValues;
        }

        /// <summary>
        /// 将数据点转换为DataTable
        /// 该DataTable有两列：观测日期，观测值
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this IEnumerable<DataPoint> dataPoints)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("观测日期", typeof(DateTime));
            dataTable.Columns.Add("观测值", typeof(double));
            foreach (DataPoint dataPoint in dataPoints)
            {
                var date = DateTime.FromOADate(dataPoint.XValue);
                var value = dataPoint.YValues[0];
                var dataRow = dataTable.NewRow();
                dataRow["观测日期"] = date;
                dataRow["观测值"] = value;
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }

        #endregion

        #region 废弃的代码

        /// <summary>
        /// 突跳处理→拟合→线性拟合
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public static Chart GetRegressionChart()
        {
            var baseChart = GetTemplateChart();
            //目标点
            DataPoint targetPoint = null;
            //绘制的矩形框
            var rectanges = new List<Rectangle>();
            //鼠标起止点，用于控制绘制矩形
            Point startPos = new Point();
            Point currentPos = new Point();
            //正在绘制矩形的标识
            bool isDrawing = false;
            //矩形选中的数据点，用于拟合
            var selectedDataPoints = new List<DataPoint>();
            baseChart.MouseDown += delegate(object sender, MouseEventArgs e)
            {
                //左键才可以画矩形
                if (e.Button != MouseButtons.Left) return;
                var ht = baseChart.HitTest(e.Location.X, e.Location.Y, false);
                //如果点击到绘图区，并且矩形数量少于2个，则可以开始绘制矩形
                if (ht.ChartElementType == ChartElementType.PlottingArea)
                {
                    if (rectanges.Count >= 2) return;
                    currentPos = startPos = e.Location;
                    isDrawing = true;
                }
                if (targetPoint != null) targetPoint = null;
                //点击类型是数据点，且已经绘制了2个矩形，则将该点设为目标点
                if (ht.ChartElementType == ChartElementType.DataPoint && rectanges.Count == 2)
                {
                    targetPoint = baseChart.Series[0].Points[ht.PointIndex];
                }
            };

            baseChart.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                //画矩形
                currentPos = e.Location;
                if (isDrawing) baseChart.Invalidate();
                //绘制数据点标签
                if (targetPoint != null && e.Button == MouseButtons.Left)
                {
                    targetPoint.YValues[0] = baseChart.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);
                    if (selectedDataPoints.Count > 0)
                    {
                        //目标点加入回归数据点集合，然后对这些数据进行排序和去重
                        selectedDataPoints.Add(targetPoint);
                        selectedDataPoints.Sort((p1, p2) => p1.XValue.CompareTo(p2.XValue));
                        selectedDataPoints = selectedDataPoints.Distinct().ToList();
                        //调用数据处理方法进行回归
                        targetPoint.Label = Processmethod.GetLinearRegressionExpression(selectedDataPoints);
                        //强制重绘Chart控件
                        baseChart.Invalidate();
                    }
                }
            };

            baseChart.MouseUp += delegate(object sender, MouseEventArgs e)
            {
                if (isDrawing)
                {
                    isDrawing = false;
                    var rc = GetRect(startPos, currentPos);
                    if (rc.Width > 0 && rc.Height > 0) rectanges.Add(rc);
                    if (rectanges.Count == 2)
                    {
                        var points1 = baseChart.Series[0].Points.ContainsBy(rectanges[0], baseChart.ChartAreas[0]);
                        var points2 = baseChart.Series[0].Points.ContainsBy(rectanges[1], baseChart.ChartAreas[0]);
                        if (points1.Count == 0 || points2.Count == 0)
                        {
                            MessageBox.Show("选取数据不完整，无法进行线性拟合！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            rectanges.Clear();
                            baseChart.Invalidate();
                            return;
                        }
                        selectedDataPoints.AddRange(points1);
                        selectedDataPoints.AddRange(points2);
                    }
                }
                if (targetPoint != null)
                {
                    targetPoint.Label = null;
                    targetPoint = null;
                    baseChart.Invalidate();
                }
            };

            baseChart.Paint += delegate(object sender, PaintEventArgs e)
            {
                //绘制已有的矩形
                if (rectanges.Count > 0)
                {
                    e.Graphics.DrawRectangles(new Pen(RECT_COLOR, RECT_FITNESS), rectanges.ToArray());
                }
                //绘制正在拖动，正在画的矩形
                if (isDrawing)
                {
                    e.Graphics.DrawRectangle(new Pen(RECT_COLOR, RECT_FITNESS), GetRect(startPos, currentPos));
                }
            };
            return baseChart;
        }

        /// <summary>
        /// 获取chart控件中由矩形选择的那些点
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        [Obsolete]
        public static Dictionary<int, List<DataPoint>> GetRectSelectedPoints(this Chart chart)
        {
            var answ = new Dictionary<int, List<DataPoint>>();
            if (chart.Series.Count > 0)
            {
                if (chart.Series[0].Tag != null)
                {
                    answ = (Dictionary<int, List<DataPoint>>) chart.Series[0].Tag;
                }
                else
                {
                    throw new ArgumentException("无法获取矩形选取的数据点！");
                }
            }
            return answ;
        }

        #endregion

    }
}
