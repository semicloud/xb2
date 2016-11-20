using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data.MySqlClient;
using Xb2.Algorithms.Core;
using Xb2.Entity.Business;
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
        }

        private void FrmDisplayCharts_Load(object sender, System.EventArgs e)
        {
            panel1.ContextMenuStrip = contextMenuStrip1;
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
            int x = 10, y = 10;
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
            var totalCount = panel1.Controls.Count;
            //分幅图的列数
            var colNumber = 3;
            var rowNumber = (int) Math.Ceiling((float) totalCount/(float) colNumber);
            var width = panel1.Controls[0].Width;
            var height = panel1.Controls[0].Height;
            var locations = new List<Point>();
            int x0 = 5, y0 = 5;
            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < colNumber; j++)
                {
                    var point = new Point(x0 + j*(width + colMargin), y0 + i*(height + rowMargin));
                    locations.Add(point);
                }
            }
            for (int i = 0; i < panel1.Controls.Count; i++)
            {
                panel1.Controls[i].Location = locations[i];
            }
            panel1.Focus();
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
                    c.SaveImage(@"c:\" + title + ".png" ,ChartImageFormat.Png);
                }
            }
        }


    }
}
