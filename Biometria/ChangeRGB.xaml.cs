using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Biometria
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class RGB : Window
    {
        public System.Drawing.Color myColor { get; set; }
        public RGB(System.Drawing.Color _myColor)
        {
            InitializeComponent();
            myColor = _myColor;

            BtextBlock.Text = (myColor.B).ToString();
            GtextBlock.Text = (myColor.G).ToString();
            RtextBlock.Text = (myColor.R).ToString();
        }

        private void textBlockChanged(object sender, TextChangedEventArgs e)
        {
            int r, g, b;
            try
            {
                r = Int32.Parse(RtextBlock.Text);
                g = Int32.Parse(GtextBlock.Text);
                b = Int32.Parse(BtextBlock.Text);
                myColor = System.Drawing.Color.FromArgb(myColor.A, r, g, b);
                RGBrect.Fill = new SolidColorBrush(Color.FromRgb(myColor.R, myColor.G, myColor.B));
            }
            catch (Exception ex)
            {
               // MessageBox.Show("Podaj liczby całkowite od 0 do 255");
            }
        }

        private void ChangeRGBbutton_Click(object sender, RoutedEventArgs e)
        {
            int r, g, b;
            try
            {
                r = Int32.Parse(RtextBlock.Text);
                g = Int32.Parse(GtextBlock.Text);
                b = Int32.Parse(BtextBlock.Text);
                myColor = System.Drawing.Color.FromArgb(myColor.A, r, g, b);
                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("IOException source: {0}", ex.Source);
            }
        }
    }
}
