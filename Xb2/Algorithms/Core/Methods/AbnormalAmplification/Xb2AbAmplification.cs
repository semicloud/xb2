// 解决方案名称：XbApp
// 工程名称：XbApp
// 文件名：Xb2AbAmplification.cs
// 作者：Semicloud
// 初次编写时间：2014-11-17
// 功能：

using System;
using System.Collections.Generic;
using System.Linq;
using Xb2.Algorithms.Core.Entity;

namespace Xb2.Algorithms.Core.Methods.AbnormalAmplification
{
    /// <summary>
    /// 异常放大算法
    /// </summary>
    public class Xb2AbAmplification
    {
        private Xb2AbAmplificationInput _input;

        /// <summary>
        /// 异常放大的输入
        /// </summary>
        /// <param name="input"></param>
        public Xb2AbAmplification(Xb2AbAmplificationInput input)
        {
            this._input = input;
        }

        private static double getYj(DateValueList collection, int abnormalTrend)
        {
            if (abnormalTrend == 1)
                return collection.Select(c => c.Value).ToList().Min();
            if (abnormalTrend == -1)
                return collection.Select(c => c.Value).ToList().Max();
            throw new Exception("not supported abnormalTrend " + abnormalTrend);
        }

        private static List<DateValueList> prepareCollectionList(List<DateValueList> srcList, DateTime start,
            DateTime end)
        {
            var preparedList = new List<DateValueList>();
            var range = new DateRange(start, end);
            srcList.ForEach(collection => preparedList.Add(collection.Between(range)));
            Action<DateValueList> action = collection =>
            {
                var yj = getYj(collection, collection.AbnormalTrend);
                collection.ForEach(aVal => aVal.Value = aVal.Value - yj);
            };
            preparedList.ForEach(action);
            return preparedList;
        }

//        [Test]
//        public void m()
//        {
//            MItemObj m = MItemObj.GetInstanceByID(1);
//            m.InitialDatabase.DataCollection.AbnormalTrend = 1;
//            m.InitialDatabase.DataCollection.Weight = 0.8;
//            var list = new List<MValueCollection>() {m.InitialDatabase.DataCollection};
//            Xb2AbAmplificationInput input = new Xb2AbAmplificationInput();
//            input.CollectionList = new List<MValueCollection>() {m.InitialDatabase.DataCollection};
//            input.Period = 1;
//            input.Start = new DateTime(1991, 1, 1);
//            input.End = new DateTime(2008, 12, 31);
//            Xb2AbAmplification amplification = new Xb2AbAmplification(input);
//
//            amplification.GetZiLineData().ForEach(Console.WriteLine);
//            Assert.True(true);
//        }

        //求测值集合的平均值，然后将此平均值乘以权重
        private Func<DateValueList, double> product =
            collection => collection.Average(c => c.Value)*collection.Weight;

        /// <summary>
        /// 获取Zi值曲线
        /// </summary>
        /// <returns></returns>
        public List<DateValue> GetZiLineData()
        {
            var dateValue = new List<DateValue>();

            //准备计算数据：按开始日期、结束日期截取，计算yj，减去yj
            var collectionList = prepareCollectionList(this._input.CollectionList, this._input.Start, this._input.End);
            var start = this._input.Start;
            var end = this._input.End;
            var period = this._input.Period;

            while (start <= end)
            {
                //在这个日期范围中，所有序列包括的子序列，组成的序列
                var inThisRange = new List<DateValueList>();
                var pointer = start.AddMonths(period);
                //找出每一个测值序列中，包括在该日期范围中的子序列
                foreach (DateValueList collection in collectionList)
                {
                    //日期范围
                    var range = new DateRange(start, pointer);
                    //子序列
                    var subCollection = collection.Between(range);
                    //如果子序列中的测值数不为0，加入集合
                    if (subCollection.Count > 0)
                        inThisRange.Add(subCollection);
                }
                //求每一个测值子序列中测值的平均值，乘以该测值序列的权重，得到一个double的数组
                var doubles = from coll in inThisRange select coll.Average(c => c.Value)*coll.Weight;
                //累乘double数组中的值
                if (doubles.Any())
                    dateValue.Add(new DateValue(pointer, doubles.Aggregate((x, y) => x*y)));
                //指针后移
                start = start.AddMonths(period);
            }
            return dateValue;
        }
    }
}