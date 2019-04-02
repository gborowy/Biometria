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
    /// Interaction logic for Size.xaml
    /// </summary>
    public partial class Size : Window
    {
        public int Width { get; set; }
        public int Height { get; set; }
        private int OriginalWidth { get; set; }
        private int OriginalHeight { get; set; }

        public Size(int w, int h)
        {
            OriginalWidth = Width = w;
            OriginalHeight = Height = h;
            InitializeComponent();
            WidthTextBox.Text = Width.ToString();
            HeightTextBox.Text = Height.ToString();

        }
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Width = Int32.Parse(WidthTextBox.Text);
                Height = Int32.Parse(HeightTextBox.Text);
                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("IOException source: {0}", ex.Source);
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Width = OriginalWidth;
            Height = OriginalHeight;
            WidthTextBox.Text = OriginalWidth.ToString();
            HeightTextBox.Text = OriginalHeight.ToString();
        }
    }
}
