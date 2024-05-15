using MS.Core.Entities;
using MS.Core.Helpers;
using MS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    public class InventoryItemSaleResponse : InventoryItem
    {
        public decimal QuantityOrder { get; set; } = 1;
        public decimal TotalAmount { get { return QuantityOrder * UnitPrice; } }
        [NotMapQuery]
        public string? ImgFullPath
        {
            get
            {
                var random = new Random();
                if (!string.IsNullOrEmpty(ImgPath))
                    return String.Format("{0}/{1}?v={2}", CommonConst.ServerFileUrl, ImgPath, random.Next(1, 999));
                return null;

            }
        }
    }
}
