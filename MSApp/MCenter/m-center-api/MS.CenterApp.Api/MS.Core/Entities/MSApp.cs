using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class MSApp:BaseEntity
    {
        public Guid MSAppId { get; set; }
        public string MSAppCode { get; set; }
        public string MSAppName { get; set; }
        public string ApiUrl { get; set; }
        public string? Description { get; set; }
    }
}
