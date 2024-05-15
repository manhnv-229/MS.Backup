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
    public class ReservationDetailService : BaseService<ReservationDetail>, IReservationDetailService
    {
        public ReservationDetailService(IRepository<ReservationDetail> repository, IUnitOfWork unitOfWork, IMapper mapper) : base(repository, unitOfWork, mapper)
        {
        }
    }
}
