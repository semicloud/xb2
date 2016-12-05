// 解决方案名称：Xb2
// 工程名称：Xb2Core
// 文件名：Xb2MultiRateMerge.cs
// 作者：Semicloud
// 初次编写时间：2014-08-25
// 功能：

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xb2.Algorithms.Core.Entity;

namespace Xb2.Algorithms.Core.Methods.Rate
{
    /// <summary>
    /// 速率差分合成输入
    /// </summary>
    public class SingleInput
    {
        /// <summary>
        /// 测项名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 原始数据，即测值集合
        /// </summary>
        public List<DateValue> Dvps { get; set; }

        /// <summary>
        /// 观测信度
        /// </summary>
        public double Reliability { get; set; }

        /// <summary>
        /// 观测周期
        /// </summary>
        public int Period { get; set; }
    }

    /// <summary>
    /// 速率合成输入，用于速率合成和速率累积强度合成
    /// </summary>
    public class Xb2SLHCInput : Xb2BaseInput
    {
        public List<SingleInput> InputColl { get; set; }
    }

    public class DiffInfo
    {
        /// <summary>
        /// 测项名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 速率差分计算结果
        /// </summary>
        public List<DateValue> DiffValues { get; set; }

        /// <summary>
        /// 观测信度
        /// </summary>
        public double Reliability { get; set; }

        public DiffInfo()
        {
        }

        public DiffInfo(string name, List<DateValue> diffValues, double reliability)
        {
            Name = name;
            DiffValues = diffValues;
            Reliability = reliability;
        }
    }

    /// <summary>
    /// 速率差分合成
    /// </summary>
    public class Xb2SLCFHC
    {
        /// <summary>
        /// 多测项-速率-合成的输入
        /// </summary>
        public Xb2SLHCInput Input { get; set; }


        /// <summary>
        /// 获得速率合成结果 v20150720
        /// </summary>
        /// <returns></returns>
        public List<DateValue> getMerge_20150720()
        {
            var answer = new List<DateValue>();
            var diffInfos = getDiffInfos();
            var start = this.Input.Start;
            var end = this.Input.End;
            var wlen = this.Input.WLen;
            var slen = this.Input.SLen;
            var delta = this.Input.Delta;
            var windows = Window.GetWindows(start.AddMonths(wlen), end, slen, wlen);
            var names = diffInfos.Select(q => q.Name).ToList();
            Debug.Print("-------开始速率合成---------");
            foreach (var win in windows)
            {
                var list = new List<Tuple<string, double, double>>();
                foreach (var name in names)
                {
                    double reliability = diffInfos.Find(t => t.Name.Equals(name)).Reliability;
                    var diffValues = diffInfos.Find(t => t.Name.Equals(name)).DiffValues;
                    var dvp = diffValues.Find(d => d.Date.Equals(win.Upper));
                    if (double.IsNaN(dvp.Value)) continue;
                    var tuple = new Tuple<string, double, double>(name, dvp.Value, reliability);
                    list.Add(tuple);
                }
                Debug.Print("窗口:{0}", win);
                list.ForEach(t => Debug.Print("\t{0},{1},{2}", t.Item1, t.Item2, t.Item3));
                double result = list.Sum(l => l.Item2*l.Item3)/list.Count;
                Debug.Print("\t合成值:" + result);
                answer.Add(new DateValue(win.Upper, result));
            }
            Debug.Print("-------速率合成结束---------");
            return answer;
        }

        /// <summary>
        /// 获取速率差分结合
        /// </summary>
        /// <returns></returns>
        private List<DiffInfo> getDiffInfos()
        {
            var answer = new List<DiffInfo>();
            var start = this.Input.Start;
            var end = this.Input.End;
            var wlen = this.Input.WLen;
            var slen = this.Input.SLen;
            var delta = this.Input.SLen;
            Func<DateValue, DateValue, double> slcf = (d1, d2) => ((d2.Value - d1.Value) * 365) / ((d2.Date - d1.Date).Days);
            foreach (var input in this.Input.InputColl)
            {
                Debug.Print("以下开始计算【{0}】的速率差分,观测周期{1},信度{2}", input.Name, input.Period,input.Reliability);
                var diffValues = QuShuDebug.GetAverageValues_20150720_v2(input.Dvps,start,end, wlen, slen, delta, input.Period, slcf);
                answer.Add(new DiffInfo(name: input.Name, diffValues: diffValues, reliability: input.Reliability));
            }
            return answer;
        }
    }

    #region old codes

//        /// <summary>
//        /// 获取速率 - 合成 线数据
//        /// </summary>
//        /// <returns>List of DateValue</returns>
//        public List<DateValue> GetRateMergeLineData()
//        {
//            var answer = new List<DateValue>();
//            //获得所有测项的速率数据
//            var diffs = getAllRateDiffs();
//            //获得这些速率数据中的最大窗长
//            var windows = getMaxDatetimes(diffs);
//            //根据速率数据和最大窗长获取二维数组
//            var matrix = getMatrix(diffs, windows);
//            //获得权重
//            var factors = this.InputCollection.CollectionsWithFactor.Keys.ToArray();
//            //迭代二维数组中的每一列，和权重相乘，得到速率-合成
//            for (int col = 0; col < matrix.GetLength(1); col++)
//            {
//                var diff = matrix.GetColumn(col);
//                answer.Add(new DateValue(date: windows[col].LastDay(), value: getMerge(factors, diff)));
//            }
//            return answer;
//        }

//        private static double getMerge(double[] factor, double[] diff)
//        {
//            int length = factor.Length;
//            var products = new List<double>();
//            for (int i = 0; i < length; i++)
//            {
//                double d1 = factor[i];
//                double d2 = diff[i];
//                if (double.IsPositiveInfinity(diff[i])) continue;
//                products.Add(d1 * d2);
//            }
//            return products.Average();
//        }

//        /// <summary>
//        /// 获得所有测项的 速率-差分
//        /// </summary>
//        /// <returns>所有测项的速率-差分值</returns>
//        private List<List<DateValue>> getAllRateDiffs()
//        {
//            var answer = new List<List<DateValue>>();
//            foreach (var collWithFactor in this.InputCollection.CollectionsWithFactor)
//            {
//                var ratevalues = CoreMethodUtils.GetRateValues(collWithFactor.Value, this.InputCollection.Start,
//                    this.InputCollection.End, this.InputCollection.WLen, this.InputCollection.SLen, this.InputCollection.Delta);
//                var datevalues = new List<DateValue>();
//                foreach (var ratevalue in ratevalues)
//                    datevalues.Add(new DateValue(ratevalue.Key.LastDay(), ratevalue.Value.Average()));
//                answer.Add(datevalues);
//            }
//            return answer;
//        }

//        /// <summary>
//        /// 根据所有测项的 速率-差分值 得到最长的那个速率-差分序列的所有观测日期
//        /// </summary>
//        /// <param name="diffs">获得所有测项的 速率-差分</param>
//        /// <returns></returns>
//        private List<DateTime> getMaxDatetimes(List<List<DateValue>> diffs)
//        {
//            var counts = diffs.Select(l => l.Count).ToList();
//            int maxedValueIndex = counts.IndexOf(counts.Max());
//            return diffs[maxedValueIndex].Select(d => d.Date).ToList();
//        }

//        /// <summary>
//        /// 将速率-差分值序列 和 最长的观测日期 构造二维矩阵，行是测项的差分序列，列是观测日期（来源于窗口值）
//        /// </summary>
//        /// <param name="diffs"></param>
//        /// <param name="windows"></param>
//        /// <returns></returns>
//        private double[,] getMatrix(List<List<DateValue>> diffs, List<DateTime> windows)
//        {
//            double[,] matrix = new double[diffs.Count, windows.Count];
//            for (int i = 0; i < diffs.Count; i++)
//            {
//                var datetimes = diffs[i].Select(d => d.Date).ToList();
//                for (int j = 0; j < windows.Count; j++)
//                {
//                    if (datetimes.Exists(d => d.Date == windows[j]))
//                        matrix[i, j] = diffs[i].Find(d => d.Date == windows[j]).Value;
//                    else
//                        matrix[i, j] = double.PositiveInfinity;
//                }
//            }
//            return matrix;
//        }

    #endregion
}