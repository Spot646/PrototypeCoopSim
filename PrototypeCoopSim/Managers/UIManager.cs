using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Events;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Managers;
using PrototypeCoopSim.Settings;

namespace PrototypeCoopSim.Managers
{
    class UIManager
    {
        private Game associatedGame;
        private Renderer associatedRenderer;
        private GraphicsDevice associatedGraphicsDevice;

        //Output
        private String currentConsoleText;

        //Assets
        //Textures
        private Texture2D blankTexture;
        private Texture2D focus;
        private Texture2D focus2;
        private Texture2D focus3;
        private Texture2D harvestIcon;
        private Texture2D iconFocus;
        //Effects
        private Effect FlatBlack;
        private Effect FlatWhite;
        private Effect FlatGreen;

        //Fonts
        private SpriteFont consoleFont;

        //UI Window position
        private Vector2 mainUIWindowOrigin;
        private int mainUIWindowLengthX;
        private int mainUIWindowLengthY;
        private Vector2 subUIWindowOrigin;
        private int subUIWindowLengthX;
        private int subUIWindowLengthY;

        //icon positions
        private int harvestIconX;
        private int harvestIconY;
        private int harvestIconDiameter;

        public UIManager(Game gameIn, Renderer rendererIn, GraphicsDevice gDeviceIn)
        {
            associatedGame = gameIn;
            associatedRenderer = rendererIn;
            associatedGraphicsDevice = gDeviceIn;
            this.RecalculateUIPositions();
            currentConsoleText = " ";

            //Load textures
            blankTexture = associatedGame.Content.Load<Texture2D>("BlankTexture");
            focus = associatedGame.Content.Load<Texture2D>("Focus");
            focus2 = associatedGame.Content.Load<Texture2D>("Focus2");
            focus3 = associatedGame.Content.Load<Texture2D>("Focus3");
            harvestIcon = associatedGame.Content.Load<Texture2D>("HarvestTreesIcon");
            iconFocus = associatedGame.Content.Load<Texture2D>("CircleIconHeighlight");
            //Load Effects
            FlatBlack = associatedGame.Content.Load<Effect>("FX/BlackShader");
            FlatWhite = associatedGame.Content.Load<Effect>("FX/WhiteShader");
            FlatGreen = associatedGame.Content.Load<Effect>("FX/GreenShader");

            //Load fonts
            consoleFont = associatedGame.Content.Load<SpriteFont>("ConsoleText");
        }

        public void updateConsole(String newText)
        {
            currentConsoleText = newText;
        }

        public void RecalculateUIPositions()
        {
            //Grab current window sizes
            int currentWindowHeight = associatedGraphicsDevice.PresentationParameters.BackBufferHeight;
            int currentWindowWidth = associatedGraphicsDevice.PresentationParameters.BackBufferWidth;

            //UI Width is static at 256
            mainUIWindowLengthX = 256;
            subUIWindowLengthX = 256;

            //Find origin working back from right bounds
            mainUIWindowOrigin = new Vector2(currentWindowWidth - mainUIWindowLengthX, 0);
            //sub window is the lower 40% of height
            subUIWindowOrigin = new Vector2(currentWindowWidth - mainUIWindowLengthX, (int)((float)currentWindowHeight * 0.6f));

            //Find the height of both windows
            mainUIWindowLengthY = (int)subUIWindowOrigin.Y;
            subUIWindowLengthY = currentWindowHeight - (int)subUIWindowOrigin.Y;

            //Find button positions
            //NOTE button positions are for the middle point
            harvestIconX = (int)subUIWindowOrigin.X + 40;
            harvestIconY = (int)subUIWindowOrigin.Y + 40;
            harvestIconDiameter = 50;
        }

        public void DrawConsole()
        {
            //Draw borders
            associatedRenderer.endDrawing(); // end current drawing as one should be going on
            associatedRenderer.startShadedDrawing(FlatBlack);
            associatedRenderer.drawTexturedRectangle((int)mainUIWindowOrigin.X, (int)mainUIWindowOrigin.Y, mainUIWindowLengthX, mainUIWindowLengthY, blankTexture);
            associatedRenderer.drawTexturedRectangle((int)subUIWindowOrigin.X, (int)subUIWindowOrigin.Y, subUIWindowLengthX, subUIWindowLengthY, blankTexture);

            //Draw main output areas
            associatedRenderer.endDrawing();
            associatedRenderer.startShadedDrawing(FlatWhite);
            associatedRenderer.drawTexturedRectangle((int)mainUIWindowOrigin.X + 5, (int)mainUIWindowOrigin.Y + 5, mainUIWindowLengthX - 10, mainUIWindowLengthY - 10, blankTexture);
            associatedRenderer.drawTexturedRectangle((int)subUIWindowOrigin.X + 5, (int)subUIWindowOrigin.Y + 5, subUIWindowLengthX - 10, subUIWindowLengthY - 10, blankTexture);

            //console text
            associatedRenderer.endDrawing();
            associatedRenderer.startDrawing();
            associatedRenderer.drawText(currentConsoleText, (int)mainUIWindowOrigin.X + 10, (int)mainUIWindowOrigin.Y + 10, consoleFont);
        }

        public int GetGameWindowWidth()
        {
            return (int)mainUIWindowOrigin.X;
        }

        public int GetGameWindowHeight()
        {
            return associatedGraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        public void DrawMouseFocus(Vector2 mouseTileFocus)
        {
            associatedRenderer.drawTexturedRectangle((int)mouseTileFocus.X * GlobalVariables.TILE_SIZE, (int)mouseTileFocus.Y * GlobalVariables.TILE_SIZE, GlobalVariables.TILE_SIZE, GlobalVariables.TILE_SIZE, focus3);
        }

        public void DrawCurrentObjectFocus(gameElement focusElement) {
            associatedRenderer.drawTexturedRectangle((int)focusElement.GetAnimationOffset().X + focusElement.getWorldPositionX() * GlobalVariables.TILE_SIZE, (int)focusElement.GetAnimationOffset().Y + focusElement.getWorldPositionY() * GlobalVariables.TILE_SIZE, GlobalVariables.TILE_SIZE, GlobalVariables.TILE_SIZE, focus2);
        }

        public void DrawCurrentFocusActionIcons(gameElement focusElement, InputManager inputManager)
        {
            //No icons
            if (focusElement.GetIconTypes() == 0)
            {
                
            }
            //Harvest icon
            if (focusElement.GetIconTypes() == 1)
            {
                associatedRenderer.drawTexturedRectangle(harvestIconX - (harvestIconDiameter / 2), harvestIconY - (harvestIconDiameter / 2), harvestIconDiameter, harvestIconDiameter, harvestIcon);
                //check for focus
                if(this.OverHarvestIcon(inputManager))
                {
                    associatedRenderer.drawTexturedRectangle(harvestIconX - (harvestIconDiameter / 2), harvestIconY - (harvestIconDiameter / 2), harvestIconDiameter, harvestIconDiameter, iconFocus);
                }
            }
        }

       public bool OverHarvestIcon(InputManager inputManager)
        {
            if(inputManager.MouseOverCircle(new Vector2(harvestIconX, harvestIconY), harvestIconDiameter / 2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public void DrawCurrentDrag(Vector2 startOfDragTile, Vector2 endOfDragTile)
        {
            //Storage for new square
            int topLeftX;
            int topLeftY;
            int lengthX;
            int lengthY;

            //Find top left origin X
            if(startOfDragTile.X > endOfDragTile.X)
            {
                topLeftX = (int)endOfDragTile.X * GlobalVariables.TILE_SIZE;
            }
            else
            {
                topLeftX = (int)startOfDragTile.X * GlobalVariables.TILE_SIZE;
            }
            //Find top left origin Y
            if (startOfDragTile.Y > endOfDragTile.Y)
            {
                topLeftY = (int)endOfDragTile.Y * GlobalVariables.TILE_SIZE;
            }
            else
            {
                topLeftY = (int)startOfDragTile.Y * GlobalVariables.TILE_SIZE;
            }
            //Find length X
            lengthX = Math.Abs((int)(startOfDragTile.X - endOfDragTile.X)) * GlobalVariables.TILE_SIZE + GlobalVariables.TILE_SIZE;
            //Find length Y
            lengthY = Math.Abs((int)(startOfDragTile.Y - endOfDragTile.Y)) * GlobalVariables.TILE_SIZE + GlobalVariables.TILE_SIZE;

            associatedRenderer.endDrawing(); // end current drawing as one should be going on
            associatedRenderer.startShadedDrawing(FlatGreen);
            //Top
            associatedRenderer.drawTexturedRectangle(topLeftX,topLeftY,lengthX,3, blankTexture);
            //Left
            associatedRenderer.drawTexturedRectangle(topLeftX, topLeftY , 3, lengthY, blankTexture);
            //Bottom
            associatedRenderer.drawTexturedRectangle(topLeftX, topLeftY + lengthY - 3, lengthX, 3, blankTexture);
            //Right
            associatedRenderer.drawTexturedRectangle(topLeftX + lengthX - 3, topLeftY, 3, lengthY, blankTexture);
            associatedRenderer.endDrawing();
            associatedRenderer.startDrawing();
        }
    }
}
