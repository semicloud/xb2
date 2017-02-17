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
    /// 跨断层流动形变资料处理软件-算法-消趋势 输入 单测项输入
    /// </summary>
    public class Xb2RegressionInput : Xb2BaseInput
    {
        /// <summary>
        /// 观测数据，或者来自原始数据，或者来自基础数据
        /// </summary>
        public DateValueList InputData { get; set; }

        /// <summary>
        /// 观测周期内的数据格式化方法
        /// 
        /// </summary>
        public FreqDataFormat FreqDataFormat { get; set; }

        
        /// <summary>
        /// 置信因子
        /// </summary>
        public double Alpha { get; set; }
    }
}