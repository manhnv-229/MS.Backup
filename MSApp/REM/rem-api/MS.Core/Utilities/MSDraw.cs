using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Utilities
{
    public class MSDraw : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            
        }
        public void DrawString(ICanvas canvas, RectF dirtyRect, string text)
        {
            IImage image;

            canvas.FontColor = Colors.Blue;
            canvas.FontSize = 18;

            canvas.Font = Font.Default;
            canvas.DrawString(text, 20, 20, 380, 100, HorizontalAlignment.Center, VerticalAlignment.Center);

            canvas.Font = Font.DefaultBold;
            canvas.DrawString("This text is displayed using the bold system font.", 20, 140, 350, 100, HorizontalAlignment.Left, VerticalAlignment.Top);

            canvas.Font = new Font("Arial");
            canvas.FontColor = Colors.Black;
            canvas.SetShadow(new SizeF(6, 6), 4, Colors.Gray);
            canvas.DrawString("This text has a shadow.", 20, 200, 300, 100, HorizontalAlignment.Left, VerticalAlignment.Top);


            //ColorMatrix cm = new ColorMatrix();
            //cm.Matrix33 = 0.55f;
            //ImageAttributes ia = new ImageAttributes();
            //ia.SetColorMatrix(cm);
            //canvas.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, ia);
            //return image;
        }
    }
}
