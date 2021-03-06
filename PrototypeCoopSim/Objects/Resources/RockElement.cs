﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Settings;

namespace PrototypeCoopSim.Objects
{
    class RockElement : ResourceElement
    {
        public Texture2D texture;

        public RockElement(Game game, int worldPositionXIn, int worldPositionYIn) : base(game, worldPositionXIn, worldPositionYIn)
        {
            elementName = "Rock";
            maxHealth = 250;
            currentHealth = 250;
            yield = 25;
            this.SetMovable(false);
            texture = game.Content.Load<Texture2D>("RockResourceV2");
        }

        public override int UpdateCurrentHealth(int damage)
        {
            //damage = 25;
            return base.UpdateCurrentHealth(damage);
        }

        override public void draw(Renderer renderer)
        {
            renderer.drawTexturedRectangle(0 + (getWorldPositionX() * GlobalVariables.TILE_SIZE), 0 + (getWorldPositionY() * GlobalVariables.TILE_SIZE), GlobalVariables.TILE_SIZE, GlobalVariables.TILE_SIZE, texture);
        }
    }
}
