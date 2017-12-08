using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Events;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Managers;
using PrototypeCoopSim.AIProfiles;
using System.Collections.Generic;
using PrototypeCoopSim.Settings;
using PrototypeCoopSim.AIProfiles.Jobs;

namespace PrototypeCoopSim
{

    public class Game1 : Game
    {
        //Render Layer
        GraphicsDeviceManager graphics;
        Renderer renderer;

        //Map
        const int mapTilesX = 24;
        const int mapTilesY = 24;
        Vector2 waterOffset;
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
        List<gameElement> elementFocus = new List<gameElement>();

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

            //XNA Settings
            //Choose one time step setting
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 300.0f); //Fixed time step
            IsFixedTimeStep = false; //Variable

            //Screen settings
            renderer.setScreenSize(graphics, 1024, 768);

            this.IsMouseVisible = true;

            //Setup Map
            currentMap = new mapManager(this, mapTilesX, mapTilesY);

            //Setup Events
            eventManager = new EventManager(this, currentMap);
            
            //Setup UI
            uiManager = new UIManager(this, renderer, graphics.GraphicsDevice);

            //Setup Input
            inputManager = new InputManager(currentMap, uiManager);

            //Manage focus
            elementFocus.Clear();

            //Add testing character
            currentMap.setOccupied(new Vector2((int)currentMap.getNumberTilesX() / 2, (int)currentMap.getNumberTilesY() / 2), true);
            currentMap.setOccupyingElement(new Vector2((int)currentMap.getNumberTilesX() / 2, (int)currentMap.getNumberTilesY() / 2), new WorkerElement(this, (int)currentMap.getNumberTilesX() / 2, (int)currentMap.getNumberTilesY() / 2));
            currentMap.setOccupied(new Vector2((int)currentMap.getNumberTilesX() / 2 + 1, (int)currentMap.getNumberTilesY() / 2), true);
            currentMap.setOccupyingElement(new Vector2((int)currentMap.getNumberTilesX() / 2 + 1, (int)currentMap.getNumberTilesY() / 2), new WorkerElement(this, (int)currentMap.getNumberTilesX() / 2 + 1, (int)currentMap.getNumberTilesY() / 2));
            currentMap.setOccupied(new Vector2((int)currentMap.getNumberTilesX() / 2 - 1, (int)currentMap.getNumberTilesY() / 2), true);
            currentMap.setOccupyingElement(new Vector2((int)currentMap.getNumberTilesX() / 2 - 1, (int)currentMap.getNumberTilesY() / 2), new WorkerElement(this, (int)currentMap.getNumberTilesX() / 2 - 1, (int)currentMap.getNumberTilesY() / 2));
            
            //Variable initialization
            //////////////////////////////////////////////////////////////
            //Generate map
            eventManager.AddEvent(new EventGenerateWorld(this, currentMap, 4, 90, 3, 5));
            waterOffset = new Vector2(0, 0);

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
            if (inputManager.SpawnTreeButtonReleased()) eventManager.AddEvent(new EventAddTrees(this, currentMap, 1, 1, 0));
            if (inputManager.SpawnRockButtonReleased()) eventManager.AddEvent(new EventAddRocks(this, currentMap, 1));

            //Check left mouse functions
            if (inputManager.LeftMouseButtonReleased())
            {
                if (inputManager.DragFinished())
                {
                    elementFocus.Clear();
                    currentMap.GetAllElementsInArea(elementFocus, inputManager.GetMouseDragStartTile(), inputManager.GetMouseDragEndTile());
                }
                else if (inputManager.MouseOverMap() && currentMap.getOccupied(inputManager.GetCurrentMouseTile(currentMap)))
                {
                    currentMap.getOccupyingElement(inputManager.GetCurrentMouseTile(currentMap)).UpdateCurrentHealth(5);
                    elementFocus.Clear();
                    elementFocus.Add(currentMap.getOccupyingElement(inputManager.GetCurrentMouseTile(currentMap)));
                }
                else if (uiManager.OverHarvestIcon(inputManager) && elementFocus.Count > 0)
                {
                    for (int i = 0; i < elementFocus.Count; i++)
                    {
                        if (elementFocus[i].GetMovable())
                        {
                            ((ActorElement)elementFocus[i]).SetJob(new GathererJob());
                            eventManager.AddEvent(new EventHarvestTrees(this, currentMap, eventManager, elementFocus[i], gameTime, true));
                        }
                    }
                }
                else if (uiManager.OverMineIcon(inputManager) && elementFocus.Count > 0)
                {
                    for (int i = 0; i < elementFocus.Count; i++)
                    {
                        if (elementFocus[i].GetMovable())
                        {
                            eventManager.AddEvent(new EventHarvestRocks(this, currentMap, eventManager, elementFocus[i], gameTime, true));
                        }
                    }
                }
                else
                {
                    elementFocus.Clear();
                }
            }

            //Check right mouse functions
            if (inputManager.RightMouseButtonReleased())
            {
                //cycle through all focuses
                for (int i = 0; i < elementFocus.Count; i++)
                {
                    if (elementFocus[i].GetMovable())
                    {
                        //This is an actor
                        ActorElement currentActor = (ActorElement)elementFocus[i];
                        //clear any previous stuck condition
                        currentActor.SetStuck(false);
                        EventMoveTo movingEvent = new EventMoveTo(this, currentMap, elementFocus[i], inputManager.GetCurrentMouseTile(currentMap), gameTime);
                        if (currentActor.Moving())
                        {
                            currentActor.ReplaceLinkedMovement(movingEvent);
                            eventManager.AddEvent(movingEvent);
                        }
                        else
                        {
                            currentActor.LinkToMoveEvent(movingEvent);
                            eventManager.AddEvent(movingEvent);
                        }
                    }
                }

                if (elementFocus.Count == 0)
                {
                    //if no focus, spawn elements to test with
                    if (!currentMap.getOccupied(inputManager.GetCurrentMouseTile(currentMap)))
                    {
                        currentMap.setOccupied(inputManager.GetCurrentMouseTile(currentMap), true);
                        //currentMap.setOccupyingElement(inputManager.GetCurrentMouseTile(currentMap), new WorkerElement(this, (int)inputManager.GetCurrentMouseTile(currentMap).X, (int)inputManager.GetCurrentMouseTile(currentMap).Y));
                        currentMap.setOccupyingElement(inputManager.GetCurrentMouseTile(currentMap), new WaterElement(this, (int)inputManager.GetCurrentMouseTile(currentMap).X, (int)inputManager.GetCurrentMouseTile(currentMap).Y, currentMap));
                    }
                }
            }

            //Process jobs
            //TODO make run for everyone
            if (elementFocus.Count > 0)
            {
                if (elementFocus[0].HasJob())
                {
                    ((ActorElement)elementFocus[0]).getJob().ProcessJobPriorities(this, currentMap, eventManager, elementFocus[0], gameTime);
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
            if (currentMap.getOccupied(inputManager.GetCurrentMouseTile(currentMap))) {
                uiManager.updateConsole(currentMap.getOccupyingElement(inputManager.GetCurrentMouseTile(currentMap)).getDetails()); }
            else { uiManager.updateConsole(""); }

            //Draw Console
            uiManager.DrawConsole();

            if (elementFocus.Count > 0)
            { 
                uiManager.DrawCurrentFocusActionIcons(elementFocus[0], inputManager);
            }

            //Draw map
            //find water offset    
            waterOffset.X += gameTime.ElapsedGameTime.Milliseconds * 0.01f;
            waterOffset.Y += gameTime.ElapsedGameTime.Milliseconds * 0.02f;
            //if (waterOffset.X > 256.0f) waterOffset.X -= 256.0f - (float)GlobalVariables.TILE_SIZE;
            //if (waterOffset.Y > 256.0f) waterOffset.Y -= 256.0f - (float)GlobalVariables.TILE_SIZE;
            currentMap.drawWater(renderer, waterOffset);
            currentMap.draw(renderer);
            currentMap.drawWaterLedge(renderer);

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
            if(elementFocus.Count > 0)
            {
                for(int i = 0; i < elementFocus.Count; i++)
                uiManager.DrawCurrentObjectFocus(elementFocus[i]);
            }

            renderer.endDrawing();

            base.Draw(gameTime);
        }
    }
}
