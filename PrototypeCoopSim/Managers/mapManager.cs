using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;

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

        //Map type 1
        public Texture2D groundTexture; //Ground Texture
        
        public mapManager(Game game, int tilesXIn, int tilesYIn)
        {
            sizeX = tilesXIn;
            sizeY = tilesYIn;
            scanX = 0;
            scanY = 0;
            time = 0;
            date = 0;
            numberTiles = tilesXIn * tilesYIn;

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
            groundTexture = game.Content.Load<Texture2D>("Browntile");
        }

        public Vector2 getTileFromMousePosition(int mouseX, int mouseY, int tileWidth, int mapOriginX, int mapOriginY, int mapWindowSizeX, int mapWindowSizeY)
        {
            Vector2 mouseTilePosition = new Vector2();

            if(mouseX > mapOriginX && mouseX < (mapOriginX + mapWindowSizeX) && mouseY > mapOriginY && mouseY < (mapOriginY + mapWindowSizeY))
            {
                mouseTilePosition.X = ((int)(((int)(mouseX + scanX - mapOriginX))/tileWidth)) * tileWidth;
                mouseTilePosition.Y = ((int)(((int)(mouseY + scanY - mapOriginY))/tileWidth)) * tileWidth;
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

        public void draw(Renderer renderer)
        {
            //Draw map base
            for (int mapUnitWidth = 0; mapUnitWidth < sizeX; mapUnitWidth++)
            {
                for (int mapUnitHeight = 0; mapUnitHeight < sizeY; mapUnitHeight++)
                {
                    renderer.drawTexturedRectangle(0 + (mapUnitWidth * 25), 0 + (mapUnitHeight * 25), 25, 25, groundTexture);
                }
            }

            //Draw objects
            for (int i = 0; i < numberTiles; i++)
            {
                if (occupied[i])
                {
                    occupyingElement[i].draw(renderer);
                }
            }
        }
    }
}
