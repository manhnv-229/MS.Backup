using MS.Core.DTOs;
using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces
{
    public interface IExpenditureRepository:IRepository<Expenditure>
    {
        Task<IEnumerable<Expenditure>> GetRevenues();
        Task<IEnumerable<Expenditure>> GetExpenditures();
        Task<FundInfo> GetGeneralInfo();
    }
}
