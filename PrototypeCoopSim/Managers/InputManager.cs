﻿using Microsoft.Xna.Framework;
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
    class InputManager
    {
        private mapManager associatedMap;
        private UIManager associatedUIManager;

        //Left Mouse
        private bool mouseLostFocus = true;
        private bool leftMousePressed = false;
        private bool leftMouseReleased = false;
        private bool leftMouseDragStart = false;
        private bool leftMouseDragEnd = false;
        private bool rightMousePressed = false;
        private bool rightMouseReleased = false;
        private Vector2 mouseTileStartDrag;
        private Vector2 mouseTileEndDrag;

        //Spawn Trees
        private bool spawnTreeButtonPressed = false;
        private bool spawnTreeButtonReleased = false;

        //Spawn Rocks
        private bool spawnRockButtonPressed = false;
        private bool spawnRockButtonReleased = false;

        //Escape
        private bool escapeButtonPressed = false;
        private bool escapeButtonReleased = false;

        public InputManager(mapManager mapIn, UIManager uiManagerIn)
        {
            associatedMap = mapIn;
            associatedUIManager = uiManagerIn;
        }

        public Vector2 GetCurrentMouseTile(mapManager mapIn)
        {
            MouseState mouseState = Mouse.GetState();
            return mapIn.getTileFromMousePosition(mouseState.X, mouseState.Y, GlobalVariables.TILE_SIZE, 0, 0, associatedUIManager.GetGameWindowWidth(), associatedUIManager.GetGameWindowHeight());
        }

        public bool MouseOverCircle(Vector2 position, float radius)
        {
            MouseState mouseState = Mouse.GetState();
            double x = (double)Math.Abs(position.X - mouseState.X);
            double y = (double)Math.Abs(position.Y - mouseState.Y);
            double distance = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

            if (distance <= radius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool MouseOverMap()
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.X > 0 && mouseState.X < associatedUIManager.GetGameWindowWidth() && mouseState.Y > 0 && mouseState.Y < associatedUIManager.GetGameWindowHeight())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Update()
        {
            //Clear button release events
            leftMouseReleased = false;
            rightMouseReleased = false;
            spawnTreeButtonReleased = false;
            spawnRockButtonReleased = false;
            escapeButtonReleased = false;
            if(leftMouseDragEnd)
            {
                leftMouseDragEnd = false;
                mouseTileStartDrag.X = 0;
                mouseTileStartDrag.Y = 0;
                mouseTileEndDrag.X = 0;
                mouseTileEndDrag.Y = 0;
            }

            if (!mouseLostFocus)
            {
                //Check mouse down conditions
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    //Start Drag
                    if (this.MouseOverMap())
                    {
                        Vector2 currentMouseTile = associatedMap.getTileFromMousePosition(Mouse.GetState().X, Mouse.GetState().Y, GlobalVariables.TILE_SIZE, 0, 0, associatedUIManager.GetGameWindowWidth(), associatedUIManager.GetGameWindowHeight());
                        if (!leftMousePressed)
                        {
                            mouseTileStartDrag = currentMouseTile;
                        }
                        if (mouseTileStartDrag.X != currentMouseTile.X || mouseTileStartDrag.Y != currentMouseTile.Y)
                        {
                            leftMouseDragStart = true;
                        }
                        if (mouseTileStartDrag.X == currentMouseTile.X && mouseTileStartDrag.Y == currentMouseTile.Y)
                        {
                            leftMouseDragStart = false;
                        }
                    }
                    else
                    {
                        mouseLostFocus = false;
                        leftMouseDragStart = false;
                        leftMouseDragEnd = false;
                        mouseTileStartDrag.X = 0;
                        mouseTileStartDrag.Y = 0;
                        mouseTileEndDrag.X = 0;
                        mouseTileEndDrag.Y = 0;
                        leftMousePressed = false;
                        leftMouseReleased = false;
                        rightMousePressed = false;
                        rightMouseReleased = false;
                    }
                    //Basic click
                    leftMousePressed = true;
                }

                if (Mouse.GetState().RightButton == ButtonState.Pressed) rightMousePressed = true;
                
                //Check for button down events
                if (Keyboard.GetState().IsKeyDown(Keys.T)) spawnTreeButtonPressed = true;
                if (Keyboard.GetState().IsKeyDown(Keys.R)) spawnRockButtonPressed = true;
                if (Keyboard.GetState().IsKeyDown(Keys.Escape)) escapeButtonPressed = true;

                //Check for button release events
                if (Mouse.GetState().LeftButton == ButtonState.Released && leftMousePressed)
                {
                    leftMousePressed = false;
                    leftMouseReleased = true;
                    if (leftMouseDragStart && this.MouseOverMap())
                    {
                        mouseTileEndDrag = associatedMap.getTileFromMousePosition(Mouse.GetState().X, Mouse.GetState().Y, GlobalVariables.TILE_SIZE, 0, 0, associatedUIManager.GetGameWindowWidth(), associatedUIManager.GetGameWindowHeight());
                        if (mouseTileStartDrag.X != mouseTileEndDrag.X || mouseTileStartDrag.Y != mouseTileEndDrag.Y)
                        {
                            leftMouseDragEnd = true;
                        }
                        leftMouseDragStart = false;
                    }
                    else
                    {
                        //clear out drag data
                        leftMouseDragStart = false;
                        leftMouseDragEnd = false;
                        mouseTileStartDrag.X = 0;
                        mouseTileStartDrag.Y = 0;
                        mouseTileEndDrag.X = 0;
                        mouseTileEndDrag.Y = 0;
                    }
                }

                if (Mouse.GetState().RightButton == ButtonState.Released && rightMousePressed)
                {
                    rightMousePressed = false;
                    rightMouseReleased = true;
                }

                if (Keyboard.GetState().IsKeyUp(Keys.T) && spawnTreeButtonPressed) { spawnTreeButtonPressed = false; spawnTreeButtonReleased = true; }
                if (Keyboard.GetState().IsKeyUp(Keys.R) && spawnRockButtonPressed) { spawnRockButtonPressed = false; spawnRockButtonReleased = true; }
                if (Keyboard.GetState().IsKeyUp(Keys.Escape) && escapeButtonPressed) { escapeButtonPressed = false; escapeButtonReleased = true; }
            }
            else
            {
                if(Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    mouseLostFocus = false;
                }
            }
        }

        public bool SpawnTreeButtonReleased()
        {
            return spawnTreeButtonReleased;
        }

        public bool SpawnRockButtonReleased()
        {
            return spawnRockButtonReleased;
        }

        public bool LeftMouseButtonPressed()
        {
            return leftMousePressed;
        }

        public bool LeftMouseButtonReleased()
        {
            return leftMouseReleased;
        }

        public bool RightMouseButtonPressed()
        {
            return rightMousePressed;
        }

        public bool RightMouseButtonReleased()
        {
            return rightMouseReleased;
        }

        public bool EscapeButtonPressed()
        {
            return escapeButtonPressed;
        }

        public bool DragStarted()
        {
            return leftMouseDragStart;
        }

        public Vector2 GetMouseDragStartTile()
        {
             return mouseTileStartDrag;        
        }

        public Vector2 GetMouseDragEndTile()
        {
            return mouseTileEndDrag;
        }

        public bool DragFinished()
        {
            return leftMouseDragEnd;
        }
    }
}
