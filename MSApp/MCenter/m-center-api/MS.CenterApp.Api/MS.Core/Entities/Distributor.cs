using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class Distributor:BaseEntity
    {
        public Guid DistributorId { get; set; }

        [DisplayName("Tên nhà phân phối")]
        [Required(ErrorMessage ="Tên nhà phân phối không được để trống.")]
        public string DistributorName { get; set; }

        [DisplayName("Số điện thoại nhà phân phối")]
        public string PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? ContactName { get; set; }

        public string? ContactPhoneNumber { get; set; }
        public string? TaxCode { get; set; }
        public string? Description { get; set; }

    }
}
