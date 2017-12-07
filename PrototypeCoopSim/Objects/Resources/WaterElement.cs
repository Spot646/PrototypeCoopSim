using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Events;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Managers;
using System.Collections.Generic;
using PrototypeCoopSim.Settings;

namespace PrototypeCoopSim.Objects
{
    class WaterElement: ResourceElement
    {
        public Texture2D texture;
        public Texture2D texture2;
        public Texture2D normalSource;
        public Texture2D normalMap1;
        public Texture2D overflowMap1;
        public Texture2D normalMap2;
        public Texture2D overflowMap2;
        Effect waterNormalMap;
        mapManager associatedMap;
        Game associatedGame;
        int normalOrigin1X;
        int normalOrigin1Y;
        int normalOrigin2X;
        int normalOrigin2Y;
        Rectangle sourceRectangle;
        Rectangle sourceRectangle2;
        Color[] data = new Color[GlobalVariables.TILE_SIZE * GlobalVariables.TILE_SIZE];
        Color[] data2 = new Color[GlobalVariables.TILE_SIZE * GlobalVariables.TILE_SIZE];

        public WaterElement(Game game, int worldPositionXIn, int worldPositionYIn, mapManager mapManagerIn) : base(game, worldPositionXIn, worldPositionYIn)
        {
            elementName = "Water";
            associatedMap = mapManagerIn;
            maxHealth = 0;
            currentHealth = 0;
            yield = 0;
            associatedGame = game;
            this.SetMovable(false);
            waterNormalMap = game.Content.Load<Effect>("FX/WaterNormal");

            //generate normal maps
            normalSource = game.Content.Load<Texture2D>("waterNormalv2");
            normalMap1 = new Texture2D(associatedGame.GraphicsDevice, GlobalVariables.TILE_SIZE, GlobalVariables.TILE_SIZE);
            normalMap2 = new Texture2D(associatedGame.GraphicsDevice, GlobalVariables.TILE_SIZE, GlobalVariables.TILE_SIZE);

            //generate underwater tile
            Texture2D waterFloorTemp = game.Content.Load<Texture2D>("UnderwaterV1.0");
            Rectangle sourceRectangle = new Rectangle(0, 32, GlobalVariables.TILE_SIZE, GlobalVariables.TILE_SIZE);
            texture = new Texture2D(associatedGame.GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
            Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
            waterFloorTemp.GetData(0, sourceRectangle, data, 0, data.Length);
            texture.SetData(data);

            //generate water wall
            Texture2D waterWallTemp = game.Content.Load<Texture2D>("UnderwaterV1.0");
            Rectangle sourceRectangle2 = new Rectangle(0, 0, GlobalVariables.TILE_SIZE, GlobalVariables.TILE_SIZE);
            texture2 = new Texture2D(associatedGame.GraphicsDevice, sourceRectangle2.Width, sourceRectangle2.Height);
            Color[] data2 = new Color[sourceRectangle2.Width * sourceRectangle2.Height];
            waterWallTemp.GetData(0, sourceRectangle2, data2, 0, data2.Length);
            texture2.SetData(data2);
        }

        public override int UpdateCurrentHealth(int damage)
        {
            damage = 10;
            return base.UpdateCurrentHealth(damage);
        }

        override public void draw(Renderer renderer)
        {
            //Find proper atlas position
            int atlasX = 0;
            int atlasY = 0;

            int numberNorth = 0;
            int numberSouth = 0;
            int numberEast = 0;
            int numberWest = 0;

            //check north
            if (associatedMap.checkOccupied(new Vector2(this.getWorldPositionX(), this.getWorldPositionY() - 1), "Water"))
            {
                numberNorth++;
                if (associatedMap.checkOccupied(new Vector2(this.getWorldPositionX(), this.getWorldPositionY() - 2), "Water"))
                {
                    numberNorth++;
                }
            }
            //check east
            if (associatedMap.checkOccupied(new Vector2(this.getWorldPositionX() + 1, this.getWorldPositionY()), "Water"))
            {
                numberEast++;
                if (associatedMap.checkOccupied(new Vector2(this.getWorldPositionX() + 2, this.getWorldPositionY()), "Water"))
                {
                    numberEast++;
                }
            }
            //check south
            if (associatedMap.checkOccupied(new Vector2(this.getWorldPositionX(), this.getWorldPositionY() + 1), "Water"))
            {
                numberSouth++;
                if (associatedMap.checkOccupied(new Vector2(this.getWorldPositionX(), this.getWorldPositionY() + 2), "Water"))
                {
                    numberSouth++;
                }
            }
            //check west
            if (associatedMap.checkOccupied(new Vector2(this.getWorldPositionX() - 1, this.getWorldPositionY()), "Water"))
            {
                numberWest++;
                if (associatedMap.checkOccupied(new Vector2(this.getWorldPositionX() - 2, this.getWorldPositionY()), "Water"))
                {
                    numberWest++;
                }
            }
            //find position
            atlasY = 0 + (Math.Abs(numberNorth * numberSouth + 1) / 2  * 32);
            atlasX = 0 + (Math.Abs(numberEast * numberWest + 1) / 2  * 32);

            //Generate the texture
            //Convert normal map
            //Find the world position of top left
            normalOrigin1X = ((this.getWorldPositionX() * GlobalVariables.TILE_SIZE) + (int)this.GetAnimationOffset().X) % (normalSource.Width / 2);
            normalOrigin1Y = (this.getWorldPositionY() * GlobalVariables.TILE_SIZE) % (normalSource.Height / 2);
            normalOrigin2X = (this.getWorldPositionX() * GlobalVariables.TILE_SIZE) % (normalSource.Width / 2);
            normalOrigin2Y = ((this.getWorldPositionY()  * GlobalVariables.TILE_SIZE) + (int)this.GetAnimationOffset().Y) % (normalSource.Height / 2);

            //generate normal map 1
            sourceRectangle = new Rectangle(normalOrigin1X, normalOrigin1Y, GlobalVariables.TILE_SIZE, GlobalVariables.TILE_SIZE);
            normalSource.GetData(0, sourceRectangle, data, 0, data.Length);
            normalMap1.SetData(data);

            //generate normal map 2
            //check for overflow on origin.x for first normal map
            sourceRectangle2 = new Rectangle(normalOrigin2X, normalOrigin2Y, GlobalVariables.TILE_SIZE, GlobalVariables.TILE_SIZE);
            normalSource.GetData(0, sourceRectangle2, data2, 0, data2.Length);
            normalMap2.SetData(data2);

            //Draw the proper normal position
            renderer.endDrawing();
            waterNormalMap.Parameters["SpriteTexture"].SetValue(texture);
            waterNormalMap.Parameters["SpriteNormal"].SetValue(normalMap1);
            waterNormalMap.Parameters["SpriteNormal2"].SetValue(normalMap2);
            renderer.startShadedDrawing(waterNormalMap);
            renderer.drawTexturedRectangle(0 + (this.getWorldPositionX() * GlobalVariables.TILE_SIZE), 0 + (this.getWorldPositionY() * GlobalVariables.TILE_SIZE + (GlobalVariables.TILE_SIZE/2)) - 5, GlobalVariables.TILE_SIZE, GlobalVariables.TILE_SIZE, this.texture);
            //Draw water wall if square above is not water
            renderer.endDrawing();
            renderer.startDrawing();  
        }

        public void DrawLedge(Renderer renderer)
        {
            if (!associatedMap.checkOccupied(new Vector2(this.getWorldPositionX(), this.getWorldPositionY() - 1), "Water"))
            {
                renderer.drawTexturedRectangle(0 + (this.getWorldPositionX() * GlobalVariables.TILE_SIZE), 0 + (this.getWorldPositionY() * GlobalVariables.TILE_SIZE) - (GlobalVariables.TILE_SIZE / 2) - 5, GlobalVariables.TILE_SIZE, GlobalVariables.TILE_SIZE, this.texture2);
            }
        }
    }
}
