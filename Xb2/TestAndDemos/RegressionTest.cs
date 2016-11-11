using System;
using System.Linq;
using MathNet.Numerics;
using NUnit.Framework;

namespace Xb2.TestAndDemos
{
    class RegressionTest
    {
        private static double[] X =
        {
            7.631368, 1.364325, 4.544628, 2.634873, 1.539989, 5.899588, 9.668544, 8.689781,
            8.540497, 1.989722
        };

        private static double[] Y =
        {
            8.778313, 2.407041, 4.742066, 6.022104, 5.587097, 3.705257, 3.476999, 3.118116,
            5.936249, 4.584078
        };

        [Test]
        public void testRegression()
        {
            Console.WriteLine("---------------------------");
            var ans = Fit.Line(X, Y);
            var a = ans.Item1;
            var b = ans.Item2;
            Console.WriteLine("intercept:{0}, slope:{1}", ans.Item1, ans.Item2);
            Console.WriteLine("r^2:" + GoodnessOfFit.RSquared(X.Select(x => a + b*x), Y));

            Console.WriteLine("-------------------------");
            var p = Fit.Polynomial(X, Y, 3);
            Console.WriteLine("p:" + string.Join(",", p));
            Console.WriteLine("r^2:" + GoodnessOfFit.RSquared(X.Select(x => p[0] + p[1]*x + p[2]*x*x + p[3]*x*x*x), Y));
            Assert.True(true);
        }


    }
}
