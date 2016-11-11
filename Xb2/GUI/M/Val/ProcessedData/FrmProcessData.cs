using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data.MySqlClient;
using Xb2.Algorithms.Numberical;
using Xb2.Config;
using Xb2.Entity.Business;
using Xb2.GUI.M.Item;
using Xb2.GUI.Main;
using Xb2.Utils;
using Xb2.Utils.Control;
using Xb2.Utils.Database;
using XbApp.View.M.Value.ProcessedData;

namespace Xb2.GUI.M.Val.ProcessedData
{
    /// <summary>
    /// 基础数据处理界面
    /// </summary>
    public partial class FrmProcessData : FrmBase
    {
        /// <summary>
        /// 测项编号
        /// </summary>
        private int _itemId;

        /// <summary>
        /// 基础数据库编号
        /// </summary>
        private int _processedDataDbId;

        public FrmProcessData(XbUser user)
        {
            InitializeComponent();
            this.CUser = user;
        }

        private void FrmProcessData_Load(object sender, EventArgs e)
        {
            this.label1.Text = string.Empty;
        }

        /// <summary>
        /// 初始化Chart，该方法用于加载原始数据
        /// </summary>
        /// <param name="itemId"></param>
        private void InitializeChart(int itemId)
        {
            var sql = "select 观测日期,观测值 from {0} where 测项编号={1} order by 观测日期";
            sql = string.Format(sql, Db.TnRData(), itemId);
            Debug.Print("_itemId:" + itemId);
            Debug.Print("sql:" + sql);
            var dt = MySqlHelper.ExecuteDataset(Db.CStr(), sql).Tables[0];

            //初始化Chart控件
            groupBox1.Controls.Add(ChartHelper.GetOrdinaryChart());
            ChartHelper.BindChartWithData(this.GetCurrentChart(), dt);
            this.GetCurrentChart().GetLogger().Clear();
            this.GetCurrentChart().GetStack().Clear();
            this.GetCurrentChart().ContextMenuStrip = null;
            this.GetCurrentChart().ContextMenuStrip = contextMenuStrip1;

            groupBox2.Controls.Add(ChartHelper.GetOrdinaryChart());
            ChartHelper.BindChartWithData(this.GetSourceChart(), dt);
            this.label1.Text = "";
        }

        /// <summary>
        /// 初始化Chart，该方法用于加载基础数据库
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="dbId"></param>
        private void InitializeChart(int itemId, int dbId)
        {
            var sql = "select 观测日期,观测值 from {0} where 测项编号={1} order by 观测日期";
            sql = string.Format(sql, Db.TnRData(), itemId);
            var dtSource = MySqlHelper.ExecuteDataset(Db.CStr(), sql).Tables[0];

            var sql2 = "select 观测日期,观测值 from {0} where 库编号={1} order by 观测日期";
            sql2 = string.Format(sql2, Db.TnProcessedDbData(), _processedDataDbId);
            var dtProcessed = MySqlHelper.ExecuteDataset(Db.CStr(), sql2).Tables[0];

            var sql3 = "select 操作记录 from {0} where 编号={1}";
            sql3 = string.Format(sql3, Db.TnProcessedDb(), _processedDataDbId);
            var strLog = MySqlHelper.ExecuteScalar(Db.CStr(), sql3);

            //初始化Chart控件
            groupBox1.Controls.Add(ChartHelper.GetOrdinaryChart());
            ChartHelper.BindChartWithData(this.GetCurrentChart(), dtProcessed);
            this.GetCurrentChart().GetLogger().Clear();
            this.GetCurrentChart().GetStack().Clear();
            this.GetCurrentChart().ContextMenuStrip = null;
            this.GetCurrentChart().ContextMenuStrip = contextMenuStrip1;
            this.GetCurrentChart().SetLogger(strLog.ToString().Split('|').ToList());

            groupBox2.Controls.Add(ChartHelper.GetOrdinaryChart());
            ChartHelper.BindChartWithData(this.GetSourceChart(), dtSource);
            this.label1.Text = "";
        }

        /// <summary>
        /// 获得当前正在编辑的Chart控件
        /// </summary>
        /// <returns></returns>
        private Chart GetCurrentChart()
        {
            if (groupBox1.Controls.Count == 0) 
                throw new ArgumentException("No [Chart] Control in Groupbox!");
            return (Chart) groupBox1.Controls[0];
        }

        /// <summary>
        /// 获取展示原始数据的Chart控件
        /// </summary>
        /// <returns></returns>
        private Chart GetSourceChart()
        {
            if (groupBox1.Controls.Count == 0) 
                throw new ArgumentException("No [Chart] Control in Groupbox!");
            return (Chart) groupBox2.Controls[0];
        }

        private void 重做ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_itemId != 0)
            {
                this.groupBox1.Controls.Clear();
                this.groupBox2.Controls.Clear();
                InitializeChart(_itemId);
            }
            else
            {
                Debug.Print("好像未选择测项啊？");
            }
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (groupBox1.Controls.Count > 0)
            {
                var dataTable = new DataTable();
                dataTable.Columns.Add("观测日期", typeof(DateTime));
                dataTable.Columns.Add("观测值", typeof(double));
                var dataPoints = GetCurrentChart().Series[0].Points;
                foreach (var dataPoint in dataPoints)
                {
                    var dataRow = dataTable.NewRow();
                    dataRow["观测日期"] = DateTime.FromOADate(dataPoint.XValue);
                    dataRow["观测值"] = Convert.ToDouble(dataPoint.YValues[0]);
                    dataTable.Rows.Add(dataRow);
                }
                FrmSaveProcessedData frmSaveProcessedData = new FrmSaveProcessedData(this.CUser);
                frmSaveProcessedData.StartPosition = FormStartPosition.CenterScreen;
                frmSaveProcessedData.ItemId = _itemId;
                frmSaveProcessedData.ProcessedDataId = _processedDataDbId;
                frmSaveProcessedData.KIndex = "To be continued...";
                frmSaveProcessedData.DataTable = dataTable;
                frmSaveProcessedData.Logger = GetCurrentChart().GetLogger();
                frmSaveProcessedData.ShowDialog();
            }
        }

        /// <summary>
        /// 撤销按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (groupBox1.Controls.Count > 0)
            {
                var chart = (Chart) groupBox1.Controls[0];
                chart.WithDraw();
            }
        }

        /// <summary>
        /// 选测项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            FrmSelectMItem frmSelectMItem = new FrmSelectMItem(this.CUser);
            frmSelectMItem.StartPosition = FormStartPosition.CenterScreen;
            var diagRslt = frmSelectMItem.ShowDialog();
            if (diagRslt == DialogResult.OK)
            {
                var result = frmSelectMItem.Result;
                if (result.Rows.Count == 0)
                {
                    MessageBox.Show("未选择测项！");
                    return;
                }
                if (result.Rows.Count > 1)
                {
                    MessageBox.Show("最多选择一个测项");
                    return;
                }
                if (result.Rows.Count == 1)
                {
                    var dataRow = result.Rows[0];
                    this.Text = "生成基础数据-" + dataRow["观测单位"]
                                + "-" + dataRow["地名"] + "-" + dataRow["方法名"] + "-" + dataRow["测项名"];
                    //在这里设置了_itemId的值
                    _itemId = Convert.ToInt32(dataRow["编号"]);
                    _processedDataDbId = 0;
                    InitializeChart(_itemId);
                    Debug.Print("图件加载成功！");
                }
                else
                {
                    MessageBox.Show("选测项失败！");
                }
            }
        }

        /// <summary>
        /// 加基础数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            FrmShowProcessedDataDb frmShowProcessedDataDb = new FrmShowProcessedDataDb(CUser);
            frmShowProcessedDataDb.StartPosition = FormStartPosition.CenterScreen;
            frmShowProcessedDataDb.Text = string.Format("用户【{0}】的基础数据库", CUser.ID);
            var confirm = frmShowProcessedDataDb.ShowDialog();
            if (confirm == DialogResult.OK)
            {
                _itemId = frmShowProcessedDataDb.ItemId;
                _processedDataDbId = frmShowProcessedDataDb.DbId;
                InitializeChart(_itemId,_processedDataDbId);
            }
        }

        /// <summary>
        /// 操作记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (groupBox1.Controls.Count > 0)
            {
                FrmShowLog frmShowLog = new FrmShowLog();
                frmShowLog.SetLogs(GetCurrentChart().GetLogger());
                frmShowLog.Show();
            }
        }

        /// <summary>
        /// 更改groupbox1中的Chart控件
        /// 由于需要使用不同的数据处理方法
        /// 每种数据处理方法绑定的鼠标事件不相同
        /// 所以就使用了“切换鼠标事件”的方法来切换
        /// 不同的数据处理方法
        /// </summary>
        /// <param name="chart"></param>
        private void ChangeChart(Chart chart)
        {
            //获取当前Chart的数据源和堆栈，其中数据源用于传递数据，堆栈用于撤销用户操作
            var dt = GetCurrentChart().GetTable();
            var stack = GetCurrentChart().GetStack();
            var list = GetCurrentChart().GetLogger();
            //然后赋值给目标Chart
            ChartHelper.BindChartWithData(chart, dt);
            chart.ContextMenuStrip = contextMenuStrip1;
            chart.SetStack(stack);
            chart.SetLogger(list);
            groupBox1.Controls.Clear();
            groupBox1.Controls.Add(chart);
        }

        #region 处理方法菜单

        #region 突跳处理

        /// <summary>
        /// 突跳处理->图解法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 图解法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.label1.Text = "突跳→图解法";
            this.toolTip1.SetToolTip(this.label1, "直接拖动数据点即可显示数据点的值");
            ChangeChart(ChartHelper.GetPointMovingChart());
        }

        /// <summary>
        /// 突跳处理->删除法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 删除法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.label1.Text = "突跳处理->删除法";
            ChangeChart(ChartHelper.GetPointDeleteChart());
        }

        /// <summary>
        /// 突跳处理→平均值法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 平均值法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.label1.Text = "突跳处理→平均值法";
            ChangeChart(ChartHelper.GetAvgMethodChart());
        }

        /// <summary>
        /// 突跳处理→活动平均法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 活动平均法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.label1.Text = "突跳处理→活动平均法";
            ChangeChart(ChartHelper.GetFlexAvgMethodChart());
        }

        /// <summary>
        /// 突跳处理→拟合→线性拟合法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 线性拟合ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.label1.Text = "突跳处理→拟合法→线性拟合";
            this.toolTip1.SetToolTip(this.label1, "");
            ChangeChart(ChartHelper.GetLinearRegressionChart());
        }

        /// <summary>
        /// 突跳处理→拟合→多项式拟合法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 多项式拟合ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.label1.Text = "突跳处理→拟合→多项式拟合";
            ChangeChart(ChartHelper.GetPolyRegressionChart());
        }

        /// <summary>
        /// 突跳处理→拟合→对数拟合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 对数拟合ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (groupBox1.Controls.Count > 0)
            {
                var dt = ((Chart) groupBox1.Controls[0]).GetTable();
                if (dt.GetColumnOfDouble("观测值").Any(v => v < 0))
                {
                    MessageBox.Show("观测值中存在负值，无法进行对数拟合！");
                    if (_itemId > 0)
                    {
                        InitializeChart(_itemId);
                    }
                }
                else
                {
                    this.label1.Text = "突跳处理→拟合→对数拟合";
                    ChangeChart(ChartHelper.GetLogitRegressionChart());
                }
            }
        }

        /// <summary>
        /// 突跳处理→拟合→幂函数拟合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 幂函数拟合ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (groupBox1.Controls.Count > 0)
            {
                var dt = ((Chart)groupBox1.Controls[0]).GetTable();
                if (dt.GetColumnOfDouble("观测值").Any(v => v < 0))
                {
                    MessageBox.Show("观测值中存在负值，无法进行幂函数拟合！");
                    if (_itemId > 0)
                    {
                        InitializeChart(_itemId);
                    }
                }
                else
                {
                    this.label1.Text = "突跳处理→拟合→幂函数拟合";
                    ChangeChart(ChartHelper.GetPowerFuncRegressChart());
                }
            }

            
        }

        /// <summary>
        /// 突跳处理→拟合→指数e拟合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 指数e拟合ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.label1.Text = "突跳处理→拟合→指数e拟合";
            ChangeChart(ChartHelper.GetExpRegressionChart());
        }

        /// <summary>
        ///  突跳处理→数学模型法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 数学模型法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.label1.Text = "突跳处理→数学模型法";
            ChangeChart(ChartHelper.GetMathModelChart());
        }

        /// <summary>
        /// 突跳处理→对比法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 对比法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //待年周变算法完成后完成
            throw new NotImplementedException();
        }

        #endregion

        #region 缺数处理

        private void 常数插值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.GetCurrentChart() != null)
            {
                this.label1.Text = "缺数处理→常数差值";
                var frmInterpolation = new FrmInterpolation {StartPosition = FormStartPosition.CenterScreen};
                if (frmInterpolation.ShowDialog() == DialogResult.OK)
                {
                    var date = frmInterpolation.GetDate();
                    var value = frmInterpolation.GetValue();
                    Debug.Print("interpolation date:{0}, value:{1}", date.ToShortDateString(), value);
                    if (this.GetCurrentChart().Interpolation(date, value,"缺数处理→常数差值"))
                    {
                        MessageBox.Show("插值成功！");
                    }
                }
                else
                {
                    MessageBox.Show("常数插值已取消！");
                }
            }
            else
            {
                MessageBox.Show("未加载数据，无法进行操作！");
            }
        }

        private void 拉格朗日插值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.GetCurrentChart() != null)
            {
                this.label1.Text = "缺数处理→朗格朗日插值";

                var dataPoints = GetCurrentChart().Series[0].Points.ToList();
                if (dataPoints.Count == 0 | dataPoints.Count == 1)
                {
                    MessageBox.Show("观测值太少，无法进行插值！");
                    return;
                }
                var frmInterpolation = new FrmInterpolation2(this.拉格朗日插值ToolStripMenuItem.Text);
                if (frmInterpolation.ShowDialog() == DialogResult.OK)
                {
                    var date = frmInterpolation.GetDate();
                    var allDate = dataPoints.Select(p => DateTime.FromOADate(p.XValue)).ToList();
                    if (allDate.Contains(date))
                    {
                        MessageBox.Show("观测值中已存在" + date.SStr() + "的观测值，取消插值！");
                        return;
                    }
                    var value = Processmethod.GetLagrangeInterpolationResult(dataPoints, date);
                    value = Math.Round(value, Xb2Config.GetPrecision());
                    Debug.Print("interpolation date:{0}, value:{1}", date.ToShortDateString(), value);
                    //在这里将插值的数据入栈了
                    if (this.GetCurrentChart().Interpolation(date, value, "缺数处理→朗格朗日插值"))
                    {
                        MessageBox.Show("插值成功！");
                    }
                }
                else
                {
                    MessageBox.Show("拉格朗日插值已取消！");
                }
            }
            else
            {
                MessageBox.Show("未加载数据，无法进行操作！");
            }
        }

        private void 平均值法插值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = "缺数处理→平均法插值";
            ChangeChart(ChartHelper.GetAvgMethodChart_LoseValue_Process());
        }

        private void 线性法插值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = "缺数处理→线性法插值";
            ChangeChart(ChartHelper.GetLinearInterpolationChart());
        }

        private void 多项式法插值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = "缺数处理→多项式法插值";
            ChangeChart(ChartHelper.GetPolyInterpolationChart());
        }

        private void 等间距插值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = "缺数处理→等间距插值";
            ChangeChart(ChartHelper.GetEqualDistInterpolationChart());
        }

        private void 对比法插值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        
        
        #endregion

        #region 台阶处理


        private void 图解法ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.label1.Text = "台阶处理→图解法";
            ChangeChart(ChartHelper.GetStepMovingChart());
        }

        private void 多点对比ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.label1.Text = "台阶处理→多点对比法";
            throw new NotImplementedException();
        }

        private void 多点平均平移法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.label1.Text = "多点平均平移法";
            ChangeChart(ChartHelper.GetMultiPointAvgSlideChart());
        }

        #region 多点趋势平移法

        private void 线性拟合ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.label1.Text = "台阶处理→多点趋势平移法→线性拟合";
            ChangeChart(ChartHelper.GetMultiPointLinearProcessChart());
        }


        private void 多项式拟合ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.label1.Text = "台阶处理→多点趋势平移法→多项式拟合";
            ChangeChart(ChartHelper.GetMultiPointPolyProcessChart());
        }

        private void 对数拟合ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (groupBox1.Controls.Count > 0)
            {
                var dt = ((Chart) groupBox1.Controls[0]).GetTable();
                if (dt.GetColumnOfDouble("观测值").Any(v => v < 0))
                {
                    MessageBox.Show("观测值中存在负值，无法进行对数拟合！");
                    if (_itemId > 0)
                    {
                        InitializeChart(_itemId);
                    }
                }
                else
                {
                    this.label1.Text = "台阶处理→多点趋势平移法→对数拟合";
                    ChangeChart(ChartHelper.GetMultiPointLogitProcessChart());
                }
            }
        }

        private void 幂函数拟合ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.label1.Text = "台阶处理→多点趋势平移法→幂函数拟合";
            ChangeChart(ChartHelper.GetMultiPointPowerProcessChart());
        }

        private void 指数e拟合ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.label1.Text = "台阶处理→多点趋势平移法→指数e拟合";
            ChangeChart(ChartHelper.GetMultiPointExpProcessChart());
        }

        #endregion

        #endregion

        #endregion

        private void 变符号处理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (groupBox1.Controls.Count > 0)
            {
                ((Chart) groupBox1.Controls[0]).InverseYValues();
            }
        }

        private void 减最小值处理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (groupBox1.Controls.Count > 0)
            {
                ((Chart)groupBox1.Controls[0]).SubtractMinValue();
            }
        }

        private void 减最大值处理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (groupBox1.Controls.Count > 0)
            {
                ((Chart)groupBox1.Controls[0]).SubtractMaxValue();
            }
        }

        private void 添加趋势线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (groupBox1.Controls.Count > 0)
            {
                this.label1.Text = "添加趋势线";
                ChangeChart(ChartHelper.GetRegressionLineChart());
            }
        }
    }
}
