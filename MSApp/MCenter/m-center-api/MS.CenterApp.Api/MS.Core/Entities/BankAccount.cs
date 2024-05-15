using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class BankAccount : BaseEntity
    {
        public Guid BankAccountId { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }

    }
}
