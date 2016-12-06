namespace Xb2.GUI.Main
{
    partial class FrmFirst
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.用户ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.地震目录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.管理地震目录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.生成地震目录子库ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除地震目录子库ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.生成地震目录标注库ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.管理地震目录标注库ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.测项ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.新建测项ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.管理测项ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.测值ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.管理原始数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.生成基础数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.管理基础数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.计算ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.原始数据绘图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.消趋势ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.年周变ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.相关系数ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.速率差分ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.速率合成ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.速率累积强度ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.速率累积强度合成ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.断层活动量ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.应变ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.异常放大ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.menuStrip1.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.用户ToolStripMenuItem,
            this.文件ToolStripMenuItem,
            this.地震目录ToolStripMenuItem,
            this.测项ToolStripMenuItem,
            this.测值ToolStripMenuItem,
            this.计算ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1281, 27);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 用户ToolStripMenuItem
            // 
            this.用户ToolStripMenuItem.Name = "用户ToolStripMenuItem";
            this.用户ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.用户ToolStripMenuItem.Text = "用户";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // 地震目录ToolStripMenuItem
            // 
            this.地震目录ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.管理地震目录ToolStripMenuItem,
            this.生成地震目录子库ToolStripMenuItem,
            this.删除地震目录子库ToolStripMenuItem,
            this.生成地震目录标注库ToolStripMenuItem,
            this.管理地震目录标注库ToolStripMenuItem});
            this.地震目录ToolStripMenuItem.Name = "地震目录ToolStripMenuItem";
            this.地震目录ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.地震目录ToolStripMenuItem.Text = "地震目录";
            // 
            // 管理地震目录ToolStripMenuItem
            // 
            this.管理地震目录ToolStripMenuItem.Name = "管理地震目录ToolStripMenuItem";
            this.管理地震目录ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.管理地震目录ToolStripMenuItem.Text = "管理地震目录";
            this.管理地震目录ToolStripMenuItem.Click += new System.EventHandler(this.管理地震目录ToolStripMenuItem_Click);
            // 
            // 生成地震目录子库ToolStripMenuItem
            // 
            this.生成地震目录子库ToolStripMenuItem.Name = "生成地震目录子库ToolStripMenuItem";
            this.生成地震目录子库ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.生成地震目录子库ToolStripMenuItem.Text = "生成地震目录子库";
            this.生成地震目录子库ToolStripMenuItem.Click += new System.EventHandler(this.生成地震目录子库ToolStripMenuItem_Click);
            // 
            // 删除地震目录子库ToolStripMenuItem
            // 
            this.删除地震目录子库ToolStripMenuItem.Name = "删除地震目录子库ToolStripMenuItem";
            this.删除地震目录子库ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.删除地震目录子库ToolStripMenuItem.Text = "删除地震目录子库";
            this.删除地震目录子库ToolStripMenuItem.Click += new System.EventHandler(this.删除地震目录子库ToolStripMenuItem_Click);
            // 
            // 生成地震目录标注库ToolStripMenuItem
            // 
            this.生成地震目录标注库ToolStripMenuItem.Name = "生成地震目录标注库ToolStripMenuItem";
            this.生成地震目录标注库ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.生成地震目录标注库ToolStripMenuItem.Text = "生成地震目录标注库";
            this.生成地震目录标注库ToolStripMenuItem.Click += new System.EventHandler(this.生成地震目录标注库ToolStripMenuItem_Click);
            // 
            // 管理地震目录标注库ToolStripMenuItem
            // 
            this.管理地震目录标注库ToolStripMenuItem.Name = "管理地震目录标注库ToolStripMenuItem";
            this.管理地震目录标注库ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.管理地震目录标注库ToolStripMenuItem.Text = "管理地震目录标注库";
            this.管理地震目录标注库ToolStripMenuItem.Click += new System.EventHandler(this.管理地震目录标注库ToolStripMenuItem_Click);
            // 
            // 测项ToolStripMenuItem
            // 
            this.测项ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新建测项ToolStripMenuItem,
            this.管理测项ToolStripMenuItem});
            this.测项ToolStripMenuItem.Name = "测项ToolStripMenuItem";
            this.测项ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.测项ToolStripMenuItem.Text = "测项";
            // 
            // 新建测项ToolStripMenuItem
            // 
            this.新建测项ToolStripMenuItem.Name = "新建测项ToolStripMenuItem";
            this.新建测项ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.新建测项ToolStripMenuItem.Text = "新建测项";
            this.新建测项ToolStripMenuItem.Click += new System.EventHandler(this.新建测项ToolStripMenuItem_Click);
            // 
            // 管理测项ToolStripMenuItem
            // 
            this.管理测项ToolStripMenuItem.Name = "管理测项ToolStripMenuItem";
            this.管理测项ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.管理测项ToolStripMenuItem.Text = "管理测项";
            this.管理测项ToolStripMenuItem.Click += new System.EventHandler(this.管理测项ToolStripMenuItem_Click);
            // 
            // 测值ToolStripMenuItem
            // 
            this.测值ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.管理原始数据ToolStripMenuItem,
            this.生成基础数据ToolStripMenuItem,
            this.管理基础数据ToolStripMenuItem});
            this.测值ToolStripMenuItem.Name = "测值ToolStripMenuItem";
            this.测值ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.测值ToolStripMenuItem.Text = "测值";
            // 
            // 管理原始数据ToolStripMenuItem
            // 
            this.管理原始数据ToolStripMenuItem.Name = "管理原始数据ToolStripMenuItem";
            this.管理原始数据ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.管理原始数据ToolStripMenuItem.Text = "管理原始数据";
            this.管理原始数据ToolStripMenuItem.Click += new System.EventHandler(this.管理原始数据ToolStripMenuItem_Click);
            // 
            // 生成基础数据ToolStripMenuItem
            // 
            this.生成基础数据ToolStripMenuItem.Name = "生成基础数据ToolStripMenuItem";
            this.生成基础数据ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.生成基础数据ToolStripMenuItem.Text = "生成基础数据";
            this.生成基础数据ToolStripMenuItem.Click += new System.EventHandler(this.生成基础数据ToolStripMenuItem_Click);
            // 
            // 管理基础数据ToolStripMenuItem
            // 
            this.管理基础数据ToolStripMenuItem.Name = "管理基础数据ToolStripMenuItem";
            this.管理基础数据ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.管理基础数据ToolStripMenuItem.Text = "管理基础数据";
            this.管理基础数据ToolStripMenuItem.Click += new System.EventHandler(this.管理基础数据ToolStripMenuItem_Click);
            // 
            // 计算ToolStripMenuItem
            // 
            this.计算ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.原始数据绘图ToolStripMenuItem,
            this.消趋势ToolStripMenuItem,
            this.年周变ToolStripMenuItem,
            this.相关系数ToolStripMenuItem,
            this.速率差分ToolStripMenuItem,
            this.速率合成ToolStripMenuItem,
            this.速率累积强度ToolStripMenuItem,
            this.速率累积强度合成ToolStripMenuItem,
            this.断层活动量ToolStripMenuItem,
            this.应变ToolStripMenuItem,
            this.异常放大ToolStripMenuItem});
            this.计算ToolStripMenuItem.Name = "计算ToolStripMenuItem";
            this.计算ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.计算ToolStripMenuItem.Text = "计算";
            // 
            // 原始数据绘图ToolStripMenuItem
            // 
            this.原始数据绘图ToolStripMenuItem.Name = "原始数据绘图ToolStripMenuItem";
            this.原始数据绘图ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.原始数据绘图ToolStripMenuItem.Text = "原始数据绘图";
            this.原始数据绘图ToolStripMenuItem.Click += new System.EventHandler(this.原始数据绘图ToolStripMenuItem_Click);
            // 
            // 消趋势ToolStripMenuItem
            // 
            this.消趋势ToolStripMenuItem.Name = "消趋势ToolStripMenuItem";
            this.消趋势ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.消趋势ToolStripMenuItem.Text = "消趋势";
            this.消趋势ToolStripMenuItem.Click += new System.EventHandler(this.消趋势ToolStripMenuItem_Click);
            // 
            // 年周变ToolStripMenuItem
            // 
            this.年周变ToolStripMenuItem.Name = "年周变ToolStripMenuItem";
            this.年周变ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.年周变ToolStripMenuItem.Text = "年周变";
            // 
            // 相关系数ToolStripMenuItem
            // 
            this.相关系数ToolStripMenuItem.Name = "相关系数ToolStripMenuItem";
            this.相关系数ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.相关系数ToolStripMenuItem.Text = "相关系数";
            // 
            // 速率差分ToolStripMenuItem
            // 
            this.速率差分ToolStripMenuItem.Name = "速率差分ToolStripMenuItem";
            this.速率差分ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.速率差分ToolStripMenuItem.Text = "速率差分";
            // 
            // 速率合成ToolStripMenuItem
            // 
            this.速率合成ToolStripMenuItem.Name = "速率合成ToolStripMenuItem";
            this.速率合成ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.速率合成ToolStripMenuItem.Text = "速率合成";
            // 
            // 速率累积强度ToolStripMenuItem
            // 
            this.速率累积强度ToolStripMenuItem.Name = "速率累积强度ToolStripMenuItem";
            this.速率累积强度ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.速率累积强度ToolStripMenuItem.Text = "速率累积强度";
            // 
            // 速率累积强度合成ToolStripMenuItem
            // 
            this.速率累积强度合成ToolStripMenuItem.Name = "速率累积强度合成ToolStripMenuItem";
            this.速率累积强度合成ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.速率累积强度合成ToolStripMenuItem.Text = "速率累积强度合成";
            // 
            // 断层活动量ToolStripMenuItem
            // 
            this.断层活动量ToolStripMenuItem.Name = "断层活动量ToolStripMenuItem";
            this.断层活动量ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.断层活动量ToolStripMenuItem.Text = "断层活动量";
            // 
            // 应变ToolStripMenuItem
            // 
            this.应变ToolStripMenuItem.Name = "应变ToolStripMenuItem";
            this.应变ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.应变ToolStripMenuItem.Text = "应变";
            // 
            // 异常放大ToolStripMenuItem
            // 
            this.异常放大ToolStripMenuItem.Name = "异常放大ToolStripMenuItem";
            this.异常放大ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.异常放大ToolStripMenuItem.Text = "异常放大";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 860);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1281, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1281, 2);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 27);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(1281, 27);
            this.toolStripContainer1.TabIndex = 9;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // FrmFirst
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1281, 882);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FrmFirst";
            this.Text = "欢迎使用，当前用户【admin】，权限为【管理员】";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 用户ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 地震目录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 测项ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 测值ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 计算ToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem 管理地震目录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 生成地震目录子库ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除地震目录子库ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 生成地震目录标注库ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 管理地震目录标注库ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 新建测项ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 管理测项ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 管理原始数据ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 生成基础数据ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 管理基础数据ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 原始数据绘图ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 消趋势ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 年周变ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 相关系数ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 速率差分ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 速率合成ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 速率累积强度ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 速率累积强度合成ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 断层活动量ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 应变ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 异常放大ToolStripMenuItem;
        public System.Windows.Forms.ToolStripContainer toolStripContainer1;


    }
}