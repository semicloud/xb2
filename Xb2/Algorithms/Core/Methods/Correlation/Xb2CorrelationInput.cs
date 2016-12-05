// 解决方案名称：Xb2
// 工程名称：Xb2Core
// 文件名：Xb2CorrelationInput.cs
// 作者：Semicloud
// 初次编写时间：2014-09-03
// 功能：相关系数的输入

using Xb2.Algorithms.Core.Entity;

namespace Xb2.Algorithms.Core.Methods.Correlation
{
    /// <summary>
    /// 跨断层流动形变资料处理软件 相关系数 输入
    /// </summary>
    public class Xb2CorrelationInput : Xb2BaseInput
    {
        /// <summary>
        /// 测值序列1
        /// </summary>
        public DateValueList Collection1 { get; set; }

        /// <summary>
        /// 测值序列2
        /// </summary>
        public DateValueList Collection2 { get; set; }
    }
}