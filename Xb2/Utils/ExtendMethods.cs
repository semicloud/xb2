using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Xb2.Algorithms.Core.Entity;

namespace Xb2.Utils
{
    public static class ExtendMethods
    {
        #region String Extend Methods

        /// <summary>
        /// 全角字符串转半角字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToDbc(this string input)
        {
            char[] array = input.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == 12288)
                {
                    array[i] = (char) 32;
                    continue;
                }
                if (array[i] > 65280 && array[i] < 65375)
                {
                    array[i] = (char) (array[i] - 65248);
                }
            }
            return new string(array);
        }

        #endregion

        public static double Round(this double d, int precision)
        {
            return Math.Round(d, precision);
        }

        public static double ToDouble(this string input)
        {
            return Convert.ToDouble(input);
        }

        public static string SStr(this DateTime dateTime)
        {
            return dateTime.ToShortDateString();
        }

        public static object GetStringOrDBNull(this string obj)
        {
            return string.IsNullOrEmpty(obj) ? DBNull.Value : (object) obj;
        }

        public static object GetInt32OrDbNull(this string obj)
        {
            return string.IsNullOrEmpty(obj) ? (object) DBNull.Value : Convert.ToInt32(obj);
        }

        public static object GetDoubleOrDBNull(this string obj)
        {
            return string.IsNullOrEmpty(obj) ? (object) DBNull.Value : Convert.ToDouble(obj);
        }

        public static List<TextBox> GetTextBoxs(this Panel panel)
        {
            return
                panel.Controls.Cast<System.Windows.Forms.Control>()
                    .ToList()
                    .FindAll(c => c.GetType() == typeof(TextBox))
                    .Cast<TextBox>()
                    .ToList();
        }


        #region 需重构的代码

        public static double R4(this double d)
        {
            return Math.Round(d, 4);
        }

        //角度转弧度
        public static double ToRad(this double d)
        {
            return d * Math.PI / 180;
        }

        //弧度转角度
        public static double ToDeg(this double r)
        {
            return r * 180 / Math.PI;
        }

        /// <summary>
        /// 该日期是否为该月的最后一天
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool isMonLastDay(this DateTime dateTime)
        {
            var o = dateTime.AddDays(1);
            return dateTime.Month != o.Month;
        }

        public static DateTime LastDay(this DateTime date)
        {
            return date.AddDays(1 - date.Day).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// 获取日期范围内的测值
        /// </summary>
        /// <param name="measureValues"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static List<DateValue> Between(this List<DateValue> measureValues, DateTime lower, DateTime upper)
        {
            var selected = measureValues.FindAll(m => m.Date >= lower && m.Date <= upper).ToList();
            Debug.Print("{0} values selected in {1} values", selected.Count, measureValues.Count);
            return selected;
        }

        /// <summary>
        /// 获取日期范围内的测值
        /// </summary>
        /// <param name="measureValues"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static List<DateValue> Between(this List<DateValue> measureValues, DateRange range)
        {
            return measureValues.Between(range.Lower, range.Upper);
        }

        /// <summary>
        /// 计算窗口中的速率值（通过归一化方法）
        /// 用于速率差分和合成的计算（2015年7月20日与吴老师讨论后修改）
        /// </summary>
        /// <param name="dateRanges"></param>
        /// <param name="list">原始数据</param>
        /// <param name="delta">时间间隔</param>
        /// <param name="function">速率差分函数</param>
        /// <returns></returns>
        public static List<Double> ComputeDiffsInWinByNormalization(this IEnumerable<DateRange> dateRanges, List<DateValue> list,
            int delta, Func<DateValue, DateValue, double> function)
        {
            var answer = new List<double>();
            //处理每一个日期范围
            foreach (var dateRange in dateRanges)
            {
                //形成当前窗口对象
                var curWin = new Window(dateRange.Lower, dateRange.Upper);
                //获得当前窗口对象的前一个窗口
                var preWin = curWin.GetPre_v20150821(delta);
                //当前窗口中的测值
                var curDvps = list.Between(curWin);
                //前一个窗口中的测值
                var preDvps = list.Between(preWin);
                //当前的窗口如果没有测值，判为缺数，处理下一个日期范围
                if (curDvps == null || curDvps.Count == 0)
                {
                    Debug.Print("{0}，缺数", dateRange);
                    continue;
                }
                //求当前窗口中所有测值的平均值（即归一化处理）
                var curMeanDate = curDvps.Select(d => d.Date).Mean();
                var curMeanValue = curDvps.Select(d => d.Value).Average();
                var curMean = new DateValue(curMeanDate, curMeanValue);
                Debug.Print("{0}，{1}个数【{2}】，平均值为{3}->", dateRange, curDvps.Count,
                    string.Join(",", curDvps), curMean);
                //前一个窗口如果没有测值，判为缺数，处理下一个日期范围
                if (preDvps == null || preDvps.Count == 0)
                {
                    Debug.Print("前范围{0}，缺数", preWin);
                    continue;
                }
                //求前一个窗口中所有测值的平均值（即归一化处理）
                var preMeanDate = preDvps.Select(d => d.Date).Mean();
                var preMeanValue = preDvps.Select(d => d.Value).Average();
                var preMean = new DateValue(preMeanDate, preMeanValue);
                Debug.Print("{0}，前范围{1}-> 找到{2}个数【{3}】，平均值{4}->", curMean, preWin,
                    preDvps.Count, string.Join(",", preDvps), preMean);
                //如果前后两个测值相等，无法计算速率，报错
                if (preMean.Date == curMean.Date && preMean.Value.Equals(curMean.Value))
                {
                    Debug.Print("找到相同的测值，请重新设置窗长、步长等值...");
                    throw new Exception("前后窗口中找到同一个数" + preMean + "无法计算速率，计算结束！");
                }
                Debug.Print("算速率{0}，{1}->{2}", curMean, preMean, function(preMean, curMean));
                answer.Add(function(preMean, curMean));
            }
            return answer;
        }

        public static DateTime Mean(this IEnumerable<DateTime> dateTimes)
        {
            dateTimes.ToList().Sort();
            DateTime start = dateTimes.First(), end = dateTimes.Last();
            return start.AddDays(((end - start).Days) / 2.0);
        }

    #endregion
    }
}
