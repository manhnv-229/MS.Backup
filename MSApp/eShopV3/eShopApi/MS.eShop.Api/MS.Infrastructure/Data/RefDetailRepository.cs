﻿using MISA.FM.Infrastructure.Repositories;
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
    public class RefDetailRepository : DapperRepository<RefDetail>, IRefDetailRepository
    {
        public RefDetailRepository(IMSDatabaseContext dbContext) : base(dbContext)
        {
        }
    }
}
