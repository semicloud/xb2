// 解决方案名称：XbApp
// 工程名称：XbApp
// 文件名：Xb2StrainMethodUtils.cs
// 作者：Semicloud
// 初次编写时间：2014-11-20
// 功能：

using System;

namespace Xb2.Algorithms.Core.Methods.Strain.TriangleCalculator
{
    /// <summary>
    /// 三角形计算器
    /// </summary>
    public abstract class TriangleCalculatorBase
    {
        protected  double _a;
        protected  double _b;
        protected  double _c;
        protected  double _A;
        protected  double _B;
        protected  double _C;
        protected  double _T;

        /// <summary>
        /// 主应变 e1
        /// </summary>
        /// <returns></returns>
        public double getEpsilog1()
        {
            double delta1 = getDelta1(_a, _b, _c, _A, _B, _C);
            double delta2 = getDelta2(_a, _b, _c, _A, _B, _C, _T);
            double delta3 = getDelta3(_a, _b, _c, _A, _B, _C, _T);
            double delta = getDelta(_A, _B, _C);
            return delta1/delta + Math.Sqrt(Math.Pow((delta3/delta), 2.0) + Math.Pow(delta2/delta, 2.0));
        }

        /// <summary>
        /// 主应变 e2
        /// </summary>
        /// <returns></returns>
        public double getEpsilog2()
        {
            double delta1 = getDelta1(_a, _b, _c, _A, _B, _C);
            double delta2 = getDelta2(_a, _b, _c, _A, _B, _C, _T);
            double delta3 = getDelta3(_a, _b, _c, _A, _B, _C, _T);
            double delta = getDelta(_A, _B, _C);
            return delta1 / delta - Math.Sqrt(Math.Pow((delta3 / delta), 2.0) + Math.Pow(delta2 / delta, 2.0));
        }

        /// <summary>
        /// 剪应变 grmmaXY
        /// </summary>
        /// <returns></returns>
        public double getGammaXY()
        {
            double delta3 = getDelta3(_a, _b, _c, _A, _B, _C, _T);
            double delta = getDelta(_A, _B, _C);
            return 2 * delta3 / delta;
        }

        /// <summary>
        /// 面膨胀 Δ
        /// </summary>
        /// <returns></returns>
        public double getBigDelta()
        {
            return getDelta(_A, _B, _C);
        }

        /// <summary>
        /// 主应变与x的夹角 fai
        /// </summary>
        /// <returns></returns>
        public double getPhi()
        {
            double delta1 = getDelta1(_a, _b, _c, _A, _B, _C);
            double delta2 = getDelta2(_a, _b, _c, _A, _B, _C, _T);
            double epsilogx = (delta1 + delta2) / 2;
            double epsilog1 = getEpsilog1();
            double grmmaxy = getGammaXY();
            return Math.Atan(2 * (epsilog1 - epsilogx) / grmmaxy);
        }

        //求Δ，输入为三角形的三个角A，B，C
        //Δ=sin(2A) + sin(2B) + sin(2C)
        private static double getDelta(double A, double B, double C)
        {
            return Math.Sin(2 * A) + Math.Sin(2 * B) + Math.Sin(2 * C);
        }

        /// <summary>
        /// 计算Δ1
        /// Δ1=aSin(2A)+bSin(2B)+cSin(2C)
        /// </summary>
        /// <param name="a">边a</param>
        /// <param name="b">边b</param>
        /// <param name="c">边c</param>
        /// <param name="A">角A</param>
        /// <param name="B">角B</param>
        /// <param name="C">角C</param>
        /// <returns></returns>
        private static double getDelta1(double a, double b, double c, double A, double B, double C)
        {
            return a * Math.Sin(2 * A) + b * Math.Sin(2 * B) + c * Math.Sin(2 * C);
        }

        /// <summary>
        /// 计算Δ2
        /// Δ1=2a·cos(C-B+2T)sin(A)+2b·Cos(A+C+2T)sin(B)-2c·Cos(C+2T)sin(C)
        /// </summary>
        /// <param name="a">边a</param>
        /// <param name="b">边b</param>
        /// <param name="c">边c</param>
        /// <param name="A">角A</param>
        /// <param name="B">角B</param>
        /// <param name="C">角C</param>
        /// <param name="T">角T</param>
        /// <returns></returns>
        private static double getDelta2(double a, double b, double c, double A, double B, double C, double T)
        {
            return 2 * a * Math.Cos(C - B + 2 * T) * Math.Sin(A) + 2 * b * Math.Cos(A + C + 2 * T) * Math.Sin(B) -
                   2 * C * Math.Cos(C + 2 * T) * Math.Sin(C);
        }

        /// <summary>
        /// 计算Δ3=2aSin(C-B+2T)Sin(A)+2bSin(A+C+2T)sin(B)-2cSin(C+2T)Sin(C)
        /// </summary>
        /// <param name="a">边a</param>
        /// <param name="b">边b</param>
        /// <param name="c">边c</param>
        /// <param name="A">角A</param>
        /// <param name="B">角B</param>
        /// <param name="C">角C</param>
        /// <param name="T">角T</param>
        /// <returns></returns>
        private static double getDelta3(double a, double b, double c, double A, double B, double C, double T)
        {
            return 2 * a * Math.Sin(C - B + 2 * T) * Math.Sin(A) + 2 * b * Math.Sin(A + C + 2 * T) * Math.Sin(B) -
                   2 * c * Math.Sin(C + 2 * T) * Math.Sin(C);
        }
    }
}