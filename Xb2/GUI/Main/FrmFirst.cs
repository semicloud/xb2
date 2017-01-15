using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Xb2.Algorithms.Core.Entity;
using Xb2.Algorithms.Core.Methods.Regression;
using Xb2.Entity;
using Xb2.Entity.Business;
using Xb2.GUI.Catalog;
using Xb2.GUI.Computing;
using Xb2.GUI.Computing.Input;
using Xb2.GUI.M.Item;
using Xb2.GUI.M.Val.ProcessedData;
using Xb2.GUI.M.Val.Rawdata;
using Xb2.Utils;
using Xb2.Utils.Control;

namespace Xb2.GUI.Main
{
    public partial class FrmFirst : FrmBase
    {
        public FrmFirst(XbUser user)
        {
            InitializeComponent();
            this.User = user;
            this.Paint += OnPaint;
        }

        private void OnPaint(object sender, PaintEventArgs paintEventArgs)
        {
        }

        private void 生成基础数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmProcessData frmProcessData = new FrmProcessData(this.User);
            frmProcessData.MdiParent = this;
            frmProcessData.Show();
        }

        private void 管理地震目录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmManageCatalog frmManageCatalog = new FrmManageCatalog(this.User);
            frmManageCatalog.MdiParent = this;
            frmManageCatalog.StartPosition = FormStartPosition.CenterScreen;
            frmManageCatalog.Show();
        }

        private void 生成地震目录子库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmGenSubDatabase frmGenSubDatabase = new FrmGenSubDatabase(this.User);
            frmGenSubDatabase.MdiParent = this;
            frmGenSubDatabase.StartPosition = FormStartPosition.CenterScreen;
            frmGenSubDatabase.Show();
        }

        private void 删除地震目录子库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDelSubDatabase frmDelSubDatabase = new FrmDelSubDatabase(this.User);
            frmDelSubDatabase.MdiParent = this;
            frmDelSubDatabase.StartPosition = FormStartPosition.CenterScreen;
            frmDelSubDatabase.Show();
        }

        private void 生成地震目录标注库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmGenLabelDatabase frmGenLabelDatabase = new FrmGenLabelDatabase(this.User);
            frmGenLabelDatabase.MdiParent = this;
            frmGenLabelDatabase.StartPosition = FormStartPosition.CenterScreen;
            frmGenLabelDatabase.Show();
        }

        private void 管理地震目录标注库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmEditLabelDatabase frmEditLabelDatabase = new FrmEditLabelDatabase(this.User);
            frmEditLabelDatabase.MdiParent = this;
            frmEditLabelDatabase.StartPosition = FormStartPosition.CenterScreen;
            frmEditLabelDatabase.Show();
        }

        private void 管理测项ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSelectMItem frmSelectMItem = new FrmSelectMItem(this.User);
            frmSelectMItem.MdiParent = this;
            frmSelectMItem.StartPosition = FormStartPosition.CenterScreen;
            frmSelectMItem.Show();
        }

        private void 管理原始数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmRawDataManage frmRawDataManage = new FrmRawDataManage(this.User);
            frmRawDataManage.MdiParent = this;
            frmRawDataManage.StartPosition = FormStartPosition.CenterScreen;
            frmRawDataManage.Show();
        }

        private void 新建测项ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmEditMItem frmEditMItem = new FrmEditMItem(Operation.Create, this.User);
            frmEditMItem.MdiParent = this;
            frmEditMItem.StartPosition = FormStartPosition.CenterScreen;
            frmEditMItem.Show();
        }

        private void 原始数据绘图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSelectMItem frmSelectMItem = new FrmSelectMItem(this.User);
            frmSelectMItem.StartPosition = FormStartPosition.CenterScreen;
            if (frmSelectMItem.ShowDialog() == DialogResult.OK)
            {
                var dt = frmSelectMItem.Result;
                var calcResults = new List<CalcResult>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var id = Convert.ToInt32(dt.Rows[i]["编号"]);
                    var title = dt.Rows[i]["观测单位"] + "-" + dt.Rows[i]["地名"] + "-" + dt.Rows[i]["方法名"] + "-" +
                                dt.Rows[i]["测项名"] + "-" + "原始数据";
                    var calcResult = new CalcResult();
                    calcResult.Title = title;
                    calcResult.NumericalTable = DateValueList.FromRawData(id).ToDataTable();
                    calcResults.Add(calcResult);
                }
                var frmDisplayCharts = OpenChartForm();
                foreach (var calcResult in calcResults)
                {
                    frmDisplayCharts.AddChart(calcResult);
                }
            }
        }

        private void 消趋势ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmRegressionInput frmSingleInput = new FrmRegressionInput(this.User);
            frmSingleInput.StartPosition = FormStartPosition.CenterScreen;
            var result = frmSingleInput.ShowDialog();
            if (result == DialogResult.OK)
            {
                var input = frmSingleInput.RegresInput;
                Xb2Regression regression = new Xb2Regression(input);
                var frmDisplayCharts = OpenChartForm();
                frmDisplayCharts.AddChart(regression.GetFittingLine());
                frmDisplayCharts.AddChart(regression.GetResidualLine());
                frmDisplayCharts.AddChart(regression.GetRawLine());
            }
        }

        private FrmDisplayCharts OpenChartForm()
        {
            var title = "分幅图";
            var mdiChildrenTitles = this.MdiChildren.Select(f => f.Text);
            FrmDisplayCharts frmDisplayCharts;
            if (mdiChildrenTitles.Contains(title))
            {
                frmDisplayCharts = (FrmDisplayCharts) (this.MdiChildren.ToList().Find(f => f.Text.Equals(title)));
                frmDisplayCharts.BringToFront();
            }
            else
            {
                frmDisplayCharts = new FrmDisplayCharts(this.User)
                {
                    MdiParent = this,
                    StartPosition = FormStartPosition.CenterScreen,
                };
                frmDisplayCharts.Show();
            }
            return frmDisplayCharts;
        }

        private void 管理基础数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmManageProcessedData frmManageProcessedData = new FrmManageProcessedData(this.User);
            frmManageProcessedData.MdiParent = this;
            frmManageProcessedData.Show();
        }


    }
}
