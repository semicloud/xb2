// 解决方案名称：Xb2
// 工程名称：Xb2Core
// 文件名：M3.cs
// 作者：Semicloud
// 初次编写时间：2014-08-25
// 功能：

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xb2.Algorithms.Core.Entity;
using Xb2.Utils;

namespace Xb2.Algorithms.Core.Methods.FaultOffset
{
    public class DchdlM3Input : BaseInput
    {
        /// <summary>
        /// 基线1
        /// </summary>
        public List<DateValue> BaseLine1 { get; set; }

        /// <summary>
        /// 基线1 观测周期
        /// </summary>
        public int BaseLine1Period { get; set; }

        /// <summary>
        /// 基线2
        /// </summary>
        public List<DateValue> BaseLine2 { get; set; }

        /// <summary>
        /// 基线2 观测周期
        /// </summary>
        public int BaseLine2Period { get; set; }

        /// <summary>
        /// 水准
        /// </summary>
        public List<DateValue> Standard { get; set; }

        /// <summary>
        /// 水准 观测周期
        /// </summary>
        public int StandardPeriod { get; set; }

        /// <summary>
        /// Alpha1
        /// </summary>
        public double Alpha1 { get; set; }

        /// <summary>
        /// Alpha2
        /// </summary>
        public double Alpha2 { get; set; }
    }

    /// <summary>
    /// 断层活动量 模式3 双基线 单水准
    /// </summary>
    public class Xb2DCHDL_M3
    {
        private List<DateValue> _baseline1, _baseline2, _standard;
        private List<Window> _windows;
        private double _alpha1, _alpha2;

        /// <summary>
        /// 断层活动量 模式3 输入
        /// </summary>
        /// <param name="input"></param>
        public Xb2DCHDL_M3(DchdlM3Input input)
        {
            //速率差分函数
            Func<DateValue, DateValue, double> slcf = (m1, m2) => ((m2.Value - m1.Value)*365)/((m2.Date - m1.Date).Days);
            _baseline1 = QuShuDebug.GetAverageValues_20150720_v2(input.BaseLine1, input.Start, input.End, input.WLen, input.SLen, input.Delta, input.BaseLine1Period, slcf);
            _baseline2 = QuShuDebug.GetAverageValues_20150720_v2(input.BaseLine2, input.Start, input.End, input.WLen, input.SLen, input.Delta, input.BaseLine2Period, slcf);
            _standard = QuShuDebug.GetAverageValues_20150720_v2(input.Standard, input.Start, input.End, input.WLen, input.SLen, input.Delta, input.StandardPeriod, slcf);
            _windows = Window.GetWindows(input.Start.AddMonths(input.Delta), input.End, input.SLen, input.WLen);
            _alpha1 = input.Alpha1;
            _alpha2 = input.Alpha2;
        }

        /// <summary>
        /// 获得曲线ΔL1
        /// </summary>
        /// <returns>list of DateValue</returns>
        public List<DateValue> GetΔL1()
        {
            var answer = new List<DateValue>();
            foreach (var e in _baseline1)
            {
                answer.Add(new DateValue(e.Date, e.Value));
            }
            Debug.Print("ΔL1:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value.R4()));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// 获得曲线ΔL2
        /// </summary>
        /// <returns>List of DateValue</returns>
        public List<DateValue> GetΔL2()
        {
            var answer = new List<DateValue>();
            foreach (var e in _baseline2)
            {
                answer.Add(new DateValue(e.Date, e.Value));
            }
            Debug.Print("ΔL2:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value.R4()));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// 获得曲线ΔH
        /// </summary>
        /// <returns>List of DateValue</returns>
        public List<DateValue> GetΔH()
        {
            var answer = new List<DateValue>();
            foreach (var e in _standard)
            {
                answer.Add(new DateValue(e.Date, e.Value));
            }
            Debug.Print("ΔH:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value.R4()));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// 获得曲线ΔS
        /// </summary>
        /// <returns>List of DateValue</returns>
        public List<DateValue> GetΔS()
        {
            var answer = new List<DateValue>();
            foreach (var window in _windows)
            {
                var baseLine1DVP = _baseline1.Find(p => p.Date.Equals(window.Upper));
                var baseLine2DVP = _baseline2.Find(p => p.Date.Equals(window.Upper));
                if (baseLine1DVP == null || baseLine2DVP == null) continue;
                double b1 = baseLine1DVP.Value, b2 = baseLine2DVP.Value;
                double s = (b1*Math.Sin(_alpha2) - b2*Math.Sin(_alpha1))/Math.Sin(_alpha2 - _alpha1);
                answer.Add(new DateValue(window.Upper, s));
            }
            Debug.Print("ΔS:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value.R4()));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// 获得曲线ΔR
        /// </summary>
        /// <returns>List of DateValue</returns>
        public List<DateValue> GetΔR()
        {
            var answer = new List<DateValue>();
            foreach (var window in _windows)
            {
                var baseLine1DVP = _baseline1.Find(p => p.Date.Equals(window.Upper));
                var baseLine2DVP = _baseline2.Find(p => p.Date.Equals(window.Upper));
                if (baseLine1DVP == null || baseLine2DVP == null) continue;
                double b1 = baseLine1DVP.Value, b2 = baseLine2DVP.Value;
                double s = (b1*Math.Sin(_alpha2) - b2*Math.Sin(_alpha1))/Math.Sin(_alpha2 - _alpha1);
                double r = (b1 + s*Math.Cos(_alpha1))/Math.Sin(_alpha1);
                Debug.Print("l1:{0},l2:{1},s:{2},r:{3},a1:{4},a2:{5}", b1, b2, s, r, _alpha1, _alpha2);answer.Add(new DateValue(window.Upper, r));
            }
            Debug.Print("ΔR:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value.R4()));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// 获得断层活动协调比ΔR/ΔS线
        /// </summary>
        /// <returns>List of DateValue</returns>
        public List<DateValue> GetΔRΔS()
        {
            var answer = new List<DateValue>();
            foreach (var window in _windows)
            {
                var baseLine1DVP = _baseline1.Find(p => p.Date.Equals(window.Upper));
                var baseLine2DVP = _baseline2.Find(p => p.Date.Equals(window.Upper));
                if (baseLine1DVP == null || baseLine2DVP == null) continue;
                double b1 = baseLine1DVP.Value, b2 = baseLine2DVP.Value;
                double s = (b1*Math.Sin(_alpha2) - b2*Math.Sin(_alpha1))/Math.Sin(_alpha2 - _alpha1);
                double r = (b1 + s*Math.Cos(_alpha1))/Math.Sin(_alpha1);
                answer.Add(new DateValue(window.Upper, r / s));
            }
            Debug.Print("ΔRΔS:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value.R4()));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// 获得断层活动协调比ΔH/ΔS线
        /// </summary>
        /// <returns>List of DateValue</returns>
        public List<DateValue> GetΔHΔS()
        {
            var answer = new List<DateValue>();
            foreach (var window in _windows)
            {
                var baseLine1DVP = _baseline1.Find(p => p.Date.Equals(window.Upper));
                var baseLine2DVP = _baseline2.Find(p => p.Date.Equals(window.Upper));
                var standardDVP = _standard.Find(p => p.Date.Equals(window.Upper));
                if (baseLine1DVP == null || baseLine2DVP == null) continue;
                double b1 = baseLine1DVP.Value, b2 = baseLine2DVP.Value, h = standardDVP.Value;
                double s = (b1*Math.Sin(_alpha2) - b2*Math.Sin(_alpha1))/Math.Sin(_alpha2 - _alpha1);
                answer.Add(new DateValue(window.Upper, h / s));
            }
            Debug.Print("ΔHΔS:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value.R4()));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// 获得断层活动协调比ΔH/ΔR线
        /// </summary>
        /// <returns>List of DateValue</returns>
        public List<DateValue> GetΔHΔR()
        {
            var answer = new List<DateValue>();
            foreach (var window in _windows)
            {
                var baseLine1DVP = _baseline1.Find(p => p.Date.Equals(window.Upper));
                var baseLine2DVP = _baseline2.Find(p => p.Date.Equals(window.Upper));
                var standardDVP = _standard.Find(p => p.Date.Equals(window.Upper));
                if (baseLine1DVP == null || baseLine2DVP == null) continue;
                double b1 = baseLine1DVP.Value, b2 = baseLine2DVP.Value, h = standardDVP.Value;
                double s = (b1*Math.Sin(_alpha2) - b2*Math.Sin(_alpha1))/Math.Sin(_alpha2 - _alpha1);
                double r = (b1 + s*Math.Cos(_alpha1))/Math.Sin(_alpha1);
                answer.Add(new DateValue(window.Upper, h / r));
            }
            Debug.Print("ΔHΔR:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value.R4()));
            Debug.Print("----------------------------------");
            return answer;
        }
    }
}