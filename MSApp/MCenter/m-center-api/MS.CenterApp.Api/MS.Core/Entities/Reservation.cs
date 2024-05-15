using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class Reservation:BaseEntity
    {
        public Guid ReservationId { get; set; }
        public DateTime ReservationDate { get; set; }
        public string? ReservationistName { get; set; }
        public Guid? CustomerId { get; set; }
        public DateTime TimeOfUse { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
    }
}
