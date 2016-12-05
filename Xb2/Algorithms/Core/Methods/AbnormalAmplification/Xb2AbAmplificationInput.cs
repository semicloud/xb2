// 解决方案名称：XbApp
// 工程名称：XbApp
// 文件名：Xb2AbAmplificationInput.cs
// 作者：Semicloud
// 初次编写时间：2014-11-17
// 功能：

using System;
using System.Collections.Generic;
using Xb2.Algorithms.Core.Entity;

namespace Xb2.Algorithms.Core.Methods.AbnormalAmplification
{
    /// <summary>
    /// 异常放大输入类
    /// </summary>
    public class Xb2AbAmplificationInput
    {
        /// <summary>
        /// 测值集合的集合
        /// </summary>
        public List<DateValueList> CollectionList { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// 观测周期，用户输入
        /// </summary>
        public int Period { get; set; }
    }
}