using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public partial class Department:BaseEntity
    {
        public Department()
        {
            Employee = new HashSet<Employee>();
        }

        public Guid DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }

        public virtual Organization Organization { get; set; }
        public virtual ICollection<Employee> Employee { get; set; }
    }
}
