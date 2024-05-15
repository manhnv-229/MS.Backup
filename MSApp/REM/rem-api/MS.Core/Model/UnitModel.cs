using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Model
{
    public class UnitModel : BaseEntity
    {
        public Guid UnitId { get; set; }
        [Required(ErrorMessage = "Tên đơn vị không được để trống")]
        public string UnitName { get; set; }
    }
}
