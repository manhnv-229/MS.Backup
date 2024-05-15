using MS.Core.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class Theme:BaseEntity
    {
        public Guid ThemeId { get; set; }
        public string? ThemeName { get; set; }
        public ThemeType ThemeType { get; set; }
        public string ThemeColor { get; set; }

    }
}
