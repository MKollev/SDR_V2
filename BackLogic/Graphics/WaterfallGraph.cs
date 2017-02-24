using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BackLogic.Logic;
using BackLogic.Model;

namespace BackLogic.Graphics
{
    public class WaterfallGraph : IDisposable
    {
        public static List<FreqDataViewModel> AllPoints { get; set; }
        private static List<FreqDataViewModel> _waterfallValues;
        public static Bitmap bmpWaterfall { get; set; }
        //private List<FreqDataViewModel> _normalValues; 
        public WaterfallGraph (List<FreqDataViewModel> allPoints )
        {
            AllPoints = allPoints;
        }
        public Bitmap CreateBitmap()
        {
            Bitmap waterfall = new Bitmap(1000, 256);
            Rectangle rect = new Rectangle(0, 0, waterfall.Width, waterfall.Height);
            BitmapData waterfallData = waterfall.LockBits(rect, ImageLockMode.ReadWrite, waterfall.PixelFormat);
            IntPtr pointer = waterfallData.Scan0;
            int bytes = Math.Abs(waterfallData.Stride) * waterfall.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(pointer, rgbValues, 0, bytes);
            for (int counter = 2; counter < rgbValues.Length; counter += 3)
                rgbValues[counter] = 40;
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, pointer, bytes);
            waterfall.UnlockBits(waterfallData);
            bmpWaterfall = waterfall;
            return waterfall;
        }
        private void ChangeRow(Bitmap bitmap)
        {
            MoveBitmapDown(bitmap);
            int r, g, b;

            for (int i = 0; i < bitmap.Width; i++)
            {

                //Sub.ColorFunction(AmplNormal[i].Ampl, out r, out g, out b);
                using (var temp = new RGBAlgorithm())
                {
                    temp.ColorFunction(_waterfallValues[i].Ampl, out r, out g, out b);
                }
                Color amplColor = Color.FromArgb(r, g, b);

                bitmap.SetPixel(i, 0, amplColor);
            }
            //return bitmap;
        }

        private static void MoveBitmapDown(Bitmap bitmap)
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);

            BitmapData rowData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            IntPtr pntrStart = rowData.Scan0;
            IntPtr pntrRow1 = rowData.Scan0 + rowData.Stride;
            int bytes = (rowData.Stride) * rowData.Height;
            int bytesToCopy = (rowData.Stride) * (rowData.Height - 1);
            byte[] rgbvalues = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(pntrStart, rgbvalues, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(rgbvalues, 0, pntrRow1, bytesToCopy);

            bitmap.UnlockBits(rowData);
        }
        public BitmapImage ToWpfBitmap(Bitmap bitmap)
        {


            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }
        public BitmapImage NewWaterfallLine()
        {
            //    if (SinglePoint == null)
            //    {
            //        timerWaterfall.Stop();
            //    }
            BitmapImage bitmap = new BitmapImage();
            using (var watlg = new WaterfallLogic())
            {
                _waterfallValues = watlg.ConvertWaterfallValues(AllPoints);
            }
            ChangeRow(bmpWaterfall);
            bitmap = ToWpfBitmap(bmpWaterfall);
            AllPoints.Clear();
            return bitmap;
        }
        public void Dispose()
        {
           
        }
    }
}
