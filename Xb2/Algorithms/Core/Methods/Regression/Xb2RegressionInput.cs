// 解决方案名称：Xb2
// 工程名称：Xb2Core
// 文件名：Xb2RegressionInput.cs
// 作者：Semicloud
// 初次编写时间：2014-09-01
// 功能：

using Xb2.Algorithms.Core.Entity;

namespace Xb2.Algorithms.Core.Methods.Regression
{
    /// <summary>
    /// 跨断层流动形变资料处理软件-算法-消趋势 输入
    /// </summary>
    public class Xb2RegressionInput : Xb2BaseInput
    {
        /// <summary>
        /// 测值序列
        /// </summary>
        public DateValueList List { get; set; }

        /// <summary>
        /// 测项编号
        /// </summary>
        public int MItemID { get; set; }

        /// <summary>
        /// 基础数据库编号
        /// </summary>
        public int DatabaseID { get; set; }

        /// <summary>
        /// 基础数据库名称
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// 置信因子
        /// </summary>
        public double Alpha { get; set; }
    }
}