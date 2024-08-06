using System;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;

class Program
{
    static void Main(string[] args)
    {
        var writer = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Height = 100,
                Width = 300,
                Margin = 10
            }
        };

        var pixelData = writer.Write("1234567890");

        using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height + 30)) // Extra space for text
        using (var graphics = Graphics.FromImage(bitmap))
        {
            // Draw the barcode
            var barcodeBitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppArgb);
            var barcodeData = barcodeBitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, barcodeData.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                barcodeBitmap.UnlockBits(barcodeData);
            }

            graphics.DrawImage(barcodeBitmap, 0, 0);

            // Draw the text below the barcode
            var font = new Font("Arial", 12);
            var brush = Brushes.Black;
            var text = "1234567890";
            var textSize = graphics.MeasureString(text, font);
            var textX = (bitmap.Width - textSize.Width) / 2; // Center the text
            var textY = bitmap.Height - 20; // Position text 20 pixels below the barcode
            graphics.DrawString(text, font, brush, textX, textY);

            // Save the image
            bitmap.Save("barcode_with_text.png", ImageFormat.Png);
        }
    }
}
