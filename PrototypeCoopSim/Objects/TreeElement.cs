﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;

namespace PrototypeCoopSim.Objects
{
    class treeElement : gameElement
    {
        public Texture2D texture;

        public treeElement(Game game, int worldPositionXIn, int worldPositionYIn) : base(game, worldPositionXIn, worldPositionYIn)
        {
            elementName = "Tree";
            texture = game.Content.Load<Texture2D>("Tree1");
        }
        
        override public void draw(Renderer renderer)
        {
            renderer.drawTexturedRectangle(0 + (this.getWorldPositionX() * 25), 0 + (this.getWorldPositionY() * 25), 25, 25, this.texture);
        }
    }
}