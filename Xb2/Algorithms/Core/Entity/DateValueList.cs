using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MySql.Data.MySqlClient;
using Xb2.Algorithms.Core.Methods;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.Algorithms.Core.Entity
{
    public class DateValueList : List<DateValue>
    {

        /// <summary>
        /// 权重
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// 异常趋势，用于异常放大计算
        /// </summary>
        public int AbnormalTrend { get; set; }

        /// <summary>
        /// 获取日期范围内的测值对象集合
        /// </summary>
        /// <param name="range">日期范围</param>
        /// <returns>List of MValue</returns>
        public DateValueList Between(DateRange range)
        {
            DateValueList dateValueList = new DateValueList();
            var result = FindAll(v => v.Date >= range.Lower && v.Date <= range.Upper);
            result.ForEach(dateValueList.Add);
            dateValueList.AbnormalTrend = this.AbnormalTrend;
            dateValueList.Weight = this.Weight;
            return dateValueList;
        }

        /// <summary>
        /// 获得距离date最近的测值
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public DateValue GetMostClosedMValue(DateTime date)
        {
            if (this.Count == 0) return null;
            var subDays = new List<int>();
            ForEach(v => subDays.Add(Math.Abs((v.Date - date).Days)));
            int index = subDays.IndexOf(subDays.Min());
            return this[index];
        }

        /// <summary>
        /// 获取最可能的观测周期
        /// </summary>
        /// <returns>int</returns>
        public int GetPossiblePeriod()
        {
            return PeriodComputer.GetPossiblePeriod(this);
        }

        /// <summary>
        /// 获取观测周期分布
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, float> GetPeriodDistribution()
        {
            return PeriodComputer.GetDistribution(this);
        }

        /// <summary>
        /// 通过日期找到测值对象
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns></returns>
        public DateValue GetDateValue(DateTime date)
        {
            return this.Find(m => m.Date.Equals(date));
        }

        #region 静态方法
        /// <summary>
        /// 生成List of DateValue
        /// </summary>
        /// <param name="dateTimes">List of DateTimes</param>
        /// <param name="values">List of Value</param>
        /// <returns></returns>
        public static DateValueList FromArrays(List<DateTime> dateTimes, List<double> values)
        {
            if (dateTimes.Count != values.Count)
                throw new Exception("The length of x's and y's is not equal!");
            var ans = new DateValueList();
            for (int i = 0; i < dateTimes.Count; i++)
            {
                ans.Add(new DateValue(dateTimes[i], values[i]));
            }
            return ans;
        }

        /// <summary>
        /// 从测项编号获取原始数据
        /// </summary>
        /// <param name="mItemId"></param>
        /// <returns></returns>
        public static DateValueList FromRawData(int mItemId)
        {
            var sql = "select 观测日期,观测值 from {0} where 测项编号={1} order by 观测日期";
            sql = string.Format(sql, DbHelper.TnRData(), mItemId);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString(), sql).Tables[0];
            var ans = dt.RetrieveDateValues();
            Debug.Print("获取原始数据，测项编号:" + mItemId + "\n 返回 {0} 条数据", ans.Count);
            return ans;
        }
        #endregion
    }
}
