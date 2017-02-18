using System;
using System.Collections.Generic;
using System.Linq;
using Xb2.Algorithms.Core.Entity;
using Xb2.Algorithms.Core.Input;
using Xb2.GUI.Catalog;
using Xb2.Utils;

namespace Xb2.Algorithms.Core.Methods.Rate
{
    /// <summary>
    /// 单测项数据差分
    /// </summary>
    public class Xb2Slcf
    {
        private SlcfInput _input;

        public Xb2Slcf(SlcfInput input)
        {
            _input = input;
        }

        public CalcResult GetSlcfLine()
        {
            return new CalcResult()
            {
                Title = _input.ItemStr + " 速率差分",
                NumericalTable = GetOutputs().ToDataTable()
            };
        }

        public DateValueList GetOutputs()
        {
            var answer = new DateValueList();
            Func<DateValue, DateValue, double> slcf = (m1, m2) => ((m2.Value - m1.Value)*365)/((m2.Date - m1.Date).Days);
            var dvps = new List<DateValue>();
            this._input.InputData.ForEach(c => dvps.Add(new DateValue(c.Date, c.Value)));
            var winNumbers = QuShuDebug.GetAverageValues_20150720_v2(dvps, _input.Start, _input.End, _input.WLen, _input.SLen, _input.Delta, _input.Freq, slcf);
            Func<List<Double>, double> average = list => list.Count > 0 ? list.Average() : double.NaN;
            winNumbers.ForEach(s => answer.Add(new DateValue(s.Date, s.Value)));
            answer.Sort((o1, o2) => DateTime.Compare(o1.Date, o2.Date));
            return answer;
        }
    }
}
