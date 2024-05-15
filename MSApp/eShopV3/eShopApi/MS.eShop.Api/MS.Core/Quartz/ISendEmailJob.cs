using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Quartz
{
    public interface ISendEmailJob: IJob
    {
        Task Execute(IJobExecutionContext context);
    }
}
