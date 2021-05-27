using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.Jobs
{
    public class JobOnce : Job
    {
        public JobOnce(DateTime _sd)
        {
            StartDate = _sd;
        }

        //Do Job once after start datetime, and then return false;
        public override bool Tick()
        {
            DateTime nowUtc = DateTime.UtcNow;
            if (StartDate > nowUtc)
                return true;
            JobAct?.Invoke();
            return false;
        }

        public override async Task<bool> TickAsync()
        {
            return false;
        }
    }
}
