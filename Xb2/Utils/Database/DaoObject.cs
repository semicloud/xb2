using System;
using System.Data;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace Xb2.Utils.Database
{
    /// <summary>
    /// Data Access Object
    /// </summary>
    public static class DaoObject
    {
        #region 用户相关操作

        public static int GetUserId(string userName, string password, bool isAdmin)
        {
            var ans = -1;
            var sql = "select 编号 from 系统_用户 where 用户名='{1}' and 密码='{2}' and 管理员={3}";
            sql = String.Format(sql, "系统_用户", userName, password, isAdmin);
            var obj = MySqlHelper.ExecuteScalar(DbHelper.ConnectionString(), sql);
            if (obj != null) ans = Convert.ToInt32(obj);
            Debug.Print("query user id by [{0},{1},{2}], return {3}", userName, password, isAdmin, ans);
            return ans;
        }

        #endregion

        #region 地震目录相关操作

        #region 地震目录总库

        /// <summary>
        /// 从地震目录子库中获取所有地震目录
        /// </summary>
        /// <returns></returns>
        public static DataTable GetCategoriesFromMainDatabase()
        {
            throw new NotImplementedException();
        }



        #endregion

        #region 地震目录查询

        /// <summary>
        /// 保存地震目录查询命令
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="commandName"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static bool SaveCategoryQueryCommand(int userId, string commandName, string commandText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据用户编号获取地震目录查询命令
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DataTable GetCategoryQueryCommands(int userId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 子库

        /// <summary>
        /// 根据用户编号获取子库列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DataTable GetSubDatabases(int userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 保存用户新建的子库
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="databaseName">地震目录子库名称</param>
        /// <param name="dataTable">用户选择的地震目录</param>
        /// <returns></returns>
        public static bool SaveSubDatabase(int userId, string databaseName, DataTable dataTable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 通过地震目录子库的名称查询地震目录子库的编号
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public static int GetSubDatabaseId(string databaseName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 该用户是否已经存在同名的子库
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public static bool ExistsSubDatabase(int userId, string databaseName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据用户编号和地震目录子库名称删除地震目录子库
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public static bool DeleteSubDatabase(int userId, string databaseName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据用户编号和地震目录子库名称获取地震目录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public static DataTable GetCategoriesFromSubDatabase(int userId, string databaseName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 标注库

        /// <summary>
        /// 获取用户标注库信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DataTable GetLabelDatabasesInfo(int userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 保存用户新建的标注库
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="databaseName">标注库名称</param>
        /// <param name="dataTable">用户选择的地震目录</param>
        /// <returns></returns>
        public static bool SaveLabelDatabase(int userId, string databaseName, DataTable dataTable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据用户编号和标注库名删除标注库
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public static bool DeleteLabelDatabase(int userId, string databaseName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 该用户是否已经存在同名的标注库
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public static bool ExistsLabelDatabase(int userId, string databaseName)
        {
            throw new NotImplementedException();
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
            sql = String.Format(sql, DbHelper.TnLabelDb(), userId, labelDbName);
            var ans = MySqlHelper.ExecuteScalar(DbHelper.ConnectionString(), sql);
            var id = ans != null ? Convert.ToInt32(ans) : -1;
            return id;
        }

        /// <summary>
        /// 根据用户编号和标注库名称获取地震目录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public static DataTable GetCategoriesFromLabelDatabase(int userId, string databaseName)
        {
            //FrmGenLabelDatabase
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region 测项相关操作

        public static bool SaveMItem()
        {
            throw new NotImplementedException();
        }


        #endregion

        #region 原始数据库相关

        /// <summary>
        /// 根据测项编号获取原始数据
        /// </summary>
        /// <param name="mitemId"></param>
        /// <returns></returns>
        public static DataTable GetRawData(int mitemId)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region 基础数据库相关

        /// <summary>
        /// 根据基础数据库编号获取基础数据
        /// </summary>
        /// <param name="databaseId">基础数据库编号</param>
        /// <returns></returns>
        public static DataTable GetProcessedData(int databaseId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据用户编号和测项编号查询用户基础数据库信息
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="mitemId">测项编号</param>
        /// <returns></returns>
        public static DataTable GetProcessedDatabasesInfo(int userId, int mitemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据测项编号获取基础数据库信息(需要连接测项表)
        /// </summary>
        /// <param name="mitemId"></param>
        /// <returns></returns>
        public static DataTable GetProcessedDatabaseInfosByMItemId(int mitemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新用户的默认基础数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mitemId"></param>
        /// <param name="databaseId"></param>
        /// <returns></returns>
        public static bool ChangeDefaultProcessedDatabase(int userId, int mitemId, int databaseId)
        {
            throw new NotImplementedException();
        }

        #endregion

       
    }
}
