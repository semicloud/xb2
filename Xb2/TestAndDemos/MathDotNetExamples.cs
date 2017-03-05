using System;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using NUnit.Framework;

namespace Xb2.TestAndDemos
{
    /// <summary>
    /// Math.Net库的使用方法
    /// </summary>
    class MathDotNetExamples
    {
        [Test]
        public void Example1()
        {
            Vector<double> v = Vector<double>.Build.Dense(10, Math.PI / 6);
            Console.WriteLine(v.Map(Math.Sin, Zeros.Include).ToString());
            Vector<double> v1 = Vector<double>.Build.Random(60, new Normal());
            Console.WriteLine(v1.ToString());
            Console.WriteLine("MAX:" + v1.Maximum());
            Console.WriteLine("MAX Index:" + v1.MaximumIndex());
            Console.WriteLine("Average:{0}, variance:{1}", v1.Mean(), v1.Variance());
            Assert.True(true);
        }
    }
}
