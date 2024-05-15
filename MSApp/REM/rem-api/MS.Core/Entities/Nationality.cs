using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public partial class Nationality:BaseEntity
    {
        public Nationality()
        {
            Employee = new HashSet<Employee>();
        }

        public int NationalityId { get; set; }
        public string NationalityCode { get; set; }
        public string NationalityName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}
