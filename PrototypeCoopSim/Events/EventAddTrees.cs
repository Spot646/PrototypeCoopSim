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
    class EventAddTrees : Event
    {
        int numberOfTrees;
        int numberOfCycles;
        int currentCycle;
        int spread;
        mapManager associatedMap;
        Game associatedGame;

        public EventAddTrees(Game gameIn, mapManager mapIn, int numberOfTreesIn, int numberOfCyclesIn, int spreadIn)
        {
            this.SetEventName("Add Trees");
            numberOfTrees = numberOfTreesIn;
            numberOfCycles = numberOfCyclesIn;
            currentCycle = 0;
            associatedMap = mapIn;
            associatedGame = gameIn;
            spread = spreadIn;
            if(numberOfTreesIn == 0 || numberOfCyclesIn == 0)
            {
                this.SetComplete();
            }
        }

        public override void RunEvent(EventManager callingEventManager)
        {
            Random random = new Random();
            if (currentCycle == 0)
            {
                Vector2 randomTreePosition = new Vector2(random.Next() % associatedMap.getNumberTilesX(), random.Next() % associatedMap.getNumberTilesY());
                if (!associatedMap.getOccupied(randomTreePosition))
                {
                    associatedMap.setOccupied(randomTreePosition, true);
                    associatedMap.setOccupyingElement(randomTreePosition, new treeElement(associatedGame, (int)randomTreePosition.X, (int)randomTreePosition.Y));
                    numberOfTrees--;
                }
            }
            else
            {
                Vector2 randomTreePosition = new Vector2(random.Next() % associatedMap.getNumberTilesX(), random.Next() % associatedMap.getNumberTilesY());
                randomTreePosition = associatedMap.FindNearest(randomTreePosition, "Tree");

                int xMod = (random.Next() % spread) - (spread / 2);
                int yMod = (random.Next() % spread) - (spread / 2);

                randomTreePosition.X = randomTreePosition.X + xMod;
                randomTreePosition.Y = randomTreePosition.Y + yMod;

                if (associatedMap.TilePositionInBounds(randomTreePosition))
                {
                    if (!associatedMap.getOccupied(randomTreePosition))
                    {
                        associatedMap.setOccupied(randomTreePosition, true);
                        associatedMap.setOccupyingElement(randomTreePosition, new treeElement(associatedGame, (int)randomTreePosition.X, (int)randomTreePosition.Y));
                        numberOfTrees--;
                    }
                }
            }

            if(numberOfTrees <= 0)
            {
                currentCycle++;
                if (currentCycle >= numberOfCycles)
                {
                    this.SetComplete();
                }
            }
        }
    }
}
