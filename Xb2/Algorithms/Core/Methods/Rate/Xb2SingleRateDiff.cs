// 解决方案名称：Xb2
// 工程名称：Xb2Core
// 文件名：Xb2SingleRateSiff.cs
// 作者：Semicloud
// 初次编写时间：2014-08-26
// 功能：单测项-速率-差分

using System.Collections.Generic;
using System.Linq;
using Xb2.Algorithms.Core.Entity;
using Xb2.Utils;

namespace Xb2.Algorithms.Core.Methods.Rate
{
    /// <summary>
    /// 单测项-速率-差分算法
    /// </summary>
    public class Xb2SingleRateDiff
    {
        /// <summary>
        /// 算法输入
        /// </summary>
        public SingleRateDiffInput Input { get; set; }

        /// <summary>
        /// 单测项-速率-差分算法构造函数
        /// </summary>
        /// <param name="input">单测项-速率-差分算法输入</param>
        public Xb2SingleRateDiff(SingleRateDiffInput input)
        {
            Input = input;
        }

        /// <summary>
        /// 得到单测项 速率 差分线的数据
        /// </summary>
        /// <returns>List of DateValue</returns>
        public List<DateValue> GetRateDiffLineData()
        {
            var dictionary = CoreMethodUtils.GetRateValues(Input.Collection, Input.Start, Input.End, Input.WLen,
                Input.SLen, Input.Delta);
            var answer = new List<DateValue>();
            foreach (var element in dictionary)
                answer.Add(new DateValue(date: element.Key.LastDay(), value: element.Value.Average()));
            return answer;
        }
    }
}