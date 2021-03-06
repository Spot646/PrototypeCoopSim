﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Settings;

namespace PrototypeCoopSim.Objects
{
    class treeElement : ResourceElement
    {
        public Texture2D texture;

        public treeElement(Game game, int worldPositionXIn, int worldPositionYIn) : base(game, worldPositionXIn, worldPositionYIn)
        {
            elementName = "Tree";
            maxHealth = 100;
            currentHealth = 100;
            yield = (age * 2) + 1;
            this.SetMovable(false);
            texture = game.Content.Load<Texture2D>("Tree1");
        }

        public override int UpdateCurrentHealth(int damage)
        {
            //damage = 10;
            return base.UpdateCurrentHealth(damage);
        }
        override public void draw(Renderer renderer)
        {
            renderer.drawTexturedRectangle(0 + (this.getWorldPositionX() * GlobalVariables.TILE_SIZE), 0 + (this.getWorldPositionY() * GlobalVariables.TILE_SIZE), GlobalVariables.TILE_SIZE, GlobalVariables.TILE_SIZE, this.texture);
        }
        
    }
}
