using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    public class ProductSaleResponse : Product
    {
        public decimal QuantityOrder { get; set; } = 1;
        public decimal TotalMoney { get { return QuantityOrder * UnitPrice; } }
    }
}
