using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.DTOs
{
    public class StatisticInvoiceByEmployee
    {
        public Guid EmployeeId { get; set; }
        public string FullName { get; set; }
        public Guid GroupProductId { get; set; }
        public string GroupProductName { get; set; }
        public decimal TotalMoneys { get; set; }
    }
}
