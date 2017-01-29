﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NLog;
using Xb2.Algorithms.Core.Entity;
using Xb2.Entity.Business;
using Xb2.GUI.M.Item;
using Xb2.GUI.Main;
using Xb2.TestAndDemos;

namespace Xb2.GUI.Input
{
    public partial class FrmXQSInput : FrmBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private List<XQSInput> _xqsInputs;

        /// <summary>
        /// 获取消趋势输入
        /// </summary>
        /// <returns></returns>
        public List<XQSInput> GetXqsInputs()
        {
            throw new NotImplementedException();
        }

        public int ItemId { get; set; }

        public FrmXQSInput(XbUser user)
        {
            InitializeComponent();
            this.User = user;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmSelectMItem frmSelectMItem = new FrmSelectMItem(this.User)
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            var dialogResult = frmSelectMItem.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                var dt = frmSelectMItem.Result;
                this.ItemId = Convert.ToInt32(dt.Rows[0]["编号"]);
                Logger.Info("用户选择了测项 {0} ", this.ItemId);
                button1.Text = "测项编号：" + ItemId;

                if (this.ItemId == 0)
                {
                    MessageBox.Show("没有选择测项！");
                    return;
                }
                Form1 form1 = new Form1(this.User, this.ItemId,
                    dateTimePicker1.Value, dateTimePicker2.Value);
                form1.ShowDialog();
            }
        }

        private void dateTimePicker2_Leave(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }

    // 消趋势算法输入
    public class XQSInput
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// 选择的测项编号
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// 基础数据库编号，-1表示原始数据，非0正数表示基础数据库的编号
        /// </summary>
        public int DatabaseId { get; set; }

        /// <summary>
        /// 观测数据，或者来自原始数据，或者来自基础数据
        /// </summary>
        public DateValueList InputData { get; set; }

        // 窗长
        public int WLen { get; set; }

        // 步长
        public int SLen { get; set; }

        /// <summary>
        /// 观测周期
        /// </summary>
        public int Freq { get; set; }

        /// <summary>
        /// 观测周期内的数据格式化方法
        /// </summary>
        public FreqDataFormat FreqDataFormat { get; set; }
    }

    public class XSQOutput
    {
        public XQSInput Input { get; set; }
    }
}