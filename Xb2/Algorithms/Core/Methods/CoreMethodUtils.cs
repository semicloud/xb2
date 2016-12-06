// 解决方案名称：Xb2
// 工程名称：Xb2Core
// 文件名：Xb2MethodUtils.cs
// 作者：Semicloud
// 初次编写时间：2014-08-25
// 功能：算法工具类，主要是如何取数

using System;
using System.Collections.Generic;
using Xb2.Algorithms.Core.Entity;
using Xb2.Algorithms.Core.Methods.Strain.StrainValueUnit;

namespace Xb2.Algorithms.Core.Methods
{
    /// <summary>
    /// 跨断层算法工具类
    /// </summary>
    internal static class CoreMethodUtils
    {
        public static List<DateValue> CreateDateValues()
        {
            return new List<DateValue>();
        }

        /// <summary>
        /// 获得测值
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static List<double> getValues(Dictionary<DateTime, List<double>> dictionary, DateTime dateTime)
        {
            return dictionary.ContainsKey(dateTime) ? dictionary[dateTime] : null;
        }

        /// <summary>
        /// 获得所有窗口日期
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="wlen"></param>
        /// <param name="slen"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static List<DateTime> GetWindows(DateTime start, DateTime end, int wlen, int slen, int delta)
        {
            List<DateTime> windows = new List<DateTime>();
            /**
            DateTime cursor = start.AddMonths(delta);
            while (cursor < end)
            {
                windows.Add(Window.GetWindow(cursor, cursor.AddMonths(wlen), end).GetLastDay());
                cursor = cursor.AddMonths(slen);
            }
             */
            return windows;
        }

        /// <summary>
        ///  根据开始日期、结束日期、窗长、步长、时间间隔、测值序列等获取速率值
        ///  形式为Dictionary&lt;窗口日期,该窗口对应的速率值&gt;
        /// </summary>
        /// <param name="coll"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="wlen"></param>
        /// <param name="slen"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, List<double>> GetRateValues(DateValueList coll, DateTime start,
            DateTime end, int wlen, int slen, int delta)
        {
            var dictionary = new Dictionary<DateTime, List<double>>();
            /**
            DateRange range = new DateRange(start, end);
            var target = coll.Between(range);
            var period = coll.GetPossiblePeriod();
            var allDateTimes = DateRange.GetDateRangeStepByStep(start, end, DateUnit.MONTH, period);
            var cursor = start.AddMonths(delta);
            while (cursor < end)
            {
                var window = Window.GetWindow(cursor, cursor.AddMonths(wlen), end);
                var dates = window.GetInterset(allDateTimes.ToList());
                var values = GetRateValuesInWin(target, delta, dates);
                if (values.Count == 0)
                {
                    cursor = cursor.AddMonths(slen);
                    continue;
                }
                dictionary.Add(window.GetLastDay(), values.ToList());
                cursor = cursor.AddMonths(slen);
            }
             * */
            return dictionary;
        }

        /// <summary>
        /// 得到窗口中测值的速率序列
        /// </summary>
        /// <param name="coll">全部测值</param>
        /// <param name="delta">时间间隔</param>
        /// <param name="inWin">窗口中的日期</param>
        /// <returns>IList of double</returns>
        public static IList<double> GetRateValuesInWin(DateValueList coll, int delta, IList<DateTime> inWin)
        {
            Func<DateValue, DateValue, double> rate = (pre, cur) => (cur.Value - pre.Value) / (cur.Date - pre.Date).Days * 365;
            return GetValuesInWin(coll, delta, inWin, rate);
        }

        /// <summary>
        /// 得到一个窗口中的应变序列
        /// </summary>
        /// <param name="coll">测值序列</param>
        /// <param name="delta">时间间隔</param>
        /// <param name="inWin">窗口中的日期</param>
        /// <returns>IList of double</returns>
        public static IList<double> GetStrainValuesInWin(DateValueList coll, int delta, IList<DateTime> inWin)
        {
            Func<DateValue, DateValue, double> strain = (pre, cur) => (cur.Value - pre.Value) / pre.Value;
            return GetValuesInWin(coll, delta, inWin, strain);
        }

        /// <summary>
        /// 获得窗口中的观测值
        /// </summary>
        /// <param name="coll">测值恊</param>
        /// <param name="delta">时间间隔</param>
        /// <param name="inWin">窗口日期</param>
        /// <param name="func">两个测值之间的操作函数</param>
        /// <returns>IList of double</returns>
        private static IList<double> GetValuesInWin(DateValueList coll, int delta, IList<DateTime> inWin,
            Func<DateValue, DateValue, double> func)
        {
            var doubles = new List<double>();
            /**
            for (int i = 0; i < inWin.Count - 1; i++)
            {
                Window cur = new Window {Lower = inWin[i], Upper = inWin[i + 1]};
                Window pre = cur.GetPreWindow(delta);
                var curValues = coll.Between(cur);
                var preValues = coll.Between(pre);
                foreach (var curValue in curValues)
                {
                    var chi = curValue.Date.AddMonths(-delta);
                    var preValue = preValues.GetMostClosedMValue(chi);
                    if (preValue == null) continue;
                    doubles.Add(func(preValue, curValue));
                }
            }
             * */
            return doubles;
        }

        /// <summary>
        /// 获取需要计算的应变值,返回结果为FirstUnit
        /// </summary>
        /// <param name="collection1">基线1</param>
        /// <param name="collection2">基线2</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="wlen">窗长</param>
        /// <param name="slen">步长</param>
        /// <param name="delta">时间间隔</param>
        /// <returns></returns>
        public static List<StrainValueUnitFirst> getStrainValueAsUnitFirst(DateValueList collection1, DateValueList collection2,
            DateTime start, DateTime end, int wlen, int slen, int delta)
        {
            var ans = new List<StrainValueUnitFirst>();
            /**
            var range = new DateRange(start, end);
            var col1 = collection1.Between(range);
            var col2 = collection2.Between(range);
            int p1 = col1.GetPossiblePeriod(), p2 = col2.GetPossiblePeriod();
            var col1AllDateTimes = DateRange.GetDateRangeStepByStep(start, end, DateUnit.MONTH, p1);
            var col2AllDateTimes = DateRange.GetDateRangeStepByStep(start, end, DateUnit.MONTH, p2);
            var cursor = start.AddMonths(delta);
            while (cursor < end)
            {
                var window = Window.GetWindow(cursor, cursor.AddMonths(wlen), end);
                var col1DateInWin = window.GetInterset(col1AllDateTimes.ToList());
                var col2DateInWin = window.GetInterset(col2AllDateTimes.ToList());
                var values1 = GetStrainValuesInWin(col1, delta, col1DateInWin);
                var values2 = GetStrainValuesInWin(col2, delta, col2DateInWin);
                if (values1.Count > 0 && values2.Count > 0)
                {
                    var value1 = values1.Average();
                    var value2 = values2.Average();
                    ans.Add(new StrainValueUnitFirst { Value1 = value1, Value2 = value2, Date = window.GetLastDay() });
                }
                cursor = cursor.AddMonths(slen);
            }
             * */
            return ans;
        }

        /// <summary>
        /// 获取需要计算的应变值,返回结果为FirstUnit
        /// </summary>
        /// <param name="collection1">基线1</param>
        /// <param name="collection2">基线2</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="wlen">窗长</param>
        /// <param name="slen">步长</param>
        /// <param name="delta">时间间隔</param>
        /// <returns></returns>
        public static List<StrainValueUnitSecond> getStrainValueAsUnitSecond(DateValueList collection1, DateValueList collection2, DateValueList collection3,
            DateTime start, DateTime end, int wlen, int slen, int delta)
        {
            var ans = new List<StrainValueUnitSecond>();
            /**
            var range = new DateRange(start, end);
            var col1 = collection1.Between(range);
            var col2 = collection2.Between(range);
            var col3 = collection3.Between(range);
            int p1 = col1.GetPossiblePeriod(), p2 = col2.GetPossiblePeriod(), p3 = col3.GetPossiblePeriod();
            var col1AllDateTimes = DateRange.GetDateRangeStepByStep(start, end, DateUnit.MONTH, p1);
            var col2AllDateTimes = DateRange.GetDateRangeStepByStep(start, end, DateUnit.MONTH, p2);
            var col3AllDateTimes = DateRange.GetDateRangeStepByStep(start, end, DateUnit.MONTH, p3);
            var cursor = start.AddMonths(delta);
            while (cursor < end)
            {
                var window = Window.GetWindow(cursor, cursor.AddMonths(wlen), end);
                var col1DateInWin = window.GetInterset(col1AllDateTimes.ToList());
                var col2DateInWin = window.GetInterset(col2AllDateTimes.ToList());
                var col3DateInWin = window.GetInterset(col3AllDateTimes.ToList());
                var values1 = GetStrainValuesInWin(col1, delta, col1DateInWin);
                var values2 = GetStrainValuesInWin(col2, delta, col2DateInWin);
                var values3 = GetStrainValuesInWin(col3, delta, col3DateInWin);
                if (values1.Count > 0 && values2.Count > 0 && values3.Count > 0)
                {
                    var value1 = values1.Average();
                    var value2 = values2.Average();
                    var value3 = values3.Average();
                    ans.Add(new StrainValueUnitSecond
                    {
                        Value1 = value1,
                        Value2 = value2,
                        Value3 = value3,
                        Date = window.GetLastDay()
                    });
                }
                cursor = cursor.AddMonths(slen);
            }
             */
            return ans;
        }
    }
}