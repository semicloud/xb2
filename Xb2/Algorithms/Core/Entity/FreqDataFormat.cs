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
        FreqEqual
    }
}
