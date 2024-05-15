using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    public class ExpenditurePlanResponse:ExpenditurePlan
    {
        public decimal TotalMoney { get; set; }
    }
}
