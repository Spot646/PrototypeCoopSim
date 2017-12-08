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
    class PriorityList
    {
        Job owner;
        int currentPriorityIndex;
        List<Priority> priorityList;

        public PriorityList(Job callingJob)
        {
            owner = callingJob;
            currentPriorityIndex = 0;
            priorityList = new List<Priority>();
        }

        public void ClearList()
        {
            priorityList.Clear();
        }

        public void AddNewPriority(Priority newPriority)
        {
            priorityList.Add(newPriority);
        }

        public void ProcessNext(Game gameIn, mapManager mapIn, EventManager eventManagerIn, gameElement focusElementIn, GameTime gameTimeIn)
        {
            if (priorityList.Count > 0)
            {
                if (currentPriorityIndex > priorityList.Count)
                {
                    currentPriorityIndex = 0;
                }

                if (priorityList[currentPriorityIndex].Process(gameIn, mapIn, eventManagerIn, focusElementIn, gameTimeIn)) //process
                {
                    currentPriorityIndex = 0;
                }                
            }
        }
    }
}
