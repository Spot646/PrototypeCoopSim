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
            this.SetSpeed(0.05f);
            this.SetMovable(true);
            texture = game.Content.Load<Texture2D>("TestCharSprite");
        }

        public override int UpdateCurrentHealth(int damage)
        {
            damage = 10;
            return base.UpdateCurrentHealth(damage);
        }

        public override int GetIconTypes()
        {
            return 1;
        }

        override public void draw(Renderer renderer)
        {
            renderer.drawTexturedRectangle((int)(GetAnimationOffset().X + (float)(this.getWorldPositionX() * 25)), (int)(this.GetAnimationOffset().Y + (float)(this.getWorldPositionY() * 25)), 25, 25, this.texture);
        }
    }
}
