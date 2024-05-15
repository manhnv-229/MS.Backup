using MS.Core.Entities;
using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    [DataTableName("Slot")]
    public class SlotDto:Slot
    {
        public Guid? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? SlotGroupName { get; set; }
        public string? CustomerName { get; set; }
        public string? EmployeeName { get; set; }
        /// <summary>
        /// Thông tin đặt hàng chưa hoàn thành
        /// </summary>
        public IEnumerable<Reservation>? ReservationsPending { get; set; }
    }
}
