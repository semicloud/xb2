using System.Configuration;
using NUnit.Framework;

namespace Xb2.Config
{
    public class Xb2Config
    {
        # region DbConfig

        public static string GetConnStr()
        {
            return ConfigurationManager.ConnectionStrings["Xb2ConnStr"].ConnectionString;
        }

        #endregion

        public static int GetPrecision()
        {
            return 2;
        }
    }
}
