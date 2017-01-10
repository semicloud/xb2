using System;
using System.Collections.Generic;
using System.Linq;
using Itenso.TimePeriod;
using NLog;
using NUnit.Framework;

namespace Xb2.Utils
{
    public static class DateHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region HelperMethods

        /// <summary>
        /// 求两个日期的平均日期
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static DateTime MeanDate(DateTime startDate, DateTime endDate)
        {
            var dateList = new List<DateTime> {startDate, endDate};
            var leftDate = dateList.Min();
            var rightDate = dateList.Max();
            var days = (rightDate - leftDate).Days;
            var meanDate = leftDate.AddDays(Math.Round(days / 2.0));
            Logger.Info("{0} 和 {1} 的平均日期为：{2}",
                startDate.ToShortDateString(), endDate.ToShortDateString(), meanDate.ToShortDateString());
            return meanDate;
        }

        /// <summary>
        /// 生成日期范围，这个函数也许是跨断层软件中最重要的函数
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static List<TimeRange> GetTimeRanges(DateTime startDate, DateTime endDate, int step)
        {
            var p = startDate;
            var timeRanges = new List<TimeRange>();
            while (p.AddMonths(step) < endDate)
            {
                var timeRange = new TimeRange(p, p.AddMonths(step).AddDays(-1));
                timeRanges.Add(timeRange);
                p = p.AddMonths(step);
            }
            timeRanges.Add(new TimeRange(p, endDate));
            Logger.Info("生成日期范围，开始时间{0}，结束时间：{1}，间隔：{2}个月，共生成{3}个日期范围",
                startDate.ToShortDateString(), endDate.ToShortDateString(), step, timeRanges.Count);
            foreach (var timeRange in timeRanges)
            {
                Logger.Debug(timeRange);
            }
            return timeRanges;
        }

        #endregion

        #region DateTime Extension Methods

        /// <summary>
        /// 判断一个日期是否在一个范围内
        /// </summary>
        /// <param name="dateTimeToCheck"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static bool IsInRange(this DateTime dateTimeToCheck, DateTime startDate, DateTime endDate)
        {
            var isInRange = dateTimeToCheck >= startDate && dateTimeToCheck <= endDate;
            Logger.Trace("{0} 是否在范围{1}~{2}中？=>{3}", dateTimeToCheck.ToShortDateString(),
                startDate.ToShortDateString(), endDate.ToShortDateString(), isInRange);
            return isInRange;
        }

        [Obsolete]
        public static DateTime MeanDate(this IEnumerable<DateTime> dateTimes)
        {
            dateTimes.ToList().Sort();
            DateTime start = dateTimes.First(), end = dateTimes.Last();
            return start.AddDays(((end - start).Days) / 2.0);
        }

        #endregion

        #region Test Case

        [Test]
        public static void TestMeanDate()
        {
            var startDate = new DateTime(1999, 5, 13);
            var endDate = new DateTime(1999, 7, 12);
            Console.WriteLine(MeanDate(startDate,endDate).ToShortDateString());
            Assert.True(true);
        }

        [Test]
        public static void TestDate()
        {
            var startDate = new DateTime(1999, 2, 2);
            Console.WriteLine("AddMonth(1):{0}", startDate.AddMonths(1).ToShortDateString());
            Console.WriteLine("AddMonth(2):{0}", startDate.AddMonths(2).ToShortDateString());
            Console.WriteLine("AddMonth(-1):{0}", startDate.AddMonths(-1).ToShortDateString());
            Console.WriteLine("AddMonth(-2):{0}", startDate.AddMonths(-2).ToShortDateString());
            //startDate = new DateTime(2000, 1, 29);
            //Console.WriteLine(startDate.ToShortDateString());
            //Console.WriteLine("AddMonth(1):{0}", startDate.AddMonths(1).ToShortDateString());
            //Console.WriteLine("AddMonth(-1):{0}", startDate.AddMonths(-1).ToShortDateString());

            var endDate = new DateTime(2009, 1, 1);
            var timeRanges = GetTimeRanges(startDate, endDate, 12);
            foreach (var timeRange in timeRanges)
            {
                Console.WriteLine(timeRange);
            }
            Assert.True(true);
        }

        #endregion
    }
}
