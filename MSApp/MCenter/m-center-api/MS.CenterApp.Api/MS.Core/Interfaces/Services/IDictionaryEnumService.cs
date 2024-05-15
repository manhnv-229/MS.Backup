using MS.Core.Entities;
using MS.Core.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces
{
    public interface IDictionaryEnumService
    {
        IEnumerable<EnumDictionary> GetGenders();
        IEnumerable<EnumDictionary> GetExpenditurePlanType(ReceiptType? type);
        IEnumerable<EnumDictionary> GetExpenditureTypes(ReceiptType? type);
        IEnumerable<EnumDictionary> GetWorkStatus();
        IEnumerable<EnumDictionary> GetTimeRange();
        IEnumerable<EnumDictionary> GetVIPOptions();
        IEnumerable<EnumDictionary> GetPaymentTypes();
        IEnumerable<EnumDictionary> GetReportTypes();
        IEnumerable<EnumDictionary> GetChargeType();
    }
}
