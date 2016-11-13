using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xb2.Algorithms.Core
{
    /// <summary>
    /// 
    /// </summary>
    class CalcDescription
    {
        public List<Int32> ItemIdList { get; set; }
        public List<DataTable> InputTableList { get; set; }
        public Dictionary<string, double> Parameters { get; set; }
    }
}
