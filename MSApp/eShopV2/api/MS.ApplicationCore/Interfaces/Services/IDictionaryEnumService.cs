﻿using MS.ApplicationCore.Entities;
using MS.ApplicationCore.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Interfaces
{
    public interface IDictionaryEnumService
    {
        IEnumerable<EnumDictionary> GetGenders();
        IEnumerable<EnumDictionary> GetExpenditurePlanType(ReceiptType? type);
        IEnumerable<EnumDictionary> GetExpenditureTypes(ReceiptType? type);
        IEnumerable<EnumDictionary> GetWorkStatus();
        IEnumerable<EnumDictionary> GetTimeRange();
        IEnumerable<EnumDictionary> GetVIPOptions();
    }
}
