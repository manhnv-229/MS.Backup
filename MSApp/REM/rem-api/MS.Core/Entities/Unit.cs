using MS.Core.Helpers;
using MS.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    [HasSystemData]
    [ModelClass<UnitModel>]
    public class Unit:BaseEntity
    {
        public Guid UnitId { get; set; }

        [Required("Mã đơn vị tính không được để trống")]
        public string UnitCode { get; set; }

        [Required("Tên đơn vị tính không được để trống")]
        public string UnitName { get; set; }
        public string? Description { get; set; }
        public bool IsSystem { get; set; }

    }
}
