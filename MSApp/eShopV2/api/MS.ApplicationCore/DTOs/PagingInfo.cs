using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.DTOs
{
    public class PagingData
    {
        public IEnumerable<object> Data { get; set; }
        public int TotalRecords { get; set; }
    }
}
