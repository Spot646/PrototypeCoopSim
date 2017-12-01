using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Events;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Managers;

namespace PrototypeCoopSim.Events
{
    class EventMineRocks : Event
    {
        mapManager associatedMap;
        Game associatedGame;
        InputManager inputManager;
        //current rock
        gameElement currentElementFocus;

        public EventMineRocks(Game gameIn, mapManager mapIn, int numberOfRocksIn)
        {
            SetEventName("Mine Rocks");
            associatedMap = mapIn;
            associatedGame = gameIn;
        }

        public override void RunEvent(EventManager callingEventManager)
        {
            if (inputManager.LeftMouseButtonReleased())
            {
                if (associatedMap.getOccupied(inputManager.GetCurrentMouseTile(associatedMap)))
                {
                    associatedMap.getOccupyingElement(inputManager.GetCurrentMouseTile(associatedMap)).UpdateCurrentHealth(20);
                    currentElementFocus = associatedMap.getOccupyingElement(inputManager.GetCurrentMouseTile(associatedMap));
                }
                else
                {
                    currentElementFocus = null;
                }
            }
        }
    }
}
