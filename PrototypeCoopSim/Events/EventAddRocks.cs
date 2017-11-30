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
    class EventAddRocks : Event
    {
        int numberOfRocks;
        mapManager associatedMap;
        Game associatedGame;

        public EventAddRocks(Game gameIn, mapManager mapIn, int numberOfRocksIn)
        {
            this.SetEventName("Add Rocks");
            numberOfRocks = numberOfRocksIn;
            associatedMap = mapIn;
            associatedGame = gameIn;
        }

        public override void RunEvent(EventManager callingEventManager)
        {
            Random random = new Random();
            Vector2 randomRockPosition = new Vector2(random.Next() % associatedMap.getNumberTilesX(), random.Next() % associatedMap.getNumberTilesY());
            if (!associatedMap.getOccupied(randomRockPosition))
            {
                associatedMap.setOccupied(randomRockPosition, true);
                associatedMap.setOccupyingElement(randomRockPosition, new RockElement(associatedGame, (int)randomRockPosition.X, (int)randomRockPosition.Y));
                numberOfRocks--;
            }

            if(numberOfRocks <= 0)
            {
                this.SetComplete();
            }
        }
    }
}
