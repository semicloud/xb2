using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Xb2.Algorithms.Core.Entity
{
    /// <summary>
    /// 窗口类 20160309
    /// </summary>
    public class Window:DateRange
    {
        public Window()
        {
        }

        public Window(DateTime lower, DateTime upper) : base(lower, upper)
        {

        }

        public List<DateRange> Interset_v20150819(List<DateTime> dateTimes)
        {
            var ans = new List<DateRange>();
            var subset = dateTimes.FindAll(d => (d >= this.Lower) && (d <= this.Upper));
            subset.Sort();
            if (!subset.Contains(this.Lower)) subset.Insert(0, this.Lower);
            if (!subset.Contains(this.Upper)) subset.Insert(subset.Count, this.Upper);
            Debug.Assert(subset.First().Equals(this.Lower), "窗口上界插入错误");
            Debug.Assert(subset.Last().Equals(this.Upper), "窗口下界插入错误");
            for (int i = 0; i < subset.Count - 1; i++)
            {
                DateTime cur = subset[i], next = subset[i + 1];
                if (next.Equals(this.Upper))
                {
                    ans.Add(new DateRange(cur, next));
                    break;
                }
                ans.Add(new DateRange(cur, next.AddDays(-1)));
            }
            return ans;
        }

        public Window GetPre_v20150821(int delta)
        {
            var lower = this.Lower.AddMonths(-delta);
            var upper = this.Upper.AddMonths(-delta);
            //            if (Lower.IsLastDayOfMonth())
            //                lower = lower.BringToLastDay();
            //            if (Upper.IsLastDayOfMonth())
            //                upper = upper.BringToLastDay();
            var win = new Window(lower, upper);
            //对于闰平年2月份的处理
            Predicate<DateTime> needChange = d => DateTime.IsLeapYear(d.Year) && d.Month.Equals(2) && d.Day.Equals(28);
            win.Lower = needChange(win.Lower) ? new DateTime(win.Lower.Year, 2, 29) : win.Lower;
            win.Upper = needChange(win.Upper) ? new DateTime(win.Upper.Year, 2, 29) : win.Upper;
            return win;
        }

        public static List<Window> GetWindows(DateTime start, DateTime end, int slen, int wlen)
        {
            var answer = new List<Window>();
            DateTime cursor = start;
            while (cursor < end)
            {
                if (cursor.AddMonths(wlen) > end)
                    break;
                var window = new Window(cursor, cursor.AddMonths(wlen).AddDays(-1));
                answer.Add(window);
                cursor = cursor.AddMonths(slen);
            }
            answer.Add(new Window(cursor, end));
            return answer;
        }
    }
}
