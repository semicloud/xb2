using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using Accord.Math;
using MathNet.Numerics;
using NUnit.Framework;
using Xb2.Algorithms.Core.Entity;

namespace Xb2.Algorithms.Numberical
{
    class Processmethod
    {
        #region 拟合方法

        /// <summary>
        /// 线性拟合
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <returns></returns>
        public static string GetLinearRegressionExpression(List<DataPoint> dataPoints)
        {
            var xs = dataPoints.Select(p => p.XValue).ToArray();
            var ys= dataPoints.Select(p => p.YValues[0]).ToArray();
            var ans = Fit.Line(xs, ys);
            var r2 = GoodnessOfFit.RSquared(xs.Select(x => ans.Item2*x + ans.Item1), ys);
            var expression = "y(x)=" + ans.Item2 + "*x + " + ans.Item1 + ", r^2=" + r2;
            Debug.Print("Linear Regression:");
            Debug.Print("x:{0}", string.Join(",", xs));
            Debug.Print("y:{0}", string.Join(",", ys));
            Debug.Print("slope:{0}, intercept{1}", ans.Item2, ans.Item1);
            Debug.Print("r^2:" + r2);
            return expression;
        }

        /// <summary>
        /// 多项式拟合
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <returns></returns>
        public static string GetPolyRegressionExpression(List<DataPoint> dataPoints)
        {
            var xs = dataPoints.Select(p => p.XValue).ToArray();
            var ys = dataPoints.Select(p => p.YValues[0]).ToArray();
            //3次多项式回归
            var ans = Fit.Polynomial(xs, ys, 3);
            var r2 = GoodnessOfFit.RSquared(xs.Select(x => ans[0] + ans[1]*x + ans[2]*x*x + ans[3]*x*x*x), ys);
            var expression = "y(x)=" + ans[3] + "*x^3+" + ans[2] + "*x^2+" + ans[1] + "*x+" + ans[0] +
                             "\nr^2=" + r2;
            Debug.Print("Polynomial Regerssion:");
            Debug.Print("x:{0}", string.Join(",", xs));
            Debug.Print("y:{0}", string.Join(",", ys));
            Debug.Print("coff:{0}" + string.Join(",", ans));
            Debug.Print("r^2:" + r2);
            return expression;
        }

        [Test]
        public static void testEstimation()
        {
            var x = new[] { 0.52, 0.09, 0.74, 0.69, 0.51, 0.15, 0.08, 0.02, 0.28, 2.22 };
            var y = new[] { 1.21, 0.18, 1.48, 0.25, 0.16, 0.74, 0.54, 0.36, 0.76, 0.24 };

            //Console.WriteLine(string.Join(",", LogitEstimation(y, x)));
            Console.WriteLine(string.Join(",", Fit.LinearCombination(x, y, d => 1.0, Math.Log))); //Fit y(x) = c + b * ln(x)

            //Console.WriteLine(string.Join(",", PowerEstimation(y, x)));
            var power_p = Fit.LinearCombination(x, y.Select(u => Math.Log(u)).ToArray(), d => 1.0, d => Math.Log(d));
            Console.WriteLine(Math.Exp(power_p[0]) + "," + power_p[1]);

            //Console.WriteLine(string.Join(",", ExpEstimation(y, x)));
            var exp_p = Fit.LinearCombination(x, y.Select(u => Math.Log(u)).ToArray(), d => 1.0, d => d);
            Console.WriteLine(Math.Exp(exp_p[0]) + "," + exp_p[1]);

            var xs = new[] { 0d, 1, 2, 3, 4 };
            var ys = new[] { 1.5, 2.5, 3.5, 5.0, 7.5 };
            //Console.WriteLine(string.Join(",", ExpEstimation(ys, xs)));
            Assert.True(true);
        }

        /// <summary>
        /// 对数拟合，y(x)=c*ln(x)+b
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <returns></returns>
        public static string GetLogitRegressionExpression(List<DataPoint> dataPoints)
        {
            var xs = dataPoints.Select(p => p.XValue).ToArray();
            var ys = dataPoints.Select(p => p.YValues[0]).ToArray();
            var ps = Fit.LinearCombination(xs, ys, d => 1.0, d => Math.Log(d));
            var r2 = GoodnessOfFit.RSquared(xs.Select(x => ps[1]*x + ps[0]), ys);
            return "y=" + ps[1] + "*ln(x)+" + ps[0] + "\nr^2=" + r2;
        }

        /// <summary>
        /// 幂函数拟合，y(x)=c*x^b
        /// 两边取对数，ln(y(x))=ln(c) + b*ln(x)
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <returns></returns>
        public static string GetPowerRegresExpression(List<DataPoint> dataPoints)
        {
            var xs = dataPoints.Select(p => p.XValue).ToArray();
            var ys = dataPoints.Select(p => p.YValues[0]).ToArray();
            var ps = Fit.LinearCombination(xs, ys.Select(y => Math.Log(y)).ToArray(), d => 1.0, d => Math.Log(d));
            var r2 = GoodnessOfFit.RSquared(xs.Select(x => Math.Exp(ps[0])*Math.Pow(x, ps[1])), ys);
            return "y=" + Math.Exp(ps[0]) + "*x^" + ps[1] + "\nr^2=" + r2;
        }

        /// <summary>
        /// 指数e拟合，y=c*e^(m*x)
        /// ln(y)=ln(c) + m*x;
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <returns></returns>
        public static string GetExpRegressionExpression(List<DataPoint> dataPoints)
        {
            var xs = dataPoints.Select(p => p.XValue).ToArray();
            var ys = dataPoints.Select(p => p.YValues[0]).ToArray();
            var ps = Fit.LinearCombination(xs, ys.Select(y => Math.Log(y)).ToArray(), d => 1.0, d => d);
            var r2 = GoodnessOfFit.RSquared(xs.Select(x => Math.Exp(ps[0])*Math.Exp(ps[1]*x)), ys);
            return "y=" + Math.Exp(ps[0]) + "*e^(" + ps[1] + "*x)" + "\nR2=" + r2;
        }

        #endregion

        #region 插值方法

        /// <summary>
        /// 用一组数据点对一个未知点进行拉格朗日插值
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <param name="x"></param>
        public static double GetLagrangeInterpolationResult(List<DataPoint> dataPoints, DateTime x)
        {
            /**
             * 如果观测数据中只有2个点或3个点，直接进行插值即可；
             * 如果观测数据大于等于4个点，那么就按下列规则处理：
             * 如果未知点x比所有观测值的x都大，那么就取观测值的最后3个点进行插值；
             * 如果未知点x比所有观测值的x都小，那么就取观测值的最前面3个点进行插值；
             * 其他情况取未知点x的前面1个观测值，后面2个观测值，共3个点进行插值（一般情形）；
             * 如果未知点x的后面不足2个观测值，那么就取x前面2个观测值，后面1个观测值进行插值；
             */
            if (dataPoints.Count == 2 | dataPoints.Count == 3)
            {
                var xs = dataPoints.Select(p => p.XValue).ToArray();
                var ys = dataPoints.Select(p => p.YValues[0]).ToArray();
                Debug.Print("unknown x:{0}", x.ToOADate());
                Debug.Print("xs:{0}", string.Join(", ", xs));
                Debug.Print("ys:{0}", string.Join(", ", ys));
                return LagrangeInterploation(xs, ys, x.ToOADate());
            }
            var allDates = dataPoints.Select(p => DateTime.FromOADate(p.XValue)).ToList();
            var n = allDates.Count;
            if (x > allDates.Last())
            {
                var xs = dataPoints.Skip(n - 3).Select(p => p.XValue).ToArray();
                var ys = dataPoints.Skip(n - 3).Select(p => p.YValues[0]).ToArray();
                Debug.Print("unknown x:{0}", x.ToOADate());
                Debug.Print("xs:{0}", string.Join(", ", xs));
                Debug.Print("ys:{0}", string.Join(", ", ys));
                return LagrangeInterploation(xs, ys, x.ToOADate());
            }
            if (x < allDates.First())
            {
                var xs = dataPoints.Take(3).Select(p => p.XValue).ToArray();
                var ys = dataPoints.Take(3).Select(p => p.YValues[0]).ToArray();
                Debug.Print("unknown x:{0}", x.ToOADate());
                Debug.Print("xs:{0}", string.Join(", ", xs));
                Debug.Print("ys:{0}", string.Join(", ", ys));
                return LagrangeInterploation(xs, ys, x.ToOADate());
            }
            allDates.Add(x);
            allDates.Sort();
            int index = allDates.IndexOf(x);
            Debug.Print("x index:{0}", index);
            //如果x位于倒数第2个位置上，即其后面只有1个观测值的情况
            if (index + 2 == n)
            {
                //取x前面的2个观测值，以及其后面仅剩的1个观测值进行插值
                var indices = new[] {index - 2, index - 1};
                Debug.Print("Fore indices:{0}", indices.ToString());
                var points = dataPoints.Get(indices);
                points.Add(dataPoints.Last());
                var xs = points.Select(p => p.XValue).ToArray();
                var ys = points.Select(p => p.YValues[0]).ToArray();
                Debug.Print("unknown x:{0}", x.ToOADate());
                Debug.Print("xs:{0}", string.Join(", ", xs));
                Debug.Print("ys:{0}", string.Join(", ", ys));
                return LagrangeInterploation(xs, ys, x.ToOADate());
            }
            else
            {
                //一般情形
                var points = new List<DataPoint> {dataPoints[index - 1]};
                var indices = new[] {index + 1, index + 2};
                points.AddRange(dataPoints.Get(indices));
                var xs = points.Select(p => p.XValue).ToArray();
                var ys = points.Select(p => p.YValues[0]).ToArray();
                Debug.Print("unknown x:{0}", x.ToOADate());
                Debug.Print("xs:{0}", string.Join(", ", xs));
                Debug.Print("ys:{0}", string.Join(", ", ys));
                return LagrangeInterploation(xs, ys, x.ToOADate());
            }
            throw new Exception("Uncatched path in lagrange interpolation.");
        }

        /// <summary>
        /// 拉格朗日插值法
        /// </summary>
        /// <param name="xs">x序列</param>
        /// <param name="ys">y序列</param>
        /// <param name="x">要进行插值的值</param>
        /// <returns></returns>
        private static double LagrangeInterploation(double[] xs, double[] ys, double x)
        {
            if (xs.Length != ys.Length)
            {
                throw new ArgumentException("数据大小不一致，无法进行插值！");
            }
            var n = ys.Length;
            var vector = Vector.Ones(n);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i != j)
                    {
                        vector[i] = vector[i]*((x - xs[j])/(xs[i] - xs[j]));
                    }
                }
            }
            double y = 0;
            for (int i = 0; i < n; i++)
            {
                y = y + ys[i]*vector[i];
            }
            return y;
        }

        /// <summary>
        /// 等间距差值
        /// </summary>
        /// <param name="points">等间距插值的两个端点</param>
        /// <param name="period">观测周期</param>
        /// <returns></returns>
        public static List<DateValue> EqualDistanceInterpolation(List<DataPoint> points, int period)
        {
            var ans = new List<DateValue>();
            var x = points.Select(p => p.XValue).ToArray();
            var y = points.Select(p => p.YValues[0]).ToArray();
            var predict = Fit.LineFunc(x, y);
            var date1 = DateTime.FromOADate(points[0].XValue);
            var date2 = DateTime.FromOADate(points[1].XValue);
            while (date1.AddMonths(period) < date2)
            {
                date1 = date1.AddMonths(period);
                ans.Add(new DateValue(date1, predict(date1.ToOADate())));
            }
            Debug.Print("equal interpolation:{0}", string.Join(",", ans));
            return ans;
        }

        /// <summary>
        /// 等间距低次拉格朗日插值
        /// </summary>
        /// <returns></returns>
        public static List<DateValue> EqualDistLagInterpolation(List<DataPoint> points, int period)
        {
            //询问甲方究竟用哪几个点做拉格朗日差值
            throw new NotImplementedException();
        }

        [Test]
        public void TestLangrange()
        {
            var x = new double[] { 6109, 6130, 6161, 6191, 7509, 7539, 7574, 7606, 7632 };
            var y = new double[] { 723.5, 724.5, 724.02, 723.7, 720.36, 722.34, 724.71, 725.27, 725.08 };
            var x1 = 6940;
            Console.Write(LagrangeInterploation(x,y,x1).ToString());
            Assert.True(true);
        }



        #endregion


    }
}
