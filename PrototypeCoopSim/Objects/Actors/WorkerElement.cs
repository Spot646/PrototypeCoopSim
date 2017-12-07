using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Settings;

namespace PrototypeCoopSim.Objects
{
    class WorkerElement : ActorElement
    {
        public Texture2D texture;

        public WorkerElement(Game game, int worldPositionXIn, int worldPositionYIn) : base(game, worldPositionXIn, worldPositionYIn)
        {
            elementName = "Worker(Sven)";
            maxHealth = 300;
            currentHealth = 300;
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
            renderer.drawTexturedRectangle((int)(GetAnimationOffset().X + (float)(this.getWorldPositionX() * GlobalVariables.TILE_SIZE)), (int)(this.GetAnimationOffset().Y + (float)(this.getWorldPositionY() * GlobalVariables.TILE_SIZE)), GlobalVariables.TILE_SIZE, GlobalVariables.TILE_SIZE, this.texture);
        }
    }
}
