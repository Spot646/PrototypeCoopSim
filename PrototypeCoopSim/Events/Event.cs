using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Events;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Managers;

namespace PrototypeCoopSim.Events
{
    class Event
    {
        Event callingEvent = null;
        private String eventName;
        bool completed = false;
        private bool active = true;

        public Event (){
            
        }

        public void SetEventName(String eventNameIn)
        {
            eventName = eventNameIn;
        }

        public void Suspend(Event completionEventIn)
        {
            active = false;
            completionEventIn.setCallingEvent(this);
        }

        public void setCallingEvent(Event callingEventIn)
        {
            callingEvent = callingEventIn;
        }

        public void Activate()
        {
            active = true;
        }

        public bool IsActive()
        {
            return active;
        }

        public void SetComplete()
        {
            completed = true;
        }

        public bool HasEnded()
        {
            if (completed && callingEvent != null)
            {
                callingEvent.Activate();
            }
            return completed;
        }
            
        public virtual void RunEvent(EventManager callingEventManager)
        {

        }
    }
}
