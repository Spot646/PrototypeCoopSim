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
    class EventGenerateWorld : Event
    {
        int numberTrees;
        int treeCycles;
        int treeSpread;
        int numberRocks;
        bool treesCreated;
        bool rocksCreated;
        bool completed;
        mapManager associatedMap;
        Game associatedGame;

        public EventGenerateWorld(Game gameIn, mapManager mapIn, int numberTreesIn, int treeCyclesIn, int treeSpreadIn, int numberRocksIn)
        {
            this.SetEventName("Generate World");
            numberTrees = numberTreesIn;
            treeCycles = treeCyclesIn;
            treeSpread = treeSpreadIn;
            numberRocks = numberRocksIn;
            treesCreated = false;
            rocksCreated = false;
            associatedMap = mapIn;
            associatedGame = gameIn;
        }

        public override void RunEvent(EventManager callingEventManager)
        {
            if (!treesCreated)
            {
                Event treeEvent = new EventAddTrees(associatedGame, associatedMap, numberTrees, treeCycles, treeSpread);
                callingEventManager.AddEvent(treeEvent);
                this.Suspend(treeEvent);
                treesCreated = true;
            }
            if (!rocksCreated)
            {
                Event rockEvent = new EventAddRocks(associatedGame, associatedMap, numberRocks);
                callingEventManager.AddEvent(rockEvent);
                this.Suspend(rockEvent);
                rocksCreated = true;
            }
            if (treesCreated && rocksCreated)
            {
                this.SetComplete();
            }
        }
    }
}
