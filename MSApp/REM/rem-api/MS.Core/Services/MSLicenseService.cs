using AutoMapper;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Services
{
    public class MSLicenseService : BaseService<MSLicense >, ILicenseService
    {
        public MSLicenseService(ILicenseRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : base(repository, unitOfWork, mapper)
        {
        }
    }
}
