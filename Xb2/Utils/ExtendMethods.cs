using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Xb2.Utils
{
    public static class ExtendMethods
    {
        #region String Extend Methods

        /// <summary>
        /// 全角字符串转半角字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToDbc(this string input)
        {
            char[] array = input.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == 12288)
                {
                    array[i] = (char) 32;
                    continue;
                }
                if (array[i] > 65280 && array[i] < 65375)
                {
                    array[i] = (char) (array[i] - 65248);
                }
            }
            return new string(array);
        }

        #endregion

        public static double Round(this double d, int precision)
        {
            return Math.Round(d, precision);
        }

        public static double ToDouble(this string input)
        {
            return Convert.ToDouble(input);
        }

        public static string SStr(this DateTime dateTime)
        {
            return dateTime.ToShortDateString();
        }

        public static object GetStringOrDBNull(this string obj)
        {
            return string.IsNullOrEmpty(obj) ? DBNull.Value : (object) obj;
        }

        public static object GetInt32OrDbNull(this string obj)
        {
            return string.IsNullOrEmpty(obj) ? (object) DBNull.Value : Convert.ToInt32(obj);
        }

        public static object GetDoubleOrDBNull(this string obj)
        {
            return string.IsNullOrEmpty(obj) ? (object) DBNull.Value : Convert.ToDouble(obj);
        }

        public static List<TextBox> GetTextBoxs(this Panel panel)
        {
            return
                panel.Controls.Cast<System.Windows.Forms.Control>()
                    .ToList()
                    .FindAll(c => c.GetType() == typeof(TextBox))
                    .Cast<TextBox>()
                    .ToList();
        }


        //External method for algorithms
        /// <summary>
        /// 该日期是否为该月的最后一天
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool isMonLastDay(this DateTime dateTime)
        {
            var o = dateTime.AddDays(1);
            return dateTime.Month != o.Month;
        }

    }
}
