using MS.Core.Entities;
using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    [DataTableName("ServiceInvoice")]
    public class ServiceInvoiceDto : ServiceInvoice
    {
        public string? ServiceName { get; set; }
        public string? EmployeeIdsString { get; set; }


        public List<string> EmployeeIds
        {
            get
            {
                var ids = new List<string>();
                if (!string.IsNullOrEmpty(EmployeeIdsString))
                {
                    var idsArray = EmployeeIdsString.Split(',');
                    foreach (var id in idsArray)
                    {
                        ids.Add(id);
                    }
                }
                return ids;

            }
        }
    }
}
