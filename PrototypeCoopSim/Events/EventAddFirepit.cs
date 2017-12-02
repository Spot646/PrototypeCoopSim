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
    class EventAddFirepit : Event
    {
        int numberOfFirepit;
        mapManager associatedMap;
        Game associatedGame;
        //Input
        InputManager inputManager;

        public EventAddFirepit(Game gameIn, mapManager mapIn, int numberOfFirepitIn)
        {
            this.SetEventName("Add Firepit");
            numberOfFirepit = numberOfFirepitIn;
            associatedMap = mapIn;
            associatedGame = gameIn;
            //Setup Input
            inputManager = new InputManager(associatedMap);
        }

        public override void RunEvent(EventManager callingEventManager)
        {
            Random random = new Random();
            Vector2 randomFirePosition = new Vector2(random.Next() % associatedMap.getNumberTilesX(), random.Next() % associatedMap.getNumberTilesY());
            if (!associatedMap.getOccupied(randomFirePosition))
            {
                associatedMap.setOccupied(inputManager.GetCurrentMouseTile(associatedMap), true);
                associatedMap.setOccupyingElement(inputManager.GetCurrentMouseTile(associatedMap), new FirepitElement(associatedGame, (int)inputManager.GetCurrentMouseTile(associatedMap).X, (int)inputManager.GetCurrentMouseTile(associatedMap).Y));
                numberOfFirepit--;
            }

            if(numberOfFirepit <= 0)
            {
                this.SetComplete();
            }
        }
    }
}
