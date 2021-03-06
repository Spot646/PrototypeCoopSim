﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Events;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.Managers;

namespace PrototypeCoopSim.Events
{
    class Event
    {
        Event callingEvent = null;
        Event suspendEvent = null;
        private String eventName;
        bool completed = false;
        bool shutdownSmoothly = false;
        private bool active = true;

        ActorElement actorToWake = null; 

        public Event (){
            
        }

        public void SetEventName(String eventNameIn)
        {
            eventName = eventNameIn;
        }

        public void Suspend(Event completionEventIn)
        {
            active = false;
            suspendEvent = completionEventIn;
            completionEventIn.setCallingEvent(this);
        }

        public void WakeAssociated(){
            if(actorToWake != null)
            {
                actorToWake.SetIdle(true);
            }
        }

        public bool SetAssociated(ActorElement associatedActorIn)
        {
            if (associatedActorIn.Idle())
            {
                associatedActorIn.SetIdle(false);
                actorToWake = associatedActorIn;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetSuspendUntilEvent(Event suspendEventIn)
        {
            suspendEvent = suspendEventIn;
        }

        public void setCallingEvent(Event callingEventIn)
        {
            callingEvent = callingEventIn;
        }

        public Event GetCallingEvent()
        {
            return callingEvent;
        }

        public Event SuspentUntilThisEventFinished()
        {
            return suspendEvent;
        }

        public void Activate()
        {
            active = true;
            suspendEvent = null;
        }

        public bool IsActive()
        {
            return active;
        }

        public void SetComplete()
        {
            completed = true;
        }

        public bool IsComplete()
        {
            return completed;
        }

        public void ShutdownSmoothly()
        {
            shutdownSmoothly = true;
        }

        public bool GetShutdownSmoothly()
        {
            return shutdownSmoothly;
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
