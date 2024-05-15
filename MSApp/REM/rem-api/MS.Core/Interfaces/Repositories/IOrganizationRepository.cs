using Microsoft.AspNetCore.Http;
using MS.Core.DTOs;
using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces.Repositories
{
    public interface IOrganizationRepository:IRepository<Organization>
    {
        Task<Organization> GetOrganizationByUserName(string userName);
        Task<IEnumerable<OrganizationStatisticDto>> GetOrgStatisticByDay(DateTime? date);
        FormFile BackUpDatabase(MemoryStream ms);
    }
}
