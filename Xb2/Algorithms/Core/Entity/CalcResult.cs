using System.Data;

namespace Xb2.Algorithms.Core.Entity
{
    /// <summary>
    /// 计算结果类
    /// 该类包含了一个算法应该输出的结果
    /// 和相关输入
    /// </summary>
    public class CalcResult
    {
        public string Title { get; set; }
        public DataTable NumericalTable { get; set; }
    }
}
