using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public partial class EmployeeFamily : BaseEntity
    {
        public Guid EmployeeFamilyId { get; set; }
        public Guid? EmployeeId { get; set; }
        public string FullName { get; set; }
        public int Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string CitizenIdentityNo { get; set; }
        public int? RelationId { get; set; }
        public int? DobdisplaySetting { get; set; }
        public int? NationalityId { get; set; }
        public int? EthnicId { get; set; }
        public string BirthVillageAddress { get; set; }
        public int? SortOrder { get; set; }

        public int? Sort { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Relation Relation { get; set; }
    }
}
