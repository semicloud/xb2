using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Xb2.GUI.Computing
{
    public partial class FrmConfigChart : Form
    {
        public FrmConfigChart()
        {
            InitializeComponent();
        }

        private void FrmConfigChart_FormClosed(object sender, FormClosedEventArgs e)
        {
            var frmDisplayCharts = (FrmDisplayCharts)this.Owner;
            var chart = GetChart();
            chart.Dock = DockStyle.None;
            chart.Location = frmDisplayCharts.EditedChartLocation;
            chart.ContextMenuStrip = null;
            frmDisplayCharts.panel1.Controls.Add(chart);
            frmDisplayCharts.panel1.Invalidate();
            this.Close();
        }

        private void FrmConfigChart_FormClosing(object sender, FormClosingEventArgs e)
        {
           
        }

        private void 改标题ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var chart = GetChart();
            chart.Titles[0].Text = "这是一个修改了的标题";
            chart.Titles[0].Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
            chart.Invalidate();
        }

        private Chart GetChart()
        {
            foreach (var control in this.Controls)
            {
                if (control is Chart)
                {
                    return (Chart) control;
                }
            }
            throw new Exception("No Chart");
        }

        private void 字体ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var chart = GetChart();
            FontDialog fontDialog = new FontDialog();
            var dialogResult = fontDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                chart.Titles[0].Font = fontDialog.Font;
            }
            chart.Invalidate();
        }

        private void 颜色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var chart = GetChart();
            ColorDialog colorDialog = new ColorDialog();
            var dialogResult = colorDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                chart.Titles[0].ForeColor = colorDialog.Color;
            }
            chart.Invalidate();
        }

        private void ChangeSeriesLineStyle(Object sender, EventArgs e)
        {
            var chart = GetChart();
            var menuItemText = ((ToolStripMenuItem) sender).Text;
            chart.Series[0].BorderDashStyle = GetChartDashStyles()[menuItemText];
            chart.Invalidate();
        }

        private void ChangeSeriesLineWidth(Object sender, EventArgs e)
        {
            var chart = GetChart();
            var menuItemText = ((ToolStripMenuItem)sender).Text;
            chart.Series[0].BorderWidth = Convert.ToInt32(menuItemText);
            chart.Invalidate();
        }

        private void ChangeAxisArrowType(Object sender, EventArgs args)
        {
            var chart = GetChart();
            var menuItemText = ((ToolStripMenuItem)sender).Text;
            chart.ChartAreas[0].AxisX.ArrowStyle = GetArrowStyles()[menuItemText];
            chart.Invalidate();
        }

        private void ChangeAxisWidth(Object sender, EventArgs args)
        {
            var chart = GetChart();
            var menuItemText = ((ToolStripMenuItem) sender).Text;
            chart.ChartAreas[0].AxisX.LineWidth = Convert.ToInt32(menuItemText);
            chart.ChartAreas[0].AxisY.LineWidth = Convert.ToInt32(menuItemText);
            chart.Invalidate();
        }

        private void ChangeAxisFont(Object sender, EventArgs args)
        {
            var chart = GetChart();
            FontDialog fontDialog = new FontDialog();
            var dialogResult = fontDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                chart.ChartAreas[0].AxisX.TitleFont = fontDialog.Font;
                chart.ChartAreas[0].AxisX.LabelStyle.Font = fontDialog.Font;

                chart.ChartAreas[0].AxisY.TitleFont = fontDialog.Font;
                chart.ChartAreas[0].AxisY.LabelStyle.Font = fontDialog.Font;
            }
            chart.Invalidate();
        }

        public  void ChangeXYTitle(string xtitle, string ytitle)
        {
            var chart = GetChart();
            chart.ChartAreas[0].AxisX.Title = xtitle;
            chart.ChartAreas[0].AxisY.Title = ytitle;
            chart.Invalidate();
        }

        private void 颜色ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var chart = GetChart();
            ColorDialog colorDialog = new ColorDialog();
            var dialogResult = colorDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
               chart.Series[0].Color = colorDialog.Color;
            }
            chart.Invalidate();
        }

        public Dictionary<String, ChartDashStyle> GetChartDashStyles()
        {
            var dictionary = new Dictionary<string, ChartDashStyle>();
            dictionary.Add("虚线", ChartDashStyle.Dash);
            dictionary.Add("虚点线", ChartDashStyle.DashDot);
            dictionary.Add("虚点点线", ChartDashStyle.DashDotDot);
            dictionary.Add("点线", ChartDashStyle.Dot);
            dictionary.Add("实线", ChartDashStyle.Solid);
            return dictionary;
        }

        public Dictionary<String, AxisArrowStyle> GetArrowStyles()
        {
            var dictionary = new Dictionary<string, AxisArrowStyle>();
            dictionary.Add("箭头式", AxisArrowStyle.SharpTriangle);
            dictionary.Add("普通式", AxisArrowStyle.None);
            return dictionary;
        }

        private void 标题ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FrmInputSth frmInputSth = new FrmInputSth();
            frmInputSth.Owner = this;

            frmInputSth.ShowDialog();

        }


    }
}
