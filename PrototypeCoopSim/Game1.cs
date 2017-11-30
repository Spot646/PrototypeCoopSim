using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Events;
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

        //Events
        EventManager eventManager;

        //Game elements
        const int elementListLength = 30;

        //Console variables
        String currentConsoleText;

        //Input keys
        bool tKeyPressed = false;

        //Mouse buttons
        bool leftButtonPressed = false;

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

            //Setup Events
            eventManager = new EventManager(this, currentMap);

            //Variable initialization
            //Generate trees
            eventManager.AddEvent(new EventAddTrees(this, currentMap, 15));

            base.Initialize();
        }

        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            renderer.initializeGraphics(this, graphics);
            
            //Load textures
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
            //Check for key presses
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.T))
            {
                tKeyPressed = true;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.T) && tKeyPressed) { 
                eventManager.AddEvent(new EventAddTrees(this, currentMap, 1));
                tKeyPressed = false;
            }

            //Check mouse functions
            MouseState mouseState = Mouse.GetState();
            currentPosX = mouseState.X;
            currentPosY = mouseState.Y;

            Vector2 mouseCurrentTile = currentMap.getTileFromMousePosition(currentPosX, currentPosY, 25, 0, 0, 500, 600);
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                leftButtonPressed = true;
            }
            if (mouseState.LeftButton == ButtonState.Released && leftButtonPressed)
            {
                currentMap.getOccupyingElement(mouseCurrentTile).UpdateCurrentHealth(5);
                leftButtonPressed = false;
            }
            //Run events
            eventManager.RunEvents();
            
            base.Update(gameTime);
        }

        /// This is called when the game should draw itself.
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            renderer.clearScreen(this);
            renderer.startDrawing();

            //Draw console
            //Find current mouse focus
            Vector2 mouseCurrentTile = currentMap.getTileFromMousePosition(currentPosX, currentPosY, 25, 0, 0, 500, 600);

            if (currentMap.getOccupied(mouseCurrentTile))
            {
                currentConsoleText = currentMap.getOccupyingElement(mouseCurrentTile).getDetails()
                    + Environment.NewLine + "MouseX:" + currentPosX
                    + Environment.NewLine + "MouseY:" + currentPosY;
            }
            else
            {
                currentConsoleText = "MouseX:" + currentPosX
                    + Environment.NewLine + "MouseY:" + currentPosY;
            }
            renderer.drawTexturedRectangle(500, 0, 300, 600, plainBlack);
            renderer.drawTexturedRectangle(505, 5, 290, 590, plainWhite);
            renderer.drawText(currentConsoleText, 510, 10, consoleFont);

            //Draw map
            currentMap.draw(renderer);

            //Heightlight mouse position
            if (currentPosX > 0 && currentPosX < 500 && currentPosY > 0 && currentPosY < 600)
            {
                renderer.drawTexturedRectangle((int)mouseCurrentTile.X * 25, (int)mouseCurrentTile.Y * 25, 25, 25, focus);
            }

            renderer.endDrawing();

            base.Draw(gameTime);
        }
    }
}
