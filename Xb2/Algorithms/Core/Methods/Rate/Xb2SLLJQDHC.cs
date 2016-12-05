// 解决方案名称：XbApp
// 工程名称：XbApp
// 文件名：Xb2SLLJQDHC.cs
// 作者：Semicloud
// 初次编写时间：2016-05-01
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
    /// 速率累积强度合成
    /// </summary>
    public class Xb2SLLJQDHC
    {
        public Xb2SLHCInput Input { get; set; }

        /// <summary>
        /// 按照测项获得速率的散点，即窗口、速率值集合
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, List<ScatterValues>> getScatterValueses_20150720()
        {
            var answer = new Dictionary<string, List<ScatterValues>>();
            Func<DateValue, DateValue, double> slcf = (m1, m2) => ((m2.Value - m1.Value) * 365) / ((m2.Date - m1.Date).Days);
            foreach (var sInput in this.Input.InputColl)
            {
                var scatterValueses = QuShuDebug.GetScatterValues_20150720_v2(sInput.Dvps, this.Input.Start, this.Input.End,
                    this.Input.WLen, this.Input.SLen, this.Input.Delta, sInput.Period, slcf);
                answer.Add(sInput.Name, scatterValueses);
            }
            return answer;
        }

        /// <summary>
        /// 获取累积强度合成线
        /// </summary>
        /// <returns></returns>
        public List<DateValue> getMerge_20150720()
        {
            var answer = new List<DateValue>();
            var scatterValuesesByName = getScatterValueses_20150720();
            var windows = Window.GetWindows(this.Input.Start.AddMonths(this.Input.Delta), this.Input.End, this.Input.SLen, this.Input.WLen);
            var names = scatterValuesesByName.Keys.ToList();
            var count = this.Input.InputColl.Count;
            foreach (var window in windows)
            {
                Debug.Print("窗口：{0}", window.Upper.ToShortDateString());
                var list = new List<Tuple<string, List<double>, double>>();
                foreach (var name in names)
                {
                    var scatterValues = scatterValuesesByName[name];
                    var scatterValue = scatterValues.Find(s => s.WinTail == window.Upper);
                    var diffs = scatterValue.Diffs;
                    var reliability = this.Input.InputColl.Find(p => p.Name == name).Reliability;
                    if (diffs.Count == 0) continue;
                    var tuple = new Tuple<string, List<double>, double>(name, diffs, reliability);
                    list.Add(tuple);
                    Debug.Print("{0}，【{1}】，{2}", name, string.Join(",", diffs), reliability);
                }
                var up = Math.Abs(list.Sum(e => e.Item2.Average() * e.Item3) / count);
                var down = (list.Sum(e => e.Item2.ToArray().Abs().Sum() / e.Item2.Count * e.Item3)) / count;
                Debug.Print("累积强度合成值:" + up / down);
                answer.Add(new DateValue(window.Upper, up / down));
                Debug.Print("-------------------------------------");
            }
            return answer;
        }
    }
}