using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MS.Core.Interfaces.Services;

namespace MS.Core.Quartz
{
    public class BackUpDatabaseJob: IJob
    {
        IOrganizationService _orgService;
        public BackUpDatabaseJob(IOrganizationService orgService)
        {
            _orgService = orgService;
        }
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                Console.WriteLine("Backup start.....");
               _orgService.BackupDatabase();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }
    }
}
