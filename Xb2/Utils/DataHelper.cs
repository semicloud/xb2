using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Xb2.Algorithms.Core;

namespace Xb2.Utils
{
    public static class DataHelper
    {
        /// <summary>
        /// 将datatable中未选择的行去掉
        /// 该datatable必须包含“选择”列
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DataTable RemoveUnCheckedRow(this DataTable dt)
        {
            if (!dt.Columns.Contains("选择"))
                throw new ArgumentException("DataTable必须包含‘选择’列");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //如果未选择该行，则将该行从dt中删除
                if (!Convert.ToBoolean(dt.Rows[i]["选择"]))
                {
                    dt.Rows[i].Delete();
                }
            }
            dt.AcceptChanges();
            return dt;
        }

        /// <summary>
        /// 获取datatable中数据行的字符串表示
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="spliter"></param>
        /// <param name="colnames"></param>
        /// <returns></returns>
        public static List<string> GetDataLine(this DataTable dt, string spliter,
            params string[] colnames)
        {
            var ans = new List<string>();
            foreach (var colname in colnames)
            {
                if (!dt.Columns.Contains(colname))
                {
                    throw new ArgumentException("DataTable不存在列:" + colname);
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var sb = new StringBuilder();
                foreach (var colname in colnames)
                {
                    sb.Append(dt.Rows[i][colname]);
                    sb.Append(spliter);
                }
                //删除最后一个分隔符
                var str = sb.ToString();
                var line = str.Remove(str.Length - 1);
                ans.Add(line);
            }
            return ans;
        }

        /// <summary>
        /// 从dataTable中抽出ListOfDateValue
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static List<DateValue> RetrieveDateValues(DataTable dataTable)
        {
            var answer = new List<DateValue>();
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    var date = Convert.ToDateTime(dataTable.Rows[i]["观测日期"]);
                    var value = Convert.ToDouble(dataTable.Rows[i]["观测值"]);
                    answer.Add(new DateValue(date, value));
                }
            }
            return answer;
        }

        /// <summary>
        /// 给某一列填充固定值
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        public static void FillColumn(this DataTable dataTable, string columnName, object value)
        {
            if (dataTable.Columns.Contains(columnName))
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    dataTable.Rows[i][columnName] = value;
                }
            }
        }

        /// <summary>
        /// 获取数据表中的一列，类型是string
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public static List<string> GetColumnOfString(this DataTable dt, string colName)
        {
            List<string> answer = new List<string>();
            if (dt != null && dt.Columns.Contains(colName))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    answer.Add(dt.Rows[i][colName].ToString());
                }
            }
            return answer;
        }

        /// <summary>
        /// 获取数据表中的一列，类型是double
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public static List<double> GetColumnOfDouble(this DataTable dt, string colName)
        {
            List<double> answer = new List<double>();
            if (dt != null && dt.Columns.Contains(colName))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    answer.Add(Convert.ToDouble(dt.Rows[i][colName]));
                }
            }
            return answer;
        }

        /// <summary>
        /// 给dataTable加一列“选择”列，该列为第1列，类型为Boolean
        /// 并默认将该列全部选中，即全部置为True
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static DataTable BuildChooseColumn(DataTable dataTable)
        {
            var column = new DataColumn("选择", typeof(bool));
            dataTable.Columns.Add(column);
            column.SetOrdinal(0); //该列放到第一列
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                dataTable.Rows[i]["选择"] = true;
            }
            return dataTable;
        }

        /// <summary>
        /// 给DataTable加一列序号列，从1开始排序
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static DataTable IdentifyDataTable(DataTable dataTable)
        {
            var column = new DataColumn {ColumnName = "序号", DataType = typeof(Int32)};
            dataTable.Columns.Add(column);
            column.SetOrdinal(0); //序号列放在第一列
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                dataTable.Rows[i]["序号"] = i + 1;
            }
            return dataTable;
        }

        /// <summary>
        /// 从DataTable中去除序号列
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static DataTable UnIdentifyDataTable(DataTable dataTable)
        {
            dataTable.Columns.RemoveAt(0);
            return dataTable;
        }

        /// <summary>
        /// 获取DataTable的所有列的列名
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static List<string> GetColNames(this DataTable dataTable)
        {
            var ans = new List<string>();
            var columns = dataTable.Columns;
            foreach (DataColumn column in columns)
            {
                ans.Add(column.ColumnName);
            }
            return ans;
        }

        /// <summary>
        /// 将DataTable导出到文本文件
        /// </summary>
        /// <param name="pathname"></param>
        /// <param name="dataTable"></param>
        public static void Export(string pathname, DataTable dataTable)
        {
            var sb = new StringBuilder();
            //分隔符是半角逗号
            var spliter = ",";
            //用分隔符连接列名
            sb.AppendLine(string.Join(spliter, dataTable.GetColNames()));
            //连接数据行
            for (int i = 0; i < dataTable.Rows.Count; i++)
                sb.AppendLine(String.Join(spliter, dataTable.Rows[i].ItemArray));
            //写入文件
            if (!File.Exists(pathname))
            {
                var fs = new FileStream(pathname, FileMode.Create);
                var sw = new StreamWriter(fs);
                sw.Write(sb.ToString());
                sw.Close();
                fs.Close();
            }
            else
            {
                MessageBox.Show("已存在文件【" + pathname + "】");
            }
        }
    }
}