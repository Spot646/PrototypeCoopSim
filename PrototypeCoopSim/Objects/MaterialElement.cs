using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Settings;

namespace PrototypeCoopSim.Objects
{
    class MaterialElement : gameElement
    {
        public int amount;

        public MaterialElement(Game game, int worldPositionXIn, int worldPositionYIn) : base(game, worldPositionXIn, worldPositionYIn)
        {
            elementName = "Material Element(Debug)";
            amount = 0;
        }

        public override String getDetails()
        {
            String detailString;
            detailString = "Name:" + elementName + Environment.NewLine
                         + "Location: " + (worldPositionX + 1) + "," + (worldPositionY + 1) + Environment.NewLine
                         + "Amount: " + amount + Environment.NewLine + Environment.NewLine
                         + "Status: " + lastStatusUpdate;
            return detailString;
        }
    }
}
