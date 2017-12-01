using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;

namespace PrototypeCoopSim.Objects
{
    class WorkerElement : gameElement
    {
        public Texture2D texture;

        public WorkerElement(Game game, int worldPositionXIn, int worldPositionYIn) : base(game, worldPositionXIn, worldPositionYIn)
        {
            elementName = "Worker(Sven)";
            maxHealth = 300;
            currentHealth = 300;
            yield = 0;
            texture = game.Content.Load<Texture2D>("TestCharSprite");
        }

        public override int UpdateCurrentHealth(int damage)
        {
            damage = 10;
            return base.UpdateCurrentHealth(damage);
        }

        override public void draw(Renderer renderer)
        {
            renderer.drawTexturedRectangle(0 + (this.getWorldPositionX() * 25), 0 + (this.getWorldPositionY() * 25), 25, 25, this.texture);
        }
    }
}
