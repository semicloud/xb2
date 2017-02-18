using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using NLog;
using Xb2.Algorithms.Core.Entity;
using Xb2.Entity.Business;
using Xb2.GUI.Catalog;
using Xb2.GUI.Main;
using Xb2.Utils.Control;

namespace Xb2.GUI.Computing
{
    public partial class FrmDisplayCharts : FrmBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public FrmDisplayCharts(XbUser user)
        {
            InitializeComponent();
            this.User = user;
            this.Closed += FrmDisplayCharts_Closed;
            this.Load += FrmDisplayCharts_Load;
        }

        //X和Y的起始坐标
        public Point EditedChartLocation { get; private set; }

        #region 菜单项和控件事件，合并、导出还未完成

        private void 整理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //行列之间的距离
            var rowMargin = 5;
            var colMargin = 5;
            var charts = GetAllCharts();
            var totalCount = charts.Count;
            //分幅图的列数（可以考虑根据显示器的大小来计算分幅图的列数）
            var colNumber = 4;
            var rowNumber = (int) Math.Ceiling((float) totalCount/(float) colNumber);
            var width = charts[0].Width;
            var height = charts[0].Height;
            var locations = new List<Point>();
            int x0 = 5, y0 = 27 + 5;
            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < colNumber; j++)
                {
                    var point = new Point(x0 + j*(width + colMargin), y0 + i*(height + rowMargin));
                    locations.Add(point);
                }
            }
            for (int i = 0; i < charts.Count; i++)
            {
                charts[i].Location = locations[i];
            }
            panel1.Focus();
        }

        private void 全选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var control in panel1.Controls)
            {
                if (control is Chart)
                {
                    ((Chart) control).GetCheckBox().Checked = true;
                }
            }
        }

        private void 反选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var control in panel1.Controls)
            {
                if (control is Chart)
                {
                    var chart = (Chart) control;
                    chart.GetCheckBox().Checked = !chart.Checked();
                }
            }
        }

        private void 清除所有ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            panel1.Invalidate();
        }

        private void 清除选中StripMenuItem5_Click(object sender, EventArgs e)
        {
            var checkedCharts = GetCheckedCharts();
            foreach (var checkedChart in checkedCharts)
            {
                panel1.Controls.Remove(checkedChart);
            }
            panel1.Invalidate();
        }

        private void 调整大小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GetCheckedCharts().Count == 0)
            {
                MessageBox.Show("未选择分幅图！");
                return;
            }
            var chart = GetCheckedCharts().First();
            var frmResizeCharts = new FrmResizeCharts();
            frmResizeCharts.Owner = this;
            frmResizeCharts.StartPosition = FormStartPosition.CenterScreen;
            frmResizeCharts.textBox1.Text = chart.Width.ToString();
            frmResizeCharts.textBox2.Text = chart.Height.ToString();
            frmResizeCharts.Show();
        }

        /// <summary>
        /// 图形修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 图形修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var charts = GetCheckedCharts();
            if (charts.Count == 0)
            {
                MessageBox.Show("请至少选中一张分幅图！");
                return;
            }
            if (charts.Count > 1)
            {
                MessageBox.Show("每次只能编辑一张分幅图！");
                return;
            }
            var chart = charts.First();
            //记住编辑的图的位置，编辑完成后需要把图放回这个位置
            this.EditedChartLocation = chart.Location;
            panel1.Controls.Remove(chart);
            
            //打开图形编辑界面，由用户编辑图件
            var frmConfigChart = new FrmConfigChart();
            chart.Dock = DockStyle.Fill;
            chart.ContextMenuStrip = frmConfigChart.contextMenuStrip1;
            frmConfigChart.Controls.Add(chart);
            frmConfigChart.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            frmConfigChart.Owner = this;
            frmConfigChart.StartPosition = FormStartPosition.CenterScreen;
            frmConfigChart.ShowDialog();
        }

        private void 标地震ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var checkedCharts = GetCheckedCharts();
            if (checkedCharts.Count != 1)
            {
                MessageBox.Show("请选中一幅图进行标地震!");
                return;
            }
            FrmEditLabelDatabase frmEditLabelDatabase = new FrmEditLabelDatabase(this.User);
            frmEditLabelDatabase.ShowDialog();
            var dt = frmEditLabelDatabase.SelectedCategories;
            if (dt != null)
            {
                Debug.Print("有" + dt.Rows.Count + "条数据将被标地震！");
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("标注库无数据！");
                    return;
                }
                var chart = checkedCharts.First();
                chart.LabelingEarthquakes(dt);
            }
        }

        private void 单测项差分ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 合并1XNYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var charts = GetCheckedCharts();
            if (charts.Count < 2)
            {
                MessageBox.Show("请至少选中2幅分幅图再合并！");
                return;
            }
            var confirm = MessageBox.Show("确定合并吗？", "提问",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (confirm == DialogResult.OK)
            {
                MessageBox.Show("合并了");
            }

        }

        private void 合并1X1YToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var charts = GetCheckedCharts();
            if (charts.Count < 2)
            {
                MessageBox.Show("请至少选中2幅分幅图再合并！");
                return;
            }
            var confirm = MessageBox.Show("确定合并吗？", "提问", 
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (confirm == DialogResult.OK)
            {
                charts.ForEach(panel1.Controls.Remove);
                panel1.Invalidate();

                var chart = new Chart();
                var titles = charts.Select(c => c.Titles[0]).ToList();
                var tables = charts.Select(c => (DataTable) c.ChartAreas[0].Tag).ToList();
                chart.ChartAreas.Add(new ChartArea());
                chart.Size = charts.First().Size;

            }
        }

        private void 导出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var checkedCharts = GetCheckedCharts();
            if (checkedCharts.Count != 1)
            {
                MessageBox.Show("请选中一张分幅图再进行导出！");
                return;
            }
            var chart = checkedCharts.First();
            chart.BorderDashStyle = ChartDashStyle.NotSet;
            chart.GetCheckBox().Visible = false;

            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Png图片(*.png)|*.png|EMF矢量图形(*.emf)|*.emf";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                //获取导出路径
                String fileName = saveFileDialog.FileName;
                String extentName = Path.GetExtension(fileName);
                Debug.Print("导出路径：{0}，扩展名：{1}", fileName, extentName);
                if (string.IsNullOrEmpty(extentName))
                {
                    MessageBox.Show("扩展名不可为空！");
                    return;
                }

                #region 保存并设置chart的各种属性，在导出图件后恢复；否则导出的图件分辨率太低，不清楚

                var times = 10; //图件导出时放大的倍数
                chart.GetCheckBox().Visible = false;
                var oldChartSize = chart.Size;
                var oldTitleFont = chart.Titles[0].Font;
                var oldAxisTitleFont = chart.ChartAreas[0].AxisX.TitleFont;
                var oldAxisLabelFont = chart.ChartAreas[0].AxisX.LabelStyle.Font;
                var oldAxisLineWidth = chart.ChartAreas[0].AxisX.LineWidth;
                var oldAxisTickMarkLineWidth = chart.ChartAreas[0].AxisX.MajorTickMark.LineWidth;
                var oldAxisYIntervals = chart.ChartAreas[0].AxisY.Interval;
                var oldAxisXIntervals = chart.ChartAreas[0].AxisX.Interval;
                var oldSeriesBorderWidth = chart.Series[0].BorderWidth;

                chart.Size = new Size(chart.Width * times, chart.Height * times);
                chart.BorderDashStyle = ChartDashStyle.NotSet;
                chart.Titles[0].Font = new Font(oldTitleFont.Name, oldTitleFont.Size * times);
                chart.ChartAreas[0].AxisX.TitleFont = new Font(oldAxisTitleFont.Name, oldAxisTitleFont.Size * times);
                chart.ChartAreas[0].AxisY.TitleFont = new Font(oldAxisTitleFont.Name, oldAxisTitleFont.Size * times);
                chart.ChartAreas[0].AxisX.LabelStyle.Font = new Font(oldAxisLabelFont.Name,
                    oldAxisLabelFont.Size * times);
                chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font(oldAxisLabelFont.Name,
                    oldAxisLabelFont.Size * times);
                chart.ChartAreas[0].AxisX.LineWidth = oldAxisLineWidth * times;
                chart.ChartAreas[0].AxisY.LineWidth = oldAxisLineWidth * times;
                chart.ChartAreas[0].AxisX.MajorTickMark.LineWidth = oldAxisTickMarkLineWidth * times;
                chart.ChartAreas[0].AxisY.MajorTickMark.LineWidth = oldAxisTickMarkLineWidth * times;
                //坐标轴上的Label间距先不调整了
                //chart.ChartAreas[0].AxisX.Interval = oldAxisXIntervals/Math.Round(times/3.0);
                //chart.ChartAreas[0].AxisY.Interval = oldAxisYIntervals/Math.Round(times/3.0);
                chart.Series[0].BorderWidth = oldSeriesBorderWidth * times;
                chart.Invalidate();

                #endregion

                if (extentName.Equals(".png"))
                {
                    chart.SaveImage(fileName, ChartImageFormat.Png);
                }
                else if (extentName.Equals(".emf"))
                {
                    chart.SaveImage(fileName, ChartImageFormat.EmfPlus);
                }
                else
                {
                    MessageBox.Show("不支持的扩展名：" + extentName + "！");
                }

                #region 恢复Chart的各种属性

                chart.GetCheckBox().Visible = true;
                chart.Size = oldChartSize;
                chart.BorderDashStyle = ChartDashStyle.Dash;
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

                #endregion
            }
            else
            {
                // 取消导出，恢复分幅图的复选框
                chart.BorderDashStyle = ChartDashStyle.Dash;
                chart.GetCheckBox().Visible = true;
            }
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //如果每个分幅图的大小不一样，整理 菜单项不可用
            var charts = GetAllCharts();
            if (charts.Count > 0)
            {
                var size = charts.First().Size;
                var allSizeEqual = true;
                foreach (var chart in charts)
                {
                    if (chart.Size != size)
                    {
                        allSizeEqual = false;
                        break;
                    }
                }
                整理ToolStripMenuItem.Enabled = allSizeEqual;
            }
        }

        void FrmDisplayCharts_Load(object sender, EventArgs e)
        {
            // 先不显示主屏幕上的工具栏
            //var toolStripContainer = this.GetMainForm().toolStripContainer1.TopToolStripPanel;
            //toolStripContainer.Controls.Add(ToolStripHelper.GetChartToolStrip());

            panel1.ContextMenuStrip = contextMenuStrip1;
        }

        void FrmDisplayCharts_Closed(object sender, EventArgs e)
        {
            this.GetMainForm().toolStripContainer1.TopToolStripPanel.Controls.Clear();
        }

        #endregion

        #region 私有函数

        /// <summary>
        /// 获取界面中选中的那些Chart
        /// </summary>
        /// <returns></returns>
        private List<Chart> GetCheckedCharts()
        {
            var ans = new List<Chart>();
            foreach (var control in panel1.Controls)
            {
                if (control is Chart)
                {
                    var chart = (Chart) control;
                    if (chart.Checked())
                    {
                        ans.Add(chart);
                    }
                }
            }
            return ans;
        }

        /// <summary>
        /// 获取界面中的所有Chart
        /// </summary>
        /// <returns></returns>
        private List<Chart> GetAllCharts()
        {
            var charts = new List<Chart>();
            if (panel1.Controls.Count > 0)
            {
                for (int i = 0; i < panel1.Controls.Count; i++)
                {
                    if (panel1.Controls[i] is Chart)
                    {
                        charts.Add((Chart) panel1.Controls[i]);
                    }
                }
            }
            return charts;
        }

        #endregion

        #region 对外接口，公有函数

        private Int32 _posX = 5;
        private Int32 _posY = 27 + 5;

        /// <summary>
        /// 向该界面中添加一个Chart
        /// </summary>
        /// <param name="calcResult"></param>
        public void AddChart(CalcResult calcResult)
        {
            var chart = ChartHelper.BuildChart(calcResult);
            chart.Location = new Point(_posX, _posY);
            panel1.Controls.Add(chart);
            _posX += 5;
            _posY += 5;
            panel1.Focus();
        }

        /// <summary>
        /// 调整选中的Chart控件的大小
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void ResizeCharts(int width, int height)
        {
            var charts = GetCheckedCharts();
            if (charts.Count > 0)
            {
                foreach (var chart in charts)
                {
                    chart.Height = height;
                    chart.Width = width;
                    chart.Refresh();
                }
            }
            else
            {
                MessageBox.Show("请选中分幅图再调整大小！");
            }
        }

        /// <summary>
        /// 1X轴1个y轴合并
        /// </summary>
        public void Merge1X1Y()
        {

        }

        /// <summary>
        /// 1X轴多个y轴合并
        /// </summary>
        public void Merage1XdY()
        {

        }

        #endregion
    }
}
