using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Events;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Managers;

namespace PrototypeCoopSim.Managers
{
    class UIManager
    {
        private Game associatedGame;
        private Renderer associatedRenderer;

        //Output
        String currentConsoleText;

        //Assets
        //Textures
        Texture2D plainWhite;
        Texture2D plainBlack;
        Texture2D plainGreen;
        Texture2D focus;
        Texture2D focus2;
        Texture2D focus3;
        Texture2D harvestIcon;
        Texture2D iconFocus;

        //Fonts
        SpriteFont consoleFont;

        //icon positions
        const int harvestIconX = 540;
        const int harvestIconY = 440;
        const int harvestIconDiameter = 50;

        public UIManager(Game gameIn, Renderer rendererIn)
        {
            associatedGame = gameIn;
            associatedRenderer = rendererIn;
            currentConsoleText = " ";

            //Load textures
            plainBlack = associatedGame.Content.Load<Texture2D>("PlainBlack");
            plainWhite = associatedGame.Content.Load<Texture2D>("PlainWhite");
            plainGreen = associatedGame.Content.Load<Texture2D>("PlainGreen");
            focus = associatedGame.Content.Load<Texture2D>("Focus");
            focus2 = associatedGame.Content.Load<Texture2D>("Focus2");
            focus3 = associatedGame.Content.Load<Texture2D>("Focus3");
            harvestIcon = associatedGame.Content.Load<Texture2D>("HarvestTreesIcon");
            iconFocus = associatedGame.Content.Load<Texture2D>("CircleIconHeighlight");

            //Load fonts
            consoleFont = associatedGame.Content.Load<SpriteFont>("ConsoleText");
        }

        public void updateConsole(String newText)
        {
            currentConsoleText = newText;
        }

        public void DrawConsole()
        {
            associatedRenderer.drawTexturedRectangle(500, 0, 300, 400, plainBlack);
            associatedRenderer.drawTexturedRectangle(505, 5, 290, 390, plainWhite);
            associatedRenderer.drawTexturedRectangle(500, 400, 300, 200, plainBlack);
            associatedRenderer.drawTexturedRectangle(505, 405, 290, 190, plainWhite);
            associatedRenderer.drawText(currentConsoleText, 510, 10, consoleFont);
        }

        public void DrawMouseFocus(Vector2 mouseTileFocus)
        {
            associatedRenderer.drawTexturedRectangle((int)mouseTileFocus.X * 25, (int)mouseTileFocus.Y * 25, 25, 25, focus3);
        }

        public void DrawCurrentObjectFocus(gameElement focusElement) {
            associatedRenderer.drawTexturedRectangle((int)focusElement.GetAnimationOffset().X + focusElement.getWorldPositionX() * 25, (int)focusElement.GetAnimationOffset().Y + focusElement.getWorldPositionY() * 25, 25, 25, focus2);
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
                topLeftX = (int)endOfDragTile.X * 25;
            }
            else
            {
                topLeftX = (int)startOfDragTile.X * 25;
            }
            //Find top left origin Y
            if (startOfDragTile.Y > endOfDragTile.Y)
            {
                topLeftY = (int)endOfDragTile.Y * 25;
            }
            else
            {
                topLeftY = (int)startOfDragTile.Y * 25;
            }
            //Find length X
            lengthX = Math.Abs((int)(startOfDragTile.X - endOfDragTile.X)) * 25 + 25;
            //Find length Y
            lengthY = Math.Abs((int)(startOfDragTile.Y - endOfDragTile.Y)) * 25 + 25;


            //Top
            associatedRenderer.drawTexturedRectangle(topLeftX,topLeftY,lengthX,3,plainGreen);
            //Left
            associatedRenderer.drawTexturedRectangle(topLeftX, topLeftY , 3, lengthY, plainGreen);
            //Bottom
            associatedRenderer.drawTexturedRectangle(topLeftX, topLeftY + lengthY - 3, lengthX, 3, plainGreen);
            //Right
            associatedRenderer.drawTexturedRectangle(topLeftX + lengthX - 3, topLeftY, 3, lengthY, plainGreen);
        }
    }
}
