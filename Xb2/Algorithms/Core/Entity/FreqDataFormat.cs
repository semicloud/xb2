using System;

namespace Xb2.Algorithms.Core.Entity
{
    /// <summary>
    /// 观测周期内时间格式预处理方法
    /// </summary>
    public enum FreqDataFormat
    {
        /// <summary>
        /// 不处理
        /// </summary>
        NoProcess,
        /// <summary>
        /// 观测周期内一值化
        /// </summary>
        FreqAverage,
        /// <summary>
        /// 观测周期内不缺数
        /// </summary>
        FreqFullData,
        /// <summary>
        /// 等间距数据生成
        /// </summary>
        FreqEqual,
        /// <summary>
        /// 观测周期内一值化+观测周期内不缺数
        /// </summary>
        FreqAveragePlusFreqFullData

    }

    public class FreqDataFormatParser
    {
        public static FreqDataFormat Parse(String str)
        {
            if (str.Equals("直接"))
                return FreqDataFormat.NoProcess;
            if (str.Equals("观测周期内一值化"))
                return FreqDataFormat.FreqAverage;
            if (str.Equals("观测周期内不缺数"))
                return FreqDataFormat.FreqFullData;
            if (str.Equals("等间距数据生成"))
                return FreqDataFormat.FreqEqual;
            if (str.Equals("观测周期内一值化+观测周期内不缺数"))
                return FreqDataFormat.FreqAveragePlusFreqFullData;
            throw new ArgumentException("不支持的方法");
        }
    }
}
