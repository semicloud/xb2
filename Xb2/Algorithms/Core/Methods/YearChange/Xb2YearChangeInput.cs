// 解决方案名称：Xb2
// 工程名称：Xb2Core
// 文件名：Xb2YearChangeInput.cs
// 作者：Semicloud
// 初次编写时间：2014-09-01
// 功能：

using Xb2.Algorithms.Core.Entity;

namespace Xb2.Algorithms.Core.Methods.YearChange
{
    /// <summary>
    /// 年周变算法 输入
    /// </summary>
    public class Xb2YearChangeInput : Xb2BaseInput
    {
        /// <summary>
        /// 测值序列
        /// </summary>
        public DateValueList DateValueList { get; set; }



        /// <summary>
        /// 相关系数
        /// </summary>
        public double Alpha { get; set; }

        public int MItemId { get; set; }
        public int DatabaseId { get; set; }
        public string DatabaseName { get; set; }
        public string MItemStr { get; set; }
    }
}