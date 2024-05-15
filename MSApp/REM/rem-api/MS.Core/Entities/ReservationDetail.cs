using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    /// <summary>
    /// Chi tiết đặt chỗ
    /// </summary>
    public class ReservationDetail:BaseEntity
    {
        public Guid ReservationDetailId { get; set; }
        public Guid ReservationId { get; set; }
        public Guid SlotID { get; set; }
        public DateTime? TimeOfUse { get; set; }
        public string? Description { get; set; }
    }
}
