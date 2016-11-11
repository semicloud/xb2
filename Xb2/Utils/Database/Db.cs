using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace Xb2.Utils.Database
{
    public class Db
    {
        #region 需要重构的方法

        /// <summary>
        /// 查询数据库中是否存在名为viewName的视图
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public static bool HasView(string viewName)
        {
            var sql = "SELECT TABLE_NAME FROM information_schema.`TABLES` "
                      + "WHERE TABLE_TYPE LIKE 'VIEW' AND TABLE_SCHEMA LIKE 'xb'";
            var dt = MySqlHelper.ExecuteDataset(CStr(), sql).Tables[0];
            var viewNames = dt.GetColumnOfString("TABLE_NAME");
            return viewNames.Contains(viewName);
        }

        /// <summary>
        /// 通过标注库名和用户编号获取标注库编号
        /// 存在标注库返回该标注库的编号
        /// 不存在返回-1
        /// </summary>
        /// <param name="labelDbName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int GetLabelDbId(string labelDbName, int userId)
        {
            var sql = "select 编号 from {0} where 用户编号={1} and 标注库名称='{2}'";
            sql = string.Format(sql, Db.TnLabelDb(), userId, labelDbName);
            var ans = MySqlHelper.ExecuteScalar(Db.CStr(), sql);
            var id = ans != null ? Convert.ToInt32(ans) : -1;
            return id;
        }

        #endregion

        #region 数据库连接字符串

        public static string CStr()
        {
            return ConfigurationManager.ConnectionStrings["Xb2ConnStr"].ConnectionString;
        }

        #endregion

        #region 表的名字
        /// <summary>
        /// 测项表名
        /// </summary>
        /// <returns></returns>
        public static string TnMItem()
        {
            return "系统_测项";
        }

        /// <summary>
        /// 测项查询条件表名
        /// </summary>
        /// <returns></returns>
        public static string TnQMItem()
        {
            return "系统_测项查询条件";
        }

        /// <summary>
        /// 测项区域查询视图表名
        /// </summary>
        /// <returns></returns>
        public static string TnRQMItem()
        {
            return "系统_测项区域查询视图";
        }

        /// <summary>
        /// 获得用户表名
        /// </summary>
        /// <returns></returns>
        public static string TnUser()
        {
            return "系统_用户";
        }

        /// <summary>
        /// 地震目录查询条件表名
        /// </summary>
        /// <returns></returns>
        public static string TnQCategory()
        {
            return "系统_地震目录查询条件";
        }

        /// <summary>
        /// 地震目录子库表名
        /// </summary>
        /// <returns></returns>
        public static string TnSubDb()
        {
            return "系统_地震目录子库";
        }

        /// <summary>
        /// 地震目录子库数据！表名
        /// </summary>
        /// <returns></returns>
        public static string TnSubDbData()
        {
            return "系统_地震目录子库数据";
        }

        /// <summary>
        /// 地震目标标注库表名
        /// </summary>
        public static string TnLabelDb()
        {
            return "系统_地震目录标注库";
        }

        /// <summary>
        /// 地震目标标注库数据！表名
        /// </summary>
        /// <returns></returns>
        public static string TnLabelDbData()
        {
            return "系统_地震目录标注库数据";
        }

        /// <summary>
        /// 获得地震目录表名
        /// </summary>
        /// <returns></returns>
        public static string TnCategory()
        {
            return "系统_地震目录";
        }

        /// <summary>
        /// 系统_用户基础数据库
        /// </summary>
        /// <returns></returns>
        public static string TnProcessedDb()
        {
            return "系统_用户基础数据库";
        }

        /// <summary>
        /// 系统_用户基础数据库数据
        /// </summary>
        /// <returns></returns>
        public static string TnProcessedDbData()
        {
            return "系统_用户基础数据库数据";
        }

        /// <summary>
        /// 原始数据表名
        /// </summary>
        /// <returns></returns>
        public static string TnRData()
        {
            return "系统_原始数据";
        }

        #endregion
    }
}
