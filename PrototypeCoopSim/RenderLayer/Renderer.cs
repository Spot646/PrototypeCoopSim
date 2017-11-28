using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PrototypeCoopSim.RenderLayer
{
    class Renderer
    {
        public int screenSizeX;
        public int screenSizeY;
        private SpriteBatch spriteBatch;

        public Renderer()
        {
            screenSizeX = 0;
            screenSizeY = 0;
        }

        public void initializeGraphics(Game game, GraphicsDeviceManager graphics)
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        public void setScreenSize(GraphicsDeviceManager graphics, int resolutionX, int resolutionY)
        {
            graphics.PreferredBackBufferWidth = resolutionX;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = resolutionY;   // set this value to the desired height of your window
            graphics.ApplyChanges();
        }

        public void clearScreen(Game game)
        {
            game.GraphicsDevice.Clear(Color.WhiteSmoke);
        }

        public void startDrawing()
        {
            spriteBatch.Begin();
        }

        public void endDrawing()
        {
            spriteBatch.End();
        }

        public void drawTexturedRectangle(int posX, int posY, int length, int width, Texture2D texture)
        {
            spriteBatch.Draw(texture, new Rectangle(posX, posY, length, width), Color.White);
        }

        public void drawText(String text, int posX, int posY, SpriteFont font)
        {
            spriteBatch.DrawString(font, text, new Vector2(510, 10), Color.Black);
        }
    }
}
