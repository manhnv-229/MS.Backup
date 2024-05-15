using MS.Core.Helpers;
using MS.Core.MSEnums;
using MS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    [ViewDataName("Views_Service")]
    public class Service:BaseEntity
    {
        public Guid ServiceId { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public string? Description { get; set; }
        public decimal CostPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public string? ImgPath { get; set; }
        public Guid? ServiceGroupId { get; set; }
        public ChargeType ChargeType { get; set; }
        public decimal UnitTime { get; set; }
        public Guid? UnitId { get; set; }

        [NotMapQuery]
        public string? ImgFullPath
        {
            get
            {
                var random = new Random();
                if (ImgPath != null)
                    return String.Format("{0}/{1}?v={2}", CommonConst.ServerFileUrl, ImgPath, random.Next(1, 999));
                return null;
            }
        }
    }
}
