using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Events;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Managers;
using System.Collections.Generic;

namespace PrototypeCoopSim.Objects
{
    class gameElement
    {
        public String elementName;
        public int maxHealth;
        public int currentHealth;
        public int age = 0;
        public int yield;
        public int worldPositionX;
        public int worldPositionY;

        //movement        
        private float speed = 1.0f;
        private bool movable;
        private bool moving;
        EventMoveTo associatedMovementEvent;

        //Status information
        String lastStatusUpdate;
        bool idle;

        //Animation
        Vector2 animationOffset;
        
        public gameElement(Game game, int worldPositionXIn, int worldPositionYIn)
        {
            elementName = "Base Element";
            worldPositionX = worldPositionXIn;
            worldPositionY = worldPositionYIn;
            animationOffset = new Vector2(0.0f, 0.0f);
            moving = false;
            idle = true;
            associatedMovementEvent = null;
            lastStatusUpdate = "";
        }

        public String getDetails()
        {
            String detailString;
            detailString = "Name:" + elementName + Environment.NewLine
                         + "Location: " + (worldPositionX + 1) + "," + (worldPositionY + 1) + Environment.NewLine
                         + "Health: " + currentHealth + "/" + maxHealth + Environment.NewLine
                         + "Yield: " + yield + Environment.NewLine + Environment.NewLine
                         + "Status: " + lastStatusUpdate;
            if (this.Idle())
            {
                detailString = detailString + Environment.NewLine + "(" + "Idle" + ")";
            }
            if (this.Moving())
            {
                detailString = detailString + Environment.NewLine + "(" + "Moving" + ")";
            }
            return detailString;
        }

        public int getWorldPositionX()
        {
            return worldPositionX;
        }

        public int getWorldPositionY()
        {
            return worldPositionY;
        }

        public Vector2 getWorldPositionVector()
        {
            return new Vector2(this.worldPositionX, this.worldPositionY);
        }

        public void SetMovable(bool state)
        {
            movable = state;
        }

        public bool GetMovable()
        {
            return movable;
        }

        public void SetSpeed(float speedIn)
        {
            speed = speedIn;
        }

        public float GetSpeed()
        {
            return speed;
        }

        public bool Moving()
        {
            return moving;
        }

        public bool Idle()
        {
            return idle;
        }

        public void LinkToMoveEvent(EventMoveTo linkedMovement)
        {
            moving = true;
            idle = false;
            associatedMovementEvent = linkedMovement;
        }

        public void KillLinkedMovement()
        {
            if (moving)
            {
                idle = true;
                moving = false;
                associatedMovementEvent.ShutdownSmoothly();
            }
        }

        public void ReplaceLinkedMovement(EventMoveTo newMovementEvent)
        {
            if (moving)
            {
                idle = true;
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

        public String GetElementName()
        {
            return elementName;
        }

        public void SetAnimationOffset(Vector2 offset)
        {
            animationOffset = offset;
        }

        public Vector2 GetAnimationOffset()
        {
            return animationOffset;
        }

        public void SetStatusMessage(String newMessage)
        {
            lastStatusUpdate = newMessage;
        }

        public virtual int UpdateCurrentHealth(int damage)
        {
            return currentHealth = currentHealth - damage;
        }

        public virtual int GetIconTypes()
        {
            return 0;
        }

        public virtual void draw(Renderer renderer)
        {
        }
    }
}
