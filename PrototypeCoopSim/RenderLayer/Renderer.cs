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

        public void switchRenderTarget(Game gameIn, RenderTarget2D renderTarget)
        {
            gameIn.GraphicsDevice.SetRenderTarget(renderTarget);
        }

        public void resetRenderTarget(Game gameIn)
        {
            gameIn.GraphicsDevice.SetRenderTarget(null);
        }

        public void startDrawing()
        {
            spriteBatch.Begin();
        }

        public void startShadedDrawing(Effect effectIn)
        {
            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, effectIn);
        }

        public void startRenderTargetDrawing(Effect effectIn)
        {
            spriteBatch.Begin(0, BlendState.NonPremultiplied, null, null, null, effectIn);
        }

        public void endDrawing()
        {
            spriteBatch.End();
        }

        public void drawTexturedRectangle(int posX, int posY, int length, int width, Texture2D texture)
        {
            spriteBatch.Draw(texture, new Rectangle(posX, posY, length, width), Color.White);
        }

        public void drawTexturedRenderTarget(int posX, int posY, int length, int width, Texture2D texture)
        {
            spriteBatch.Draw(texture, new Rectangle(posX, posY, length, width), Color.White);
        }

        public void drawPartialTexturedRectangle(int posX, int posY, int length, int width, int atlasPosX, int atlasPosY, int atlasWidth, int atlasHeight, Texture2D texture)
        {
            spriteBatch.Draw(texture, new Rectangle(posX, posY, length, width), new Rectangle(atlasPosX, atlasPosY, atlasWidth, atlasHeight), Color.White);
        }

        public void drawText(String text, int posX, int posY, SpriteFont font)
        {
            spriteBatch.DrawString(font, text, new Vector2(posX, posY), Color.Black);
        }
    }
}
