using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.Events;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Settings;
using PrototypeCoopSim.AIProfiles;

namespace PrototypeCoopSim.Objects
{
    class ActorElement : gameElement
    {
        //movement        
        private float speed = 1.0f;
        private bool moving;
        private bool stuck;
        bool idle;
        EventMoveTo associatedMovementEvent;
        Job currentJob;

        public ActorElement(Game game, int worldPositionXIn, int worldPositionYIn) : base(game, worldPositionXIn, worldPositionYIn)
        {
            elementName = "Actor Element(Debug)";
            moving = false;
            stuck = false;
            idle = true;
            associatedMovementEvent = null;
            currentJob = new Job();
        }

        public void SetJob(Job newJob)
        {
            currentJob = newJob;
            currentJob.CompilePriorities();
            this.hasJob = true;
        }

        public Job GetJob()
        {
            return currentJob;
        }

        public override String getDetails()
        {
            String detailString;
            detailString = "Name:" + elementName + Environment.NewLine
                         + "Location: " + (worldPositionX + 1) + "," + (worldPositionY + 1) + Environment.NewLine
                         + "Job:" + currentJob.jobName + Environment.NewLine
                         + "Health: " + currentHealth + "/" + maxHealth + Environment.NewLine + Environment.NewLine
                         + "Status: " + lastStatusUpdate;

            if (this.Idle())
            {
                detailString = detailString + Environment.NewLine + "(" + "Idle" + ")";
            }
            if (this.Moving())
            {
                detailString = detailString + Environment.NewLine + "(" + "Moving" + ")";
            }
            if (this.GetStuck())
            {
                detailString = detailString + Environment.NewLine + "(" + "Stuck" + ")";
            }
            return detailString;
        }

        public void SetStuck(bool state)
        {
            stuck = state;
        }

        public override bool GetStuck()
        {
            return stuck;
        }

        public void SetSpeed(float speedIn)
        {
            speed = speedIn;
        }

        public float GetSpeed()
        {
            return speed;
        }

        public override bool Moving()
        {
            return moving;
        }

        public override bool Idle()
        {
            return idle;
        }

        public void SetIdle(bool state)
        {
            idle = state;
        }

        public void LinkToMoveEvent(EventMoveTo linkedMovement)
        {
            moving = true;
            associatedMovementEvent = linkedMovement;
        }

        public void KillLinkedMovement()
        {
            if (moving)
            {
                moving = false;
                associatedMovementEvent.ShutdownSmoothly();
            }
        }

        public void ReplaceLinkedMovement(EventMoveTo newMovementEvent)
        {
            if (moving)
            {
                moving = false;
                newMovementEvent.Suspend(associatedMovementEvent);
                associatedMovementEvent.ShutdownSmoothly();
                this.LinkToMoveEvent(newMovementEvent);
            }
            else
            {
                this.LinkToMoveEvent(newMovementEvent);
            }
        }

        public Vector2 GetFinalDestination()
        {
            return associatedMovementEvent.ReturnDestination();
        }
    }
}
