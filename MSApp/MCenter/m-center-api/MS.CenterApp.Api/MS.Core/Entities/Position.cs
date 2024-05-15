using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public partial class Position: BaseEntity
    {
        public Position()
        {
            Employee = new HashSet<Employee>();
        }

        public Guid PositionId { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public string Description { get; set; }
        public Guid? ParentId { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<Employee> Employee { get; set; }
    }
}
