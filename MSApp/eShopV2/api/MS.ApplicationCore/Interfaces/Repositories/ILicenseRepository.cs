using MS.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Interfaces.Repositories
{
    public interface ILicenseRepository:IRepository<MSLicense >
    {
        Task<MSLicense > GetLicenseByUserId(Guid userId);

        Task<MSLicense > GetLicenseByUserName(string userName);

        Task<MSLicense> GetLicenseByOrganizationId(Guid orgId);

        Task<int> DeleteLicenseByOrganizationId(Guid orgId);
    }
}
