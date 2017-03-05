using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Xb2.Computing.CoreAlgorithms.Entities;
using Xb2.Computing.CoreAlgorithms.Entities.Number;
using Xb2.Computing.CoreAlgorithms.Methods._11YCFD;
using Xb2.Computing.CoreAlgorithms.Methods._1XQS;
using Xb2.Computing.CoreAlgorithms.Methods._2NZB;
using Xb2.Computing.CoreAlgorithms.Methods._4SLCF;
using Xb2.Computing.CoreAlgorithms.Methods._5SLCFHC;
using Xb2.Computing.CoreAlgorithms.Methods._6SLLJQD;
using Xb2.Computing.CoreAlgorithms.Methods._7SLLJQDHC;
using Xb2.Computing.CoreAlgorithms.Methods._8DCHDL_M1;
using Xb2.Computing.CoreAlgorithms.Methods._8DCHDL_M2;
using Xb2.Computing.CoreAlgorithms.Methods._8DCHDL_M3;
using Xb2.Computing.CoreAlgorithms.Methods._8DCHDL_M4;
using Xb2.Computing.CoreAlgorithms.Methods._9YB_M1;
using Xb2.Computing.CoreAlgorithms.Methods._9YB_M2;
using Xb2.Computing.CoreAlgorithms.Methods._9YB_M3;
using Xb2.Entity;
using Xb2.Entity.Business;
using Xb2.GUI.Catalog;
using Xb2.GUI.Computing;
using Xb2.GUI.M.Item;
using Xb2.GUI.M.Val.ProcessedData;
using Xb2.GUI.M.Val.Rawdata;
using Xb2.Utils;
using Xb2.Utils.Database;

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
                    calcResult.NumericalTable = DaoObject.GetRawData(id);
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
            Xb2XQSInputForm xb2XqsInputForm = new Xb2XQSInputForm(this.User);
            if (xb2XqsInputForm.ShowDialog() == DialogResult.OK)
            {
                var chartForm = GetChartForm();
                var inputs = xb2XqsInputForm.GetXqsInputs();
                foreach (var input in inputs)
                {
                    var regres = new Xb2XQS(input);
                    chartForm.AddChart(regres.GetFittingLine());
                    chartForm.AddChart(regres.GetRawLine());
                    chartForm.AddChart(regres.GetResidualLine());
                }
            }
        }

        private void 年周变ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Xb2NZBInputForm xb2NzbInputXqs = new Xb2NZBInputForm(this.User);
            if (xb2NzbInputXqs.ShowDialog() == DialogResult.OK)
            {
                var chartForm = GetChartForm();
                var inputs = xb2NzbInputXqs.GetNzbInputs();
                foreach (var input in inputs)
                {
                    var yc = new Xb2NZB(input);
                    chartForm.AddChart(yc.GetNianZhouBianLine());
                    chartForm.AddChart(yc.GetYueJuPingLine());
                }
            }
        }

        private void 速率差分ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Xb2SLCFInputForm xb2SLCFInputForm = new Xb2SLCFInputForm(this.User);
            if (xb2SLCFInputForm.ShowDialog() == DialogResult.OK)
            {
                var chartForm = GetChartForm();
                var inputs = xb2SLCFInputForm.GetSlcfInputs();
                foreach (var input in inputs)
                {
                    var yc = new XB2SLCF(input);
                    chartForm.AddChart(yc.GetSlcfLine());
                }
            }
        }

        private void 速率合成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Xb2SLCFHCInputForm xb2SlcfhcInputForm = new Xb2SLCFHCInputForm(this.User);
            if (xb2SlcfhcInputForm.ShowDialog() == DialogResult.OK)
            {
                var inputs = xb2SlcfhcInputForm.GetInputs();
                Xb2SLCFHC xb2Slcfhc = new Xb2SLCFHC {Input = inputs};
                var chartForm = GetChartForm();
                chartForm.AddChart(xb2Slcfhc.GetSLHCLine());
            }
        }

        private void 速率累积强度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Xb2SLLJQDInputForm xb2SLLJQDInputForm = new Xb2SLLJQDInputForm(this.User);
            if (xb2SLLJQDInputForm.ShowDialog() == DialogResult.OK)
            {
                var chartForm = GetChartForm();
                var inputs = xb2SLLJQDInputForm.GetSLLJQDInputs();
                foreach (var input in inputs)
                {
                    var xb2Slljqd = new Xb2SLLJQD(input);
                    chartForm.AddChart(xb2Slljqd.GetLine());
                }
            }
        }

        private void 速率累积强度合成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Xb2SLCFHCInputForm xb2SlcfhcInputForm = new Xb2SLCFHCInputForm(this.User);
            if (xb2SlcfhcInputForm.ShowDialog() == DialogResult.OK)
            {
                var inputs = xb2SlcfhcInputForm.GetInputs();
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
            Xb2DCHDL_M1InputForm xb2DCHDL_M1InputForm = new Xb2DCHDL_M1InputForm(this.User);
            if (xb2DCHDL_M1InputForm.ShowDialog() == DialogResult.OK)
            {
                var inputs = xb2DCHDL_M1InputForm.GetInputs();
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
            Xb2DCHDL_M2InputForm xb2DCHDL_M2InputForm = new Xb2DCHDL_M2InputForm(this.User);
            if (xb2DCHDL_M2InputForm.ShowDialog() == DialogResult.OK)
            {
                var inputs = xb2DCHDL_M2InputForm.GetInputs();
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
            Xb2DCHDL_M3InputForm xb2DCHDL_M3InputForm = new Xb2DCHDL_M3InputForm(this.User);
            if (xb2DCHDL_M3InputForm.ShowDialog() == DialogResult.OK)
            {
                var inputs = xb2DCHDL_M3InputForm.GetInputs();
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

        private void 模式4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Xb2DCHDL_M4InputForm xb2DchdlM4InputForm = new Xb2DCHDL_M4InputForm(this.User);
            if (xb2DchdlM4InputForm.ShowDialog() == DialogResult.OK)
            {
                var inputs = xb2DchdlM4InputForm.GetInputs();
                Xb2DCHDL_M4 dchdlM4 = new Xb2DCHDL_M4(inputs);
                var chartForm = GetChartForm();
                chartForm.AddChart(dchdlM4.GetL1());
                chartForm.AddChart(dchdlM4.GetL2());
                chartForm.AddChart(dchdlM4.GetH1());
                chartForm.AddChart(dchdlM4.GetH2());
                chartForm.AddChart(dchdlM4.GetH());
                chartForm.AddChart(dchdlM4.GetS());
                chartForm.AddChart(dchdlM4.GetR());
                chartForm.AddChart(dchdlM4.GetRS());
                chartForm.AddChart(dchdlM4.GetHS());
                chartForm.AddChart(dchdlM4.GetHR());
            }
        }

        private void 应变模式1ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Xb2YB_M1InputForm xb2YbM1InputForm = new Xb2YB_M1InputForm(this.User);
            if (xb2YbM1InputForm.ShowDialog() == DialogResult.OK)
            {
                var inputs = xb2YbM1InputForm.GetInputs();
                Xb2YB_M1 ybM1 = new Xb2YB_M1(inputs);
                var chartForm = GetChartForm();
                chartForm.AddChart(ybM1.GetEp1());
                chartForm.AddChart(ybM1.GetEp2());
                chartForm.AddChart(ybM1.GetDel());
                chartForm.AddChart(ybM1.GetGXY());
                chartForm.AddChart(ybM1.GetP());
            }
        }

        private void 应变模式2ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Xb2YB2InputForm xb2Yb2InputForm = new Xb2YB2InputForm(this.User);
            if (xb2Yb2InputForm.ShowDialog() == DialogResult.OK)
            {
                var inputs = xb2Yb2InputForm.GetInputs();
                Xb2YB_M2 ybM2 = new Xb2YB_M2(inputs);
                var chartForm = GetChartForm();
                chartForm.AddChart(ybM2.GetEp1());
                chartForm.AddChart(ybM2.GetEp2());
                chartForm.AddChart(ybM2.GetDel());
                chartForm.AddChart(ybM2.GetGXY());
                chartForm.AddChart(ybM2.GetP());
            }
        }

        private void 应变模式3ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Xb2YB_M3InputForm xb2YbM3InputForm = new Xb2YB_M3InputForm(this.User);
            if (xb2YbM3InputForm.ShowDialog() == DialogResult.OK)
            {
                var inputs = xb2YbM3InputForm.GetInputs();
                Xb2YB_M3 xb2YbM3 = new Xb2YB_M3(inputs);
                var chartForm = GetChartForm();
                chartForm.AddChart(xb2YbM3.GetFirstEp1());
                chartForm.AddChart(xb2YbM3.GetFirstEp2());
                chartForm.AddChart(xb2YbM3.GetFirstGammXY());
                chartForm.AddChart(xb2YbM3.GetFirstPhi());
                chartForm.AddChart(xb2YbM3.GetFirstDelta());

                chartForm.AddChart(xb2YbM3.GetSecondEp1());
                chartForm.AddChart(xb2YbM3.GetSecondEp2());
                chartForm.AddChart(xb2YbM3.GetSecondGammXY());
                chartForm.AddChart(xb2YbM3.GetSecondPhi());
                chartForm.AddChart(xb2YbM3.GetSecondDelta());

                chartForm.AddChart(xb2YbM3.GetCompEp1());
                chartForm.AddChart(xb2YbM3.GetCompEp2());
                chartForm.AddChart(xb2YbM3.GetCompGammXY());
                chartForm.AddChart(xb2YbM3.GetCompPhi());
                chartForm.AddChart(xb2YbM3.GetCompDelta());
            }
        }

        private void 异常放大ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Xb2YCFDInputForm xb2YCFDInputForm = new Xb2YCFDInputForm(this.User);
            if (xb2YCFDInputForm.ShowDialog() == DialogResult.OK)
            {
                var intputs = xb2YCFDInputForm.GetInputs();
                Xb2YCFD amplification = new Xb2YCFD(intputs);
                var chartForm = GetChartForm();
                chartForm.AddChart(amplification.GetZi());
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
