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
        Texture2D focus;
        
        //Fonts
        SpriteFont consoleFont;

        public UIManager(Game gameIn, Renderer rendererIn)
        {
            associatedGame = gameIn;
            associatedRenderer = rendererIn;
            currentConsoleText = " ";

            //Load textures
            plainBlack = associatedGame.Content.Load<Texture2D>("PlainBlack");
            plainWhite = associatedGame.Content.Load<Texture2D>("PlainWhite");
            focus = associatedGame.Content.Load<Texture2D>("Focus");

            //Load fonts
            consoleFont = associatedGame.Content.Load<SpriteFont>("ConsoleText");
        }

        public void updateConsole(String newText)
        {
            currentConsoleText = newText;
        }

        public void DrawConsole()
        {
            associatedRenderer.drawTexturedRectangle(500, 0, 300, 600, plainBlack);
            associatedRenderer.drawTexturedRectangle(505, 5, 290, 590, plainWhite);
            associatedRenderer.drawText(currentConsoleText, 510, 10, consoleFont);
        }

        public void DrawMouseFocus(Vector2 mouseTileFocus)
        {
            associatedRenderer.drawTexturedRectangle((int)mouseTileFocus.X * 25, (int)mouseTileFocus.Y * 25, 25, 25, focus);
        }

    }
}
