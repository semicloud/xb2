using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Xb2.Algorithms.Core.Entity;
using Xb2.Algorithms.Core.Methods.FaultOffset;
using Xb2.Algorithms.Core.Methods.Rate;
using Xb2.Algorithms.Core.Methods.Regression;
using Xb2.Algorithms.Core.Methods.YearChange;
using Xb2.Entity;
using Xb2.Entity.Business;
using Xb2.GUI.Catalog;
using Xb2.GUI.Computing;
using Xb2.GUI.Input.Forms;
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
                var frmDisplayCharts = GetChartForm();
                foreach (var calcResult in calcResults)
                {
                    frmDisplayCharts.AddChart(calcResult);
                }
            }
        }

        private void 消趋势ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmInputXqs frmInputXqs = new FrmInputXqs(this.User);
            if (frmInputXqs.ShowDialog() == DialogResult.OK)
            {
                var chartForm = GetChartForm();
                var inputs = frmInputXqs.GetXqsInputs();
                foreach (var input in inputs)
                {
                    var regres = new Xb2Regression(input);
                    chartForm.AddChart(regres.GetFittingLine());
                    chartForm.AddChart(regres.GetRawLine());
                    chartForm.AddChart(regres.GetResidualLine());
                }
            }
        }

        private void 年周变ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmInputNzb frmInputXqs = new FrmInputNzb(this.User);
            if (frmInputXqs.ShowDialog() == DialogResult.OK)
            {
                var chartForm = GetChartForm();
                var inputs = frmInputXqs.GetNzbInputs();
                foreach (var input in inputs)
                {
                    var yc = new Xb2YearChange(input);
                    chartForm.AddChart(yc.GetNianZhouBianLine());
                    chartForm.AddChart(yc.GetYueJuPingLine());
                }
            }
        }

        private void 速率差分ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmInputSlcf frmInputXqs = new FrmInputSlcf(this.User);
            if (frmInputXqs.ShowDialog() == DialogResult.OK)
            {
                var chartForm = GetChartForm();
                var inputs = frmInputXqs.GetSlcfInputs();
                foreach (var input in inputs)
                {
                    var yc = new Xb2Slcf(input);
                    chartForm.AddChart(yc.GetSlcfLine());
                }
            }
        }

        private void 速率合成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmInputSLHC frmInputSlhc = new FrmInputSLHC(this.User);
            if (frmInputSlhc.ShowDialog() == DialogResult.OK)
            {
                var inputs = frmInputSlhc.GetInputs();
                Xb2SLCFHC xb2Slcfhc = new Xb2SLCFHC {Input = inputs};
                var chartForm = GetChartForm();
                chartForm.AddChart(xb2Slcfhc.GetSLHCLine());
            }
        }

        private void 速率累积强度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmInputSLLJQD frmInputSlljqd = new FrmInputSLLJQD(this.User);
            if (frmInputSlljqd.ShowDialog() == DialogResult.OK)
            {
                var chartForm = GetChartForm();
                var inputs = frmInputSlljqd.GetSLLJQDInputs();
                foreach (var input in inputs)
                {
                    var xb2Slljqd = new Xb2SLLJQD(input);
                    chartForm.AddChart(xb2Slljqd.GetLine());
                }
            }
        }

        private void 速率累积强度合成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmInputSLHC frmInputSlhc = new FrmInputSLHC(this.User);
            if (frmInputSlhc.ShowDialog() == DialogResult.OK)
            {
                var inputs = frmInputSlhc.GetInputs();
                Xb2SLLJQDHC xb2Slcfhc = new Xb2SLLJQDHC { Input = inputs };
                var chartForm = GetChartForm();
                chartForm.AddChart(xb2Slcfhc.GetLine());
            }
        }

        private void 断层活动量ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 模式1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmInputDCHDLM1 frmInputDchdlm1 = new FrmInputDCHDLM1(this.User);
            if (frmInputDchdlm1.ShowDialog() == DialogResult.OK)
            {
                var inputs = frmInputDchdlm1.GetInputs();
                Xb2DCHDL_M1 xb2Slcfhc = new Xb2DCHDL_M1(inputs);
                var chartForm = GetChartForm();
                chartForm.AddChart(xb2Slcfhc.GetL());
                chartForm.AddChart(xb2Slcfhc.GetH());
                chartForm.AddChart(xb2Slcfhc.GetS());
                chartForm.AddChart(xb2Slcfhc.GetHS());
            }
        }

        private void 模式2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmInputDCHDLM2 frmInputDchdlm1 = new FrmInputDCHDLM2(this.User);
            if (frmInputDchdlm1.ShowDialog() == DialogResult.OK)
            {
                var inputs = frmInputDchdlm1.GetInputs();
                Xb2DCHDL_M2 xb2DchdlM2 = new Xb2DCHDL_M2(inputs);
                var chartForm = GetChartForm();
                chartForm.AddChart(xb2DchdlM2.GetL1());
                chartForm.AddChart(xb2DchdlM2.GetL2());
                chartForm.AddChart(xb2DchdlM2.GetS());
                chartForm.AddChart(xb2DchdlM2.GetR());
                chartForm.AddChart(xb2DchdlM2.GetRS());
            }
        }

        private void 模式3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmInputDCHDLM3 frmInputDchdlm1 = new FrmInputDCHDLM3(this.User);
            if (frmInputDchdlm1.ShowDialog() == DialogResult.OK)
            {
                var inputs = frmInputDchdlm1.GetInputs();
                Xb2DCHDL_M3 dchdlM3 = new Xb2DCHDL_M3(inputs);
                var chartForm = GetChartForm();
                chartForm.AddChart(dchdlM3.GetL1());
                chartForm.AddChart(dchdlM3.GetL2());
                chartForm.AddChart(dchdlM3.GetH());
                chartForm.AddChart(dchdlM3.GetS());
                chartForm.AddChart(dchdlM3.GetR());
                chartForm.AddChart(dchdlM3.GetRS());
                chartForm.AddChart(dchdlM3.GetHS());
                chartForm.AddChart(dchdlM3.GetHR());
            }
        }

        public FrmDisplayCharts GetChartForm()
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
