using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Events;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Managers;
using System.Collections.Generic;

namespace PrototypeCoopSim.Events
{
    class EventHarvestRocks : Event
    {
        private Game associatedGame;
        private EventManager associatedEventManager;
        private mapManager associatedMap;
        private ActorElement focusElement;
        private GameTime gameTime;

        //flags
        bool nextToRock;
        bool repeating;
        bool seekingRock;

        //counters
        public int retryCounter;
        private const int retryAttempts = 5;

        //Info
        Vector2 rockLocation;

        public EventHarvestRocks(Game gameIn, mapManager mapIn, EventManager eventManagerIn, gameElement focusElementIn, GameTime gameTimeIn, bool repeatingIn)
        {
            repeating = repeatingIn;
            associatedGame = gameIn;
            associatedEventManager = eventManagerIn;
            associatedMap = mapIn;
            gameTime = gameTimeIn;
            focusElement = (ActorElement)focusElementIn;
            retryCounter = 0;
            rockLocation = new Vector2(-1, -1);
            nextToRock = false;
            seekingRock = false;
        }
        
        public override void RunEvent(EventManager callingEventManager)
        {
            if (!nextToRock)
            {
                if (seekingRock){
                    retryCounter++;
                    focusElement.SetStatusMessage("Couldn't get to a Rock");
                    //keep a retry counter and retry up to 5 times
                    if(retryCounter < retryAttempts)
                    {
                        EventHarvestRocks retryEvent = new EventHarvestRocks(associatedGame, associatedMap, associatedEventManager, focusElement, gameTime, repeating);
                        retryEvent.retryCounter = retryCounter;
                        callingEventManager.AddEvent(retryEvent);
                    }
                    //reset on success
                    seekingRock = false;
                    this.SetComplete();
                }
                //check if next to Rock
                Vector2 testLocation = new Vector2(-1, -1);
                for (int i = 0; i < 4; i++)
                {
                    if (i == 0)  {testLocation = new Vector2(focusElement.getWorldPositionX() + 1, focusElement.getWorldPositionY()); }
                    if (i == 1) { testLocation = new Vector2(focusElement.getWorldPositionX() - 1, focusElement.getWorldPositionY()); }
                    if (i == 2) { testLocation = new Vector2(focusElement.getWorldPositionX() , focusElement.getWorldPositionY() + 1); }
                    if (i == 3) { testLocation = new Vector2(focusElement.getWorldPositionX() , focusElement.getWorldPositionY() - 1); }

                    if (associatedMap.TilePositionInBounds(testLocation))
                    {
                        if (associatedMap.getOccupied(testLocation))
                        {
                            if(associatedMap.getOccupyingElement(testLocation).GetElementName() == "Rock")
                            {
                                nextToRock = true;
                                i = 4;
                            }
                        }
                    }
                }
                if (nextToRock)
                {
                    rockLocation.X = testLocation.X;
                    rockLocation.Y = testLocation.Y;
                }
                //if not, find Rock
                else
                {
                    seekingRock = true;
                    Vector2 RockToTarget = associatedMap.FindNearest(focusElement.getWorldPositionVector(), "Rock");
                    //check if a real Rock was found
                    if (RockToTarget.X == -1 && RockToTarget.Y == -1)
                    {
                        //couldn't find a Rock
                        focusElement.SetStatusMessage("Can't see any Rocks!");
                        this.SetComplete();
                    }
                    else
                    {
                        //move to Rock
                        EventMoveTo movingEvent = new EventMoveTo(associatedGame, associatedMap, focusElement, RockToTarget, gameTime);
                        if (focusElement.Moving())
                        {
                            focusElement.ReplaceLinkedMovement(movingEvent);
                            associatedEventManager.AddEvent(movingEvent);
                            this.Suspend(movingEvent);
                        }
                        else
                        {
                            focusElement.LinkToMoveEvent(movingEvent);
                            associatedEventManager.AddEvent(movingEvent);
                            this.Suspend(movingEvent);
                        }                        
                    }
                }
            }
            if (nextToRock)
            {
                focusElement.SetStatusMessage("Getting ready to harvest Rock");
                //hit Rock
                if (associatedMap.getOccupied(rockLocation))
                {
                    //success, reset retry counter
                    retryCounter = 0;
                    if(associatedMap.getOccupyingElement(rockLocation).GetElementName() == "Rock")
                    {
                        associatedMap.getOccupyingElement(rockLocation).UpdateCurrentHealth(5);
                        if(associatedMap.getOccupyingElement(rockLocation).currentHealth <= 0){
                            //Rock is out of HP - generate the rock pile
                            associatedMap.setOccupyingElement(rockLocation, new rockResourceElement(associatedGame, (int)rockLocation.X, (int)rockLocation.Y, (RockElement)associatedMap.getOccupyingElement(rockLocation)));
                            if (repeating)
                            {
                                associatedEventManager.AddEvent(new EventHarvestRocks(associatedGame, associatedMap, associatedEventManager, focusElement, gameTime, true));
                            }
                            this.SetComplete();
                        }
                    }
                    //it's likely someone else has mined it down already
                    else
                    {
                        if (repeating)
                        {
                            associatedEventManager.AddEvent(new EventHarvestRocks(associatedGame, associatedMap, associatedEventManager, focusElement, gameTime, true));
                        }
                    }
                }
                //this.SetComplete();
            }       
        }
    }
}
