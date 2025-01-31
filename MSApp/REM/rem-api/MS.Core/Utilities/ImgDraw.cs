﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace MS.Core.Utilities
{
    public class ImgDraw
    {
        public Image Draw(string text)
        {
            var textImg = "MS";
            if (!String.IsNullOrEmpty(text))
            {
                var nameSplit = text.Split(" ");
                var length = nameSplit.Length;
                var first = nameSplit[0].Substring(0, 1);
                var last = nameSplit[length - 1].Substring(0, 1);
                textImg = $"{first}{last}".ToUpper();
            }
            Random rnd = new Random();
            Font font = new Font(FontFamily.GenericSansSerif, 40, FontStyle.Bold);
            Color textColor = Color.White;
            Color backColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            var img = DrawText(textImg, font, textColor, backColor);
            return img;
        }
        private Image DrawText(String text, Font font, Color textColor, Color backColor)
        {
            //first, create a dummy bitmap just to get a graphics object
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);
            Size sizeImg = new Size(64, 64);
            float width = (float)sizeImg.Width;
            float height = (float)sizeImg.Height;
            float emSize = height;

            font = new Font(FontFamily.GenericSansSerif, emSize, FontStyle.Bold);
            font = FindBestFitFont(drawing, text, font, sizeImg);

            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap((int)width, (int)height);

            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(text, font, textBrush, (width - textSize.Width) / 2, (height - font.Height) / 2);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return img;

        }

        private Font FindBestFitFont(Graphics g, String text, Font font, Size proposedSize)
        {
            // Compute actual size, shrink if needed
            while (true)
            {
                SizeF size = g.MeasureString(text, font);

                // It fits, back out
                if (size.Height <= proposedSize.Height &&
                     size.Width <= proposedSize.Width) { return font; }

                // Try a smaller font (90% of old size)
                Font oldFont = font;
                font = new Font(font.Name, (float)(font.Size * .6), font.Style);
                oldFont.Dispose();
            }
        }

        public static System.Drawing.Image ResizeImage(System.Drawing.Image imgToResize, Size size)
        {
            //Get the image current width  
            int sourceWidth = imgToResize.Width;
            //Get the image current height  
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //Calulate  width with new desired size  
            nPercentW = ((float)size.Width / (float)sourceWidth);
            //Calculate height with new desired size  
            nPercentH = ((float)size.Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //New Width  
            int destWidth = (int)(sourceWidth * nPercent);
            //New Height  
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height  
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (System.Drawing.Image)b;
        }
    }
}
