// 解决方案名称：Xb2
// 工程名称：Xb2Core
// 文件名：Xb2YearChange2.cs
// 作者：Semicloud
// 初次编写时间：2014-09-01
// 功能：

using System;
using System.Collections.Generic;
using System.Linq;
using Xb2.Algorithms.Core.Entity;

namespace Xb2.Algorithms.Core.Methods.YearChange
{
    /// <summary>
    /// 跨断层流动形变资料处理软件-年周变算法
    /// </summary>
    public class Xb2YearChange
    {
        private Xb2YearChangeInput _input;

        /// <summary>
        /// 跨断层流动形变资料处理软件-年周变算法 构造函数
        /// </summary>
        /// <param name="input">年周变算法-输入</param>
        public Xb2YearChange(Xb2YearChangeInput input)
        {
            _input = input;
        }

        /// <summary>
        /// 获得日期范围，这是一个主要的算法，主要仔细写一下
        /// </summary>
        /// <returns></returns>
        private List<DateTime> getDateRanges()
        {
            int period = 1; //TODO 观测周期先置1
            DateTime t1 = this._input.Start;
            DateTime t2 = this._input.End;
            var dateTimes = GetDateTimesByStep(t1, t2,  period).ToList();
            return dateTimes;
        }

        /// <summary>
        /// 获得年变数据
        /// </summary>
        /// <returns></returns>
        private Dictionary<DateTime, double> getYearChangeData()
        {
            var answer = new Dictionary<DateTime, double>();
            var dates = this.getDateRanges();
            for (int i = 0; i < dates.Count - 1; i++)
            {
                int m1 = dates[i].Month, m2 = dates[i + 1].AddDays(-1).Month;
                var them = _input.Collection.FindAll(m => m.Date.Month >= m1 && m.Date.Month <= m2);
                answer.Add(dates[i + 1].AddDays(-1), them.Average(m => m.Value));
            }
            return answer;
        }

        /// <summary>
        /// 获得月距平线数据
        /// </summary>
        /// <returns></returns>
        public List<DateValue> GetDistanceLineData()
        {
            var answer = new List<DateValue>();
            var dates = this.getDateRanges();
            for (int i = 0; i < dates.Count - 1; i++)
            {
                int m1 = dates[i].Month, m2 = dates[i + 1].AddDays(-1).Month;
                var them = _input.Collection.FindAll(m => m.Date.Month >= m1 && m.Date.Month <= m2);
                double avg = them.Average(t => t.Value);
                foreach (var t in them)
                    answer.Add(new DateValue(t.Date, t.Value - avg));
            }
            answer.Sort((d1, d2) => DateTime.Compare(d1.Date, d2.Date));
            return answer;
        }

        /// <summary>
        /// 获得年周变线数据
        /// </summary>
        /// <returns></returns>
        public List<DateValue> GetYearChangeLineData()
        {
            var answer = new List<DateValue>();
            var yearchanges = this.getYearChangeData();
            var years = _input.Collection.Select(m => m.Date.Year).Distinct();
            foreach (var year in years)
            {
                foreach (var yearchange in yearchanges)
                {
                    DateTime date = new DateTime();
                    double value = 0;
                    if (yearchange.Key.Month == 2 && yearchange.Key.Day == 29)
                    {
                        date = new DateTime(year, 2, 28);
                        value = yearchange.Value;
                    }
                    else
                    {
                        date = new DateTime(year, yearchange.Key.Month, yearchange.Key.Day);
                        value = yearchange.Value;
                    }
                    answer.Add(new DateValue(date, value));
                }
            }
            answer.Sort((d1, d2) => d1.Date.CompareTo(d2.Date));
            return answer;
        }

        public static IEnumerable<DateTime> GetDateTimesByStep(DateTime start, DateTime end, int delta)
        {
            //日数大于30，统一取到28
            var answer = new List<DateTime>();
            var cursor = start.AddMonths(delta);
            answer.Add(start);
            while (cursor<=end)
            {
                if (cursor.Date.Month == 2 && cursor.Date.Day == 29)
                {
                    cursor = cursor.AddDays(-1);
                }
                answer.Add(cursor);
                start = cursor;
                cursor = cursor.AddMonths(delta);
                if(cursor>end) break;
            }
            answer.Add(end);
            return answer;
        }
    }
}