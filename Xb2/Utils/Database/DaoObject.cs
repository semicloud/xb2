using System;
using System.Collections.Generic;
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
        public static DataTable GetSubDatabaseInfos(int userId)
        {
            var commandText = "select * from {0} where 用户编号={1}";
            commandText = string.Format(commandText, DbHelper.TnSubDb(), userId);
            Logger.Debug(commandText);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
            Logger.Info("查询用户 {0} 的地震目录子库， 返回 {1} 条记录", userId, dt.Rows.Count);
            return DataTableHelper.IdentifyDataTable(dt);
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
        /// 根据用户编号和地震目录子库名称删除地震目录子库，然后继续删除地震子库数据
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="databaseId">地震目录子库编号</param>
        /// <param name="databaseName">地震目录子库名称</param>
        /// <returns></returns>
        public static bool DeleteSubDatabase(int userId, int databaseId, string databaseName)
        {
            // 注意：只需从主表中删除即可
            // 因为有外键关联，删除主表中的内容即可以删除附表中的数据了
            var commandText = "delete from {0} where 用户编号={1} and 子库名称='{2}'";
            commandText = string.Format(commandText, DbHelper.TnSubDb(), userId, databaseName);
            Logger.Debug(commandText);
            var isDatabaseDeleted = MySqlHelper.ExecuteNonQuery(DbHelper.ConnectionString, commandText) > 0;
            Logger.Info("删除用户 {0} 的地震目录子库 {1}，结果：{2}", userId, databaseName, isDatabaseDeleted);
            return isDatabaseDeleted;
        }

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
        /// 获取userId用户的所有地震目录库，显示为地震标注库
        /// 1. 地震目录总库
        /// 2. 地震目录子库
        /// 3. 地震目录标注库
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>返回DataTable，由两列构成：子库名称，类别</returns>
        public static DataTable GetLabelDatabaseInfosAll(int userId)
        {
            var commandText = "select 子库名称 from 系统_地震目录子库 where 用户编号=" + userId;
            var dtMain = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
            dtMain.Columns.Add("类别", typeof(string));
            dtMain.FillColumn("类别", "子库");
            var subDatabaseCount = dtMain.Rows.Count;
            // 应甲方要求，将标注库也放在标注库里
            commandText = "select 标注库名称 from 系统_地震目录标注库 where 用户编号=" + userId;
            var dtLabelDatabase = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
            var labelDatabaseCount = dtLabelDatabase.Rows.Count;
            for (int i = 0; i < dtLabelDatabase.Rows.Count; i++)
            {
                var row = dtMain.NewRow();
                row["子库名称"] = dtLabelDatabase.Rows[i]["标注库名称"].ToString();
                row["类别"] = "标注库";
                dtMain.Rows.Add(row);
            }
            // 应甲方要求，地震目录也放在子库里，即用户可以选择从直接从地震目录中生成子库
            var dataRow = dtMain.NewRow();
            dataRow["子库名称"] = DbHelper.TnCategory();
            dataRow["类别"] = "地震目录";
            dtMain.Rows.InsertAt(dataRow, 0);
            // 增加序号列
            dtMain = DataTableHelper.IdentifyDataTable(dtMain);
            Logger.Info("查询用户 {0} 的地震子库，标注库信息：子库 {1} 个，" +
                        "标注库 {2} 个，共 {3} 个（加入了地震目录总库）",
                userId, subDatabaseCount, labelDatabaseCount, dtMain.Rows.Count);
            return dtMain;
        }

        /// <summary>
        /// 获取用户标注库信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DataTable GetLabelDatabasesInfo(int userId)
        {
            var commandText = "select * from {0} " + "where 用户编号={1}";
            commandText = string.Format(commandText, DbHelper.TnLabelDb(), userId);
            Logger.Debug(commandText);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
            Logger.Info("查询用户 {0} 的标注库信息，返回 {1} 条", userId, dt.Rows.Count);
            return dt;
        }

        /// <summary>
        /// 保存用户新建的标注库
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="databaseName">标注库名称</param>
        /// <param name="dataTable">用户选择的地震目录</param>
        /// <returns></returns>
        public static int SaveLabelDatabase(int userId, string databaseName, DataTable dataTable)
        {
            // 保存用户新建的标注库操作分为两步
            // 1. 保存标注库基本信息：用户编号和标注库名，返回标注库编号(标注库名不允许重复)
            // 2. 利用返回的标注库编号将标注库数据存入 系统_地震目录标注库数据 表

            #region 查询是否存在同名的地震目录标注库

            var commandText = "select * from 系统_地震目录标注库 where 用户编号=" + userId + " and 标注库名称='" + databaseName + "'";
            var exists = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0].Rows.Count > 0;
            Logger.Debug(commandText);
            Logger.Info("查询用户 {0} 是否已经存在名为 {1} 的地震目录标注库？=>{2}", userId, databaseName, exists);

            #endregion

            if (!exists)
            {
                #region 如果不存在同名标注库，则新建地震目录标注库 记录

                commandText = "insert into 系统_地震目录标注库(用户编号,标注库名称) values (" + userId + ",'" + databaseName + "')";
                var isOk = MySqlHelper.ExecuteNonQuery(DbHelper.ConnectionString, commandText) > 0;
                Logger.Debug(commandText);
                Logger.Info("创建地震目录标注库记录：用户编号：{0}，标注库名称：{1}，创建结果：{2}", userId, databaseName, isOk);

                #endregion

                if (isOk)
                {
                    #region 获取新建的地震目录标注库编号

                    commandText = "select * from 系统_地震目录标注库 where 用户编号=" + userId + " and 标注库名称='" + databaseName + "'";
                    var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
                    var databaseId = Convert.ToInt32(dt.Rows[0]["编号"]);
                    Logger.Debug(commandText);
                    Logger.Info("查询用户 {0} 的名称为 {1} 的地震目录标注库编号为 {2}", userId, databaseName, databaseId);

                    #endregion

                    #region 保存地震目录标注库数据

                    Logger.Info("开始保存地震目录标注库数据...，库编号：{0}，共 {1} 条地震目录...", databaseId, dataTable.Rows.Count);
                    dt = new DataTable();
                    commandText = "select * from 系统_地震目录标注库数据 where 标注库编号=" + databaseId;
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
                            dr["标注库编号"] = databaseId;
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
                    Logger.Info("保存地震目录标注库数据结果：" + isSave);
                    if (isSave)
                    {
                        return 0;
                    }
                    else
                    {
                        Logger.Error("创建地震目录标注库 数据 失败");
                        return 3;
                    }

                    #endregion
                }
                else
                {
                    Logger.Error("创建地震目录标注库 记录 失败");
                    return 2;
                }
            }
            else
            {
                Logger.Warn("用户 {0} 已存在名为 {1} 的地震目录标注库");
                return 1;
            }
        }

        /// <summary>
        /// 在标注库中删除一个地震目录
        /// </summary>
        /// <param name="recordId">地震目录编号</param>
        /// <returns></returns>
        public static bool DeleteLabelDatabaseRecord(int recordId)
        {
            var commandText = "delete from {0} where 编号={1}";
            commandText = string.Format(commandText, DbHelper.TnLabelDbData(), recordId);
            Logger.Debug(commandText);
            var ans = MySqlHelper.ExecuteNonQuery(DbHelper.ConnectionString, commandText) > 0;
            Logger.Info("从标注库数据中删除编号为 {0} 的地震目录，结果：{1}", recordId, ans);
            return ans;
        }

        public static bool CreateOrSaveLabelDatabaseRecord()
        {
            // 暂缓实现
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
        /// <param name="userId">用户编号</param>
        /// <param name="databaseId">标注库编号</param>
        /// <param name="databaseName">标注库名称</param>
        /// <returns></returns>
        public static DataTable GetCategoriesFromLabelDatabase(int userId,int databaseId, string databaseName)
        {
            var commandText =
                "select 编号, date(发震日期) as 发震日期,time(发震时间) " +
                "as 发震时间,经度,纬度,震级单位,round(震级值,1) as 震级值,定位参数,参考地点 from {0}  where 标注库编号={1}";
            commandText = string.Format(commandText, DbHelper.TnLabelDbData(), databaseId);
            Logger.Debug(commandText);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
            Logger.Info("查询用户 {0} 的编号为 {1} 的地震目录标注库，共返回 {2} 条记录", userId, databaseId, dt.Rows.Count);
            return dt;
        }

        #endregion

        #endregion

        #region 测项相关操作

        /// <summary>
        /// 查询测项编号，返回测项的字符串表示：
        /// 测项编号,观测单位,地名,方法名,测项名
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static string GetMItemDescription(int itemId)
        {
            var commandText = "select 编号,观测单位,地名,方法名,测项名 from {0} where 编号={1}";
            commandText = string.Format(commandText, DbHelper.TnMItem(), itemId);
            Logger.Debug(commandText);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
            if (dt.Rows.Count < 1)
            {
                Logger.Error("没有查询到编号为 {0} 的测项信息！", itemId);
                return "错误！";
            }
            var description = String.Format("{0}，{1}，{2}，{3}，{4}", dt.Rows[0]["编号"], dt.Rows[0]["观测单位"],
                dt.Rows[0]["地名"], dt.Rows[0]["方法名"], dt.Rows[0]["测项名"]);
            Logger.Info("测项 {0} 的描述：{1}", itemId, description);
            return description;
        }

        /// <summary>
        /// 根据测项编号列表返回测项DataTable
        /// 返回的测项表中应有权重信息
        /// </summary>
        /// <param name="itemIdList"></param>
        /// <returns></returns>
        public static DataTable GetMItemById(List<int> itemIdList)
        {
            var commandText = "select 编号 as 测项编号,观测单位,地名,方法名,测项名 from {0} where 编号 in ({1}) order by 编号";
            commandText = String.Format(commandText, DbHelper.TnMItem(), String.Join(",", itemIdList));
            Logger.Info(commandText);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
            Logger.Debug(commandText);
            Logger.Debug("Input: ItemIdList={0}, Returns {1} Records", String.Join(",", itemIdList), dt.Rows.Count);
            return dt;
        }

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
            var commandText = string.Format("select * from {0} where 测项编号={1} order by 观测日期",
                DbHelper.TnRData(), mitemId);
            Logger.Debug(commandText);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
            Logger.Info("查询测项 {0} 的原始数据，共返回 {1} 条观测数据", mitemId, dt.Rows.Count);
            return dt;
        }

        /// <summary>
        /// 创建某测项的原始数据
        /// </summary>
        /// <param name="itemId">测项编号</param>
        /// <param name="date">观测日期</param>
        /// <param name="value">观测值</param>
        /// <param name="memo1">备注1</param>
        /// <param name="memo2">备注2</param>
        /// <returns></returns>
        public static bool CreateRawData(int itemId, DateTime date, double value, object memo1, object memo2)
        {
            var commandText = "select * from {0} where 测项编号={1}";
            commandText = string.Format(commandText, DbHelper.TnRData(), itemId);
            var dt = new DataTable();
            var adapter = new MySqlDataAdapter(commandText, DbHelper.ConnectionString);
            var builder = new MySqlCommandBuilder(adapter);
            adapter.Fill(dt);
            var row = dt.NewRow();
            row["测项编号"] = itemId;
            row["观测日期"] = date;
            row["观测值"] = value;
            row["备注1"] = memo1;
            row["备注2"] = memo2;
            dt.Rows.Add(row);
            Logger.Debug(commandText);
            var n = adapter.Update(dt);
            Logger.Info("新建测项 {0} 的原始数据，Date：{1}，Value：{2}，Memo1：{3}，Memo2：{4}，返回：{5}",
                itemId, date.ToShortDateString(), value, memo1, memo2, n > 0);
            return n > 0;
        }

        /// <summary>
        /// 更新某测项的原始数据
        /// </summary>
        /// <param name="itemId">测项编号，BTW，其实只有原始数据编号就够用了</param>
        /// <param name="dataId">原始数据编号</param>
        /// <param name="date">观测日期</param>
        /// <param name="value">观测值</param>
        /// <param name="memo1">备注1</param>
        /// <param name="memo2">备注2</param>
        /// <returns></returns>
        public static bool EditSaveRawData(int itemId, int dataId, DateTime date, double value, object memo1,
            object memo2)
        {
            var commandText = "select * from {0} where 编号={1}";
            var id = dataId;
            commandText = string.Format(commandText, DbHelper.TnRData(), id);
            Debug.Print(commandText);
            var dt = new DataTable();
            var adapter = new MySqlDataAdapter(commandText, DbHelper.ConnectionString);
            var builder = new MySqlCommandBuilder(adapter);
            adapter.Fill(dt);
            var row = dt.Rows[0];
            row["观测日期"] = date;
            row["观测值"] = value;
            row["备注1"] = memo1;
            row["备注2"] = memo2;
            var n = adapter.Update(dt);
            Logger.Debug(commandText);
            Logger.Info("更新编号为 {0} 的原始数据，Date：{1}，Value：{2}，Memo1：{3}，Memo2：{4}，返回：{5}",
                dataId, date.ToShortDateString(), value, memo1, memo2, n > 0);
            return n > 0;
        }

        #endregion

        #region 基础数据库相关

        /// <summary>
        /// 获得某用户某测项的默认基础数据库编号
        /// 如果没有默认基础数据库，就使用原始数据库，编号是-1
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static int GetDefaultProcessedDatabaseId(int userId, int itemId)
        {
            var databaseId = int.MinValue;
            var commandText = "select 编号 from {0} where 用户编号={1} and 测项编号={2} and 是否默认=1";
            commandText = String.Format(commandText, DbHelper.TnProcessedDb(), userId, itemId);
            Logger.Debug(commandText);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
            if (dt.Rows.Count == 1)
            {
                databaseId = Convert.ToInt32(dt.Rows[0]["编号"]);
                Logger.Info("查询用户 {0} 测项编号为 {1} 的默认基础数据库编号为 {2}", userId, itemId, databaseId);
            }
            else
            {
                Debug.Assert(dt.Rows.Count==0,"dt.Rows.Count should be 0.");
                databaseId = -1;
                Logger.Info("未查询到用户 {0} 在测项 {1} 中有基础数据，使用原始数据", userId, itemId);
            }
            return databaseId;
        }

        public static Dictionary<int, string> GetDefaultProcessedDatabaseIdAndName(int userId, int itemId)
        {
            var dictionary = new Dictionary<int, string>();
            var commandText = "select 编号,库名 from {0} where 用户编号={1} and 测项编号={2} and 是否默认=1";
            commandText = String.Format(commandText, DbHelper.TnProcessedDb(), userId, itemId);
            Logger.Debug(commandText);
            var dt = MySqlHelper.ExecuteDataset(DbHelper.ConnectionString, commandText).Tables[0];
            int databaseId;
            string databaseName;
            if (dt.Rows.Count == 1)
            {
                databaseId = Convert.ToInt32(dt.Rows[0]["编号"]);
                databaseName = Convert.ToString(dt.Rows[0]["库名"]);
                dictionary.Add(databaseId, databaseName);
            }
            else
            {
                databaseId = -1;
                databaseName = "原始数据";
                dictionary.Add(databaseId, databaseName);
            }
            Logger.Debug("Input: UserId={0}, ItemId={1},Query Default Db Count={2}, Return: ,DbId={3}, Default Db Name={4}",
                userId, itemId, dt.Rows.Count, databaseId, databaseName);
            return dictionary;
        }



        public static int GetDefaultFreq(int userId, int itemId, int databaseId)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 获取参与计算的数据
        /// </summary>
        /// <param name="itemId">测项编号</param>
        /// <param name="databaseId">基础数据库编号，若基础数据库取-1，则取原始数据库数据</param>
        /// <returns></returns>
        public static DataTable GetData(int itemId, int databaseId)
        {
            return databaseId != -1 ? GetProcessedData(databaseId) : GetRawData(itemId);
        }

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
            Logger.Debug("GetUserProcessedDatabaseInfos,CommandText={0}", commandText);
            Logger.Debug("GetUserProcessedDatabaseInfos,Input: UserId={0},ItemId={1},ColNames={2},Return {3} Records."
                , userId, mitemId, String.Join(",", columnNames), dt.Rows.Count);
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
