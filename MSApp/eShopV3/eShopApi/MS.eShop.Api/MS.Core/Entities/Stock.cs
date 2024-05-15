using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class Stock : BaseEntity
    {
        public Guid StockId { get; set; }

        [Required("Mã Kho hàng tính không được để trống")]
        public string StockCode { get; set; }

        [Required("Tên kho hàng tính không được để trống")]
        public string StockName { get; set; }
        public Guid? BranchId { get; set; }
        public int? StockType { get; set; }
        public bool Inactive { get; set; }
        public bool IsSystem { get; set; }
        public bool IsDefault { get; set; }
        public string? Description { get; set; }
    }
}
