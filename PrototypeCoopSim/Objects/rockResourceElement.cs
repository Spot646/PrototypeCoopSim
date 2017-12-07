using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Settings;

namespace PrototypeCoopSim.Objects
{
    class rockResourceElement : gameElement
    {
        public Texture2D texture;

        public rockResourceElement(Game game, int worldPositionXIn, int worldPositionYIn, RockElement rockElementIn) : base(game, worldPositionXIn, worldPositionYIn)
        {
            elementName = "Rock Pile";
            maxHealth = 0;
            currentHealth = 0;
            yield = rockElementIn.yield;
            this.SetMovable(false);
            texture = game.Content.Load<Texture2D>("rock pile");
        }

        public override int UpdateCurrentHealth(int damage)
        {
            damage = 0;
            return base.UpdateCurrentHealth(damage);
        }
        override public void draw(Renderer renderer)
        {
            renderer.drawTexturedRectangle(0 + (this.getWorldPositionX() * GlobalVariables.TILE_SIZE), 0 + (this.getWorldPositionY() * GlobalVariables.TILE_SIZE), GlobalVariables.TILE_SIZE, GlobalVariables.TILE_SIZE, this.texture);
        }
    }
}
