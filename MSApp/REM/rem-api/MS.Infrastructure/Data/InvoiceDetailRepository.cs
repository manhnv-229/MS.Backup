using MISA.FM.Infrastructure.Repositories;
using MS.Core.Entities;
using MS.Core.Interfaces.Repositories;
using MS.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MS.Infrastructure.Interfaces;

namespace MS.Infrastructure.Data
{
    public class InvoiceDetailRepository : DapperRepository<InvoiceDetail>,IInvoiceDetailRepository
    {
        public InvoiceDetailRepository(IMSDatabaseContext dbContext,AutoMapper.IMapper mapper) : base(dbContext,mapper)
        {
        }
    }
}
