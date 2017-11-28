using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Managers;

namespace PrototypeCoopSim
{

    public class Game1 : Game
    {
        //Render Layer
        GraphicsDeviceManager graphics;
        Renderer renderer;

        //Textures
        Texture2D brownTile;
        Texture2D plainWhite;
        Texture2D plainBlack;
        Texture2D focus;

        //Fonts
        SpriteFont consoleFont;

        //Focus info
        int currentPosX;
        int currentPosY;

        //Map
        const int mapTilesX = 20;
        const int mapTilesY = 24;
        mapManager currentMap;

        //Game elements
        const int elementListLength = 30;
        //gameElement[] elementList = new gameElement[elementListLength];

        //Console variables
        String currentConsoleText;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        protected override void Initialize()
        {
            //Define the render layer
            renderer = new Renderer();
            this.IsMouseVisible = true;

            //Screen settings
            renderer.setScreenSize(graphics, 800, 600);

            //Setup Map
            currentMap = new mapManager(this, mapTilesX, mapTilesY);

            //Variable initialization
            //Generate trees
            Random random = new Random();
            for (int i = 0; i < elementListLength; i++)
            {
                Vector2 randomTreePosition = new Vector2(random.Next() % 20, random.Next() % 24);
                if (!currentMap.getOccupied(randomTreePosition))
                {
                    currentMap.setOccupied(randomTreePosition, true);
                    currentMap.setOccupyingElement(randomTreePosition, new treeElement(this, (int)randomTreePosition.X, (int)randomTreePosition.Y));
                }
            }

            base.Initialize();
        }

        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            renderer.initializeGraphics(this, graphics);
            
            //Load textures
            brownTile = Content.Load<Texture2D>("Browntile");
            plainBlack = Content.Load<Texture2D>("PlainBlack");
            plainWhite = Content.Load<Texture2D>("PlainWhite");
            focus = Content.Load<Texture2D>("Focus");

            //Load fonts
            consoleFont = Content.Load<SpriteFont>("ConsoleText");
        }

        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Check mouse functions
            MouseState mouseState = Mouse.GetState();
            currentPosX = mouseState.X;
            currentPosY = mouseState.Y;
            
            base.Update(gameTime);
        }

        /// This is called when the game should draw itself.
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            renderer.clearScreen(this);
            renderer.startDrawing();

            //Draw console
            //currentConsoleText = elementList[0].getDetails()
            //    + Environment.NewLine + "MouseX:" + currentPosX
            //    + Environment.NewLine + "MouseY:" + currentPosY;
            currentConsoleText = "MouseX:" + currentPosX + Environment.NewLine + "MouseY:" + currentPosY;
            renderer.drawTexturedRectangle(500, 0, 300, 600, plainBlack);
            renderer.drawTexturedRectangle(505, 5, 290, 590, plainWhite);
            renderer.drawText(currentConsoleText, 510, 10, consoleFont);

            //Draw map
            currentMap.draw(renderer);

            //Heightlight mouse position
            if (currentPosX > 0 && currentPosX < 500 && currentPosY > 0 && currentPosY < 600)
            {
                int focusPosX = (int)(currentPosX / 25);
                int focusPosY = (int)(currentPosY / 25);
                renderer.drawTexturedRectangle(focusPosX * 25, focusPosY * 25, 25, 25, focus);
            }

            renderer.endDrawing();

            base.Draw(gameTime);
        }
    }
}
