using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using Itenso.TimePeriod;
using MySql.Data.MySqlClient;
using NLog;

namespace Xb2.Utils.Database
{
    /// <summary>
    /// Data Access Object
    /// </summary>
    public static class DaoObject
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region 用户相关操作

        public static int GetUserId(string userName, string password, bool isAdmin)
        {
            var ans = -1;
            try
            {
                var sql = "select 编号 from 系统_用户 where 用户名='{1}' and 密码='{2}' and 管理员={3}";
                var commandText = String.Format(sql, "系统_用户", userName, password, isAdmin);
                var obj = MySqlHelper.ExecuteScalar(DbHelper.ConnectionString, commandText);
                if (obj != null) ans = Convert.ToInt32(obj);
                Logger.Info("查询用户编号[{0},{1},{2}], 返回 {3}", userName, password, isAdmin, ans);
                Logger.Debug(commandText);
                return ans;
            }
            catch (MySqlException e)
            {
                Logger.Error("连接数据库出现异常：" + e.Message);
                MessageBox.Show(e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return -9999;
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

        /// <summary>
        /// 提取地震目录的最小和最大发震日期
        /// </summary>
        /// <param name="tableName">数据库名，或为地震目录总库名，或为地震目录子库名，或为地震目录标注库名</param>
        /// <returns></returns>
        public static TimeRange GetEarthquakeDateRange(string tableName)
        {
            var commandText = string.Format("select min(发震日期) as mindate ,max(发震日期) as maxdate from {0}", tableName);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
            var minDate = Convert.ToDateTime(dt.Rows[0]["mindate"]);
            var maxDate = Convert.ToDateTime(dt.Rows[0]["maxdate"]);
            Logger.Debug(commandText);
            Logger.Info("查询 {0} 的最小和最大发震日期：{1}~{2}", tableName,
                minDate.ToShortDateString(), maxDate.ToShortDateString());
            var timeRange = new TimeRange(minDate, maxDate);
            return timeRange;
        }

        /// <summary>
        /// 提取地震目录的最小和最大震级
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static double[] GetEarthquakeMagnitudeRange(string tableName)
        {
            var commandText = string.Format("select min(震级值) as minm,max(震级值) as maxm from {0}", tableName);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
            var minMagnitude = Convert.ToDouble(dt.Rows[0]["minm"]);
            var maxMagnitude = Convert.ToDouble(dt.Rows[0]["maxm"]);
            Logger.Debug(commandText);
            Logger.Info("查询 {0} 的最小和最大震级：{1}~{2}", tableName, minMagnitude, maxMagnitude);
            return new[] {minMagnitude, maxMagnitude};
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
        /// 保存用户新建的地震目录子库
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="databaseName">地震目录子库名称</param>
        /// <param name="dataTable">用户选择的地震目录</param>
        /// <returns>
        /// 0:地震目录子库创建成功
        /// 1:已存在同名的地震目录子库
        /// 2:创建地震目录子库 记录 失败
        /// 3:创建地震目录子库 数据 失败
        /// </returns>
        public static int SaveSubDatabase(int userId, string databaseName, DataTable dataTable)
        {
            // 保存用户新建的子库操作分为两步
            // 1. 保存子库基本信息：用户编号和子库名，返回子库编号(子库名不允许重复)
            // 2. 利用返回的子库编号将子库数据存入 系统_地震目录子库数据 表

            #region 查询是否存在同名的地震目录子库

            var commandText = "select * from 系统_地震目录子库 where 用户编号=" + userId + " and 子库名称='" + databaseName + "'";
            var exists = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0].Rows.Count > 0;
            Logger.Debug(commandText);
            Logger.Info("查询用户 {0} 是否已经存在名为 {1} 的地震目录子库？=>{2}", userId, databaseName, exists);

            #endregion
            
            if (!exists)
            {
                #region 如果不存在同名子库，则新建地震目录子库 记录

                commandText = "insert into 系统_地震目录子库(用户编号,子库名称) values (" + userId + ",'" + databaseName + "')";
                var isOk = MySqlHelper.ExecuteNonQuery(DbHelper.ConnectionString, commandText) > 0;
                Logger.Debug(commandText);
                Logger.Info("创建地震目录子库记录：用户编号：{0}，子库名称：{1}，创建结果：{2}", userId, databaseName, isOk);

                #endregion
                
                if (isOk)
                {
                    #region 获取新建的地震目录子库编号

                    commandText = "select * from 系统_地震目录子库 where 用户编号=" + userId + " and 子库名称='" + databaseName + "'";
                    var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
                    var databaseId = Convert.ToInt32(dt.Rows[0]["编号"]);
                    Logger.Debug(commandText);
                    Logger.Info("查询用户 {0} 的名称为 {1} 的地震目录子库编号为 {2}", userId, databaseName, databaseId);

                    #endregion

                    #region 保存地震目录子库数据

                    Logger.Info("开始保存地震目录子库数据...，库编号：{0}，共 {1} 条地震目录...", databaseId, dataTable.Rows.Count);
                    dt = new DataTable();
                    commandText = "select * from 系统_地震目录子库数据 where 子库编号=" + databaseId;
                    var adapter = new MySqlDataAdapter(commandText, DbHelper.ConnectionString);
                    var builder = new MySqlCommandBuilder(adapter);
                    adapter.Fill(dt);

                    #region 填充 DataTable

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        //找到选中的数据行，只保存选中的地震目录
                        if (Convert.ToInt32(dataTable.Rows[i]["选择"]) == 1)
                        {
                            DataRow dr = dt.NewRow();
                            //由于从数据库中截取了发震时间的时间部分，所有在这里得把
                            //日期补上，才能重新填入数据库中
                            var date = Convert.ToDateTime(dataTable.Rows[i]["发震日期"]);
                            var time = DateTime.ParseExact(dataTable.Rows[i]["发震时间"].ToString(), "HH:mm:ss",
                                CultureInfo.InvariantCulture);
                            dr["子库编号"] = databaseId;
                            dr["发震日期"] = Convert.ToDateTime(dataTable.Rows[i]["发震日期"]);
                            dr["发震时间"] = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute,
                                time.Second);
                            dr["纬度"] = Convert.ToInt32(dataTable.Rows[i]["纬度"]);
                            dr["经度"] = Convert.ToInt32(dataTable.Rows[i]["经度"]);
                            dr["震级单位"] = Convert.ToString(dataTable.Rows[i]["震级单位"]);
                            dr["震级值"] = Convert.ToSingle(dataTable.Rows[i]["震级值"]);
                            dr["定位参数"] = Convert.ToInt32(dataTable.Rows[i]["定位参数"]);
                            dr["参考地点"] = Convert.ToString(dataTable.Rows[i]["参考地点"]);
                            dt.Rows.Add(dr);
                        }
                    }

                    #endregion

                    var isSave = adapter.Update(dt) > 0;
                    Logger.Info("保存地震目录子库数据结果：" + isSave);
                    if (isSave)
                    {
                        return 0;
                    }
                    else
                    {
                        Logger.Error("创建地震目录子库 数据 失败");
                        return 3;
                    }

                    #endregion
                }
                else
                {
                    Logger.Error("创建地震目录子库 记录 失败");
                    return 2;
                }
            }
            else
            {
                Logger.Warn("用户 {0} 已存在名为 {1} 的地震目录子库");
                return 1;
            }
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
            var commandText = String.Format(sql, DbHelper.TnLabelDb(), userId, labelDbName);
            var ans = MySqlHelper.ExecuteScalar(DbHelper.ConnectionString, commandText);
            var id = ans != null ? Convert.ToInt32(ans) : -1;
            Logger.Info("查询用户 {0} 的名称为 {1} 的标注库编号为 {2}. 返回-1表示该标注库不存在", userId, labelDbName, id);
            Logger.Debug(commandText);
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
            var commandText = string.Format("select 编号,观测日期,观测值 from {0} where 测项编号={1} order by 观测日期",
                DbHelper.TnRData(), mitemId);
            Debug.Print(commandText);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
            Logger.Info("查询测项 {0} 的原始数据，共返回 {1} 条观测数据", mitemId, dt.Rows.Count);
            Logger.Debug(commandText);
            return dt;
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
            var sql = "select 编号,观测日期,观测值 from {0} where 库编号={1} order by 观测日期";
            var commandText = string.Format(sql, DbHelper.TnProcessedDbData(), databaseId);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
            Logger.Debug(commandText);
            Logger.Info("查询编号为 {0} 的基础数据库，共返回 {1} 条观测数据", databaseId, dt.Rows.Count);
            return dt;
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
        /// 获取用户某测项的基础数据库信息
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="mitemId">测项编号</param>
        /// <param name="columnNames">要查询的列名</param>
        /// <returns></returns>
        public static DataTable GetUserProcessedDatabaseInfos(int userId, int mitemId, params string[] columnNames)
        {
            var sql = "select {0} from {1} where 用户编号={2} and 测项编号={3}";
            var colNames = string.Join(",", columnNames);
            var commandText = string.Format(sql, colNames, DbHelper.TnProcessedDb(), userId, mitemId);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
            Logger.Info("查询用户{0}，测项{1}的基础数据库信息共{2}个", userId, mitemId, dt.Rows.Count);
            Logger.Debug(commandText);
            return dt;
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
