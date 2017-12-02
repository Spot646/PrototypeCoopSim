using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;

namespace PrototypeCoopSim.Objects
{
    class FirepitElement : gameElement
    {
        public Texture2D texture;

        public FirepitElement(Game game, int worldPositionXIn, int worldPositionYIn) : base(game, worldPositionXIn, worldPositionYIn)
        {
            elementName = "Firepit";
            maxHealth = 250;
            currentHealth = 250;
            yield = 25;
            this.SetMovable(false);
            texture = game.Content.Load<Texture2D>("fire");
        }

        public override int UpdateCurrentHealth(int damage)
        {
            damage = 2;
            return base.UpdateCurrentHealth(damage);
        }

        override public void draw(Renderer renderer)
        {
            renderer.drawTexturedRectangle(0 + (getWorldPositionX() * 25), 0 + (getWorldPositionY() * 25), 25, 25, texture);
        }
    }
}
