// 解决方案名称：KDC
// 工程名称：KDCAlgorithmLibrary
// 文件名：QuShu.cs
// 作者：Semicloud
// 初次编写时间：2015-06-25
// 功能：

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xb2.Algorithms.Core.Entity;
using Xb2.Utils;
using DT = System.DateTime;
using DVP = Xb2.Algorithms.Core.Entity.DateValue;
using DVPS = System.Collections.Generic.List<Xb2.Algorithms.Core.Entity.DateValue>;

namespace Xb2.Algorithms.Core.Methods
{
    /// <summary>
    /// 取数算法-Debug版
    /// </summary>
    public class QuShuDebug
    {
        /// <summary>
        /// 取数，按照输入取得离散值的平均值<br/>
        /// 表示为（日期（窗尾），离散值的平均值）
        /// </summary>
        /// <param name="dvps">原始数据</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="wlen">窗长</param>
        /// <param name="slen">步长</param>
        /// <param name="delta">时间间隔</param>
        /// <param name="period">观测周期</param>
        /// <param name="func">速率差分函数</param>
        /// <returns></returns>
        public static DVPS GetAverageValues_20150720_v2(DVPS dvps, DT start, DT end, int wlen, int slen, int delta,
            int period, Func<DVP, DVP, double> func)
        {
            Debug.Print("开始计算窗口中差分值平均值");
            var answer = new DVPS();
            var scatters = GetScatterValues_20150720_v2(dvps, start, end, wlen, slen, delta, period, func);
            Func<List<Double>, double> average = list => list.Count > 0 ? list.Average() : double.NaN;
            scatters.ForEach(s => answer.Add(new DVP(s.WinTail, average(s.Diffs))));
            answer.ForEach(q => Debug.Print(q.Date.ToShortDateString() + "," + q.Value));
            Debug.Print("差分平均值计算完毕");
            Debug.Print("----------------------------------------");
            return answer;
        }

        /// <summary>
        /// 取数，按照输入取得离散值<br/>
        /// 离散值ScatterValues表示为（日期（即窗尾），速率值序列）
        /// </summary>
        /// <param name="dvps">原始数据</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="wlen">窗长</param>
        /// <param name="slen">步长</param>
        /// <param name="delta">时间间隔</param>
        /// <param name="period">观测周期</param>
        /// <param name="function">速率计算函数</param>
        /// <returns></returns>
        public static List<ScatterValues> GetScatterValues_20150720_v2(DVPS dvps, DT start, DT end, int wlen, int slen,
            int delta, int period, Func<DVP, DVP, double> function)
        {
            Debug.Print("开始取数（离散点，窗口中每个观测周期内一个点），输入如下：开始日期:{0}，结束日期{1}，窗长{2}，步长{3}，时间间隔{4}，观测周期{5}",
                start.ToShortDateString(), end.ToShortDateString(), wlen, slen, delta, period);
            var scatters = new List<ScatterValues>();

            var periodPartitions = DateRange.ByStep_v20150819(start, end, period).ToList();
            Debug.Print("按照观测周期（{0}个月）划分起止时间：", period);
            periodPartitions.ForEach(q => Debug.Print(q.ToShortDateString()));

            var windows = Window.GetWindows(start.AddMonths(delta), end, slen, wlen);
            Debug.Print("按照步长（{0}个月）窗长（{1}个月）划分窗口（起点已加时间间隔{2}个月）：", slen, wlen, delta);
            windows.ForEach(q => Debug.Print(q.ToString()));

            Debug.Print("开始窗口处理，窗长{0}个月：", wlen);
            foreach (var win in windows)
            {
                Debug.Print("\n窗口:" + win);
                var ranges = win.Interset_v20150819(periodPartitions);
                Debug.Print("【{0}】", string.Join(", ", ranges));
                var diffs = ranges.ComputeDiffsInWinByNormalization(dvps, delta, function);
                scatters.Add(new ScatterValues(winTail: win.Upper, diffs: diffs));
                if (diffs.Count > 0)
                {
                    Debug.Print("窗口{0}中得到的速率如下", win.Upper.ToShortDateString());
                    diffs.ForEach(q => Debug.Print("\t{0:N3}", q));
                }
                else
                {
                    Debug.Print("窗口{0}中缺数", win);
                }
            }
            Debug.Print("离散点取数结束,取到的数如下所示");
            scatters.ForEach(s => Debug.Print("{0},【{1}】", s.WinTail.ToShortDateString(), string.Join(",", s.Diffs)));
            Debug.Print("----------------------------------------");
            return scatters;
        }
    }
}