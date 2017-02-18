using Xb2.Algorithms.Core.Entity;

namespace Xb2.Algorithms.Core.Methods.Correlation
{
    /// <summary>
    /// 跨断层流动性变资料处理软件 - 相关系数计算
    /// <remarks>未完成！！！</remarks>
    /// </summary>
    public class Xb2Correlation
    {
        private CorrelationInput _input;

        /// <summary>
        /// 未完成
        /// </summary>
        public Xb2Correlation()
        {
        }

        /// <summary>
        /// 未完成
        /// </summary>
        /// <param name="input"></param>
        public Xb2Correlation(CorrelationInput input)
        {
            _input = input;
        }

        /// <summary>
        /// 获得相关系数
        /// </summary>
        /// <returns></returns>
        public double GetCorrelation()
        {
            DateRange dateRange = new DateRange(_input.Start, _input.End);
            /**
            var coll1 = _input.Collection1.Between(dateRange);
            var coll2 = _input.Collection2.Between(dateRange);
            int period = coll1.GetPossiblePeriod();
            //TODO 这里和吴老师再确认一下,观测周期的格子怎么打
            var datetimes = DateRange.GetDateRangeStepByStep(_input.Start, _input.End, DateUnit.MONTH, period);
            
             */
            return 0.2;
        }
    }

    internal class ValuePair
    {
        public double Value1 { get; set; }
        public double Value2 { get; set; }
    }
}