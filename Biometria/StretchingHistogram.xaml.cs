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
    /// Interaction logic for StretchingHistogram.xaml
    /// </summary>
    public partial class StretchingHistogram : Window
    {
        private HistogramLogic histogram;
        private byte option;
        public BitmapSource image { get; set; }
        public StretchingHistogram(HistogramLogic _h, BitmapSource img)
        {
            histogram = _h;
            image = img;
            InitializeComponent();
            Histogram.Points = HistogramView.PointCollection(histogram.HistogramA);
            Histogram.Fill = Brushes.Gray;
            option = 3;
        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = (ComboBox)sender;

            ((MainWindow)System.Windows.Application.Current.MainWindow).EditedImage.Source = image; //reset view
            switch (combobox.SelectedItem.ToString().Split(new[] { ": " }, StringSplitOptions.None).Last())
            {
                case "Czerwony":
                    Histogram.Points = HistogramView.PointCollection(histogram.HistogramR);
                    Histogram.Fill = Brushes.Red;
                    option = 0;
                    break;
                case "Zielony":
                    Histogram.Points = HistogramView.PointCollection(histogram.HistogramG);
                    Histogram.Fill = Brushes.Green;
                    option = 1;

                    break;
                case "Niebieski":
                    Histogram.Points = HistogramView.PointCollection(histogram.HistogramB);
                    Histogram.Fill = Brushes.Blue;
                    option = 2;
                    break;
                case "Uśredniony":
                    Histogram.Points = HistogramView.PointCollection(histogram.HistogramA);
                    Histogram.Fill = Brushes.Gray;
                    option = 3;
                    break;
                default:
                    break;
            }
        }

        private void TextBoxMinValueOnChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBoxMin.Text == "")
                return;
            try
            {
                if (int.Parse(TextBoxMin.Text) > 255)
                    return;
                if (int.Parse(TextBoxMin.Text) <= 255 && int.Parse(TextBoxMin.Text) < int.Parse(TextBoxMax.Text))
                {
                    Stretch();
                }
            }
            catch (Exception exception)
            {
            }
        }

        private void TextBoxMaxValueChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBoxMax.Text == "")
                return;
            if (int.Parse(TextBoxMax.Text) > 255)
                return;
            if (int.Parse(TextBoxMax.Text) <= 255 && int.Parse(TextBoxMin.Text) < int.Parse(TextBoxMax.Text))
            {
                Stretch();
            }
        }

        private void Stretch()
        {
            try
            {
                var lut = histogram.GetLutStretching(int.Parse(TextBoxMin.Text), int.Parse(TextBoxMax.Text));

                ((MainWindow)Application.Current.MainWindow).EditedImage.Source = image = histogram.StretchHistogram(lut, option, image);

                histogram.CalculateHistograms(image);

                switch (option)
                {
                    case 0:
                        Histogram.Points = HistogramView.PointCollection(histogram.HistogramR);
                        Histogram.Fill = Brushes.Red;

                        break;
                    case 1:
                        Histogram.Points = HistogramView.PointCollection(histogram.HistogramG);
                        Histogram.Fill = Brushes.Green;

                        break;
                    case 2:
                        Histogram.Points = HistogramView.PointCollection(histogram.HistogramB);
                        Histogram.Fill = Brushes.Blue;

                        break;
                    case 3:
                        Histogram.Points = HistogramView.PointCollection(histogram.HistogramA);
                        Histogram.Fill = Brushes.Gray;

                        break;
                }
            }
            catch (Exception e)
            {

            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            image = ((MainWindow)System.Windows.Application.Current.MainWindow).editedImage;
            this.Close();
        }
    }
}
