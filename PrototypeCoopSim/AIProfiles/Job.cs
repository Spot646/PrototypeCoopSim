using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Events;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Managers;
using System.Collections.Generic;
using PrototypeCoopSim.Settings;

namespace PrototypeCoopSim.AIProfiles
{
    class Job
    {
        //Store priorities
        public PriorityList jobTaskList;
        public String jobName;

        public Job()
        {
            jobName = "Default Job";
            jobTaskList = new PriorityList(this);
        }

        public virtual void CompilePriorities()
        {

        }
        
        //Check the next priority entry for this Job
        public void ProcessJobPriorities(Game gameIn, mapManager mapIn, EventManager eventManagerIn, gameElement focusElementIn, GameTime gameTimeIn)
        {
            jobTaskList.ProcessNext(gameIn, mapIn, eventManagerIn, focusElementIn, gameTimeIn);
        }
    }
}
