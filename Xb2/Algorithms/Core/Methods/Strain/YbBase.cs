using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xb2.Algorithms.Core.Entity;
using Xb2.Utils;

namespace Xb2.Algorithms.Core.Methods.Strain
{
    public class YingBianOutput
    {
        public DateTime Date { get; set; }
        public double Epsilon1 { get; set; }
        public double Epsilon2 { get; set; }
        public double GarmmaXy { get; set; }
        public double Delta { get; set; }
        public double Phi { get; set; }
    }

    public abstract class YbBase
    {
        //生成窗口
        protected static List<Window> getFilteredWindows(DateTime start, DateTime end, Int32 slen, Int32 wlen, List<DateValue> list1)
        {
            var windows = Window.GetWindows(start, end, slen, wlen);
            Debug.Print("共生成{0}个窗口", windows.Count);
            var date = list1.First().Date;
            Debug.Print("测项的第1个观测日期为：{0}", date.ToShortDateString());
            //把前面没数的窗口删除
            for (int i = 0; i < windows.Count; i++)
            {
                if (list1.Between(windows[i].Lower, windows[i].Upper).Count == 0)
                {
                    windows.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }
//            foreach (var window in windows)
//            {
//                if (list1.Between(window.Lower, window.Upper).Count == 0)
//                    windows.Remove(window);
//                else
//                    break;
//            }
            Debug.Print("筛选后还剩下{0}个窗口", windows.Count);
            return windows;
        }

        protected static double COSTherom(double a, double b, double angC)
        {
            return Math.Sqrt(a * a + b * b - 2 * a * b * Math.Cos(angC.ToRad()));
        }

        //获取应变结果的第一种方法：两边加一角求第三边
        protected static List<YingBianOutput> GetYingBianOutputsMethod1(List<DateValue> list1, List<DateValue> list2, double angC, double angT, List<Window> windows)
        {
            var ans = new List<YingBianOutput>();
            //这里可能缺数
            var a0 = list1.Between(windows.First()).Select(m => m.Value).Average();
            var b0 = list2.Between(windows.First()).Select(m => m.Value).Average();
            var c0 = COSTherom(a0, b0, angC);
            Debug.Print("a0:{0},b0:{1},c0:{2}", a0, b0, c0);
            foreach (var window in windows)
            {
                var a = list1.Between(window).Select(z => z.Value).Average();
                var b = list2.Between(window).Select(z => z.Value).Average();
                //TODO 缺数在这里跳过
                var c = COSTherom(a, b, angC);
                var angA = Math.Asin(a * Math.Sin(angC.ToRad()) / c);
                var angB = Math.Asin(b * Math.Sin(angC.ToRad()) / c);
                Debug.Print("a:{0},b:{1},c:{2},angA:{3},angB:{4}", a, b, c, angA, angB);

                double epa = (a - a0) / a0, epb = (b - b0) / b0, epc = (c - c0) / c0;
                Debug.Print("epa:{0},epb:{1},epc:{2}", epa, epb, epc);

                //Δ用d表示
                double d = Delta(angA, angB, angC.ToRad()); //Δ
                double d1 = Delta1(angA, angB, angC.ToRad(), epa, epb, epc); //Δ1
                double d2 = Delta2(angA, angB, angC.ToRad(), angT.ToRad(), epa, epb, epc); //Δ2
                double d3 = Delta3(angA, angB, angC.ToRad(), angT.ToRad(), epa, epb, epc); //Δ3
                //double epx = (d1 + d2)/2; //εx 
                double epx = (d1 + d2) / d;

                //该窗口内的计算结果
                double epsilon1 = d1 / d + Math.Sqrt(Math.Pow(d3 / d, 2) + Math.Pow(d2 / d, 2)); //线应变ε1
                double epsilon2 = d1 / d - Math.Sqrt(Math.Pow(d3 / d, 2) + Math.Pow(d2 / d, 2)); //线应变ε2
                double garmmaXy = 2 * d3 / d; //剪应变γxy
                double delta = 2 * d1 / d; //面膨胀Δ，此Δ与上面中间变量Δ不一致，注意！
                double phi = Math.Atan(2 * (epsilon1 - epx) / garmmaXy).ToDeg(); //夹角
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

        /// <summary>
        /// 角度值的单位都是弧度
        /// </summary>
        /// <param name="angA"></param>
        /// <param name="angB"></param>
        /// <param name="angC"></param>
        /// <returns></returns>
        protected static double Delta(double angA, double angB, double angC)
        {
            return Math.Sin(2 * angA) + Math.Sin(2 * angB) + Math.Sin(2 * angC);
        }

        protected static double Delta1(double angA, double angB, double angC, double a, double b, double c)
        {
            return Math.Sin(2 * angA) * a + Math.Sin(2 * angB) * b + Math.Sin(2 * angC) * c;
        }

        protected static double Delta2(double angA, double angB, double angC, double angT, double a, double b, double c)
        {
            return 2 * a * Math.Cos(angC - angB + 2 * angT) * Math.Sin(angA) +
                   2 * b * Math.Cos(angA + angC + 2 * angT) * Math.Sin(angB) +
                   2 * c * Math.Cos(angC + 2 * angT) * Math.Sin(angC);
        }

        protected static double Delta3(double angA, double angB, double angC, double angT, double a, double b, double c)
        {
            return 2 * a * Math.Sin(angC - angB + 2 * angT) * Math.Sin(angA) +
                   2 * b * Math.Sin(angA + angC + 2 * angT) * Math.Sin(angB) +
                   2 * c * Math.Sin(angC + 2 * angT) * Math.Sin(angC);
        }
    }
}
