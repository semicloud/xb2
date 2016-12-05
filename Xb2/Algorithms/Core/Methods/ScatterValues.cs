using System;
using System.Collections.Generic;

namespace Xb2.Algorithms.Core.Methods
{
    public class ScatterValues
    {
        /// <summary>
        /// 窗尾
        /// </summary>
        public DateTime WinTail { get; set; }

        /// <summary>
        /// 速率差分值
        /// </summary>
        public List<Double> Diffs { get; set; }


        public ScatterValues()
        {
        }

        public ScatterValues(DateTime winTail, List<double> diffs)
        {
            WinTail = winTail;
            Diffs = diffs;
        }
    }
}
