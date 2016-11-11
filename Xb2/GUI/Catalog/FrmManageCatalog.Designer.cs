namespace Xb2.GUI.Catalog
{
    partial class FrmManageCatalog
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.添加Q01文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除Q01文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.q01文件改名ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.将Q01文件导入数据库ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.从数据库中卸载Q01文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(2);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 25);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.Size = new System.Drawing.Size(853, 646);
            this.dataGridView1.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(853, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.添加Q01文件ToolStripMenuItem,
            this.删除Q01文件ToolStripMenuItem,
            this.q01文件改名ToolStripMenuItem,
            this.将Q01文件导入数据库ToolStripMenuItem,
            this.从数据库中卸载Q01文件ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(209, 114);
            // 
            // 添加Q01文件ToolStripMenuItem
            // 
            this.添加Q01文件ToolStripMenuItem.Name = "添加Q01文件ToolStripMenuItem";
            this.添加Q01文件ToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.添加Q01文件ToolStripMenuItem.Text = "添加Q01文件";
            this.添加Q01文件ToolStripMenuItem.Click += new System.EventHandler(this.添加Q01文件ToolStripMenuItem_Click);
            // 
            // 删除Q01文件ToolStripMenuItem
            // 
            this.删除Q01文件ToolStripMenuItem.Name = "删除Q01文件ToolStripMenuItem";
            this.删除Q01文件ToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.删除Q01文件ToolStripMenuItem.Text = "删除Q01文件";
            // 
            // q01文件改名ToolStripMenuItem
            // 
            this.q01文件改名ToolStripMenuItem.Name = "q01文件改名ToolStripMenuItem";
            this.q01文件改名ToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.q01文件改名ToolStripMenuItem.Text = "Q01文件改名";
            // 
            // 将Q01文件导入数据库ToolStripMenuItem
            // 
            this.将Q01文件导入数据库ToolStripMenuItem.Name = "将Q01文件导入数据库ToolStripMenuItem";
            this.将Q01文件导入数据库ToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.将Q01文件导入数据库ToolStripMenuItem.Text = "将Q01文件导入数据库";
            // 
            // 从数据库中卸载Q01文件ToolStripMenuItem
            // 
            this.从数据库中卸载Q01文件ToolStripMenuItem.Name = "从数据库中卸载Q01文件ToolStripMenuItem";
            this.从数据库中卸载Q01文件ToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.从数据库中卸载Q01文件ToolStripMenuItem.Text = "从数据库中卸载Q01文件";
            // 
            // FrmManageCatalog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(853, 671);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "FrmManageCatalog";
            this.Text = "管理地震目录";
            this.Load += new System.EventHandler(this.FrmQ01Managing_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        public System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 添加Q01文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除Q01文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem q01文件改名ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 将Q01文件导入数据库ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 从数据库中卸载Q01文件ToolStripMenuItem;


    }
}