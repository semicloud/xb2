// 解决方案名称：XbApp
// 工程名称：XbApp
// 文件名：PeriodComputer.cs
// 作者：Semicloud
// 初次编写时间：2014-11-05
// 功能：

using System;
using System.Collections.Generic;
using System.Linq;
using Xb2.Algorithms.Core.Entity;

namespace Xb2.Algorithms.Core.Methods
{
    public class PeriodComputer
    {
        private static int[] monthSpan = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12};
        private static int[] daySpan = {30, 61, 91, 121, 152, 182, 213, 243, 273, 313, 335, 365};

        /// <summary>
        /// 获取观测周期的分布
        /// </summary>
        /// <param name="collection">测值集合</param>
        /// <returns></returns>
        public static Dictionary<int, float> GetDistribution(DateValueList collection)
        {
            List<MatchItem> items = InitMatchItems();
            ComputeMatchItems(ref items, collection);
            Array.ForEach(items.ToArray(), i => i.P = (float) i.Number/(collection.Count - 1));
            return items.ToDictionary(e => e.MonthSpan, e => e.P);
        }

        /// <summary>
        /// 获取最可能的观测周期值
        /// </summary>
        /// <param name="aList">测值集合</param>
        /// <returns></returns>
        public static int GetPossiblePeriod(DateValueList aList)
        {
            Dictionary<int, float> distribution = GetDistribution(aList);
            return distribution.First(d => Math.Abs(d.Value - distribution.Values.Max()) < 0.0001).Key;
        }

        //计算每个月份数的出现次数
        private static void ComputeMatchItems(ref List<MatchItem> items, DateValueList collection)
        {
            int n = collection.Count;
            for (int i = 0; i < n - 1; i++)
            {
                if (i == n - 1) continue;
                int span = (collection[i + 1].Date - collection[i].Date).Days;
                for (int j = 0; j < items.Count; j++)
                {
                    if (span <= items[j].Upper && span >= items[j].Lower)
                    {
                        items[j].Number++;
                        break;
                    }
                }
            }
        }

        private static List<MatchItem> InitMatchItems()
        {
            List<MatchItem> items = new List<MatchItem>();
            for (int i = 0; i < monthSpan.Length; i++)
            {
                items.Add(new MatchItem
                {
                    MonthSpan = monthSpan[i],
                    DaySpan = daySpan[i],
                    Lower = daySpan[i] - daySpan[i]/6,
                    Upper = daySpan[i] + daySpan[i]/6,
                    Number = 0,
                    P = 0.0f
                });
            }
            return items;
        }
    }

    internal class MatchItem
    {
        public int MonthSpan { get; set; }
        public int DaySpan { get; set; }
        public int Lower { get; set; }
        public int Upper { get; set; }
        public int Number { get; set; }
        public float P { get; set; }

        public override string ToString()
        {
            return string.Format("MonthSpan: {0}, DaySpan: {1}, Lower: {2}, Upper: {3}, Number: {4}, P: {5}",
                MonthSpan, DaySpan, Lower, Upper, Number, P);
        }
    }
}