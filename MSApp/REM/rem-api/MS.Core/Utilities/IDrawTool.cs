using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Utilities
{
    public interface IDrawTool
    {
        IFormFile ResizeImg(IFormFile img, int newWidth = 64, int? newHeight = null);
        FormFile MakeImgText(string text, int imageWidth = 64, int imageHeight = 64, int textSize = 24);
    }
}
