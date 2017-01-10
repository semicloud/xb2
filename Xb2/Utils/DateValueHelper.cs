using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using NLog;
using NUnit.Framework;
using Xb2.Algorithms.Core.Entity;
using Xb2.Utils.Database;

namespace Xb2.Utils
{
    /// <summary>
    /// 日期类
    /// </summary>
    public static class DateValueHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region Helper Methods

        /// <summary>
        /// 两个测值和一个日期做线性插值
        /// </summary>
        /// <param name="dateValue1"></param>
        /// <param name="dateValue2"></param>
        /// <param name="dateToInterpolation">要进行线性插值的日期</param>
        /// <returns></returns>
        public static double GetLinearInterpolation(DateValue dateValue1, DateValue dateValue2,
            DateTime dateToInterpolation)
        {
            var dateValueList = new List<DateValue> {dateValue1, dateValue2};
            dateValueList.Sort((p1, p2) => p1.Date.CompareTo(p2.Date));
            var leftDateValue = dateValueList.First();
            var rightDateValue = dateValueList.Last();
            var value = leftDateValue.Value +
                        (rightDateValue.Value - leftDateValue.Value) /
                        (rightDateValue.Date.ToOADate() - leftDateValue.Date.ToOADate()) *
                        (dateToInterpolation.ToOADate() - leftDateValue.Date.ToOADate());
            Logger.Info("线性插值，开始值：{0}，结束值：{1}，插值日期：{2}，插值结果：{3}", leftDateValue, rightDateValue,
                dateToInterpolation.ToShortDateString(), Math.Round(value,2));
            return Math.Round(value, 2);
        }

        /// <summary>
        /// 计算数据的K指数
        /// </summary>
        /// <param name="list">观测数据</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束如期</param>
        /// <returns></returns>
        public static double[] GetKIndexes(List<DateValue> list, DateTime startDate, DateTime endDate)
        {
            Logger.Info("开始计算观测数据的K指数，开始日期{0}，结束日期{1}，共{2}个观测数据",
                startDate.ToShortDateString(), endDate.ToShortDateString(), list.Count);
            var periods = new[] {1, 2, 3, 6, 12};
            var ret = new List<double>();
            foreach (var period in periods)
            {
                double n = 0, f = 0;
                var timeRanges = DateHelper.GetTimeRanges(startDate, endDate, period);
                foreach (var timeRange in timeRanges)
                {
                    n++;
                    if (list.Any(d => d.Date.IsInRange(timeRange.Start, timeRange.End)))
                    {
                        f++;
                    }
                }
                Logger.Info("K={0}, N={1},F={2}, K-Index:{3}", period, n, f, Math.Round(f / n, 4));
                ret.Add(Math.Round(f / n, 4));
            }
            Logger.Info("K指数计算完毕，计算结果为{0}", string.Join(",", ret));
            return ret.ToArray();
        }

        #endregion

        #region DateValueExtensions

        /// <summary>
        /// 获得观测数据中距离某个给定日期最近的k个观测数据
        /// </summary>
        /// <param name="list">观测数据</param>
        /// <param name="interpolationDate">给定日期</param>
        /// <param name="k">k</param>
        /// <returns></returns>
        public static List<DateValue> GetMostClosedDateValues(this List<DateValue> list, DateTime interpolationDate,
            int k)
        {
            list.Sort((p1, p2) => p1.Date.CompareTo(p2.Date));
            var anoList = list.ToList();
            Logger.Debug("观测数据有 {0} 条", anoList.Count);
            Func<DateValue, int> func = dv => Math.Abs((dv.Date - interpolationDate).Days);
            var ret = new List<DateValue>();
            for (int i = 0; i < k; i++)
            {
                var dayDiffs = anoList.Apply(func).ToList();
                var index = dayDiffs.IndexOf(dayDiffs.Min());
                ret.Add(new DateValue(anoList[index].Date, anoList[index].Value));
                anoList.RemoveAt(index);
            }
            Logger.Info("观测数据中距离 {0} 最近的 {1} 个观测数据：", interpolationDate.ToShortDateString(), k);
            foreach (var dateValue in ret)
            {
                Logger.Info("  " + dateValue);
            }
            return ret;
        }

        /// <summary>
        /// 观测周期内不缺数计算（线性插值）
        /// </summary>
        /// <param name="list"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public static List<DateValue> Filling(this List<DateValue> list, DateTime startDate, DateTime endDate, int period)
        {
            Logger.Info("对观测数据进行不缺数（线性插值）处理，开始日期：{0}，结束日期：{1}，共{2}个观测数据，观测周期：{3}",
                startDate.ToShortDateString(), endDate.ToShortDateString(), list.Count, period);

            list.Sort((p1, p2) => p1.Date.CompareTo(p2.Date));
            Logger.Debug("对观测数据进行排序...");
            foreach (var dateValue in list)
            {
                Logger.Debug(dateValue);
            }
            var timeRanges = DateHelper.GetTimeRanges(startDate, endDate, period);
            var anoList = new List<DateValue>();
            foreach (var timeRange in timeRanges)
            {
                var betweens = list.Betweens(timeRange.Start, timeRange.End);
                if (betweens.Count == 0) // 缺数处理，即进行线性插值
                {
                    var meanDate = DateHelper.MeanDate(timeRange.Start, timeRange.End);
                    var closedDateValues = list.GetMostClosedDateValues(meanDate, 2);
                    var interpolationValue = GetLinearInterpolation(closedDateValues.First(),
                        closedDateValues.Last(), meanDate);
                    anoList.Add(new DateValue(meanDate, interpolationValue));
                }
                else
                {
                    anoList.AddRange(betweens);
                }
            }
            Logger.Info("不缺数处理后的观测数据共 {0} 条:", anoList.Count);
            foreach (var dateValue in anoList)
            {
                Logger.Debug(dateValue);
            }
            return anoList;
        }

        /// <summary>
        /// 将每个观测周期内的数据进行平滑（取平均，一值化）处理
        /// </summary>
        /// <param name="list"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public static List<DateValue> Smoothing(this List<DateValue> list, DateTime startDate, DateTime endDate,
            int period)
        {
            Logger.Info("对观测数据进行平滑（一值化）处理，开始日期：{0}，结束日期：{1}，共{2}个观测数据，观测周期：{3}",
                startDate.ToShortDateString(), endDate.ToShortDateString(), list.Count, period);

            list.Sort((p1, p2) => p1.Date.CompareTo(p2.Date));
            Logger.Debug("对观测数据进行排序...");
            foreach (var dateValue in list)
            {
                Logger.Debug(dateValue);
            }

            var timeRanges = DateHelper.GetTimeRanges(startDate, endDate, period);
            var anoList = new List<DateValue>();
            foreach (var timeRange in timeRanges)
            {
                var betweens = list.Betweens(timeRange.Start, timeRange.End);
                if (betweens.Count == 0) //缺数情况
                {
                    //Logger.Warn("在范围{0}~{1}内未找到任何观测数据",
                    //    timeRange.Start.ToShortDateString(), timeRange.End.ToShortDateString());
                    // 哈哈，恶魔的微笑
                }
                else if (betweens.Count == 1) //只找到1个数的情况
                {
                    anoList.Add(betweens.First());
                }
                else //找到多个数的情况
                {
                    foreach (var dateValue in betweens)
                    {
                        Logger.Debug("  " + dateValue);
                    }
                    Logger.Debug("求其平均值为:{0}", betweens.Mean());
                    anoList.Add(betweens.Mean());
                }
            }
            Logger.Info("平滑后的观测数据个数为{0}个", anoList.Count);
            Logger.Debug("平滑后的观测数据为：");
            foreach (var dateValue in anoList)
            {
                Logger.Debug(dateValue);
            }
            return anoList;
        }

        public static List<DateValue> Betweens(this List<DateValue> list, DateTime leftDate, DateTime rightDate)
        {
            var betweens = list.FindAll(p => p.Date.IsInRange(leftDate, rightDate));
            if (betweens.Count == 0)
            {
                Logger.Warn("在日期范围{0}~{1}中共找到0个测值",
                    leftDate.ToShortDateString(), rightDate.ToShortDateString());
            }
            else
            {
                Logger.Debug("在日期范围{0}~{1}中共找到{2}个测值",
                    leftDate.ToShortDateString(), rightDate.ToShortDateString(), betweens.Count);
            }
            return betweens;
        }

        /// <summary>
        /// 计算一组观测数据的平均值
        /// </summary>
        /// <param name="dateValues"></param>
        /// <returns></returns>
        public static DateValue Mean(this IEnumerable<DateValue> dateValues)
        {
            var enumerable = dateValues as DateValue[] ?? dateValues.ToArray();
            enumerable.ToList().Sort((p1, p2) => p1.Date.CompareTo(p2.Date));
            var first = enumerable.First();
            var last = enumerable.Last();
            var meanDate = first.Date.AddDays((last.Date - first.Date).Days / 2.0);
            var meanValue = Math.Round(enumerable.Average(p => p.Value), 2);
            return new DateValue(meanDate, meanValue);
        }


        #endregion

        #region Test Case

        [Test]
        public static void TestSmooth()
        {
            var startDate = new DateTime(1999, 1, 13);
            var endDate = new DateTime(2009, 12, 31);
            var list = DaoObject.GetRawData(606).RetrieveDateValues().Betweens(startDate, endDate);
            var kIndexs = DateValueHelper.GetKIndexes(list, startDate, endDate);
            Console.WriteLine("K-Indexs:" + string.Join(",", kIndexs));
            var smoothedList = list.Smoothing(startDate, endDate, 3);
            Assert.True(true);
        }

        [Test]
        public static void TestFill()
        {
            var startDate = new DateTime(1999, 1, 13);
            var endDate = new DateTime(2009, 12, 31);
            var list = DaoObject.GetRawData(606).RetrieveDateValues().Betweens(startDate, endDate);
            var anoList = list.Filling(startDate, endDate, 2);
            Assert.True(true);
        }

        [Test]
        public static void TestGetMostClosedDateValues()
        {
            var list = DaoObject.GetRawData(606).RetrieveDateValues();
            var dateTimeToCheck = new DateTime(2009, 1, 19);
            list.GetMostClosedDateValues(dateTimeToCheck, 2);
            Assert.True(true);
        }

        #endregion


    }
}