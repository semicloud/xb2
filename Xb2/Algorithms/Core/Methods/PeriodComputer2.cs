// 解决方案名称：XbApp
// 工程名称：XbApp
// 文件名：PeriodComputer2.cs
// 作者：Semicloud
// 初次编写时间：2015-04-05
// 功能：

using System;
using System.Collections.Generic;
using System.Linq;
using Xb2.Algorithms.Core.Entity;

namespace Xb2.Algorithms.Core.Methods
{
    public class PeriodComputer2
    {
        private static Int32[] PERIOD_COLLECTION = {1, 2, 3, 6, 12};
        //private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 计算输入测值序列在日期t0到ts之间的各个观测周期的K值
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="t0"></param>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static Dictionary<Int32, Double> GetKValues(DateValueList collection, DateTime t0, DateTime ts)
        {
            Dictionary<Int32, Double> ans = new Dictionary<int, double>();
            List<DateTime> allDateTimes =
                collection.FindAll(m => m.Date >= t0 && m.Date <= ts).Select(m => m.Date).ToList();
            allDateTimes.Sort();

//            Console.WriteLine("输入测值序列如下：");
//            (from e in collection select new { e.Date, e.Value }).ToList()
//                .ForEach(e => Console.WriteLine(e.Date.ToShortDateString() + ", " + e.Value));

            Int32 monthCount = (ts.Year - t0.Year)*12 + ts.Month - t0.Month;
            // Console.WriteLine("第一个观测日期{0}，最后一个观测日期：{1}，之间间隔{2}个月", t0, ts, monthCount);

            foreach (Int32 thePeriod in PERIOD_COLLECTION)
            {
                // Console.WriteLine("计算观测周期为" + thePeriod + "个月的K值");
                Double N = 0, F = 0;
                //N = (monthCount%thePeriod == 0) ? monthCount/thePeriod : (monthCount/thePeriod) + 1;
                //Console.WriteLine("N=" + N);
                DateTime p = t0;
                while (p.AddMonths(thePeriod) <= ts)
                {
                    N = N + 1;
                    DateTime pointer = p.AddMonths(thePeriod);
                    // Console.WriteLine("t0+T:" + pointer.ToShortDateString());

                    var result = allDateTimes.FindAll(d => d >= p && d <= pointer);
                    //  Console.WriteLine("在{0}到{1}间是否有测值:{2}", p.ToShortDateString(), pointer.ToShortDateString(),
                    //   result.Count > 0);

                    if (result.Count > 0)
                        F = F + 1;
                    p = p.AddMonths(thePeriod);
                }
                var lasted = allDateTimes.FindAll(d => d >= p && d <= ts);
                // Console.WriteLine("在最后一个时间段内：{0}-{1}，是否有测值:{2}", p.ToShortDateString(), ts.ToShortDateString(),
                // lasted.Count);
                N = N + 1;
                if (lasted.Count > 0)
                    F = F + 1;
                //Console.WriteLine("Period={0},F={1},N={2}", thePeriod, F, N);
                ans.Add(thePeriod, F/N);
            }
            return ans;
        }


        public static Int32 GetPeriod(Dictionary<Int32, Double> dictionary)
        {
            if (dictionary[1] == 1) return 1;
            return dictionary.Where(q => q.Value < 1).Select(q => q.Key).First();
        }

        //[Test]
        //public void testGetKValues()
        //{
        //    for (int i = 6; i <= 7; i++)
        //    {
        //        try
        //        {
        //            var obj = MItemObj.GetInstanceByID(i);
        //            MValueCollection collection = obj.InitialDatabase.DataCollection;
        //            collection.Sort((m1, m2) => m1.Date.CompareTo(m2.Date));
        //            var start = collection.First().Date;
        //            var end = collection.Last().Date;
        //            var dic = GetKIndexes(collection, start, end);
        //            Console.WriteLine("测项" + i + " : "  +  String.Join(",", dic));
        //        }
        //        catch (Exception)
        //        {
        //            Console.WriteLine(i + ", ERROR!");
        //        }
        //    }
        //    Assert.True(true);
        //}
    }
}