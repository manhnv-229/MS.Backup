using MS.ApplicationCore.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.DTOs
{
    public class ColumnSortInfo
    {
        public string ColumnName { get; set; }
        public SortType SortType { get; set; }
    }
}
