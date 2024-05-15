using Microsoft.AspNetCore.Mvc;
using MS.Core.DTOs;
using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces.Services
{
    public interface IOrganizationService:IBaseService<Organization>
    {

        /// <summary>
        /// Gia hạn VIP
        /// </summary>
        /// <param name="licenseInfo">Thông tin gia hạn VIP</param>
        /// <returns></returns>
        Task<int> RenewLicense(MSLicenseRenew licenseInfo);
        Task<int> DeleteLicense(Guid organizationId);
        Task BackupDatabase();
        Task<IEnumerable<OrganizationStatisticDto>> GetOrgStatisticByDay(DateTime date, bool sentEmail = false);
    }
}
