using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Settings;

namespace PrototypeCoopSim.Objects
{
    class ResourceElement : gameElement
    {
        public int yield;

        public ResourceElement(Game game, int worldPositionXIn, int worldPositionYIn) : base(game, worldPositionXIn, worldPositionYIn)
        {
            elementName = "Resource Element(Debug)";
            yield = 0;
        }

        public override String getDetails()
        {
            String detailString;
            detailString = "Name:" + elementName + Environment.NewLine
                         + "Location: " + (worldPositionX + 1) + "," + (worldPositionY + 1) + Environment.NewLine
                         + "Health: " + currentHealth + "/" + maxHealth + Environment.NewLine
                         + "Yield: " + yield + Environment.NewLine + Environment.NewLine
                         + "Status: " + lastStatusUpdate;
            return detailString;
        }
    }
}
