using AutoMapper;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Services
{
    public class ReportService : BaseService<Report>, IReportService
    {
        public ReportService(IRepository<Report> repository, IUnitOfWork unitOfWork, IMapper mapper) : base(repository, unitOfWork, mapper)
        {
        }
    }
}
