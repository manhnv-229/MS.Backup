using MS.Core.Helpers;
using MS.Core.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    [ViewDataName("Views_Slot")]
    public class Slot:BaseEntity
    {
        public Slot()
        {
            RefDetails = new HashSet<RefDetail>();
            ServiceInvoices = new HashSet<ServiceInvoice>();
            SlotInvoices = new HashSet<SlotInvoice>();
        }
        public Guid SlotId { get; set; }

        [Required("Mã bàn không được phép để trống.")]
        public string SlotCode { get; set; }

        [Required("Tên bàn không được phép để trống.")]
        public string SlotName { get; set; }

        /// <summary>
        /// Loại chỗ, loại bàn, loại phòng
        /// </summary>
        public SlotType SlotType { get; set; }

        /// <summary>
        /// Tình trạng chỗ
        /// </summary>
        public SlotStatus SlotStatus { get; set; }
        public Guid SlotGroupId { get; set; }
        public Guid? RefId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? CustomerId { get; set; }
        public string? OrdererName { get; set; }
        public string? PayerName { get; set; }
        public double TotalAmount { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }

        /// <summary>
        /// Tính tiền theo giờ (0 -không tính; 1- có tính)
        /// </summary>
        public Boolean BilledByHours { get; set; }

        /// <summary>
        /// Giá tính tiền theo giờ
        /// </summary>
        public double PriceByHour { get; set; }

        /// <summary>
        /// Ngưng sử dụng
        /// </summary>
        public Boolean Inactive { get; set; }
        public string? Description { get; set; }

        public ICollection<RefDetail> RefDetails { get; set; }
        public ICollection<ServiceInvoice> ServiceInvoices { get; set; }
        public ICollection<SlotInvoice> SlotInvoices { get; set; }
    }
}
