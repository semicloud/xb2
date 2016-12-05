// 解决方案名称：Xb2
// 工程名称：Xb2Core
// 文件名：Xb2SingleRateSiffInput.cs
// 作者：Semicloud
// 初次编写时间：2014-08-26
// 功能：

using Xb2.Algorithms.Core.Entity;

namespace Xb2.Algorithms.Core.Methods.Rate
{
    /// <summary>
    /// 单测项 速率 差分 输入
    /// </summary>
    public class Xb2SingleRateDiffInput : Xb2BaseInput
    {
        /// <summary>
        /// 测值序列
        /// </summary>
        public DateValueList Collection { get; set; }
    }
}