using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.MSEnums;
using MS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Services
{
    public class DictionaryEnumService : IDictionaryEnumService
    {
        List<EnumDictionary> dics = new List<EnumDictionary>();
        public IEnumerable<EnumDictionary> GetExpenditurePlanType(ReceiptType? type)
        {
            var dics = GetListEnumDictionary(typeof(ExpenditurePlanType).GetEnumValues());
            switch (type)
            {
                case ReceiptType.Income:
                    return dics.Where(item => (int)item.Value < 200);
                case ReceiptType.Outcome:
                    return dics.Where(item => (int)item.Value >= 200);
                default:
                    return dics;
            }
        }

        public IEnumerable<EnumDictionary> GetExpenditureTypes(ReceiptType? type)
        {
            var dics = GetListEnumDictionary(typeof(ExpenditureType).GetEnumValues());
            switch (type)
            {
                case ReceiptType.Income:
                    return dics.Where(item => (int)item.Value < 20);
                case ReceiptType.Outcome:
                    return dics.Where(item => (int)item.Value >= 20);
                default:
                    return dics;
            }
        }

        public IEnumerable<EnumDictionary> GetGenders()
        {
            return GetListEnumDictionary(typeof(Gender).GetEnumValues());
        }

        public IEnumerable<EnumDictionary> GetTimeRange()
        {
            return GetListEnumDictionary(typeof(TimeRange).GetEnumValues());
        }

        public IEnumerable<EnumDictionary> GetWorkStatus()
        {
            return GetListEnumDictionary(typeof(WorkStatus).GetEnumValues());
        }

        public IEnumerable<EnumDictionary> GetVIPOptions()
        {
            return GetListEnumDictionary(typeof(VIPOptions).GetEnumValues());
        }

        public IEnumerable<EnumDictionary> GetPaymentTypes()
        {
            return GetListEnumDictionary(typeof(PaymentType).GetEnumValues());
        }

        public IEnumerable<EnumDictionary> GetReportTypes()
        {
            return GetListEnumDictionary(typeof(ReportType).GetEnumValues());
        }

        private IEnumerable<EnumDictionary> GetListEnumDictionary(Array enumProp)
        {
            foreach (var item in enumProp)
            {
                var itemString = CommonFunction.GetResourceStringByEnum(item as Enum);
                dics.Add(new EnumDictionary() { Text = itemString, Value = (int)item });
            }
            return dics;
        }
    }
}
