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
        public int worldPositionX;
        public int worldPositionY;
        public bool movable;

        //Status information
        public String lastStatusUpdate;

        //Animation
        private Vector2 animationOffset;
        
        public gameElement(Game game, int worldPositionXIn, int worldPositionYIn)
        {
            elementName = "Game Element(Debug)";
            movable = false;
            worldPositionX = worldPositionXIn;
            worldPositionY = worldPositionYIn;
            animationOffset = new Vector2(0.0f, 0.0f);
            lastStatusUpdate = "";
        }

        public virtual String getDetails()
        {
            return "Invalid Display";
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

        public bool GetMovable()
        {
            return movable;
        }

        public void SetMovable(bool state)
        {
            movable = state;
        }

        public virtual bool Moving()
        {
            return false;
        }

        public virtual bool Idle()
        {
            return true;
        }

        public virtual bool GetStuck()
        {
            return false;
        }

        public virtual void draw(Renderer renderer)
        {
        }
    }
}
