using System;
using System.Collections.Generic;
using System.Linq;
using Xb2.Algorithms.Core.Entity;

namespace Xb2.Algorithms.Core.Methods.Rate
{
    /// <summary>
    /// 单测项数据差分
    /// </summary>
    public class Xb2SLCF
    {
        public Xb2SLCFInput Input { get; set; }

        public Xb2SLCF(Xb2SLCFInput input)
        {
            this.Input = input;
        }

        public List<Xb2SLCFOutput> GetOutputs()
        {
            var answer = new List<Xb2SLCFOutput>();
            Func<DateValue, DateValue, double> slcf = (m1, m2) => ((m2.Value - m1.Value)*365)/((m2.Date - m1.Date).Days);
            var dvps = new List<DateValue>();
            this.Input.MeasureValues.ForEach(c => dvps.Add(new DateValue(c.Date, c.Value)));
            var winNumbers = QuShuDebug.GetAverageValues_20150720_v2(dvps, this.Input.Start, this.Input.End, this.Input.WLen, this.Input.SLen, this.Input.Delta, this.Input.Period, slcf);
            Func<List<Double>, double> average = list => list.Count > 0 ? list.Average() : double.NaN;
            winNumbers.ForEach(s => answer.Add(new Xb2SLCFOutput(s.Date, s.Value)));
            answer.Sort((o1, o2) => DateTime.Compare(o1.Date, o2.Date));
            return answer;
        }
    }

    public class Xb2SLCFOutput
    {
        public DateTime Date { get; set; }
        public Double Number { get; set; }

        public Xb2SLCFOutput()
        {

        }

        public Xb2SLCFOutput(DateTime date, double number)
        {
            this.Date = date;
            this.Number = number;
        }
    }

    //差分输入
    public class Xb2SLCFInput : Xb2BaseInput
    {
        /// <summary>
        /// 测值序列
        /// </summary>
        public DateValueList MeasureValues { get; set; }

        /// <summary>
        /// 观测周期
        /// </summary>
        public Int32 Period { get; set; }
    }

}
