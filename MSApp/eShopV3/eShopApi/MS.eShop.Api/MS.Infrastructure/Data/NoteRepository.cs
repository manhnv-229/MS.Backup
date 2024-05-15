using Dapper;
using MS.Core.Entities;
using MS.Core.Interfaces.Repositories;
using MS.Core.Utilities;
using MS.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Infrastructure.Data
{
    public class NoteRepository : DapperRepository<Note>, INoteRepository
    {
        public NoteRepository(IMSDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async override Task<IEnumerable<Note>> GetAllAsync()
        {
            var sql = "SELECT * FROM Note n Where n.UserId = @UserId AND n.OrganizationId = @OrgId";
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", CommonFunction.UserId);
            parameters.Add("@OrgId", CommonFunction.OrganizationId);
            var notes = await DbContext.Connection.QueryAsync<Note>(sql, parameters);
            return notes;
        }
    }
}
