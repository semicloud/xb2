// 解决方案名称：Xb2
// 工程名称：Xb2Core
// 文件名：Xb2StrainM1.cs
// 作者：Semicloud
// 初次编写时间：2014-08-26
// 功能：

using System.Collections.Generic;
using Xb2.Algorithms.Core.Entity;

namespace Xb2.Algorithms.Core.Methods.Strain
{
    public class YbM1Input : BaseInput
    {
        /// <summary>
        /// 基线1测值序列
        /// </summary>
        public List<DateValue> BaseLine1 { get; set; }

        /// <summary>
        /// 基线2测值序列
        /// </summary>
        public List<DateValue> BaseLine2 { get; set; }

        /// <summary>
        /// 基线1与基线2之间的夹角C
        /// </summary>
        public double AngC { get; set; }

        /// <summary>
        /// 基线1与水平方向的夹角θ
        /// </summary>
        public double AngT { get; set; }
    }

    /// <summary>
    /// 应变-模式1
    /// </summary>
    public class Xb2YbM1: YbBase
    {
        public YbM1Input Input { get; set; }
        private List<YingBianOutput> m_outputs;
 
        /// <summary>
        /// 应变模式1：注意，输入必须截取完成后才能传递给算法处理
        /// </summary>
        /// <param name="input"></param>
        public Xb2YbM1(YbM1Input input)
        {
            this.Input = input;
            this.m_outputs = this.GetOutputs();
        }

        public List<YingBianOutput> GetOutputs()
        {
            var windows = getFilteredWindows(this.Input.Start, this.Input.End, this.Input.SLen, this.Input.WLen, this.Input.BaseLine1);
            var outputs = GetYingBianOutputsMethod1(this.Input.BaseLine1, this.Input.BaseLine2, this.Input.AngC, this.Input.AngT, windows);
            return outputs;
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
            return ret;
        }
    }
}