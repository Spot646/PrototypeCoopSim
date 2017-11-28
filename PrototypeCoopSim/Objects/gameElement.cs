using Microsoft.Xna.Framework;
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
                            "Location: " + (worldPositionX + 1) + "," + (worldPositionY + 1);
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

        public virtual void draw(Renderer renderer)
        {
        }
    }
}
