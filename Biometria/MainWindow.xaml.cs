using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Biometria
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapSource originalImage { get; set; } 
        public BitmapSource editedImage { get; set; } 

        private TransformGroup group = new TransformGroup(); // zmienna do ustawiania skalowania obrazu
        private double zoomValue = 100;  // skalowanie w procentach

        private double xO = 0, xE = 0; // zmienne pomocnicze do sprawdzania pixeli
        private double yO = 0, yE = 0;

        public MainWindow()
        {
            InitializeComponent();
            ZoomValue.Content = zoomValue.ToString() + "%";

            ScaleTransform scale = new ScaleTransform();  //skalowanie obrazu 
            group.Children.Add(scale);
        }

        private void CheckRGB()
        {
            var x = (int)PixelX.Content * (originalImage.PixelWidth / OriginalImage.ActualWidth);
            var y = (int)PixelY.Content * (originalImage.PixelHeight / OriginalImage.ActualHeight);

            int width = editedImage.PixelWidth * 4;
            int size = editedImage.PixelHeight * width;
            byte[] pixels = new byte[size];
            editedImage.CopyPixels(pixels, width, 0);

            int index = (int)y * width + (int)x * 4;   //BGRA
            byte blue = pixels[index];
            byte green = pixels[index + 1];
            byte red = pixels[index + 2];
            byte alpha = pixels[index + 3];
            B.Header = "B: " + (int)blue;
            G.Header = "G: " + (int)green;
            R.Header = "R: " + (int)red;
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "All Files (*.*)|*.*|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|PNG (*.png)|*.png|GIF (*.gif)|*.gif|TIFF (*.tif;*.tiff)|*.tif;*.tiff|BMP (.bmp)|*bmp";
            dialog.ShowDialog();
            string name = dialog.FileName;
            if (dialog.CheckPathExists && name != "")
            {
                originalImage = new BitmapImage(new Uri(name));
                editedImage = originalImage.Clone();
                OriginalImage.Source = originalImage;
                EditedImage.Source = editedImage;
            }
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ScaleTransform flipTrans = new ScaleTransform();
            flipTrans.ScaleX = ((zoomValue = zoomValue + 10) / 100);
            flipTrans.ScaleY = zoomValue / 100;
            group.Children[0] = flipTrans;
            OriginalImage.RenderTransform = group;
            EditedImage.RenderTransform = group;
            ZoomValue.Content = zoomValue.ToString() + "%";
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ScaleTransform flipTrans = new ScaleTransform();
            flipTrans.ScaleX = ((zoomValue = zoomValue - 10) / 100);
            flipTrans.ScaleY = zoomValue / 100;
            group.Children[0] = flipTrans;
            OriginalImage.RenderTransform = group;
            EditedImage.RenderTransform = group;
            ZoomValue.Content = zoomValue.ToString() + "%";
        }

        private void OriginalImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            xO = e.GetPosition(OriginalImage).X;
            yO = e.GetPosition(OriginalImage).Y;
            xE = e.GetPosition(EditedImage).X;
            yE = e.GetPosition(EditedImage).Y;

            int x = (int)xO;
            int y = (int)yO;
            PixelX.Content = x;
            PixelY.Content = y;
            CheckRGB();
        }

        private void OriginalImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            if (e.Delta > 0)
            {
                zoomValue += 10;
            }
            else
            {
                zoomValue -= 10;
            }
            ScaleTransform flipTrans = new ScaleTransform();
            flipTrans.ScaleX = (zoomValue / 100);
            flipTrans.ScaleY = zoomValue / 100;
            group.Children[0] = flipTrans;
            OriginalImage.RenderTransform = group;
            EditedImage.RenderTransform = group;
            ZoomValue.Content = zoomValue.ToString() + "%";
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            if (editedImage != null)
            {
                Size newWindow = new Size((int)editedImage.PixelWidth, (int)editedImage.PixelHeight);
                newWindow.ShowDialog();
                SaveFileDialog dialog = new SaveFileDialog();

                dialog.Filter = "All Files (*.*)|*.*|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|PNG (*.png)|*.png|GIF (*.gif)|*.gif|TIFF (*.tif;*.tiff)|*.tif;*.tiff|BMP (.bmp)|*bmp";
                dialog.ShowDialog();
                var name = dialog.FileName;
                if (dialog.CheckPathExists && name != "")
                {
                    var type = dialog.FilterIndex;
                    switch (type)
                    {
                        case 1:
                            var encoderJpeg = new JpegBitmapEncoder();
                            using (var fileStream = new FileStream(name, FileMode.Create))
                            {
                                encoderJpeg.Frames.Add(CreateResizedImage(editedImage, newWindow.Width, newWindow.Height, 0));
                                encoderJpeg.Save(fileStream);
                            }
                            break;
                        case 2:
                            var encoderPng = new PngBitmapEncoder();
                            using (var fileStream = new FileStream(name, FileMode.Create))
                            {
                                encoderPng.Frames.Add(CreateResizedImage(editedImage, newWindow.Width, newWindow.Height, 0));
                                encoderPng.Save(fileStream);
                            }
                            break;
                        case 3:
                            encoderJpeg = new JpegBitmapEncoder();
                            using (var fileStream = new FileStream(name, FileMode.Create))
                            {
                                encoderJpeg.Frames.Add(CreateResizedImage(editedImage, newWindow.Width, newWindow.Height, 0));
                                encoderJpeg.Save(fileStream);
                            }
                            break;
                        case 4:
                            var encoderGif = new GifBitmapEncoder();
                            using (var fileStream = new FileStream(name, FileMode.Create))
                            {
                                encoderGif.Frames.Add(CreateResizedImage(editedImage, newWindow.Width, newWindow.Height, 0));
                                encoderGif.Save(fileStream);
                            }
                            break;
                        case 5:
                            var encoderTiff = new TiffBitmapEncoder();
                            using (var fileStream = new FileStream(name, FileMode.Create))
                            {
                                encoderTiff.Frames.Add(CreateResizedImage(editedImage, newWindow.Width, newWindow.Height, 0));
                                encoderTiff.Save(fileStream);
                            }
                            break;
                        default:
                            break;
                    }

                }
            }
        }

        private void ChangeRGB_Click(object sender, RoutedEventArgs e)
        {
            var x = (int)PixelX.Content * (originalImage.PixelWidth / OriginalImage.ActualWidth);
            var y = (int)PixelY.Content * (originalImage.PixelHeight / OriginalImage.ActualHeight);

            int width = editedImage.PixelWidth * 4;
            int size = editedImage.PixelHeight * width;
            byte[] pixels = new byte[size];
            editedImage.CopyPixels(pixels, width, 0);

            int index = (int)y * width + (int)x * 4; //bgra
            byte blue = pixels[index];
            byte green = pixels[index + 1];
            byte red = pixels[index + 2];
            byte alpha = pixels[index + 3];

            RGB newWindow = new RGB(System.Drawing.Color.FromArgb(alpha, red, green, blue));
            newWindow.ShowDialog();
            pixels[index] = newWindow.myColor.B;
            pixels[index + 1] = newWindow.myColor.G;
            pixels[index + 2] = newWindow.myColor.R;
            pixels[index + 3] = newWindow.myColor.A;

            WriteableBitmap wb = new WriteableBitmap(editedImage);
            wb.WritePixels(new Int32Rect(0, 0, editedImage.PixelWidth, editedImage.PixelHeight), pixels, width, 0);
            editedImage = wb;
            EditedImage.Source = editedImage;
        }

        private void resetScale_Click(object sender, RoutedEventArgs e)
        {
            ScaleTransform flipTrans = new ScaleTransform();
            zoomValue = 100;
            flipTrans.ScaleX = (zoomValue / 100);
            flipTrans.ScaleY = zoomValue / 100;
            group.Children[0] = flipTrans;
            OriginalImage.RenderTransform = group;
            EditedImage.RenderTransform = group;
            ZoomValue.Content = zoomValue.ToString() + "%;";
        }

        private static BitmapFrame CreateResizedImage(BitmapSource source, int width, int height, int margin)
        {
            var rect = new Rect(margin, margin, width - margin * 2, height - margin * 2);

            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
            group.Children.Add(new ImageDrawing(source, rect));

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawDrawing(group);
            }

            var resizedImage = new RenderTargetBitmap(
                width, height, // Resized dimensions
                96, 96, // Default DPI values
                PixelFormats.Default); // Default pixel format
            resizedImage.Render(drawingVisual);

            return BitmapFrame.Create(resizedImage);
        }

        private void Histogram_Click(object sender, RoutedEventArgs e)
        {
            HistogramView newHistogram = new HistogramView(editedImage);
            newHistogram.ShowDialog();
            EditedImage.Source = editedImage;
        }
    } 
}
