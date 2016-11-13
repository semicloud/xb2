using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.Algorithms.Core.Entity
{
    public class DateValues
    {

        /// <summary>
        /// 生成List of DateValue
        /// </summary>
        /// <param name="dateTimes">List of DateTimes</param>
        /// <param name="values">List of Value</param>
        /// <returns></returns>
        public static List<DateValue> FromArrays(List<DateTime> dateTimes, List<double> values)
        {
            if (dateTimes.Count != values.Count)
                throw new Exception("The length of x's and y's is not equal!");
            var ans = new List<DateValue>();
            for (int i = 0; i < dateTimes.Count; i++)
                ans.Add(new DateValue(dateTimes[i], values[i]));
            return ans;
        }

        /// <summary>
        /// 从测项编号获取原始数据
        /// </summary>
        /// <param name="mItemId"></param>
        /// <returns></returns>
        public static List<DateValue> FromRawData(int mItemId)
        {
            var sql = "select 观测日期,观测值 from {0} where 测项编号={1} order by 观测日期";
            sql = string.Format(sql, Db.TnRData(), mItemId);
            var dt = MySqlHelper.ExecuteDataset(Db.CStr(), sql).Tables[0];
            var ans = dt.RetrieveDateValues();
            Debug.Print("Retrieve date values from raw data, item id:" + mItemId + "\n return {0} values", ans.Count);
            return ans;
        }
    }
}
