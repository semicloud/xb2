using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Xb2.Config;
using Xb2.Utils;
using Xb2.Utils.Database;

namespace Xb2.Entity.Business.Catalog
{
    public class Q01File
    {
        private static readonly string Q01_PATH = @"$q01files";

        public Int32 ID { get; private set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime Lower { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime Upper { get; set; }

        /// <summary>
        /// 是否已经导入
        /// </summary>
        public bool IsImportedToDb { get; set; }

        /// <summary>
        /// 添加日期
        /// </summary>
        public DateTime AddDate { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 记录数
        /// </summary>
        public Int32 RecordCount { get; set; }

        private bool Delete()
        {
            //var sql = "delete from Q01文件 where 编号=" + this.ID;
            //var ret = MySqlHelper.ExecuteNonQuery(Xb2Config.GetConnStr(), sql);
            //return ret > 0;
            return false;
        }

        private bool Update()
        {
            //var sql = "update Q01文件 set 文件名=@文件名, 已导入数据库=@已导入数据库 where 编号=" + this.ID;
            //var parameters = new List<MySqlParameter>
            //{
            //    new MySqlParameter {ParameterName = "@文件名", MySqlDbType = MySqlDbType.VarChar, Value = this.Name},
            //    new MySqlParameter
            //    {
            //        ParameterName = "@已导入数据库",
            //        DbType = DbType.Boolean,
            //        Value = this.IsImportedToDb
            //    }
            //};
            //var ret = MySqlHelper.ExecuteNonQuery(Xb2Config.GetConnStr(), sql, parameters.ToArray());
            //return ret > 0;
            return false;
        }

        private bool Insert()
        {
            var sql = "select * from q01文件";
            var dt = new DataTable();
            var adapter = new MySqlDataAdapter(sql, Db.CStr());
            var commandBuilder = new MySqlCommandBuilder(adapter);
            adapter.Fill(dt);
            var dr = dt.NewRow();
            dr["文件名"] = this.Name;
            dr["开始日期"] = this.Lower;
            dr["结束日期"] = this.Upper;
            dr["记录数"] = this.RecordCount;
            dr["加入时间"] = this.AddDate;
            dr["用户"] = this.UserName;
            dr["已导入数据库"] = false;
            dt.Rows.Add(dr);
            return adapter.Update(dt) > 0;
        }

        //插入数据库，并去重（目前还没做，咨询甲方）
        public static bool ImportToDatabase(string fileName)
        {
            return false;
        }

        private static DataTable GetDataTableFromFile(string fileName)
        {
            string sql = "select * from 地震目录";
            DataTable dataTable = MySqlHelper.ExecuteDataset(Xb2Config.GetConnStr(), sql).Tables[0];
            //只需要DataTable的架构，数据行丢掉
            dataTable = dataTable.Clone();
            if (File.Exists(fileName))
            {
                var lines = File.ReadAllLines(fileName, Encoding.UTF8);
                foreach (var line in lines)
                {
                    var row = dataTable.NewRow();
                    row["发震日期"] = DateTime.ParseExact(ParseRecord(line, 0, 8), "yyyyMMdd", CultureInfo.InvariantCulture);
                    row["发震时间"] = DateTime.ParseExact(ParseRecord(line, 0, 14), "yyyyMMddHHmmss",
                        CultureInfo.InvariantCulture);
                    row["纬度"] = Int32.Parse(ParseRecord(line, 17, 4));
                    row["经度"] = Int32.Parse(ParseRecord(line, 22, 5));
                    row["震级单位"] = ParseRecord(line, 28, 2);
                    string sm = ParseRecord(line, 30, 3).Trim();
                    if (sm.Equals("null"))
                    {
                        row["震级值"] = double.NaN;
                    }
                    else
                    {
                        row["震级值"] = Double.Parse(sm);
                    }
                    row["定位参数"] = Int32.Parse(ParseRecord(line, 33, 10).Replace(" ", "").Trim().ToDbc());
                    row["参考地点"] = ParseRecord(line, 43, -1);
                    dataTable.Rows.Add(row);
                }
                return dataTable;
            }
            throw new Exception("找不到文件");
        }

        private static string ParseRecord(String line, int start, int length)
        {
            string scalar;
            if (length == -1)
            {
                //最后一个数据项的处理
                scalar = line.Substring(start).Trim();
            }
            else
            {
                //非最后一行数据项的处理
                scalar = line.Substring(start, length).Trim();
            }
            return scalar.Equals("") ? "null" : scalar;
        }

        //删除系统内的Q01文件，并删除数据库中的记录
        public static bool DeleteQ01File(String fileName)
        {
            bool isSaved = false;
            bool isDeleted = false;
            DirectoryInfo directory = new DirectoryInfo(Q01_PATH);
            string fullFileName = directory.FullName + "\\" + fileName;
            if (File.Exists(fullFileName))
            {
                File.Delete(fullFileName);
                Debug.Print("delete {0}", fullFileName);
                string sql = "delete from Q01文件 where 文件名='" + fileName + "'";
                isSaved = MySqlHelper.ExecuteNonQuery(Xb2Config.GetConnStr(),  sql) > 0;
                Debug.Print("delete q01 file record in database, {0}", isSaved);
                isDeleted = !File.Exists(fullFileName);
                return isSaved & isDeleted;
            }
            return false;
        }

        //按照发震时间 从数据库中卸载安装的Q01
        public static bool UnimportQ01RecordFromDb(String fileName)
        {
            bool isRemovedFromDb = false;
            bool isSaved = false;
            DirectoryInfo directoryInfo = new DirectoryInfo(Q01_PATH);
            String fullFileName = directoryInfo.FullName + "\\" + fileName;
            if (File.Exists(fullFileName))
            {
                String[] lines = File.ReadAllLines(fullFileName);
                Debug.Print("Read {0} lines from {1}", lines.Length, fullFileName);
                Func<String, DateTime> Parse = s => DateTime.ParseExact(s, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                DateTime lower = Parse(lines[0].Substring(0, 14));
                DateTime upper = Parse(lines[lines.Length - 1].Substring(0, 14));
                if (lower > upper)
                {
                    DateTime temp = upper;
                    upper = lower;
                    lower = temp;
                }
                Debug.Print("lower date:{0}, upper date:{1}", lower.Date.ToShortDateString(), upper.Date.ToShortDateString());
                String sql = "select count(*) from 地震目录 where 发震时间 between #" + lower.ToString("yyyy-MM-dd HH:mm:ss") + "# and #" + upper.ToString("yyyy-MM-dd HH:mm:ss") + "#";
                var count = MySqlHelper.ExecuteScalar(Xb2Config.GetConnStr(),  sql);
                Debug.Print("will delete {0} rows", count);
                sql = "delete from 地震目录 where 发震时间 between #" + lower.ToString("yyyy-MM-dd HH:mm:ss") + "# and #" + upper.ToString("yyyy-MM-dd HH:mm:ss") + "#";
                Debug.Print(sql);
                var affectedRows = MySqlHelper.ExecuteNonQuery(Xb2Config.GetConnStr(),  sql);
                isRemovedFromDb = affectedRows > 0;
                if (isRemovedFromDb)
                {
                    sql = "update Q01文件 set 已导入数据库=0 where 文件名='" + fileName + "'";
                    isSaved = MySqlHelper.ExecuteNonQuery(Xb2Config.GetConnStr(), sql) > 0;
                }
                return isRemovedFromDb & isSaved;
            }
            throw new FileNotFoundException("找不到文件【" + fullFileName + "】！");
        }

        //修改Q01的文件名，并修改数据库中的记录
        public static bool ChangeQ01FileName(int id,string old, string newer)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Q01_PATH);
            string oldy = directoryInfo.FullName + "\\" + old;
            string newy = directoryInfo.FullName + "\\" + newer;
            bool isChanged = false;
            if (File.Exists(oldy))
            {
                //复制一个，同时删除原文件，相当于改名字
                File.Copy(oldy, newy, true);
                File.Delete(oldy);
                String sql = "update Q01文件 set 文件名='" + newer + "' where 编号=" + id;
                isChanged = MySqlHelper.ExecuteNonQuery(Xb2Config.GetConnStr(), sql) > 0;
                return isChanged;
            }
            else
            {
                MessageBox.Show("找不到【" + oldy + "】文件！");
            }
            return isChanged;
        }

        /// <summary>
        /// 复制源Q01文件至软件内,并保存在数据库中
        /// </summary>
        /// <param name="q01SrcPathName"></param>
        /// <returns></returns>
        public static bool AddQ01File(String q01SrcPathName, XbUser user)
        {
            string to;
            bool isCopy = CopyFileToAppFolder(q01SrcPathName, out to);
            if (isCopy)
            {
                bool isSave = SaveQ01ToDatabase(to, user);
                return isSave;
            }
            return false;
        }

        /// <summary>
        /// 将Q01文件拷贝至工程根目录的Q01Files文件夹中
        /// </summary>
        /// <param name="q01SrcPathName"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private static bool CopyFileToAppFolder(string q01SrcPathName, out string to)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Q01_PATH);
            string from = q01SrcPathName;
            to = directoryInfo.FullName + "\\" + Path.GetFileName(q01SrcPathName);
            bool isCopied;
            if (File.Exists(to))
            {
                MessageBox.Show("已存在名为【" + Path.GetFileName(to) + "】的文件，请改个名字再加入！", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            File.Copy(from, to);
            Debug.Print("copy file {0} to {1}", from, to);
            isCopied = File.Exists(to);
            Debug.Print("has file {0} : {1}", to, isCopied);
            return isCopied;
        }

        /// <summary>
        /// 将Q01文件记录写入数据库
        /// </summary>
        /// <param name="to"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private static bool SaveQ01ToDatabase(string to, XbUser user)
        {
            String[] lines = File.ReadAllLines(to);
            Debug.Print("reading {0} lines from {1}", lines.Length, to);
            Q01File q01File = new Q01File();
            q01File.Name = Path.GetFileName(to);
            q01File.AddDate = DateTime.Now.Date;
            q01File.RecordCount = lines.Length;
            q01File.Lower =
                DateTime.ParseExact(lines[0].Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture).Date;
            q01File.Upper =
                DateTime.ParseExact(lines[lines.Length - 1].Substring(0, 8), "yyyyMMdd",
                    CultureInfo.InvariantCulture).Date;
            q01File.IsImportedToDb = false;
            q01File.UserName = user.Name;
            bool isSaved = q01File.Insert();
            if (isSaved)
            {
                Debug.Print("save Q01File {0} to database success!", to);
            }
            return isSaved;
        }

       

        #region Obsolete Codes
       
        [Obsolete]
        public static bool CopyToAppResourceFolder(String q01SrcPathName, XbUser user)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Q01_PATH);
            string from = q01SrcPathName;
            String to = directoryInfo.FullName + "\\" + Path.GetFileName(q01SrcPathName) + ".q01";
            var isCopied = false;
            if (File.Exists(to))
            {
                MessageBox.Show("已存在名为【" + Path.GetFileName(to) + "】的q01文件，请改个名字再加入！", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
                //if (dialogResult == DialogResult.Yes)
                //{
                //    File.Delete(to);
                //    Debug.Print("delete file {0}", to);
                //    File.Copy(from, to);
                //    Debug.Print("copy file {0} to {1}", from, to);
                //    isCopied = File.Exists(to);
                //    Debug.Print("has file {0} : {1}", to, isCopied);
                //}
                //if (dialogResult == DialogResult.No)
                //{
                //    var saveFileDialog = new SaveFileDialog {InitialDirectory = Path.GetDirectoryName(to)};
                //    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                //    {
                //        to = saveFileDialog.FileName;
                //        File.Copy(from, to);
                //        Debug.Print("copy file {0} to {1}", from, to);
                //        isCopied = File.Exists(to);
                //        Debug.Print("has file {0} : {1}", to, isCopied);
                //    }
                //}
                //if (dialogResult == DialogResult.Cancel)
                //{
                //    return false;
                //}
            }
            File.Copy(from, to);
            Debug.Print("copy file {0} to {1}", from, to);
            isCopied = File.Exists(to);
            Debug.Print("has file {0} : {1}", to, isCopied);
            if (isCopied)
            {
                String[] lines = File.ReadAllLines(to);
                Debug.Print("reading {0} lines from {1}", lines.Length, to);
                Q01File q01File = new Q01File();
                q01File.Name = Path.GetFileName(to);
                q01File.AddDate = DateTime.Now.Date;
                q01File.RecordCount = lines.Length;
                q01File.Lower =
                    DateTime.ParseExact(lines[0].Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                q01File.Upper =
                    DateTime.ParseExact(lines[lines.Length - 1].Substring(0, 8), "yyyyMMdd",
                        CultureInfo.InvariantCulture).Date;
                q01File.IsImportedToDb = false;
                q01File.UserName = user.Name;
                bool isSaved = q01File.Insert();
                if (isSaved)
                {
                    Debug.Print("save Q01File {0} to database success!", to);
                }
                else
                {
                    Debug.Fail("save Q01File {0} to database failed!", to);
                }
            }
            return isCopied;
        }
        
        #endregion
    }

}
