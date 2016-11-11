using System.Windows.Forms;
using Xb2.Utils;

namespace Xb2.GUI.Controls
{
    public partial class CircleQuery : UserControl
    {
        public CircleQuery()
        {
            InitializeComponent();
        }

        public double Lng
        {
            get { return textBox1.Text.Trim().ToDouble(); }
        }

        public double Lat
        {
            get { return textBox2.Text.Trim().ToDouble(); }
        }

        public double Dist
        {
            get { return textBox3.Text.Trim().ToDouble(); }
        }

        public bool IsLegalInput()
        {
            return (!textBox1.Text.Trim().Equals("")) && (!textBox2.Text.Trim().Equals("")) &&
                   (!textBox3.Text.Trim().Equals(""));
        }
    }
}
