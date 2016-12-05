using System;
using System.Collections.Generic;
using Xb2.Algorithms.Core.Entity;

namespace Xb2.Algorithms.Core.Methods.Strain
{
    public class Xb2YBM3 : YbBase
    {
        public Xb2YB_M3_Input Input { get; set; }
        private List<YingBianOutput> first_outputs;
        private List<YingBianOutput> second_outputs;
        private List<YingBianOutput> comp_outputs;

        public Xb2YBM3(Xb2YB_M3_Input input)
        {
            this.Input = input;
            this.first_outputs = this.GetOutputsFirst();
            this.second_outputs = this.GetOutputsSecond();
            this.comp_outputs = this.GetOutputsCombining();
        }

        public List<DateValue> GetFirstTrianglePhi()
        {
            var ret = new List<DateValue>();
            this.first_outputs.ForEach(o => ret.Add(new DateValue(o.Date, o.Phi)));
            ret.RemoveAt(0);
            return ret;
        }

        public List<DateValue> GetFirstTriangleDelta()
        {
            var ret = new List<DateValue>();
            this.first_outputs.ForEach(o => ret.Add(new DateValue(o.Date, o.Delta)));
            return ret;
        }

        public List<DateValue> GetFirstTriangleEpsilog1()
        {
            var ret = new List<DateValue>();
            this.first_outputs.ForEach(o => ret.Add(new DateValue(o.Date, o.Epsilon1)));
            return ret;

        }

        public List<DateValue> GetFirstTriangleEpsilog2()
        {
            var ret = new List<DateValue>();
            this.first_outputs.ForEach(o => ret.Add(new DateValue(o.Date, o.Epsilon2)));
            return ret;
        }

        public List<DateValue> GetFirstTriangleGammaXY()
        {
            var ret = new List<DateValue>();
            this.first_outputs.ForEach(o => ret.Add(new DateValue(o.Date, o.GarmmaXy)));
            return ret;
        }

        public List<DateValue> GetSecondTriangleEpsilog1()
        {
            var ret = new List<DateValue>();
            this.second_outputs.ForEach(o => ret.Add(new DateValue(o.Date, o.Epsilon1)));
            return ret;
        }

        public List<DateValue> GetSecondTriangleEpsilog2()
        {
            var ret = new List<DateValue>();
            this.second_outputs.ForEach(o => ret.Add(new DateValue(o.Date, o.Epsilon2)));
            return ret;
        }

        public List<DateValue> GetSecondTriangleGammaXY()
        {
            var ret = new List<DateValue>();
            this.second_outputs.ForEach(o => ret.Add(new DateValue(o.Date, o.GarmmaXy)));
            return ret;
        }

        public List<DateValue> GetSecondTrianglePhi()
        {
            var ret = new List<DateValue>();
            this.second_outputs.ForEach(o => ret.Add(new DateValue(o.Date, o.Phi)));
            ret.RemoveAt(0);
            return ret;
        }

        public List<DateValue> GetSecondTriangleDelta()
        {
            var ret = new List<DateValue>();
            this.second_outputs.ForEach(o => ret.Add(new DateValue(o.Date, o.Delta)));
            return ret;
        }

        private List<Window> getWins()
        {
            return getFilteredWindows(this.Input.Start, this.Input.End, this.Input.SLen, this.Input.WLen, this.Input.BaseLine1);
        }

        public List<YingBianOutput> GetOutputsFirst()
        {
            return GetYingBianOutputsMethod1(this.Input.BaseLine1, this.Input.BaseLine2, this.Input.AngC1, this.Input.AngT1, this.getWins());
        }

        public List<YingBianOutput> GetOutputsSecond()
        {
            return GetYingBianOutputsMethod1(this.Input.BaseLine2, this.Input.BaseLine3, this.Input.AngC2, this.Input.AngT2, this.getWins());
        }

        public List<YingBianOutput> GetOutputsCombining()
        {
            var outputsCombining = new List<YingBianOutput>();
            var outputs1 = this.GetOutputsFirst();
            var outputs2 = this.GetOutputsSecond();
            var windows = this.getWins();
            Func<double, double, double> compWeightAverage = (d1, d2) => (d1*this.Input.W1 + d2*this.Input.W2)/2.0;
            foreach (var window in windows)
            {
                if (outputs1.Exists(o => DateTime.Equals(o.Date, window.Upper)))
                {
                    if (outputs2.Exists(o => DateTime.Equals(o.Date, window.Upper)))
                    {
                        var output1 = outputs1.Find(o => DateTime.Equals(o.Date, window.Upper));
                        var output2 = outputs2.Find(o => DateTime.Equals(o.Date, window.Upper));
                        var outputCombing = new YingBianOutput();
                        outputCombing.Epsilon1 = compWeightAverage(output1.Epsilon1, output2.Epsilon1);
                        outputCombing.Epsilon2 = compWeightAverage(output1.Epsilon2, output2.Epsilon2);
                        outputCombing.GarmmaXy = compWeightAverage(output1.GarmmaXy, output2.GarmmaXy);
                        outputCombing.Delta = compWeightAverage(output1.Delta, output2.Delta);
                        outputCombing.Phi = compWeightAverage(output1.Phi, output2.Phi);
                        outputCombing.Date = window.Upper;
                        outputsCombining.Add(outputCombing);
                    }
                }
            }
            return outputsCombining;
        }
    }

    public class Xb2YB_M3_Input : Xb2BaseInput
    {
        //基线1
        public List<DateValue> BaseLine1 { get; set; }
        //基线2
        public List<DateValue> BaseLine2 { get; set; }
        //基线3
        public List<DateValue> BaseLine3 { get; set; }
        //基线1与基线2之夹角
        public double AngC1 { get; set; }
        //基线2与基线3之夹角
        public double AngC2 { get; set; }
        //基线1与基线2形成之三角形与水平方向之夹角，单位是角度
        public double AngT1 { get; set; }
        //基线2与基线3形成之三角形与水平方向之夹角，单位是角度
        public double AngT2 { get; set; }
        //基线1与基线2形成之三角形之权重
        public double W1 { get; set; }
        //基线2与基线3形成之三角形之权重
        public double W2 { get; set; }
    }
}
