﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;

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

        public gameElement(Game game, int worldPositionXIn, int worldPositionYIn)
        {
            elementName = "Base Element";
            worldPositionX = worldPositionXIn;
            worldPositionY = worldPositionYIn;
        }

        public String getDetails()
        {
            String detailString;
            detailString = "Name:" + elementName + Environment.NewLine +
                            "Location: " + (worldPositionX + 1) + "," + (worldPositionY + 1) + Environment.NewLine +
                            "Health: " + currentHealth + "/" + maxHealth + Environment.NewLine +
                            "Yield: " + yield;
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

        public virtual int UpdateCurrentHealth(int damage)
        {
            return currentHealth = currentHealth - damage;
        }

        public virtual void draw(Renderer renderer)
        {
        }
    }
}
