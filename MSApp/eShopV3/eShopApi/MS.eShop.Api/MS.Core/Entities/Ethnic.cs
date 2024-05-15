using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public partial class Ethnic:BaseEntity
    {
        public Ethnic()
        {
            Employee = new HashSet<Employee>();
        }

        public int EthnicId { get; set; }
        public string EthnicCode { get; set; }
        public string EthnicName { get; set; }
        public DateTime? ActiveDate { get; set; }
        public DateTime? ExpireDate { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}
