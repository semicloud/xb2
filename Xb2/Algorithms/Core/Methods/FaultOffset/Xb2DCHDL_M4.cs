// 解决方案名称：Xb2
// 工程名称：Xb2Core
// 文件名：Xb2FaultOffsetM4.cs
// 作者：Semicloud
// 初次编写时间：2014-08-25
// 功能：

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xb2.Algorithms.Core.Entity;

namespace Xb2.Algorithms.Core.Methods.FaultOffset
{
    public class Xb2DCHDL_M4_Input : Xb2BaseInput
    {
        //水准1
        public List<DateValue> Standard1 { get; set; }

        // 水准1观测周期
        public int Standard1Period { get; set; }

        // 水准2
        public List<DateValue> Standard2 { get; set; }

        // 水准2观测周期
        public int Standard2Period { get; set; }

        // 基线1
        public List<DateValue> Baseline1 { get; set; }

        // 基线1观测周期
        public int BaseLine1Period { get; set; }

        // 基线2
        public List<DateValue> Baseline2 { get; set; }

        // 基线2观测周期
        public int BaseLine2Period { get; set; }

        // α1
        public double Alpha1 { get; set; }

        // α2
        public double Alpha2 { get; set; }
    }

    /// <summary>
    /// 断层活动量 模式4 双基线 双水准
    /// </summary>
    public class Xb2DCHDL_M4
    {
        private List<Window> _windows;
        private List<DateValue> _baseline1, _baseline2, _standard1, _standard2; 
        private double _alpha1, _alpha2;
        private int _delta;

        /// <summary>
        /// 断层活动量 模式4 构造函数
        /// </summary>
        /// <param name="input"></param>
        public Xb2DCHDL_M4(Xb2DCHDL_M4_Input input)
        {
            //速率差分函数
            Func<DateValue, DateValue, double> slcf = (m1, m2) => ((m2.Value - m1.Value) * 365) / ((m2.Date - m1.Date).Days);
            
            this._alpha1 = input.Alpha1;
            this._alpha2 = input.Alpha2;
            this._windows = Window.GetWindows(input.Start.AddMonths(input.Delta), input.End, input.SLen, input.WLen);
            this._baseline1 = QuShuDebug.GetAverageValues_20150720_v2(input.Baseline1, input.Start, input.End, input.WLen, input.SLen, input.Delta, input.BaseLine1Period, slcf);
            this._baseline2 = QuShuDebug.GetAverageValues_20150720_v2(input.Baseline2, input.Start, input.End, input.WLen, input.SLen, input.Delta, input.BaseLine2Period, slcf);
            this._standard1 = QuShuDebug.GetAverageValues_20150720_v2(input.Standard1, input.Start, input.End, input.WLen, input.SLen, input.Delta, input.Standard1Period, slcf);
            this._standard2 = QuShuDebug.GetAverageValues_20150720_v2(input.Standard2, input.Start, input.End, input.WLen, input.SLen, input.Delta, input.Standard2Period, slcf);
            _delta = input.Delta;
        }

        /// <summary>
        /// ΔL1线
        /// </summary>
        /// <returns></returns>
        public List<DateValue> GetΔL1()
        {
            var answer = new List<DateValue>();
            foreach (var e in _baseline1)
            {
                answer.Add(new DateValue(e.Date, e.Value));
            }
            Debug.Print("ΔL1:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// ΔL2线
        /// </summary>
        /// <returns></returns>
        public List<DateValue> GetΔL2()
        {
            var answer = new List<DateValue>();
            foreach (var e in _baseline2)
            {
                answer.Add(new DateValue(e.Date, e.Value));
            }
            Debug.Print("ΔL2:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");
            return answer;
        }

        public List<DateValue> GetΔH()
        {
            var answer = new List<DateValue>();
            foreach (var window in _windows)
            {
                var standard1DVP = _standard1.Find(p => p.Date.Equals(window.Upper));
                var standard2DVP = _standard2.Find(p => p.Date.Equals(window.Upper));
                if(standard1DVP == null || standard2DVP == null) continue;
                answer.Add(new DateValue(window.Upper, (standard1DVP.Value + standard2DVP.Value) / 2.0));
            }
            Debug.Print("ΔH:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// ΔH1线
        /// </summary>
        /// <returns></returns>
        public List<DateValue> GetΔH1()
        {
            var answer = new List<DateValue>();
            foreach (var e in _standard1)
            {
                answer.Add(new DateValue(e.Date, e.Value));
            }
            Debug.Print("ΔH1:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// ΔH2线
        /// </summary>
        /// <returns></returns>
        public List<DateValue> GetΔH2()
        {
            var answer = new List<DateValue>();
            foreach (var e in _standard2)
            {
                answer.Add(new DateValue(e.Date, e.Value));
            }
            Debug.Print("ΔH2:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// ΔS线
        /// </summary>
        /// <returns></returns>
        public List<DateValue> GetΔS()
        {
            var answer = new List<DateValue>();
            foreach (var window in _windows)
            {
                var baseLine1DVP = _baseline1.Find(p => p.Date.Equals(window.Upper));
                var baseLine2DVP = _baseline2.Find(p => p.Date.Equals(window.Upper));
                if (baseLine1DVP == null || baseLine2DVP == null) continue;
                double b1 = baseLine1DVP.Value, b2 = baseLine2DVP.Value;
                answer.Add(new DateValue(window.Upper, getDeltaS(b1, b2, _alpha1, _alpha2)));
            }
            Debug.Print("ΔS:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");
            return answer;
        }

        private static Func<double, double, double, double, double> getDeltaS =
            (l1, l2, a1, a2) => (l1*Math.Sin(a2) - l2*Math.Sin(a1))/(Math.Sin(a2 - a1));

        /// <summary>
        /// ΔR线
        /// </summary>
        /// <returns></returns>
        public List<DateValue> GetΔR()
        {
            var answer = new List<DateValue>();
            foreach (var window in _windows)
            {
                var baseLine1DVP = _baseline1.Find(p => p.Date.Equals(window.Upper));
                var baseLine2DVP = _baseline2.Find(p => p.Date.Equals(window.Upper));
                if (baseLine1DVP == null || baseLine2DVP == null) continue;
                double b1 = baseLine1DVP.Value, b2 = baseLine2DVP.Value;
                double ds = getDeltaS(b1, b2, _alpha1, _alpha2);
                double dr = getDeltaR(b1, ds, _alpha1);
                answer.Add(new DateValue(window.Upper, dr));
            }
            Debug.Print("ΔR:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");
            return answer;
        }

        private static Func<double, double, double, double> getDeltaR =
            (l1, ds, a1) => (l1 + ds*Math.Cos(a1))/Math.Sin(a1);

        /// <summary>
        /// 断层活动协调比ΔR/ΔS线
        /// </summary>
        /// <returns></returns>
        public List<DateValue> GetΔRΔS()
        {
            var answer = new List<DateValue>();
            foreach (var window in _windows)
            {
                var baseLine1DVP = _baseline1.Find(p => p.Date.Equals(window.Upper));
                var baseLine2DVP = _baseline2.Find(p => p.Date.Equals(window.Upper));
                if (baseLine1DVP == null || baseLine2DVP == null) continue;
                double b1 = baseLine1DVP.Value, b2 = baseLine2DVP.Value;
                double ds = getDeltaS(b1, b2, _alpha1, _alpha2);
                double dr = getDeltaR(b1, ds, _alpha1);
                answer.Add(new DateValue(window.Upper, dr / ds));
            }
            Debug.Print("ΔRΔS:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// 断层活动协调比ΔH/ΔS线
        /// </summary>
        /// <returns></returns>
        public List<DateValue> GetΔHΔS()
        {
            var answer = new List<DateValue>();
            foreach (var window in _windows)
            {
                var baseLine1DVP = _baseline1.Find(p => p.Date.Equals(window.Upper));
                var baseLine2DVP = _baseline2.Find(p => p.Date.Equals(window.Upper));
                var standard1DVP = _standard1.Find(p => p.Date.Equals(window.Upper));
                var standard2DVP = _standard2.Find(p => p.Date.Equals(window.Upper));
                if (baseLine1DVP == null || baseLine2DVP == null || standard1DVP == null || standard2DVP == null)
                    continue;
                double b1 = baseLine1DVP.Value;
                double b2 = baseLine2DVP.Value;
                double s1 = standard1DVP.Value;
                double s2 = standard2DVP.Value;
                double h = (s1 + s2)/2.0d;
                double ds = getDeltaS(b1, b2, _alpha1, _alpha2);
                answer.Add(new DateValue(window.Upper, h / ds));
            }
            Debug.Print("ΔHΔS:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// 断层活动协调比ΔH/ΔR线
        /// </summary>
        /// <returns></returns>
        public List<DateValue> GetΔHΔR()
        {
            var answer = new List<DateValue>();
            foreach (var window in _windows)
            {
                var baseLine1DVP = _baseline1.Find(p => p.Date.Equals(window.Upper));
                var baseLine2DVP = _baseline2.Find(p => p.Date.Equals(window.Upper));
                var standard1DVP = _standard1.Find(p => p.Date.Equals(window.Upper));
                var standard2DVP = _standard2.Find(p => p.Date.Equals(window.Upper));
                if (baseLine1DVP == null || baseLine2DVP == null || standard1DVP == null || standard2DVP == null)
                    continue;
                double b1 = baseLine1DVP.Value;
                double b2 = baseLine2DVP.Value;
                double s1 = standard1DVP.Value;
                double s2 = standard2DVP.Value;
                double h = (s1 + s2)/2.0d;
                double ds = getDeltaS(b1, b2, _alpha1, _alpha2);
                double dr = getDeltaR(b1, ds, _alpha1);
                answer.Add(new DateValue(window.Upper, h / dr));
            }
            Debug.Print("ΔHΔR:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");
            return answer;
        }
    }
}