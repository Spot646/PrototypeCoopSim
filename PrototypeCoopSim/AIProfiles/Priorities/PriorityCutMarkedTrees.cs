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

namespace PrototypeCoopSim.AIProfiles.Priorities
{
    class PriorityCutMarkedTrees : Priority
    {
        public PriorityCutMarkedTrees()
        {
            this.priorityName = "Cut Marked Trees";
        }

        public override bool Process(Game gameIn, mapManager mapIn, EventManager eventManagerIn, gameElement focusElementIn, GameTime gameTimeIn)
        {            
            if (PriorityCondition.MarkedTreesExist())
            {
                eventManagerIn.AddEvent(new EventHarvestTrees(gameIn, mapIn, eventManagerIn, focusElementIn, gameTimeIn, false));
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
