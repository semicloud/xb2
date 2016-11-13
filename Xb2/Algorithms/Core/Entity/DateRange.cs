using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xb2.Utils;

namespace Xb2.Algorithms.Core.Entity
{
    public class DateRange
    {
        public DateTime Lower { get; set; }
        public DateTime Upper { get; set; }

        public DateRange()
        {
        }

        public DateRange(DateTime lower, DateTime upper)
        {
            this.Lower = lower;
            this.Upper = upper;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", this.Lower.ToShortDateString(), this.Upper.ToShortDateString());
        }

        public static List<DateRange> GetDateRangesByStep(DateTime start, DateTime end, int delta)
        {
            if (start > end) throw new Exception("起始日期必须小于终止日期");
            Debug.Print("generate date ranges from {0} to {1} with delta {2}", start.ToShortDateString(), end.ToShortDateString(), delta);
            var answer = new List<DateRange>();
            var cursor = start.AddMonths(delta);
            while (cursor <= end)
            {
                //日期不能有重复值，所以要取值的前一天
                answer.Add(new DateRange(start, cursor.AddDays(-1)));
                Debug.Print("--start:{0},cursor:{1}", start.ToShortDateString(), cursor.AddDays(-1).ToShortDateString());
                start = cursor;
                cursor = cursor.AddMonths(delta);
                if (cursor > end) break;
            }
            if (start != end)
            {
                answer.Add(new DateRange(start, end));
                Debug.Print("--start:{0},end:{1}", start.ToShortDateString(), end.ToShortDateString());
            }
            Debug.Print("date ranges generate complete");
            return answer;
        }

        public static IEnumerable<DateTime> ByStep_v20150819(DateTime start, DateTime end, int span)
        {
            var ans = new List<DateTime>();
            var pointer = new DateTime(start.Year, start.Month, start.Day);
            //对开始日期取到月末的特殊处理
            if (pointer.isMonLastDay())
            {
                pointer = pointer.AddDays(1);
                while (pointer <= end)
                {
                    ans.Add(pointer.AddDays(-1));
                    pointer = pointer.AddMonths(span);
                }
            }
            else
            {
                while (pointer <= end)
                {
                    ans.Add(pointer);
                    pointer = pointer.AddMonths(span);
                }
            }
            if (!ans.Contains(end)) ans.Add(end);
            ans.Sort();
            return ans;
        }

        public static IEnumerable<DateTime> GetDateTimesByStep(DateTime start, DateTime end, int delta)
        {
            //日数大于30，统一取到28
            var answer = new List<DateTime>();
            var cursor = start.AddMonths(delta);
            answer.Add(start);
            while (cursor<=end)
            {
                answer.Add(cursor);
                start = cursor;
                cursor = cursor.AddMonths(delta);
                if(cursor>end) break;
            }
            answer.Add(end);
            return answer;
        }

        /// <summary>
        /// 按照时间间隔划分时间点，但是要取整月，不能去整月多1天，比如1月12日，观测周期是2个月，则取到3月11日，如果是1月1日，观测周期是2个月，则取到2月28日；
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<DateTime> GetDateTimesByStepWithSubDay(DateTime start, DateTime end, int delta)
        {
            var answer = new List<DateTime>();
            var cursor = start.AddMonths(delta);
            answer.Add(start);
            while (cursor <= end)
            {
                cursor = cursor.AddDays(-1);
                answer.Add(cursor);
                start = cursor;
                cursor = cursor.AddMonths(delta);
                if (cursor > end) break;
            }
            answer.Add(end);
            return answer;
        }

        private static DateTime getDateTime(DateTime date, DateUnit unit, int step)
        {
            DateTime answer = new DateTime();
            switch (unit)
            {
                case DateUnit.YEAR:
                    answer = date.AddYears(step);
                    break;
                case DateUnit.MONTH:
                    answer = date.AddMonths(step);
                    break;
                case DateUnit.DAY:
                    answer = date.AddDays(step);
                    break;
            }
            return answer;
        }
    }
}
