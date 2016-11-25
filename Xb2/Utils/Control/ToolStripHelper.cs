// 解决方案名称：Xb2
// 工程名称：Xb2
// 文件名：ToolStrips.cs
// 作者：Semicloud
// 初次编写时间：2016-11-25
// 功能：

using System.Drawing;
using System.Windows.Forms;

namespace Xb2.Utils.Control
{
    public class ToolStripHelper
    {
        public static ToolStrip GetChartToolStrip()
        {
            ToolStrip toolStrip = new ToolStrip() {Name = "ChartToolStrip"};
            ToolStripItem item1 = new ToolStripButton
            {
                Text = "合并",
                DisplayStyle = ToolStripItemDisplayStyle.Text
            };
            ToolStripItem item2 = new ToolStripButton
            {
                Text = "拼图",
                DisplayStyle = ToolStripItemDisplayStyle.Text
            };
            ToolStripItem item3 = new ToolStripSeparator();
            toolStrip.Items.AddRange(new[] { item1, item2, item3 });
            return toolStrip;
        }
    }
}