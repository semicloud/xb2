using System.Windows.Forms;
using Xb2.Utils;

namespace Xb2.GUI.Controls
{
    public partial class RectQuery : UserControl
    {
        public RectQuery()
        {
            InitializeComponent();
        }

        public double Lng1
        {
            get { return textBox1.Text.Trim().ToDouble(); }
        }

        public double Lat1
        {
            get { return textBox2.Text.Trim().ToDouble(); }
        }

        public double Lng2
        {
            get { return textBox3.Text.Trim().ToDouble(); }
        }

        public double Lat2
        {
            get { return textBox4.Text.Trim().ToDouble(); }
        }


        public bool IsLegalInput()
        {
            return (!textBox1.Text.Trim().Equals("")) && (!textBox2.Text.Trim().Equals("")) &&
                   (!textBox3.Text.Trim().Equals("")) && (!textBox4.Text.Trim().Equals(""));
        }
    }
}
