// 解决方案名称：Xb2
// 工程名称：Xb2
// 文件名：ToolStrips.cs
// 作者：Semicloud
// 初次编写时间：2016-11-25
// 功能：

using System.Windows.Forms;

namespace Xb2.Utils.Control
{
    public class ToolStripHelper
    {
        public static ToolStrip GetChartToolStrip()
        {
            ToolStrip toolStrip = new ToolStrip() {Name = "ChartToolStrip"};
            ToolStripItem item0 = new ToolStripButton()
            {
                Text = "调整大小",
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                ToolTipText = "调整大小"
            };

            ToolStripItem item1 = new ToolStripButton
            {
                Text = "合并(1X,dY)",
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                ToolTipText = "将多个图合并到一个X轴多个Y轴下"
            };
            ToolStripItem item2 = new ToolStripButton
            {
                Text = "合并(1X,1Y)",
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                ToolTipText = "将多个图合并到一个X轴一个Y轴下"
            };
            toolStrip.Items.AddRange(new[]
            {
                item0,
                new ToolStripSeparator(),
                item1,
                item2,
                new ToolStripSeparator()
            });
            return toolStrip;
        }
    }
}