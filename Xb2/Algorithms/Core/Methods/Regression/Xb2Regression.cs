﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NUnit.Framework;
using Xb2.Algorithms.Core.Entity;

namespace Xb2.Algorithms.Core.Methods.Regression
{
    public class Xb2Regression
    {
        private Xb2RegressionInput _input;
        //观测日期(数值型)
        private Vector<double> _x;
        //观测值
        private Vector<double> _y;
        //估计的y值
        private Vector<double> _yCap;

        private List<DateTime> _dates;

        public Xb2Regression(Xb2RegressionInput input)
        {
            this._input = input;
            this._x = Vector.Build.Dense(_input.Collection.Select(d => d.Date.ToOADate()).ToArray());
            this._y = Vector.Build.Dense(_input.Collection.Select(d => d.Value).ToArray());
            this._yCap = _x.Map(Fit.LineFunc(_x.ToArray(), _y.ToArray()));
            this._dates = _input.Collection.Select(p => p.Date).ToList();
            Debug.Print("Linear Regression:");
            Debug.Print("_x:" + String.Join(",",_x));
            Debug.Print("_y:" + String.Join(",", _y));
            Debug.Print("_yCap:" + string.Join(",",_yCap));
        }

        public double GetR()
        {
            var r = GoodnessOfFit.R(_yCap, _y);
            Debug.Print("R:" + r);
            return r;
        }

        public Tuple<double, double> GetCoff()
        {
            return Fit.Line(_x.ToArray(), _y.ToArray());
        }

        public double GetRThreshold()
        {
            throw new NotImplementedException();
        }

        public List<DateValue> GetFittingLineData()
        {
            return DateValueList.FromArrays(_dates, _yCap.ToList());
        }

        public List<DateValue> GetResidualLineData()
        {
            return DateValueList.FromArrays(_dates, (_y - _yCap).ToList());
        }

        public string GetFittingLineFormula()
        {
            var ab = GetCoff();
            return "y(x)=" + ab.Item1 + "+" + ab.Item2 + "*x";
        }
    }

    /// <summary>
    /// 测试类 
    /// </summary>
    public class TestXb2Regression
    {
        [Test]
        public void Test()
        {
            var input = new Xb2RegressionInput();
            var dateValues = DateValueList.FromRawData(39).GetRange(0, 10);
            Console.WriteLine(String.Join(",", dateValues.Select(p => p.Date.ToOADate())));
            Console.WriteLine(String.Join(",", dateValues.Select(p => p.Value)));
            //25659,25689,25720,25750,25781,25812,25842,25873,25903,25934
            //-2203.29,-2203.2,-2203.21,-2203.19,-2203.12,-2203.02,-2203.11,-2203.1,-2203.1,-2203.09
            input.Collection = dateValues;
            var regression = new Xb2Regression(input);
            Console.WriteLine("R:" + regression.GetR());
            Console.WriteLine("a={0},b={1}", regression.GetCoff().Item1, regression.GetCoff().Item2);
            Console.WriteLine(regression.GetFittingLineFormula());
            Assert.AreEqual(Math.Sqrt(0.6362), regression.GetR(), 0.01);
            Assert.AreEqual(-2.220488e+03,regression.GetCoff().Item1,0.01);
            Assert.AreEqual(6.723749e-04, regression.GetCoff().Item2, 0.01);
            Assert.AreEqual(3.896352e-19, regression.GetResidualLineData().Select(p => p.Value).Average(), 0.01);

        }
    }
}