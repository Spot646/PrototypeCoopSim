using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Events;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Managers;

namespace PrototypeCoopSim.Managers
{
    class EventManager
    {
        Game associatedGame;
        mapManager associatedMap;
        List<Event> eventList = new List<Event>();

        public EventManager(Game gameIn, mapManager mapIn)
        {
            associatedGame = gameIn;
            associatedMap = mapIn;
        }

        public void AddEvent(Event newEvent)
        {
            eventList.Add(newEvent);
        }

        public void RunEvents()
        {
            //Run all events
            if (eventList.Count > 0)
            {
                for (int i = 0; i < eventList.Count; i++)
               {
                    if (eventList[i].IsActive())
                    {
                       eventList[i].RunEvent(this);
                    }
                }

                //Delete completed events
                for (int i = 0; i < eventList.Count; i++)
                {
                    if (eventList[i].HasEnded())
                    {
                        eventList[i].WakeAssociated();
                        eventList.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }
}
