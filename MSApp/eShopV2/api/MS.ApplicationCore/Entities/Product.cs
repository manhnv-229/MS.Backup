using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Entities
{
    /// <summary>
    /// Sản phẩm
    /// </summary>
    public class Product:BaseEntity
    {
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public Guid UnitId { get; set; }
        public Guid? GroupProductId { get; set; }
        public string? Imgs { get; set; }
        public decimal UnitPrice { get; set; }
        //public decimal Quantity { get; set; }
    }
}
