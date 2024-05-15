using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MS.Core.Utilities
{
    public class SkiaDraw : IDrawTool
    {
        private IWebHostEnvironment _environment;

        public SkiaDraw(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public FormFile MakeImgText(string text, int imageWidth = 64, int imageHeight = 64, int textSize = 24)
        {
            try
            {
                var info = new SKImageInfo(imageWidth, imageHeight);
                var strokeWidth = 3;
                SKBitmap bitmap = new SKBitmap(imageWidth, imageHeight, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                SKCanvas canvas = new SKCanvas(bitmap);

                using (var paint = new SKPaint())
                {
                    paint.TextSize = textSize;
                    paint.IsAntialias = true;
                    paint.Color = SKColors.White;
                    paint.IsStroke = false;
                    paint.StrokeWidth = strokeWidth;
                    paint.TextAlign = SKTextAlign.Center;

                    using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("MS.Core.Resources.RobotoBlack.ttf"))
                    {
                        var result = SKTypeface.FromStream(resource);
                        // even though result is not null setting the typeface its always null...why???
                        paint.Typeface = result;
                        canvas.Clear(GetRandomColor());//BG color:
                        canvas.DrawText(text, info.Width / 2f, (imageHeight + textSize) / 2 - 4, paint);
                        SKImage skImage = SKImage.FromBitmap(bitmap);
                        SKData data = skImage.Encode(SKEncodedImageFormat.Png, 100);
                        var stream = data.AsStream();
                        FormFile file = new FormFile(stream, 0, stream.Length, "imageStreams", "imageStream.png");
                        return file;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex?.InnerException?.Message);
                throw;
            }

        }

        public IFormFile ResizeImg(IFormFile img, int newWidth = 64, int? newHeight = null)
        {
            var imgStream = img.OpenReadStream();
            var extFileName = img.FileName.Split(".").Last();
            Console.WriteLine(extFileName);
            using (var skData = SKData.Create(imgStream))
            {
                using (var skImage = SKBitmap.Decode(skData))
                {
                    Console.WriteLine(skImage.Width);
                    var scalePecent = (float)newWidth / (float)skImage.Width;

                    int newHeightAfterScalse = 0;
                    if (newHeight == null)
                    {
                        newHeightAfterScalse = (int)(((float)skImage.Height) * scalePecent);
                    }
                    else
                    {
                        newHeightAfterScalse = (int)newHeight;
                    }
                    //int newHeight = (int)(skImage.Height * scaleFactor);
                    //int newWidth = (int)(skImage.Width * scaleFactor);

                    using (var scaledBitmap = skImage.Resize(new SKImageInfo(newWidth, newHeightAfterScalse), SKFilterQuality.Low))
                    {
                        using (var image = SKImage.FromBitmap(scaledBitmap))
                        {
                            using (var encodedImage = image.Encode(SKEncodedImageFormat.Png, 100))
                            {
                                var stream = encodedImage.AsStream();
                                FormFile file = new FormFile(stream, 0, stream.Length, "imageStreams", "imageStream.png");
                                return file;
                                //var stream = new MemoryStream();
                                //encodedImage.SaveTo(stream);
                                //stream.Seek(0, SeekOrigin.Begin);
                                //return stream;
                            }
                        }
                    }
                }
            }
        }


        private SKColor GetRandomColor()
        {
            var fields = typeof(SKColors).GetFields();
            var fieldsChoose = fields.Where(item =>
            {
                var colorValue = item?.GetValue(item)?.ToString();
                return colorValue != SKColors.White.ToString() && colorValue != SKColors.Transparent.ToString() && colorValue != SKColors.Empty.ToString();
            }).ToList();
            var count = fieldsChoose.Count();
            var numberRanDom = new Random();
            var field = fieldsChoose[numberRanDom.Next(0, count - 1)];

            if (field != null)
            {
                return (SKColor)field.GetValue(field);
            }
            else
            {
                return SKColors.Blue;
            }
        }
    }
}
