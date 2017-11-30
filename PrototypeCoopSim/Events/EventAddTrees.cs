﻿using Microsoft.Xna.Framework;
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
        mapManager associatedMap;
        Game associatedGame;

        public EventAddTrees(Game gameIn, mapManager mapIn, int numberOfTreesIn)
        {
            this.SetEventName("Add Trees");
            numberOfTrees = numberOfTreesIn;
            associatedMap = mapIn;
            associatedGame = gameIn;
        }

        public override void RunEvent(EventManager callingEventManager)
        {
            Random random = new Random();
            Vector2 randomTreePosition = new Vector2(random.Next() % associatedMap.getNumberTilesX(), random.Next() % associatedMap.getNumberTilesY());
            if (!associatedMap.getOccupied(randomTreePosition))
            {
                associatedMap.setOccupied(randomTreePosition, true);
                associatedMap.setOccupyingElement(randomTreePosition, new treeElement(associatedGame, (int)randomTreePosition.X, (int)randomTreePosition.Y));
                numberOfTrees--;
            }

            if(numberOfTrees <= 0)
            {
                this.SetComplete();
            }
        }
    }
}