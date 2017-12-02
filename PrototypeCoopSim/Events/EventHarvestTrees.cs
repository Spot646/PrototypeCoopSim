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
    class EventHarvestTrees : Event
    {
        private Game associatedGame;
        private EventManager associatedEventManager;
        private mapManager associatedMap;
        private gameElement focusElement;
        private GameTime gameTime;

        //flags
        bool nextToTree;
        bool repeating;

        //Info
        Vector2 treeLocation;

        public EventHarvestTrees(Game gameIn, mapManager mapIn, EventManager eventManagerIn, gameElement focusElementIn, GameTime gameTimeIn, bool repeatingIn)
        {
            repeating = repeatingIn;
            associatedGame = gameIn;
            associatedEventManager = eventManagerIn;
            associatedMap = mapIn;
            gameTime = gameTimeIn;
            focusElement = focusElementIn;
            treeLocation = new Vector2(-1, -1);
            nextToTree = false;
        }
        
        public override void RunEvent(EventManager callingEventManager)
        {
            if (!nextToTree)
            {
                //check if next to tree
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
                            if(associatedMap.getOccupyingElement(testLocation).GetElementName() == "Tree")
                            {
                                nextToTree = true;
                                i = 4;
                            }
                        }
                    }
                }
                if (nextToTree)
                {
                    treeLocation.X = testLocation.X;
                    treeLocation.Y = testLocation.Y;
                }
                //if not, find tree
                else
                {
                    Vector2 treeToTarget = associatedMap.FindNearest(focusElement.getWorldPositionVector(), "Tree");
                    //check if a real tree was found
                    if (treeToTarget.X == -1 && treeToTarget.Y == -1)
                    {
                        //couldn't find a tree
                        focusElement.SetStatusMessage("Can't see any trees!");
                        this.SetComplete();
                    }
                    else
                    {
                        //move to tree
                        if (focusElement.Moving())
                        {
                            focusElement.KillLinkedMovement();
                        }
                        EventMoveTo movingEvent = new EventMoveTo(associatedGame, associatedMap, focusElement, treeToTarget, gameTime);
                        focusElement.LinkToMoveEvent(movingEvent);
                        associatedEventManager.AddEvent(movingEvent);
                        this.Suspend(movingEvent);
                    }
                }
            }
            if (nextToTree)
            {
                focusElement.SetStatusMessage("Getting ready to harvest tree");
                //hit tree
                if (associatedMap.getOccupied(treeLocation))
                {
                    if(associatedMap.getOccupyingElement(treeLocation).GetElementName() == "Tree")
                    {
                        associatedMap.getOccupyingElement(treeLocation).UpdateCurrentHealth(1);
                        if(associatedMap.getOccupyingElement(treeLocation).currentHealth <= 0){
                            //Tree is out of HP - generate the logs
                            associatedMap.setOccupyingElement(treeLocation, new woodResourceElement(associatedGame, (int)treeLocation.X, (int)treeLocation.Y, (treeElement)associatedMap.getOccupyingElement(treeLocation)));
                            if (repeating)
                            {
                                associatedEventManager.AddEvent(new EventHarvestTrees(associatedGame, associatedMap, associatedEventManager, focusElement, gameTime, true));
                            }
                            this.SetComplete();
                        }
                    }
                    //if's likely someone else has chopped it down already
                    else
                    {
                        if (repeating)
                        {
                            associatedEventManager.AddEvent(new EventHarvestTrees(associatedGame, associatedMap, associatedEventManager, focusElement, gameTime, true));
                        }
                    }
                }
                //this.SetComplete();
            }       
        }
    }
}
