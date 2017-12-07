using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Settings;

namespace PrototypeCoopSim.Managers
{
    class mapManager
    {
        //General storage
        private int sizeX;
        private int sizeY;
        private int time;
        private int date;
        private int numberTiles;
        private int scanX = 0;
        private int scanY = 0;

        //tile states
        bool[] passable;
        int[] groundType;
        bool[] occupied;
        gameElement[] occupyingElement;
        Game associatedGame;

        //Map type 1
        public Texture2D groundTexture; //Ground Texture
        public Texture2D testTreeTexture; //Ground Texture
        //Drawing information
        RenderTarget2D waterRenderTarget1;
        RenderTarget2D waterRenderTarget2;
        Effect waterEffect;

        public mapManager(Game game, int tilesXIn, int tilesYIn)
        {
            sizeX = tilesXIn;
            sizeY = tilesYIn;
            scanX = 0;
            scanY = 0;
            time = 0;
            date = 0;
            numberTiles = tilesXIn * tilesYIn;
            associatedGame = game;
            //Rendering information
            //waterRenderTarget1 = new RenderTarget2D(
            //    game.GraphicsDevice,
            //    game.GraphicsDevice.PresentationParameters.BackBufferWidth,
            //    game.GraphicsDevice.PresentationParameters.BackBufferHeight,
            //    false,
            //    SurfaceFormat.Color,
            //    DepthFormat.Depth24);
            waterEffect = game.Content.Load<Effect>("FX/Water");

            passable = new bool[numberTiles];
            for(int i = 0; i < numberTiles; i++)
            {
                passable[i] = true;
            }

            groundType = new int[numberTiles];
            for (int i = 0; i < numberTiles; i++)
            {
                groundType[i] = 0;
            }

            occupied = new bool[numberTiles];
            for (int i = 0; i < numberTiles; i++)
            {
                occupied[i] = false;
            }

            occupyingElement = new gameElement[numberTiles];
            groundTexture = game.Content.Load<Texture2D>("Grassv2");
            testTreeTexture = game.Content.Load<Texture2D>("Treev2");
        }

        public Vector2 getTileFromMousePosition(int mouseX, int mouseY, int tileWidth, int mapOriginX, int mapOriginY, int mapWindowSizeX, int mapWindowSizeY)
        {
            Vector2 mouseTilePosition = new Vector2();

            if(mouseX > mapOriginX && mouseX < (mapOriginX + mapWindowSizeX) && mouseY > mapOriginY && mouseY < (mapOriginY + mapWindowSizeY))
            {
                mouseTilePosition.X = ((int)(((int)(mouseX + scanX - mapOriginX)) / tileWidth));// * tileWidth;
                mouseTilePosition.Y = ((int)(((int)(mouseY + scanY - mapOriginY)) / tileWidth));// * tileWidth;
            }
            else
            {
                mouseTilePosition.X = 0;
                mouseTilePosition.Y = 0;
            }
            return mouseTilePosition;
        }

        public bool getPassable(Vector2 focusTile)
        {
            return passable[(int)focusTile.X + ((int)focusTile.Y * sizeX)];
        }

        public bool getOccupied(Vector2 focusTile)
        {
            return occupied[(int)focusTile.X + ((int)focusTile.Y * sizeX)];
        }

        public bool checkOccupied(Vector2 focusTile, String elementName)
        {
            if (this.TilePositionInBounds(focusTile))
            {
                if (this.getOccupied(focusTile))
                {
                    if (this.getOccupyingElement(focusTile).GetElementName() == elementName)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public int getNumberTilesX()
        {
            return sizeX;
        }

        public int getNumberTilesY()
        {
            return sizeY;
        }

        public bool TilePositionInBounds(Vector2 posToCheck)
        {
            int x = (int)posToCheck.X;
            int y = (int)posToCheck.Y;
            if (x >= 0 && x < this.getNumberTilesX()  && y >= 0 && y < this.getNumberTilesY())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Vector2 FindNearest(Vector2 startOfSearch, String typeOfElement)
        {
            //spiral search until proper element hit
            Vector2 target = new Vector2(-1, -1);
            bool searching = true;
            Vector2 searchPosition = new Vector2(startOfSearch.X, startOfSearch.Y);
            bool increaseLength = false;
            int lineLength = 1;
            int direction = 4; //0 = north, 1 = east, 2 = south, 3 = west
            bool searchShouldContinue = true;
            
            while (searching)
            {
                //roll direction
                if(direction == 4)
                {
                    //check if no further squares to check
                    if (searchShouldContinue)
                    {
                        searchShouldContinue = false;
                    }
                    else
                    {
                        searching = false;
                    }
                    direction = 0;
                }

                //translate direction
                int xMod = 0;
                int yMod = 0;
                if (direction == 0) yMod--;
                if (direction == 1) xMod++;
                if (direction == 2) yMod++;
                if (direction == 3) xMod--;

                for (int l = 0; l < lineLength; l++)
                {
                    //increment search position
                    searchPosition.X = searchPosition.X + xMod;
                    searchPosition.Y = searchPosition.Y + yMod;
                    //conduct search
                    if (this.TilePositionInBounds(searchPosition))
                    {
                        searchShouldContinue = true;
                        if (this.getOccupied(searchPosition))
                        {
                            if(this.getOccupyingElement(searchPosition).GetElementName() == typeOfElement)
                            {
                                //check to ensure a free space exists around the item
                                for(int s = 0; s < 4; s++)
                                {
                                    int xSMod = 0;
                                    int ySMod = 0;
                                    if (direction == 0) ySMod--;
                                    if (direction == 1) xSMod++;
                                    if (direction == 2) ySMod++;
                                    if (direction == 3) xSMod--;
                                    Vector2 secondarySearchPosition = new Vector2(searchPosition.X + xSMod, searchPosition.Y + ySMod);
                                    if (this.TilePositionInBounds(secondarySearchPosition)){
                                        if (!this.getOccupied(secondarySearchPosition)){
                                            searching = false;
                                            l = lineLength;
                                            target.X = secondarySearchPosition.X;
                                            target.Y = secondarySearchPosition.Y;
                                        }
                                    }
                                }                                
                            }
                        }
                        else
                        {
                            if(typeOfElement == "Empty")
                            {
                                searching = false;
                                l = lineLength;
                                target = searchPosition;
                            }
                        }
                    }
                }
                //incriment direction
                direction++;
                //check if the next search line will be longer
                if (increaseLength)
                {
                    lineLength++;
                    increaseLength = false;
                }
                else
                {
                    increaseLength = true;
                }
            }

            return target;
        }

        public void setPassable(Vector2 focusTile, bool state)
        {
            passable[(int)focusTile.X + ((int)focusTile.Y * sizeX)] = state;
        }

        public void setOccupied(Vector2 focusTile, bool state)
        {
            occupied[(int)focusTile.X + ((int)focusTile.Y * sizeX)] = state;
        }

        public void setOccupyingElement(Vector2 focusTile, gameElement element)
        {
            occupyingElement[(int)focusTile.X + ((int)focusTile.Y * sizeX)] = element;
        }

        public gameElement getOccupyingElement(Vector2 focusTile)
        {
            if (this.getOccupied(focusTile))
            {
                return occupyingElement[(int)focusTile.X + ((int)focusTile.Y * sizeX)];
            }
            else
            {
                return null;
            }
        }

        public void GetAllElementsInArea(List<gameElement> listIn, Vector2 startDrag, Vector2 endDrag)
        {
            int startingX;
            int startingY;
            int endingX;
            int endingY;

            if(startDrag.X > endDrag.X)
            {
                startingX = (int)endDrag.X;
                endingX = (int)startDrag.X;
            }
            else
            {
                startingX = (int)startDrag.X;
                endingX = (int)endDrag.X;
            }

            if (startDrag.Y > endDrag.Y)
            {
                startingY = (int)endDrag.Y;
                endingY = (int)startDrag.Y;
            }
            else
            {
                startingY = (int)startDrag.Y;
                endingY = (int)endDrag.Y;
            }

            for(int x = startingX; x < endingX + 1; x++)
            {
                for(int y =startingY; y < endingY + 1; y++)
                {
                    if(this.getOccupied(new Vector2(x, y)))
                    {
                        listIn.Add(this.getOccupyingElement(new Vector2(x, y)));
                    }
                }
            }
        }

        public void drawWater(Renderer renderer, Vector2 waterOffsetIn)
        {
            //Draw water
            //Draw the base
            for (int i = 0; i < numberTiles; i++)
            {
                if (occupied[i])
                {
                    if (occupyingElement[i].GetElementName() == "Water")
                    {
                        occupyingElement[i].SetAnimationOffset(waterOffsetIn);
                        occupyingElement[i].draw(renderer);
                    }
                }
            }
        }

        public void drawWaterLedge(Renderer renderer)
        {
            //Draw water
            //Draw the base
            WaterElement reference;
            for (int i = 0; i < numberTiles; i++)
            {
                if (occupied[i])
                {
                    if (occupyingElement[i].GetElementName() == "Water")
                    {
                        reference = (WaterElement)occupyingElement[i];
                        reference.DrawLedge(renderer);
                    }
                }
            }
        }

        public void draw(Renderer renderer)
        {
            //Draw map base
            for (int mapUnitWidth = 0; mapUnitWidth < sizeX; mapUnitWidth++)
            {
                for (int mapUnitHeight = 0; mapUnitHeight < sizeY; mapUnitHeight++)
                {
                    if (!this.checkOccupied(new Vector2(mapUnitWidth, mapUnitHeight),"Water"))
                    {
                        renderer.drawTexturedRectangle(0 + (mapUnitWidth * GlobalVariables.TILE_SIZE), 0 + (mapUnitHeight * GlobalVariables.TILE_SIZE), GlobalVariables.TILE_SIZE, GlobalVariables.TILE_SIZE, groundTexture);
                    }
                }
            }

            //TEST
            renderer.drawTexturedRectangle(300, 300, GlobalVariables.TILE_SIZE * 3, GlobalVariables.TILE_SIZE * 3, testTreeTexture);

            //Draw water pass 2
            //renderer.endDrawing();
            //waterEffect.CurrentTechnique = waterEffect.Techniques["SpriteDrawingVerticalBlur"];
            //renderer.startRenderTargetDrawing(waterEffect);
            //renderer.drawTexturedRectangle(0, 0, associatedGame.GraphicsDevice.PresentationParameters.BackBufferWidth, associatedGame.GraphicsDevice.PresentationParameters.BackBufferHeight, waterRenderTarget2);
            //renderer.endDrawing();
            //renderer.startDrawing();

            //Draw objects
            for (int i = 0; i < numberTiles; i++)
            {
                if (occupied[i])
                {
                    if (occupyingElement[i].GetElementName() != "Water")
                    {
                        occupyingElement[i].draw(renderer);
                    }
                }
            }
        }
    }
}
