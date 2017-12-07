using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Settings;

namespace PrototypeCoopSim.Objects
{
    class woodResourceElement : MaterialElement
    {
        public Texture2D texture;

        public woodResourceElement(Game game, int worldPositionXIn, int worldPositionYIn, treeElement treeElementIn) : base(game, worldPositionXIn, worldPositionYIn)
        {
            elementName = "Logs";
            maxHealth = 0;
            currentHealth = 0;
            this.amount = treeElementIn.yield;
            this.SetMovable(false);
            texture = game.Content.Load<Texture2D>("WoodPile");
        }

        override public void draw(Renderer renderer)
        {
            renderer.drawTexturedRectangle(0 + (this.getWorldPositionX() * GlobalVariables.TILE_SIZE), 0 + (this.getWorldPositionY() * GlobalVariables.TILE_SIZE), GlobalVariables.TILE_SIZE, GlobalVariables.TILE_SIZE, this.texture);
        }
    }
}
