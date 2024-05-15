using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{

    public partial class Relation :BaseEntity
    {
        public Relation()
        {
            EmployeeFamily = new HashSet<EmployeeFamily>();
        }

        public int RelationId { get; set; }
        public string RelationCode { get; set; }
        public string RelationName { get; set; }
        public int? Sort { get; set; }

        public virtual ICollection<EmployeeFamily> EmployeeFamily { get; set; }
    }
}
