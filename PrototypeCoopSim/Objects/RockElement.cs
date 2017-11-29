using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;

namespace PrototypeCoopSim.Objects
{
    class RockElement : gameElement
    {
        public Texture2D texture;

        public RockElement(Game game, int worldPositionXIn, int worldPositionYIn) : base(game, worldPositionXIn, worldPositionYIn)
        {
            elementName = "Rock";
            texture = game.Content.Load<Texture2D>("rock1");
        }
        
        override public void draw(Renderer renderer)
        {
            renderer.drawTexturedRectangle(0 + (getWorldPositionX() * 25), 0 + (getWorldPositionY() * 25), 25, 25, texture);
        }
    }
}
