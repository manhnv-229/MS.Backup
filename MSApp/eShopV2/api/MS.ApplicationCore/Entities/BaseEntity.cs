using MS.ApplicationCore.MSEnums;
using MS.ApplicationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS.ApplicationCore.Entities
{
    public class BaseEntity
    {
        [NotMapQuery]
        public EntityState EntityState { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? ModifiedBy { get; set; }

        [NotMapQuery]
        public Guid? OrganizationId { get; set; }
    }
}
