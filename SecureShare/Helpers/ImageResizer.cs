using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace ShareGrid.Helpers
{
	public class ImageResizer
	{
		public static void Resize(Stream input, string outputPath, int maxWidth, int maxHeight)
		{
			using (Bitmap originalBMP = new Bitmap(input))
			{
				int imageWidth = originalBMP.Width;
				int imageHeight = originalBMP.Height;

				double aspectRatio = (double)imageWidth / imageHeight;
				double boxRatio = (double)maxWidth / maxHeight;
				double scaleFactor = 0;

				if (boxRatio > aspectRatio)
					scaleFactor = (double)maxHeight / imageHeight;
				else
					scaleFactor = (double)maxWidth / imageWidth;

				int newWidth = (int)Math.Floor(imageWidth * scaleFactor);
				int newHeight = (int)Math.Floor(imageHeight * scaleFactor);

				using (Bitmap newBMP = new Bitmap(newWidth, newHeight))
				using (Graphics graphics = Graphics.FromImage(newBMP))
				{
					graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
					graphics.SmoothingMode = SmoothingMode.HighQuality;
					graphics.CompositingQuality = CompositingQuality.HighQuality;
					graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
					
					graphics.DrawImage(originalBMP, 0, 0, newWidth, newHeight);

					newBMP.Save(outputPath, ImageFormat.Png);
				}
			}
		}
	}
}