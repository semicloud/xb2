// 解决方案名称：Xb2
// 工程名称：Xb2Core
// 文件名：Xb2SingleRateAccStre.cs
// 作者：Semicloud
// 初次编写时间：2014-08-25
// 功能：

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Accord.Math;
using Xb2.Algorithms.Core.Entity;

namespace Xb2.Algorithms.Core.Methods.Rate
{
    /// <summary>
    /// 单测项-速率-累积强度
    /// </summary>
    public class Xb2SLLJQD
    {
        /// <summary>
        /// 单测项-速率-累积强度 的输入
        /// </summary>
        public SlljqdInput Input { get; set; }

        /// <summary>
        /// 单测项-速率-累积强度算法的构造函数
        /// </summary>
        /// <param name="input">单测项-速率-累积强度输入</param>
        public Xb2SLLJQD(SlljqdInput input)
        {
            this.Input = input;
        }

        //计算累积强度
        private static double Q(List<double> numbers)
        {
            return Math.Abs(numbers.Average())/(numbers.ToArray().Abs().Sum()/numbers.Count);
        }

        /// <summary>
        /// 获得单测项-速率-累积强度曲线的数据
        /// </summary>
        /// <returns>List of DateValue</returns>
        public List<DateValue> GetRateAccStreLineData()
        {
            //解析输入
            var answer = new List<DateValue>();
            var dvps = this.Input.Collection;
            var start = this.Input.Start;
            var end = this.Input.End;
            var wlen = this.Input.WLen;
            var slen = this.Input.SLen;
            var delta = this.Input.SLen;
            var period = this.Input.Period;
            //速率差分计算函数
            Func<DateValue, DateValue, double> slcf = (d1, d2) => ((d2.Value - d1.Value)*365)/((d2.Date - d1.Date).Days);
            //累积强度计算函数
            Func<List<double>, double> ljqd = numbers => Math.Abs(numbers.Average())/(numbers.ToArray().Abs().Sum()/numbers.Count);
            //取离散值
            var scatters = QuShuDebug.GetScatterValues_20150720_v2(dvps, start, end, wlen, slen, delta, period, slcf);
            var numOfNoData = scatters.FindAll(p => p.Diffs.Count == 0).Count;
            Debug.Print("找到{0}个缺数的窗口，删除", numOfNoData);
            scatters.RemoveAll(p => p.Diffs.Count == 0);
            foreach (var scatter in scatters)
            {
                var dvp = new DateValue(scatter.WinTail, ljqd(scatter.Diffs));
                answer.Add(dvp);
            }
            return answer;
        }
    }

    /// <summary>
    /// 单测项 速率 累积合成 输入
    /// </summary>
    public class SlljqdInput : BaseInput
    {
        /// <summary>
        /// 测值序列
        /// </summary>
        public DateValueList Collection { get; set; }

        /// <summary>
        /// 测值序列的观测周期
        /// </summary>
        public Int32 Period { get; set; }
    }
}