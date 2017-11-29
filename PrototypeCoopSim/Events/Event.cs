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
        private String eventName;
        private bool active;

        public Event (){

        }

        public void SetEventName(String eventNameIn)
        {
            eventName = eventNameIn;
        }

        public void Suspend(bool state)
        {
            active = !state;
        }

        public bool IsActive()
        {
            return active;
        }

        public virtual bool HasEnded()
        {
            return true;
        }
            
        public virtual void RunEvent()
        {

        }
    }
}
