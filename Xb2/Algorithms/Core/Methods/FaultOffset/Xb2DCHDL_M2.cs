// 解决方案名称：Xb2
// 工程名称：Xb2Core
// 文件名：M2.cs
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
    public class Xb2DCHDL_M2_Input : Xb2BaseInput
    {
        /// <summary>
        /// 水准1
        /// </summary>
        public List<DateValue> BaseLine1 { get; set; }

        /// <summary>
        /// 水准2
        /// </summary>
        public List<DateValue> BaseLine2 { get; set; }

        /// <summary>
        /// 水准1观测周期
        /// </summary>
        public int BaseLine1Period { get; set; }

        /// <summary>
        /// 水准2观测周期
        /// </summary>
        public int BaseLine2Period { get; set; }

        public double Alpha1 { get; set; }
        public double Alpha2 { get; set; }
    }

    /// <summary>
    /// 断层活动量 模式2 双基线 无水准
    /// </summary>
    public class Xb2DCHDL_M2
    {
       private List<DateValue> _baseline1, _baseline2;
        private List<Window> _windows;
        private double _alpha1, _alpha2;

        /// <summary>
        /// 断层活动量 模式2 构造函数
        /// </summary>
        /// <param name="input"></param>
        public Xb2DCHDL_M2(Xb2DCHDL_M2_Input input)
        {
            Func<DateValue, DateValue, double> slcf = (m1, m2) => ((m2.Value - m1.Value) * 365) / ((m2.Date - m1.Date).Days);
            _baseline1 = QuShuDebug.GetAverageValues_20150720_v2(input.BaseLine1, input.Start, input.End, input.WLen, input.SLen,
                input.Delta, input.BaseLine1Period,slcf);
            _baseline2 = QuShuDebug.GetAverageValues_20150720_v2(input.BaseLine2, input.Start, input.End, input.WLen, input.SLen,
                input.Delta, input.BaseLine2Period, slcf);
            // be careful, 这里算窗口必须加上一个时间间隔
            _windows = Window.GetWindows(input.Start.AddMonths(input.Delta), input.End, input.SLen, input.WLen);
            _alpha1 = input.Alpha1;
            _alpha2 = input.Alpha2;
        }

        /// <summary>
        /// 获得ΔL1线，即水平线数据
        /// </summary>
        /// <returns>List of DateValue</returns>
        public List<DateValue> GetΔL1()
        {
            var answer = new List<DateValue>();
            foreach (var e in _baseline1)
            {
                answer.Add(new DateValue(e.Date, e.Value.R4()));
            }
            Debug.Print("ΔL1:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// 获得ΔL2线，即水平线数据
        /// </summary>
        /// <returns>List of DateValue</returns>
        public List<DateValue> GetΔL2()
        {
            var answer = new List<DateValue>();
            foreach (var e in _baseline2)
            {
                answer.Add(new DateValue(e.Date, e.Value.R4()));
            }
            Debug.Print("ΔL2:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// 获得ΔS线数据
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
                var delta_l1 = baseLine1DVP.Value;
                var delta_l2 = baseLine2DVP.Value;
                double delta_s = (delta_l1*Math.Sin(_alpha2) - delta_l2*Math.Sin(_alpha1))/Math.Sin(_alpha2 - _alpha1);
                Debug.Print("l1:{0},l2:{1},a1:{2},a2{3},s={4}", delta_l1, delta_l2, _alpha1, _alpha2, delta_s.R4()); answer.Add(new DateValue(window.Upper, delta_s.R4()));
            }
            Debug.Print("ΔS:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// 获得ΔR线数据 </summary>
        /// <returns>List of DateValue</returns>
        public List<DateValue> GetΔR()
        {
            var answer = new List<DateValue>();
            foreach (var window in _windows)
            {
                var baseLine1DVP = _baseline1.Find(p => p.Date.Equals(window.Upper));
                var baseLine2DVP = _baseline2.Find(p => p.Date.Equals(window.Upper));
                if (baseLine1DVP == null || baseLine2DVP == null) continue;
                var delta_l1 = baseLine1DVP.Value;
                var delta_l2 = baseLine2DVP.Value;
                //double Δl1 = baseLine1DVP.Average(), Δl2 = baseLine2DVP.Average();
                double delta_s = (delta_l1*Math.Sin(_alpha2) - delta_l2*Math.Sin(_alpha1))/Math.Sin(_alpha2 - _alpha1);
                double delta_r = (delta_l1 + delta_s*Math.Cos(_alpha1))/Math.Sin(_alpha1);
                answer.Add(new DateValue(window.Upper, delta_r.R4()));
            }
            Debug.Print("ΔR:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");
            return answer;
        }

        /// <summary>
        /// 获得断层活动协调比ΔRΔS线数据
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
                var delta_l1 = baseLine1DVP.Value;
                var delta_l2 = baseLine2DVP.Value;
                //double Δl1 = baseLine1DVP.Average(), Δl2 = baseLine2DVP.Average();
                double delta_s = (delta_l1 * Math.Sin(_alpha2) - delta_l2 * Math.Sin(_alpha1)) / Math.Sin(_alpha2 - _alpha1);
                double delta_r = (delta_l1 + delta_s * Math.Cos(_alpha1)) / Math.Sin(_alpha1);
                answer.Add(new DateValue(window.Upper, (delta_r / delta_s).R4()));
            }
            Debug.Print("ΔR/ΔS:");
            answer.ForEach(d => Debug.Print("{0},{1}", d.Date.ToShortDateString(), d.Value));
            Debug.Print("----------------------------------");
            return answer;
        }
    }
}