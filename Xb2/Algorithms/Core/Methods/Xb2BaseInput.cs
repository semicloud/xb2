// 解决方案名称：Xb2
// 工程名称：Xb2Core
// 文件名：Xb2BaseInput.cs
// 作者：Semicloud
// 初次编写时间：2014-08-25
// 功能：

using System;

namespace Xb2.Algorithms.Core.Methods
{
    /// <summary>
    /// 跨断层流动形变资料处理软件 算法输入 基类 不可实例化 - OLD
    /// </summary>
    public abstract class Xb2BaseInput
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime Start { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime End { get; set; }
        /// <summary>
        /// 窗长
        /// </summary>
        public int WLen { get; set; }
        /// <summary>
        /// 测项描述字符串
        /// </summary>
        public String ItemStr { get; set; }
        /// <summary>
        /// 步长
        /// </summary>
        public int SLen { get; set; }
        /// <summary>
        /// 时间间隔
        /// </summary>
        public int Delta { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 选择的测项编号
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// 观测周期
        /// </summary>
        public int Freq { get; set; }

        /// <summary>
        /// 基础数据库编号，-1表示原始数据，非0正数表示基础数据库的编号
        /// </summary>
        public int DatabaseId { get; set; }

        /// <summary>
        /// 获取第一个计算位置
        /// </summary>
        /// <returns></returns>
        public DateTime GetFirstPoint()
        {
            return this.Start.AddMonths(this.Delta);
        }
    }
}