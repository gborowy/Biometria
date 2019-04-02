using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Biometria
{
    public class HistogramLogic
    {
        public int[] HistogramR;
        public int[] HistogramG;
        public int[] HistogramB;
        public int[] HistogramA;
        public int MaxPixels;

        public HistogramLogic()
        {
        }

        public void CalculateHistograms(BitmapSource imgSource)
        {
            //ile razy wystepuje pixel
            HistogramR = new int[256];
            HistogramG = new int[256];
            HistogramB = new int[256];
            HistogramA = new int[256];

            MaxPixels = imgSource.PixelWidth * imgSource.PixelHeight;

            var stride = imgSource.PixelWidth * ((imgSource.Format.BitsPerPixel + 7) / 8);

            var pixels = new byte[stride * imgSource.PixelHeight];

            imgSource.CopyPixels(pixels, stride, 0);

            var j = 0;
            // petla liczaca pixele
            for (var i = 0; i < pixels.Length / 4; i++)
            {
                var r = pixels[j + 2];
                var g = pixels[j + 1];
                var b = pixels[j];

                HistogramR[r]++;
                HistogramG[g]++;
                HistogramB[b]++;

                var temp = (r + g + b) / 3;
                HistogramA[temp]++;

                j += 4;
            }
        }

        public double[] GetDystrybuanta(int[] histogram)
        {
            double suma = 0;
            var dystrybuanta = new double[histogram.Length];
            for (var i = 0; i < histogram.Length; i++)
            {
                suma += histogram[i];
                dystrybuanta[i] = suma / MaxPixels;
            }
            return dystrybuanta;
        }

        public int[] GetLutStretching(double min, double max)
        {
            var lut = new int[256];
            for (var i = 0; i < 256; i++)
            {
                var value = (255.0 / (max - min)) * (i - min);//liczeni lut[i]
                if (value > 255)
                {
                    lut[i] = 255;
                }
                else if (value < 0)
                {
                    lut[i] = 0;
                }
                else
                {
                    lut[i] = (int)Math.Round(value, 0, MidpointRounding.AwayFromZero);
                }

            }
            return lut;
        }

        public int[] LutEqualization(double[] dystrybuanta, int max)
        {
            var i = 0;
            while (dystrybuanta[i] == 0)
                i++;
            var non = dystrybuanta[i];

            var lut = new int[256];

            for (i = 0; i < 256; i++)
            {
                //oblicz
                lut[i] = (int)((dystrybuanta[i] - non) / (1 - non) * (max - 1));
                if (lut[i] > 255)
                {
                    lut[i] = 255;
                }
                if (lut[i] < 0)
                {
                    lut[i] = 0;
                }
            }
            return lut;
        }

        public BitmapSource StretchHistogram(int[] lut, byte layer, BitmapSource imgSource)
        {
            var bitmap = new WriteableBitmap(imgSource);
            var stride = bitmap.PixelWidth * ((bitmap.Format.BitsPerPixel + 7) / 8);
            var pixels = new byte[stride * bitmap.PixelHeight];

            bitmap.CopyPixels(pixels, stride, 0);

            var j = 0;

            for (var i = 0; i < pixels.Length / 4; i++)
            {
                var r = pixels[j + 2];
                var g = pixels[j + 1];
                var b = pixels[j];

                switch (layer)
                {
                    case 0:
                        pixels[j + 2] = (byte)lut[r];
                        break;
                    case 1:
                        pixels[j + 1] = (byte)lut[g];
                        break;
                    case 2:
                        pixels[j] = (byte)lut[b];
                        break;
                    case 3:
                        pixels[j + 2] = (byte)lut[r];
                        pixels[j + 1] = (byte)lut[g];
                        pixels[j] = (byte)lut[b];
                        break;
                }

                j += 4;
            }

            var rect = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            bitmap.WritePixels(rect, pixels, stride, 0);

            return bitmap;
        }

        public BitmapSource EqualizeHistogram(int[] lutR, int[] lutG, int[] lutB, BitmapSource imgSource)
        {
            var bitmap = new WriteableBitmap(imgSource);
            var stride = bitmap.PixelWidth * ((bitmap.Format.BitsPerPixel + 7) / 8);
            var pixels = new byte[stride * bitmap.PixelHeight];
            bitmap.CopyPixels(pixels, stride, 0);

            var j = 0;

            for (var i = 0; i < pixels.Length / 4; i++)
            {
                var r = pixels[j + 2];
                var g = pixels[j + 1];
                var b = pixels[j];

                pixels[j + 2] = (byte)lutR[r];
                pixels[j + 1] = (byte)lutG[g];
                pixels[j] = (byte)lutB[b];

                j += 4;
            }

            var rect = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            bitmap.WritePixels(rect, pixels, stride, 0);

            return bitmap;
        }
    }
}
