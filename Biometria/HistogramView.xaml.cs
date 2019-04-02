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
    /// Interaction logic for HistogramView.xaml
    /// </summary>
    public partial class HistogramView : Window
    {
        private BitmapSource imageHistogram { get; set; }

        private HistogramLogic histogram;

        public HistogramView(BitmapSource bitmap)
        {
            InitializeComponent();
            imageHistogram = bitmap;
            histogram = new HistogramLogic();
            HistogramRed.Fill = Brushes.Red;
            HistogramGreen.Fill = Brushes.Green;
            HistogramBlue.Fill = Brushes.Blue;
            HistogramAvg.Fill = Brushes.Gray;
            updateHistogram();

            //MainWindow mainW = ;
            //    Application.MainWindow.c;
            //((MainWindow)System.Windows.Application.Current.MainWindow).EditedImage.Text = "Setting Text from My Program";

        }

        private void updateHistogram()
        {
            histogram.CalculateHistograms(imageHistogram);

            HistogramAvg.Points = PointCollection(histogram.HistogramA);
            HistogramRed.Points = PointCollection(histogram.HistogramR);
            HistogramGreen.Points = PointCollection(histogram.HistogramG);
            HistogramBlue.Points = PointCollection(histogram.HistogramB);

            MaxHistogramValueAverage.Content = histogram.HistogramA.Max();
            MaxHistogramValueBlue.Content = histogram.HistogramB.Max();
            MaxHistogramValueGreen.Content = histogram.HistogramG.Max();
            MaxHistogramValueRed.Content = histogram.HistogramR.Max();
        }

        public static PointCollection PointCollection(int[] values)
        {
            var max = values.Max();

            var points = new PointCollection();
            // pierwszy
            points.Add(new Point(0, max));
            for (var i = 0; i < values.Length; i++)
                points.Add(new Point(i, max - values[i]));
            // ostatni
            points.Add(new Point(values.Length - 1, max));

            return points;
        }

        private void ResetHistogram_Click(object sender, RoutedEventArgs e)
        {
            imageHistogram = ((MainWindow)System.Windows.Application.Current.MainWindow).editedImage;
            ((MainWindow)System.Windows.Application.Current.MainWindow).EditedImage.Source = imageHistogram;
            updateHistogram();

        }

        private void Equalize_Click(object sender, RoutedEventArgs e)
        {
            var dystR = histogram.GetDystrybuanta(histogram.HistogramR);
            var dystG = histogram.GetDystrybuanta(histogram.HistogramG);
            var dystB = histogram.GetDystrybuanta(histogram.HistogramB);

            var lutR = histogram.LutEqualization(dystR, histogram.HistogramR.Length);
            var lutG = histogram.LutEqualization(dystG, histogram.HistogramG.Length);
            var lutB = histogram.LutEqualization(dystB, histogram.HistogramB.Length);

            BitmapSource bmp = histogram.EqualizeHistogram(lutR, lutG, lutB, imageHistogram);
            ((MainWindow)System.Windows.Application.Current.MainWindow).EditedImage.Source = imageHistogram = bmp;

            updateHistogram();
        }

        private void Stretching_Click(object sender, RoutedEventArgs e)
        {
            StretchingHistogram stretch = new StretchingHistogram(histogram, imageHistogram);
            stretch.ShowDialog();
            imageHistogram = stretch.image;
            updateHistogram();

        }

        private void Brighten_Click(object sender, RoutedEventArgs e)
        {
            var lut = new int[256];
            var max = 0;

            for (var i = 255; i >= 0; i--)
                if (histogram.HistogramA[i] > 0)
                {
                    max = i;
                    break;
                }


            for (int i = 0; i < 256; i++)
            {
                //funkcja logarytmiczna
                double licznik = Math.Log(1 + i);
                double mianownik = Math.Log(1 + max);
                lut[i] = (int)Math.Round(255.0 * (licznik / mianownik), 0, MidpointRounding.AwayFromZero);
                if (lut[i] > 255)
                {
                    lut[i] = 255;
                }
                if (lut[i] < 0)
                {
                    lut[i] = 0;
                }
            }

            var bitmap = new WriteableBitmap(imageHistogram);

            var width = bitmap.PixelWidth;
            var height = bitmap.PixelHeight;

            var stride = width * ((bitmap.Format.BitsPerPixel + 7) / 8);

            var arraySize = stride * height;
            var pixels = new byte[arraySize];
            bitmap.CopyPixels(pixels, stride, 0);

            var j = 0;

            for (var i = 0; i < pixels.Length / 4; i++)
            {
                var r = pixels[j + 2];
                var g = pixels[j + 1];
                var b = pixels[j];

                pixels[j + 2] = (byte)lut[r];
                pixels[j + 1] = (byte)lut[g];
                pixels[j] = (byte)lut[b];

                j += 4;
            }

            var rect = new Int32Rect(0, 0, width, height);
            bitmap.WritePixels(rect, pixels, stride, 0);
            imageHistogram = bitmap;
            updateHistogram();
            ((MainWindow)System.Windows.Application.Current.MainWindow).EditedImage.Source = imageHistogram;
        }

        private void Darken_Click(object sender, RoutedEventArgs e)
        {
            var lut = new int[256];
            var max = 0;

            for (var i = 255; i >= 0; i--)
                if (histogram.HistogramA[i] > 0)
                {
                    max = i;
                    break;
                }
            for (int i = 0; i < 256; i++)
            {
                //funkcja kwadratowa
                lut[i] = (int)Math.Round(255.0 * (Math.Pow((double)i / max, 2.0)), 0, MidpointRounding.AwayFromZero);


                if (lut[i] > 255)
                {
                    lut[i] = 255;
                }
                if (lut[i] < 0)
                {
                    lut[i] = 0;
                }
            }

            var bitmap = new WriteableBitmap(imageHistogram);

            var width = bitmap.PixelWidth;
            var height = bitmap.PixelHeight;

            var stride = width * ((bitmap.Format.BitsPerPixel + 7) / 8);

            var arraySize = stride * height;
            var pixels = new byte[arraySize];

            bitmap.CopyPixels(pixels, stride, 0);

            var j = 0;

            for (var i = 0; i < pixels.Length / 4; i++)
            {

                var r = pixels[j + 2];
                var g = pixels[j + 1];
                var b = pixels[j];

                pixels[j + 2] = (byte)lut[r];
                pixels[j + 1] = (byte)lut[g];
                pixels[j] = (byte)lut[b];

                j += 4;
            }

            var rect = new Int32Rect(0, 0, width, height);
            bitmap.WritePixels(rect, pixels, stride, 0);
            imageHistogram = bitmap;
            updateHistogram();
            ((MainWindow)System.Windows.Application.Current.MainWindow).EditedImage.Source = imageHistogram;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).editedImage = imageHistogram;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}