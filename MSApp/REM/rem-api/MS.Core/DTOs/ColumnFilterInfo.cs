using MS.Core.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    public class ColumnFilterInfo
    {
        public string ColumnName { get; set; }
        public FilterType FilterType { get; set; }
        public object FilterValue { get; set; }
    }
}
