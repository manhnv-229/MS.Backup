using MS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    public class PagingData
    {
        public IEnumerable<object> Data { get; set; }
        public int TotalRecords { get; set; }
        public string ServerFileUrl
        {
            get
            {
                return CommonConst.ServerFileUrl;
            }
        }
    }
}
