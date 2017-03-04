// 解决方案名称：Xb2
// 工程名称：Xb2Core
// 文件名：M1.cs
// 作者：Semicloud
// 初次编写时间：2014-08-25
// 功能：断层活动量

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xb2.Algorithms.Core.Entity;
using Xb2.Algorithms.Core.Input;
using Xb2.Entity.Computing;
using Xb2.Utils;

namespace Xb2.Algorithms.Core.Methods.FaultOffset
{
    public class DchdlM1Input : BaseInput
    {
        //基线
        public List<DateValue> BaseLine { get; set; }

        /// <summary>
        /// 基线数据的观测周期
        /// </summary>
        public int BaseLinePeriod { get; set; }

        //水准
        public List<DateValue> Standard { get; set; }

        /// <summary>
        /// 水准数据的观测周期
        /// </summary>
        public int StandardPeriod { get; set; }

        public double Alpha { get; set; }
        public double Beta { get; set; }
    }

    /// <summary>
    /// 断层活动量 模式1
    /// </summary>
    public class Xb2DCHDL_M1
    {
        //基线数据和水准数据
        private List<DateValue> _baseline, _standard;
        //窗口集合
        private List<Window> _windows;
        //参数
        private double _alpha, _beta;

        /// <summary>
        /// 断层活动量 模式1 构造函数
        /// </summary>
        /// <param name="input"></param>
        public Xb2DCHDL_M1(DchdlM1Input input)
        {
            Func<DateValue, DateValue, double> slcf = (m1, m2) => ((m2.Value - m1.Value)*365)/((m2.Date - m1.Date).Days);
            _baseline = QuShuDebug.GetAverageValues_20150720_v2(input.BaseLine, input.Start, input.End, input.WLen, input.SLen,
                input.Delta, input.BaseLinePeriod, slcf);
            _standard = QuShuDebug.GetAverageValues_20150720_v2(input.Standard, input.Start, input.End, input.WLen, input.SLen,
                input.Delta, input.StandardPeriod, slcf);
            _windows = Window.GetWindows(input.Start.AddMonths(input.Delta), input.End, input.SLen, input.WLen);
            _alpha = input.Alpha;
            _beta = input.Beta;
        }

        public Xb2DCHDL_M1(List<XbDCHDLM1Input> inputs)
        {
            var baseline = inputs.Find(i => i.ItemStr.Contains("基线"));
            var standard = inputs.Find(i => i.ItemStr.Contains("水准"));
            _alpha = inputs[0].Alpha;
            _beta = inputs[0].Beta;
            int wlen = inputs[0].WLen;
            int slen = inputs[0].SLen;
            int delta = inputs[0].Delta;
            var start = inputs[0].DateStart;
            var end = inputs[0].DateEnd;
            Func<DateValue, DateValue, double> slcf =
                (m1, m2) => ((m2.Value - m1.Value) * 365) / ((m2.Date - m1.Date).Days);
            _baseline = QuShuDebug.GetAverageValues_20150720_v2(baseline.FormattedValueList, start, end, wlen, slen,
                delta, baseline.Freq, slcf);
            _standard = QuShuDebug.GetAverageValues_20150720_v2(standard.FormattedValueList, start, end, wlen, slen,
                delta, standard.Freq, slcf);
            _windows = Window.GetWindows(start.AddMonths(delta), end, slen, wlen);
        }

        public CalcResult GetL()
        {
            return new CalcResult() {Title = "ΔL", NumericalTable = GetΔLLine().ToDateValueList().ToDataTable()};
        }

        public CalcResult GetH()
        {
            return new CalcResult() {Title = "ΔH", NumericalTable = GetΔHLine().ToDateValueList().ToDataTable()};
        }
        public CalcResult GetS()
        {
            return new CalcResult() { Title = "ΔS", NumericalTable = GetΔSLine().ToDateValueList().ToDataTable() };
        }

        public CalcResult GetHS()
        {
            return new CalcResult() {Title = "ΔHΔS", NumericalTable = GetΔHΔSLine().ToDateValueList().ToDataTable()};
        }

        /// <summary>
        /// 获得ΔL线，即基线数据
        /// </summary>
        /// <returns>List of DateValue</returns>
        public List<DateValue> GetΔLLine()
        {
            var answer = new List<DateValue>();
            foreach (var e in _baseline)
            {
                if (!Double.IsNaN(e.Value))
                {
                    answer.Add(new DateValue(e.Date, e.Value));
                }
            }
            Debug.Print("ΔL1:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");

            return answer;
        }

        /// <summary>
        /// 获得ΔH线，即水准数据
        /// </summary>
        /// <returns>List of DateValue</returns>
        public List<DateValue> GetΔHLine()
        {
            var answer = new List<DateValue>();
            foreach (var e in _standard)
            {
                if (!Double.IsNaN(e.Value))
                {
                    answer.Add(new DateValue(e.Date, e.Value));
                }
            }
            Debug.Print("Delta H(水准) Line Data:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// 获得ΔS线数据
        /// </summary>
        /// <returns>List of DateValue</returns>
        public List<DateValue> GetΔSLine()
        {
            var answer = new List<DateValue>();
            foreach (var window in _windows)
            {
                var baseLineDVP = _baseline.Find(p => p.Date.Equals(window.Upper));
                var standardLineDVP = _standard.Find(p => p.Date.Equals(window.Upper));
                if (baseLineDVP == null || standardLineDVP == null || Double.IsNaN(baseLineDVP.Value) ||
                    Double.IsNaN(standardLineDVP.Value))
                {
                    Debug.Print("{0},水准或基线缺数，无法计算S线数据", window.Upper.ToShortDateString());
                    continue;
                }
                var ΔL = Math.Round(baseLineDVP.Value, 3);
                var ΔH = Math.Round(standardLineDVP.Value, 3);
                double ΔS = Math.Round(ΔL/Math.Cos(_alpha) + ΔH*Math.Tan(_alpha)/Math.Tan(_beta), 3);
                Debug.Print("ΔL={0}, ΔH={1}, ΔS={2}", ΔL, ΔH, ΔS);
                answer.Add(new DateValue(window.Upper, value: ΔS));
            }
            Debug.Print("Delta S Line Data:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// 断层活动协调比ΔHΔS线数据
        /// </summary>
        /// <returns>List of DateValue</returns>
        public List<DateValue> GetΔHΔSLine()
        {
            var answer = new List<DateValue>();
            foreach (var window in _windows)
            {
                var baseLineDVP = _baseline.Find(p => p.Date.Equals(window.Upper));
                var standardLineDVP = _standard.Find(p => p.Date.Equals(window.Upper));
                if (baseLineDVP == null || standardLineDVP == null || Double.IsNaN(baseLineDVP.Value) ||
                    Double.IsNaN(standardLineDVP.Value))
                {
                    Debug.Print("{0},水准或基线缺数，无法计算H/S线(断层活动协调比)数据", window.Upper.ToShortDateString());
                    continue;
                }
                var delta_L = baseLineDVP.Value.R4();
                var delta_H = standardLineDVP.Value.R4();
                double delta_S = delta_L/Math.Cos(_alpha) + delta_H*Math.Tan(_alpha)/Math.Tan(_beta);
                delta_S = delta_S.R4();
                Debug.WriteLine("{0}, s={1}, h={2}, h/s={3}", window.Upper.ToShortDateString(), delta_S, delta_H, delta_H/delta_S);
                answer.Add(new DateValue(window.Upper, value: (delta_H/delta_S).R4()));
            }
            Debug.Print("Delta H/S Line (断层活动协调比) Data:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");

            return answer;
        }
    }
}