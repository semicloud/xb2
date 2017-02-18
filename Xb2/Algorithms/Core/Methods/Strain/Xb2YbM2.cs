// 解决方案名称：XbApp
// 工程名称：XbApp
// 文件名：Xb2YbM2.cs
// 作者：Semicloud
// 初次编写时间：2016-04-11
// 功能：

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xb2.Algorithms.Core.Entity;
using Xb2.Utils;

namespace Xb2.Algorithms.Core.Methods.Strain
{
    public class YbM2Input : BaseInput
    {
        public List<DateValue> BaseLine1 { get; set; }
        public List<DateValue> BaseLine2 { get; set; }
        public List<DateValue> BaseLine3 { get; set; }
        public double AngT { get; set; }
    }

    public class Xb2YbM2 : YbBase
    {
        public YbM2Input Input { get; set; }
        private List<YingBianOutput> m_outputs;

        public Xb2YbM2(YbM2Input input)
        {
            this.Input = input;
            this.m_outputs = this.GetYingBianM2Outputs();
        }

        private List<Window> getWindows()
        {
            var windows = Window.GetWindows(this.Input.Start, this.Input.End, this.Input.SLen, this.Input.WLen);
            Debug.Print("共生成{0}个窗口", windows.Count);
            var date = this.Input.BaseLine1.First().Date;
            Debug.Print("测项的第1个观测日期为：{0}", date.ToShortDateString());
            //把前面没数的窗口删除
            foreach (var window in windows)
            {
                if (this.Input.BaseLine1.Between(window.Lower, window.Upper).Count == 0)
                    windows.Remove(window);
                else
                    break;
            }
            Debug.Print("筛选后还剩下{0}个窗口", windows.Count);
            return windows;
        }

        public List<YingBianOutput> GetYingBianM2Outputs()
        {
            var ans = new List<YingBianOutput>();
            List<Window> windows = getWindows();
            var a0 = this.Input.BaseLine1.Between(windows.First()).Select(m => m.Value).Average();
            var b0 = this.Input.BaseLine2.Between(windows.First()).Select(m => m.Value).Average();
            var c0 = this.Input.BaseLine3.Between(windows.First()).Select(m => m.Value).Average();
            Debug.Print("a0:{0},b0:{1},c0:{2}", a0, b0, c0);
            foreach (var window in windows)
            {
                var acoll = this.Input.BaseLine1.Between(window);
                var bcoll = this.Input.BaseLine2.Between(window);
                var ccoll = this.Input.BaseLine3.Between(window);
                if (acoll.Count == 0 || bcoll.Count == 0 || ccoll.Count == 0)
                {
                    continue;
                }
                var a = this.Input.BaseLine1.Between(window).Select(m => m.Value).Average();
                var b = this.Input.BaseLine2.Between(window).Select(m => m.Value).Average();
                var c = this.Input.BaseLine3.Between(window).Select(m => m.Value).Average();
                var angA = Math.Acos((b*b + c*c - a*a)/(2*b*c));
                var angB = Math.Acos((a*a + c*c - b*b)/(2*a*c));
                var angC = Math.Acos((a*a + b*b - c*c)/(2*a*b));
                Debug.Print("a:{0},b:{1},c:{2},angA:{3},angB:{4},angC:{5}", a, b, c, angA, angB, angC);

                double epa = (a - a0)/a0, epb = (b - b0)/b0, epc = (c - c0)/c0;
                Debug.Print("epa:{0},epb:{1},epc:{2}", epa, epb, epc);

                //Δ用d表示
                double d = Delta(angA, angB, angC); //Δ
                double d1 = Delta1(angA, angB, angC, epa, epb, epc); //Δ1
                double d2 = Delta2(angA, angB, angC, this.Input.AngT.ToRad(), epa, epb, epc); //Δ2
                double d3 = Delta3(angA, angB, angC, this.Input.AngT.ToRad(), epa, epb, epc); //Δ3
                //double epx = (d1 + d2)/2; //εx 
                double epx = (d1 + d2)/d;

                //该窗口内的计算结果
                double epsilon1 = d1/d + Math.Sqrt(Math.Pow(d3/d, 2) + Math.Pow(d2/d, 2)); //线应变ε1
                double epsilon2 = d1/d - Math.Sqrt(Math.Pow(d3/d, 2) + Math.Pow(d2/d, 2)); //线应变ε2
                double garmmaXy = 2*d3/d; //剪应变γxy
                double delta = 2*d1/d; //面膨胀Δ，此Δ与上面中间变量Δ不一致，注意！
                double phi = Math.Atan(2*(epsilon1 - epx)/garmmaXy).ToDeg(); //夹角
                if (phi < 0) phi = 180 + phi;
                var output = new YingBianOutput();
                output.Date = window.Upper;
                output.Epsilon1 = epsilon1;
                output.Epsilon2 = epsilon2;
                output.GarmmaXy = garmmaXy;
                output.Delta = delta;
                output.Phi = phi;
                ans.Add(output);
            }
            return ans;
        }

        public List<DateValue> GetEpsilog1()
        {
            var ret = new List<DateValue>();
            this.m_outputs.ForEach(o => ret.Add(new DateValue(o.Date, o.Epsilon1)));
            return ret;
        }

        public List<DateValue> GetEpsilog2()
        {
            var ret = new List<DateValue>();
            this.m_outputs.ForEach(o => ret.Add(new DateValue(o.Date, o.Epsilon2)));
            return ret;
        }

        public List<DateValue> GetDelta()
        {
            var ret = new List<DateValue>();
            this.m_outputs.ForEach(o => ret.Add(new DateValue(o.Date, o.Delta)));
            return ret;
        }

        public List<DateValue> GetGammeXY()
        {
            var ret = new List<DateValue>();
            this.m_outputs.ForEach(o => ret.Add(new DateValue(o.Date, o.GarmmaXy)));
            return ret;
        }

        public List<DateValue> GetPhi()
        {
            var ret = new List<DateValue>();
            this.m_outputs.ForEach(o => ret.Add(new DateValue(o.Date, o.Phi)));
            ret.RemoveAt(0);
            return ret;
        }
    }
}