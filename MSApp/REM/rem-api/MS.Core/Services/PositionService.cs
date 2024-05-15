using AutoMapper;
using MS.Core.Entities;
using MS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Services
{
    public class PositionService : BaseService<EmployeePosition>, IPositionService
    {
        public PositionService(IPositionRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : base(repository, unitOfWork, mapper)
        {
        }
    }
}
