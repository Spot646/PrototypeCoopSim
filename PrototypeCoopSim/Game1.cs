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

        //Map
        const int mapTilesX = 20;
        const int mapTilesY = 24;
        mapManager currentMap;

        //Events
        EventManager eventManager;

        //UI
        UIManager uiManager;

        //Input
        InputManager inputManager;

        //Game elements
        const int elementListLength = 30;

        //Track focus
        gameElement currentElementFocus;

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
            //Define Systems
            ///////////////////////////////////////////////////////////////
            //Define the render layer
            renderer = new Renderer();

            //Screen settings
            renderer.setScreenSize(graphics, 800, 600);
            this.IsMouseVisible = true;

            //Setup Map
            currentMap = new mapManager(this, mapTilesX, mapTilesY);

            //Setup Events
            eventManager = new EventManager(this, currentMap);

            //Setup UI
            uiManager = new UIManager(this, renderer);

            //Setup Input
            inputManager = new InputManager(currentMap);

            //Manage focus
            currentElementFocus = null;

            //Variable initialization
            //////////////////////////////////////////////////////////////
            //Generate map
            eventManager.AddEvent(new EventGenerateWorld(this, currentMap, 15,5));

            base.Initialize();
        }

        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            renderer.initializeGraphics(this, graphics);
        }

        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        protected override void UnloadContent() { }

        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Update key presses
            inputManager.Update();

            //Check for key presses
            if (inputManager.EscapeButtonPressed()) Exit();
            if (inputManager.SpawnTreeButtonReleased()) eventManager.AddEvent(new EventAddTrees(this, currentMap, 1));
            if (inputManager.SpawnRockButtonReleased()) eventManager.AddEvent(new EventAddRocks(this, currentMap, 1));

            //Check mouse functions
            if (inputManager.LeftMouseButtonReleased())
            {
                if (currentMap.getOccupied(inputManager.GetCurrentMouseTile(currentMap)))
                {
                    currentMap.getOccupyingElement(inputManager.GetCurrentMouseTile(currentMap)).UpdateCurrentHealth(5);
                    currentElementFocus = currentMap.getOccupyingElement(inputManager.GetCurrentMouseTile(currentMap));
                }
                else
                {
                    currentElementFocus = null;
                }
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
            if (currentMap.getOccupied(inputManager.GetCurrentMouseTile(currentMap))){
                uiManager.updateConsole(currentMap.getOccupyingElement(inputManager.GetCurrentMouseTile(currentMap)).getDetails());}
            else { uiManager.updateConsole(""); }

            //Draw Console
            uiManager.DrawConsole();

            //Draw map
            currentMap.draw(renderer);

            //Drag drawing
            if (inputManager.DragStarted())
            {
                uiManager.DrawCurrentDrag(inputManager.GetMouseDragStartTile(), inputManager.GetCurrentMouseTile(currentMap));
            }
            else
            {
                //Heighlight mouse position
                if (inputManager.MouseOverMap()) uiManager.DrawMouseFocus(inputManager.GetCurrentMouseTile(currentMap));
            }

            //Heighlight focus
            if(currentElementFocus != null)
            {
                uiManager.DrawCurrentObjectFocus(currentElementFocus);
            }

            renderer.endDrawing();

            base.Draw(gameTime);
        }
    }
}
