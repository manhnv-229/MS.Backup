using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    [HasSystemData]
    public class InventoryItemCategory:BaseEntity
    {
        public Guid InventoryItemCategoryId { get; set; }

        [Required("Mã nhóm hàng hoá không được để trống")]
        public string InventoryItemCategoryCode { get; set; }

        [Required("Tên nhóm hàng hoá không được để trống")]
        public string InventoryItemCategoryName { get; set; }
        public string? Description { get; set; }
        public Guid? ParentId { get; set; }
        public bool Inactive { get; set; }
        public bool? IsSystem { get; set; }
    }
}
