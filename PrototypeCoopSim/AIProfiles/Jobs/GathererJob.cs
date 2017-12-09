using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrototypeCoopSim.Managers;
using PrototypeCoopSim.AIProfiles.Priorities;

namespace PrototypeCoopSim.AIProfiles.Jobs
{
    class GathererJob : Job
    {
        public GathererJob()
        {
            base.jobName = "Gatherer";
        }

        public override void CompilePriorities()
        {
            this.jobTaskList.ClearList();
            this.jobTaskList.AddNewPriority(new PriorityCutMarkedTrees());
        }
    }
}
