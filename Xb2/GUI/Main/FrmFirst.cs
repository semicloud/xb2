using System;
using System.Linq;
using System.Windows.Forms;
using Xb2.Algorithms.Core.Entity;
using Xb2.Algorithms.Core.Methods.Regression;
using Xb2.Entity.Business;
using Xb2.GUI.Catalog;
using Xb2.GUI.Computing;
using Xb2.GUI.Computing.Input;
using Xb2.GUI.M.Item;
using Xb2.GUI.M.Val.ProcessedData;
using Xb2.GUI.M.Val.Rawdata;
using Xb2.Utils;

namespace Xb2.GUI.Main
{
    public partial class FrmFirst : FrmBase
    {
        public FrmFirst(XbUser user)
        {
            InitializeComponent();
            this.CUser = user;
            this.Paint += OnPaint;
        }

        private void OnPaint(object sender, PaintEventArgs paintEventArgs)
        {
        }

        private void 生成基础数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmProcessData frmProcessData = new FrmProcessData(this.CUser);
            frmProcessData.MdiParent = this;
            frmProcessData.Show();
        }

        private void 管理地震目录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmManageCatalog frmManageCatalog = new FrmManageCatalog(this.CUser);
            frmManageCatalog.MdiParent = this;
            frmManageCatalog.StartPosition = FormStartPosition.CenterScreen;
            frmManageCatalog.Show();
        }

        private void 生成地震目录子库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmGenSubDatabase frmGenSubDatabase = new FrmGenSubDatabase(this.CUser);
            frmGenSubDatabase.MdiParent = this;
            frmGenSubDatabase.StartPosition = FormStartPosition.CenterScreen;
            frmGenSubDatabase.Show();
        }

        private void 删除地震目录子库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDelSubDatabase frmDelSubDatabase = new FrmDelSubDatabase(this.CUser);
            frmDelSubDatabase.MdiParent = this;
            frmDelSubDatabase.StartPosition = FormStartPosition.CenterScreen;
            frmDelSubDatabase.Show();
        }

        private void 生成地震目录标注库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmGenLabelDatabase frmGenLabelDatabase = new FrmGenLabelDatabase(this.CUser);
            frmGenLabelDatabase.MdiParent = this;
            frmGenLabelDatabase.StartPosition = FormStartPosition.CenterScreen;
            frmGenLabelDatabase.Show();
        }

        private void 管理地震目录标注库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmEditLabelDatabase frmEditLabelDatabase = new FrmEditLabelDatabase(this.CUser);
            frmEditLabelDatabase.MdiParent = this;
            frmEditLabelDatabase.StartPosition = FormStartPosition.CenterScreen;
            frmEditLabelDatabase.Show();
        }

        private void 管理测项ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSelectMItem frmSelectMItem = new FrmSelectMItem(this.CUser);
            frmSelectMItem.MdiParent = this;
            frmSelectMItem.StartPosition = FormStartPosition.CenterScreen;
            frmSelectMItem.Show();
        }

        private void 管理原始数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmRawDataManage frmRawDataManage = new FrmRawDataManage(this.CUser);
            frmRawDataManage.MdiParent = this;
            frmRawDataManage.StartPosition = FormStartPosition.CenterScreen;
            frmRawDataManage.Show();
        }

        private void 新建测项ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmEditMItem frmEditMItem = new FrmEditMItem(Operation.Create, this.CUser);
            frmEditMItem.MdiParent = this;
            frmEditMItem.StartPosition = FormStartPosition.CenterScreen;
            frmEditMItem.Show();
        }

        private void 数据绘图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDisplayCharts frmDisplayCharts = new FrmDisplayCharts(this.CUser);
            frmDisplayCharts.MdiParent = this;
            frmDisplayCharts.StartPosition = FormStartPosition.CenterScreen;
            frmDisplayCharts.Show();
        }

        private void 消趋势ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmRegressionInput frmSingleInput = new FrmRegressionInput(this.CUser)
            {
                StartPosition = FormStartPosition.CenterScreen,
            };
            var result = frmSingleInput.ShowDialog();
            if (result == DialogResult.OK)
            {
                var input = frmSingleInput.RegresInput;
                Xb2Regression regression = new Xb2Regression(input);

                CalcResult calcResult1 = new CalcResult
                {
                    NumericalTable = regression.GetResidualLineData().ToDataTable(),
                    Title = "残差值线"
                };

                CalcResult calcResult2 = new CalcResult
                {
                    NumericalTable = input.List.ToDataTable(),
                    Title = "基础数据"
                };

                var frmDisplayCharts = OpenChartForm();
                frmDisplayCharts.AddChart(calcResult1);
                frmDisplayCharts.AddChart(calcResult2);
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
                frmDisplayCharts = new FrmDisplayCharts(this.CUser)
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
            FrmManageProcessedData frmManageProcessedData = new FrmManageProcessedData(this.CUser);
            frmManageProcessedData.MdiParent = this;
            frmManageProcessedData.Show();
        }


    }
}
