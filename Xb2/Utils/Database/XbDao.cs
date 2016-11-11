using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Xb2.Utils.Database
{
    public static class XbDao
    {
        #region 用户相关操作

        public static int GetUserId(string userName, string password, bool isAdmin)
        {
            var ans = -1;
            var sql = "select 编号 from 系统_用户 where 用户名='{1}' and 密码='{2}' and 管理员={3}";
            sql = string.Format(sql, "系统_用户", userName, password, isAdmin);
            var obj = DbHelper.GetScalar(sql);
            if (obj != null) ans = Convert.ToInt32(obj);
            Debug.Print("query user id by [{0},{1},{2}], return {3}", userName, password, isAdmin, ans);
            return ans;
        }

        [Test]
        public static void testGetUserId()
        {
            Assert.AreEqual(-1, GetUserId("not appear", "no passowrd", true));
            Assert.AreEqual(-1, GetUserId("admin", "admin", false));
            Assert.AreEqual(1, GetUserId("admin", "admin", true));
        }

        #endregion


        #region 地震目录相关操作



        #endregion

        #region 测项相关操作

        #endregion


        #region 测值相关操作

        #endregion

    }
}
