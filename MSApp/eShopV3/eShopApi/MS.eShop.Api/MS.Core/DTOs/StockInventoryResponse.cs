using MS.Core.Entities;
using MS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    public class StockInventoryResponse: StockInventory
    {
        public string? InventoryItemName { get; set; }
        public string? InventoryItemCode { get; set; }
        public string? UnitName { get; set; }
        public string? UnitCode { get; set; }
        public string? ImgPath { get; set; }
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
