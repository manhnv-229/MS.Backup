using MS.Core.MSEnums;
using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS.Core.Entities
{
    public class BaseEntity
    {
        [NotMapQuery]
        public MSEntityState EntityState { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? ModifiedBy { get; set; }

        [NotMapQuery]
        public Guid? OrganizationId { get; set; }
    }
}
