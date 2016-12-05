// 解决方案名称：XbApp
// 工程名称：XbApp
// 文件名：TriangleCalculatorSecond.cs
// 作者：Semicloud
// 初次编写时间：2014-11-20
// 功能：

using System;

namespace Xb2.Algorithms.Core.Methods.Strain.TriangleCalculator
{
    /// <summary>
    /// 第二种三角形计算方法
    /// </summary>
    public class TriangleCalculatorSecond:TriangleCalculatorBase
    {
        public TriangleCalculatorSecond(double a,double b,double c,double T)
        {
            this._a = a;
            this._b = b;
            this._c = c;

            this._A = Math.Acos((_b * _b + _c * _c - _a * _a) / 2 * _b * _c);
            this._B = Math.Acos((_a * _a + _c * _c - _b * _b) / 2 * _a * _c);
            this._C = Math.Acos((_a * _a + _b * _b - _c * _c) / 2 * _a * _b);
        }
    }
}