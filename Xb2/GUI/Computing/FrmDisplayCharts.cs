using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data.MySqlClient;
using Xb2.Algorithms.Core;
using Xb2.Entity.Business;
using Xb2.GUI.Catalog;
using Xb2.GUI.M.Item;
using Xb2.GUI.Main;
using Xb2.Utils.Control;
using Xb2.Utils.Database;

namespace Xb2.GUI.Computing
{
    public partial class FrmDisplayCharts : FrmBase
    {
        public FrmDisplayCharts(XbUser user)
        {
            InitializeComponent();
            this.CUser = user;
            this.Closed += FrmDisplayCharts_Closed;
            this.Load += FrmDisplayCharts_Load;
        }

        void FrmDisplayCharts_Load(object sender, EventArgs e)
        {
            var toolStripContainer = this.GetMainForm().toolStripContainer1.TopToolStripPanel;
            toolStripContainer.Controls.Add(ToolStripHelper.GetChartToolStrip());

            panel1.ContextMenuStrip = contextMenuStrip1;
        }

        void FrmDisplayCharts_Closed(object sender, EventArgs e)
        {
            this.GetMainForm().toolStripContainer1.TopToolStripPanel.Controls.Clear();
        }

        /// <summary>
        /// 1X轴多个y轴合并
        /// </summary>
        public void Merage1XdY()
        {

        }

        /// <summary>
        /// 1X轴1个y轴合并
        /// </summary>
        public void Merge1X1Y()
        {

        }

        public static DataTable GetDataTable(int id)
        {
            var sql = "select 观测日期,观测值 from 系统_原始数据 where 测项编号={0} order by 观测日期";
            var dt = MySqlHelper.ExecuteDataset(Db.CStr(), string.Format(sql, id)).Tables[0];
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("测项" + id + "无数据！");
            }
            return dt;
        }

        private void 选测项ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            FrmSelectMItem frmSelectMItem = new FrmSelectMItem(this.CUser);
            frmSelectMItem.StartPosition = FormStartPosition.CenterScreen;
            DialogResult dlRslt = frmSelectMItem.ShowDialog();
            int x = 5, y = 27 + 5;
            if (dlRslt == DialogResult.OK)
            {
                var dt = frmSelectMItem.Result;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var id = Convert.ToInt32(dt.Rows[i]["编号"]);
                    var title = dt.Rows[i]["观测单位"] + "-" + dt.Rows[i]["地名"] + "-" + dt.Rows[i]["方法名"] + "-" +
                                dt.Rows[i]["测项名"];
                    var cc = new CalcResult {Title = title, NumericalTable = GetDataTable(id)};
                    var list = new List<CalcResult>() {cc};
                    var chart = ChartHelper.BuildChart(list);
                    chart.Location = new Point(x, y);
                    panel1.Controls.Add(chart);
                    x += 5;
                    y += 5;
                }
            }
            panel1.Focus();
        }

        /// <summary>
        /// 排列菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void arrangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //行列之间的距离
            var rowMargin = 5;
            var colMargin = 5;
            var charts = GetAllCharts();
            var totalCount = charts.Count;
            //分幅图的列数
            var colNumber = 3;
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

        private List<Chart> GetAllCharts()
        {
            var charts = new List<Chart>();
            if (panel1.Controls.Count > 1)
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

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            panel1.Invalidate();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var control in panel1.Controls)
            {
                var c = (Chart) control;
                if (c.Checked())
                {
                    var title = c.Titles[0];
                    c.SaveImage(@"c:\" + title + ".png", ChartImageFormat.Png);
                }
            }
        }

        private void sizeToolStripMenuItem_Click(object sender, EventArgs e)
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

        //全选
        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var control in panel1.Controls)
            {
                if (control is Chart)
                {
                    ((Chart) control).GetCheckBox().Checked = true;
                }
            }
        }

        //反选
        private void invertToolStripMenuItem_Click(object sender, EventArgs e)
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

        public Point EditedChartLocation { get; private set; }

        /// <summary>
        /// 图形修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
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
            this.EditedChartLocation = chart.Location;
            panel1.Controls.Remove(chart);
            
            var frmConfigChart = new FrmConfigChart();
            chart.Dock = DockStyle.Fill;
            chart.ContextMenuStrip = frmConfigChart.contextMenuStrip1;
            frmConfigChart.Controls.Add(chart);
            frmConfigChart.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            frmConfigChart.Owner = this;
            frmConfigChart.StartPosition = FormStartPosition.CenterScreen;
            frmConfigChart.ShowDialog();
        }

        //清除选中
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            var checkedCharts = GetCheckedCharts();
            foreach (var checkedChart in checkedCharts)
            {
                panel1.Controls.Remove(checkedChart);
            }
            panel1.Invalidate();
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

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //如果每个分幅图的大小不一样，排列 菜单项不可用
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
                arrangeToolStripMenuItem.Enabled = allSizeEqual;
            }
        }

        private void 标地震ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmEditLabelDatabase frmEditLabelDatabase = new FrmEditLabelDatabase(this.CUser);
            frmEditLabelDatabase.ShowDialog();
            var dt = frmEditLabelDatabase.ConfirmedDataTable;
            MessageBox.Show("有" + dt.Rows.Count + "条数据将被标地震！");
        }
    }
}
