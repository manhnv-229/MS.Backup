using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    /// <summary>
    /// Sản phẩm
    /// </summary>
    public class InventoryItem : BaseEntity
    {
        public Guid InventoryItemId { get; set; }

        [Required("Mã hàng hoá không được phép để trống.")]
        [NotDuplicate]
        [LabelText("Số điện thoại")]
        public string InventoryItemCode { get; set; }

        [Required("Tên hàng hoá không được phép để trống.")]
        [LabelText("Tên hàng hoá")]
        public string InventoryItemName { get; set; }
        public string? Description { get; set; }

        [Required("Thông tin đơn vị tính không được phép để trống.")]
        public Guid? UnitId { get; set; }
        public Guid? InventoryItemCategoryId { get; set; }
        public string? ImgPath { get; set; }

        /// <summary>
        /// Số lượng tồn kho ban đầu
        /// </summary>
        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal CostPrice { get; set; }

        /// <summary>
        /// Giá thị trường
        /// </summary>
        public decimal MarketPrice { get; set; }


        [LabelText("Mã vạch")]
        [NotDuplicate]
        [Required("Mã vạch không được phép để trống.")]
        public string Barcode { get; set; }

        public Guid? StockId { get; set; }
        public Guid? BranchId { get; set; }
        public int? InventoryItemType { get; set; }
        public string? Color { get; set; }
        public string? ColorCode { get; set; }
        public string? Size { get; set; }
        public string? Material { get; set; }
        public bool IsInventoryItem { get; set; }
        public bool IsImageByColor { get; set; }
        public bool Inactive { get; set; }
        public decimal? VATRate { get; set; }
        //public decimal Quantity { get; set; }
    }
}
