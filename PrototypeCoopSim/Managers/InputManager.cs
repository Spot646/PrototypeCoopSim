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
    class InputManager
    {
        //Left Mouse
        private bool leftMousePressed = false;
        private bool leftMouseReleased = false;

        //Spawn Trees
        private bool spawnTreeButtonPressed = false;
        private bool spawnTreeButtonReleased = false;

        //Spawn Rocks
        private bool spawnRockButtonPressed = false;
        private bool spawnRockButtonReleased = false;

        //Escape
        private bool escapeButtonPressed = false;
        private bool escapeButtonReleased = false;

        public InputManager()
        {
            
        }

        public Vector2 GetCurrentMouseTile(mapManager mapIn)
        {
            MouseState mouseState = Mouse.GetState();
            return mapIn.getTileFromMousePosition(mouseState.X, mouseState.Y, 25, 0, 0, 500, 600);
        }

        public bool MouseOverMap()
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.X > 0 && mouseState.X < 500 && mouseState.Y > 0 && mouseState.Y < 600)
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
            spawnTreeButtonReleased = false;
            spawnRockButtonReleased = false;
            escapeButtonReleased = false;

            //Check for button down events
            if (Mouse.GetState().LeftButton == ButtonState.Pressed) leftMousePressed = true;
            if (Keyboard.GetState().IsKeyDown(Keys.T)) spawnTreeButtonPressed = true;           
            if (Keyboard.GetState().IsKeyDown(Keys.R)) spawnRockButtonPressed = true;            
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) escapeButtonPressed = true;
            
            //Check for button release events
            if (Mouse.GetState().LeftButton == ButtonState.Released && leftMousePressed) { leftMousePressed = false; leftMouseReleased = true; }
            if (Keyboard.GetState().IsKeyUp(Keys.T) && spawnTreeButtonPressed) { spawnTreeButtonPressed = false; spawnTreeButtonReleased = true; }
            if (Keyboard.GetState().IsKeyUp(Keys.R) && spawnRockButtonPressed) { spawnRockButtonPressed = false; spawnRockButtonReleased = true; }
            if (Keyboard.GetState().IsKeyUp(Keys.Escape) && escapeButtonPressed) { escapeButtonPressed = false; escapeButtonReleased = true; }
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

        public bool EscapeButtonPressed()
        {
            return escapeButtonPressed;
        }
    }
}
