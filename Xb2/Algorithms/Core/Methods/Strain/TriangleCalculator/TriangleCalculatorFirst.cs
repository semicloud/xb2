// 解决方案名称：XbApp
// 工程名称：XbApp
// 文件名：Xb2StrainM3Calculator.cs
// 作者：Semicloud
// 初次编写时间：2014-11-20
// 功能：

using System;

namespace Xb2.Algorithms.Core.Methods.Strain.TriangleCalculator
{
    public class TriangleCalculatorFirst : TriangleCalculatorBase
    {
        /// <summary>
        /// 三角形计算器
        /// </summary>
        /// <param name="a">测项1</param>
        /// <param name="b">测项2</param>
        /// <param name="C">测项1和2之间的夹角</param>
        /// <param name="T">与x轴的夹角</param>
        public TriangleCalculatorFirst(double a, double b, double C, double T)
        {
            this._a = a;
            this._b = b;
            this._C = C;
            this._T = T;

            this._c = Math.Sqrt(_a*_a + _b*_b - 2*_a*_b*Math.Cos(_C));
            this._A = Math.Asin(_a*Math.Sin(_C)/_c);
            this._B = Math.Asin(_b*Math.Sin(_C)/_c);
        }
    }
}