using System;

namespace Xb2.Algorithms.Core.Entity
{
    public class DateValue
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }

        public DateValue()
        {
        }

        public DateValue(DateTime date, double value)
        {
            this.Date = date;
            this.Value = value;
        }

        public override string ToString()
        {
            return string.Format("d:{0:yyyy-MM-dd}, v:{1:#0.00}", this.Date, this.Value);
        }

        public static string Desc(DateValue dv1, DateValue dv2)
        {
            if (dv1.Date == dv2.Date)
                return "【" + dv1.Date.ToString("yyyy-MM-dd") + ", " + dv1.Value + "→" + dv2.Value + "】";
            return dv1.ToString() + dv2;
        }
    }
}
