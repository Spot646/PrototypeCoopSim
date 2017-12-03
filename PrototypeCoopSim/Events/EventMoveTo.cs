using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Events;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Managers;
using System.Collections.Generic;
using PrototypeCoopSim.Settings;

namespace PrototypeCoopSim.Events
{
    class EventMoveTo : Event
    {
        //Constants and limits
        
        //Related classes
        private mapManager associatedMap;
        private Game associatedGame;
        private gameElement elementToMove;
        private EventManager callingEventManager;

        //Target
        private Vector2 destination;

        //Boolean gates
        private bool flowMapProduced;
        private bool pathFound;
        private bool noPathExists;

        //Current path
        private List<Vector2> currentPath;

        //animation controls
        GameTime gameTime;
        private bool moveGoingOn;
        
        //Flow Map
        struct MapNode
        {
            public Vector2 nodeLocation;
            public Vector2 nodePreviousLocation;
        }
        private bool[,] checkedMap;
        private int nextListTarget;
        private List<MapNode> nodeList;

        public EventMoveTo(Game gameIn, mapManager mapIn, gameElement elementToMoveIn, Vector2 destinationIn, GameTime gameTimeIn)
        {
            this.SetEventName("Move To");
            associatedMap = mapIn;
            associatedGame = gameIn;
            elementToMove = elementToMoveIn;
            destination = destinationIn;
            gameTime = gameTimeIn;
            pathFound = false;
            flowMapProduced = false;
            noPathExists = false;
            nextListTarget = 0;
            moveGoingOn = false;
            currentPath = new List<Vector2>();
            currentPath.Clear();
            nodeList = new List<MapNode>();
            nodeList.Clear();
            //setup a bool array the size of the map to determine which places have already been reviewed
            checkedMap = new bool[associatedMap.getNumberTilesX(), associatedMap.getNumberTilesY()];
            for(int x = 0; x < associatedMap.getNumberTilesX(); x++)
            {
                for(int y = 0; y <associatedMap.getNumberTilesY(); y++)
                {
                    checkedMap[x, y] = false;
                }
            }
        }

        public override void RunEvent(EventManager callingEventManagerIn)
        {

            callingEventManager = callingEventManagerIn;
            //Check if the goal is one move away and if it is not occupied
            if (Math.Abs(elementToMove.getWorldPositionX() - destination.X) + Math.Abs(elementToMove.getWorldPositionY() - destination.Y) <= 1)
            {
                //In this case we either only have to move one step - or this is a component of a larger path
                this.MoveToDestination();                
            }
            else
            {
                if (!this.GetShutdownSmoothly())
                {
                    if (pathFound)
                    {
                        this.PerformNextMove();
                    }
                    else
                    {
                        if (flowMapProduced)
                        {
                            this.GeneratePath();
                        }
                        else
                        {
                            this.DevelopFlowMap();
                        }
                    }
                }
                else
                {
                    this.SetComplete();
                }
            }
            if (!this.GetShutdownSmoothly())
            {
                //reached destination
                if ((Math.Abs(elementToMove.getWorldPositionX() - elementToMove.GetFinalDestination().X) + Math.Abs(elementToMove.getWorldPositionY() - elementToMove.GetFinalDestination().Y) == 0))
                {
                    elementToMove.KillLinkedMovement();
                }
                //closest I can get
                if ((Math.Abs(elementToMove.getWorldPositionX() - elementToMove.GetFinalDestination().X) + Math.Abs(elementToMove.getWorldPositionY() - elementToMove.GetFinalDestination().Y) == 1) && associatedMap.getOccupied(elementToMove.GetFinalDestination()))
                {
                    if (associatedMap.getOccupyingElement(elementToMove.GetFinalDestination()).Idle())
                    {
                        elementToMove.KillLinkedMovement();
                    }
                }
            }
        }

        private void MoveToDestination()
        {
            //elementToMove.SetStatusMessage("Trying to move");
            //Are we starting a move?
            if (!moveGoingOn && !associatedMap.getOccupied(destination))
            {
                moveGoingOn = true;
                //set Destination As Occupied
                associatedMap.setOccupied(destination, true);
                associatedMap.setOccupyingElement(destination, elementToMove);
            }
            if (moveGoingOn && !this.GetShutdownSmoothly())
            {
                //Process the move
                //Find direction of move
                float xMove = 0.0f;
                float yMove = 0.0f;
                if (elementToMove.getWorldPositionX() > destination.X) xMove = -1.0f;
                if (elementToMove.getWorldPositionX() < destination.X) xMove = 1.0f;
                if (elementToMove.getWorldPositionY() > destination.Y) yMove = -1.0f;
                if (elementToMove.getWorldPositionY() < destination.Y) yMove = 1.0f;

                Vector2 newAnimationOffset = new Vector2(0, 0);
                newAnimationOffset.X = elementToMove.GetAnimationOffset().X + (gameTime.ElapsedGameTime.Milliseconds * elementToMove.GetSpeed() * xMove);
                newAnimationOffset.Y = elementToMove.GetAnimationOffset().Y + (gameTime.ElapsedGameTime.Milliseconds * elementToMove.GetSpeed() * yMove);
                elementToMove.SetAnimationOffset(newAnimationOffset);

                //Has the move finished?
                if (Math.Abs(newAnimationOffset.X) > GlobalVariables.TILE_SIZE || Math.Abs(newAnimationOffset.Y) > GlobalVariables.TILE_SIZE)
                {
                    associatedMap.setOccupied(new Vector2(elementToMove.getWorldPositionX(), elementToMove.getWorldPositionY()), false);
                    associatedMap.setOccupyingElement(new Vector2(elementToMove.getWorldPositionX(), elementToMove.getWorldPositionY()), null);
                    elementToMove.worldPositionX = (int)destination.X;
                    elementToMove.worldPositionY = (int)destination.Y;
                    elementToMove.SetAnimationOffset(new Vector2(0, 0));
                    if (this.GetCallingEvent() == null)
                    {
                        elementToMove.SetStatusMessage("I'm Here!");
                    }
                    this.SetComplete();
                }
                else
                {
                    elementToMove.SetStatusMessage("Walking");
                }
            }
            else
            {
                this.SetComplete();
            }
        }

        private void PerformNextMove()
        {
            bool canContinue = true;
            //Check if we have successfully moved, otherwise reattempt is occupier is moving
            if((int)currentPath[1].X == elementToMove.getWorldPositionX() && (int)currentPath[1].Y == elementToMove.getWorldPositionY())
            {
                //remove that step
                currentPath.RemoveAt(0);
            }
            else
            {
                if (associatedMap.getOccupied(new Vector2(currentPath[1].X, currentPath[1].Y)))
                {
                    if (associatedMap.getOccupyingElement(new Vector2(currentPath[1].X, currentPath[1].Y)).GetMovable())
                    {
                        bool attemptPush = false;
                        elementToMove.SetStatusMessage("Someone is in my spot!");
                        //Try to push them
                        //TODO fix out of range on push
                        if (associatedMap.getOccupyingElement(new Vector2(currentPath[1].X, currentPath[1].Y)).Idle())
                        {
                            Vector2 safeSpotToPush = new Vector2(-1, -1);
                            if (!associatedMap.getOccupied(new Vector2(currentPath[1].X + 1, currentPath[1].Y)))
                            {
                                safeSpotToPush.X = currentPath[1].X + 1;
                                safeSpotToPush.Y = currentPath[1].Y;
                                attemptPush = true;
                            }
                            else if (!associatedMap.getOccupied(new Vector2(currentPath[1].X - 1, currentPath[1].Y)))
                            {
                                safeSpotToPush.X = currentPath[1].X - 1;
                                safeSpotToPush.Y = currentPath[1].Y;
                                attemptPush = true;
                            }
                            else if (!associatedMap.getOccupied(new Vector2(currentPath[1].X, currentPath[1].Y + 1)))
                            {
                                safeSpotToPush.X = currentPath[1].X;
                                safeSpotToPush.Y = currentPath[1].Y + 1;
                                attemptPush = true;
                            }
                            else if (!associatedMap.getOccupied(new Vector2(currentPath[1].X, currentPath[1].Y - 1)))
                            {
                                safeSpotToPush.X = currentPath[1].X;
                                safeSpotToPush.Y = currentPath[1].Y - 1;
                                attemptPush = true;
                            }
                            if (attemptPush)
                            {
                                EventMoveTo pushEvent = new EventMoveTo(associatedGame, associatedMap, associatedMap.getOccupyingElement(new Vector2(currentPath[1].X, currentPath[1].Y)), safeSpotToPush, gameTime);
                                associatedMap.getOccupyingElement(new Vector2(currentPath[1].X, currentPath[1].Y)).KillLinkedMovement();
                                associatedMap.getOccupyingElement(new Vector2(currentPath[1].X, currentPath[1].Y)).LinkToMoveEvent(pushEvent);
                                callingEventManager.AddEvent(pushEvent);
                            }
                        }
                        elementToMove.KillLinkedMovement();
                        canContinue = false;
                        if (attemptPush || associatedMap.getOccupyingElement(new Vector2(currentPath[1].X, currentPath[1].Y)).Moving()) { 
                            EventMoveTo reattempt = new EventMoveTo(associatedGame, associatedMap, elementToMove, destination, this.gameTime);
                            elementToMove.LinkToMoveEvent(reattempt);
                            callingEventManager.AddEvent(reattempt);
                        }                        
                    }
                }
            }

            if (canContinue)
            {
                Event nextMove = new EventMoveTo(associatedGame, associatedMap, elementToMove, currentPath[1], this.gameTime);
                callingEventManager.AddEvent(nextMove);
                //suspend until that move event ends
                this.Suspend(nextMove);
            }
            else
            {
                this.SetComplete();
            }
        }

        private void GeneratePath()
        {
            elementToMove.SetStatusMessage("Preparing to move");
            //at this point you have found the destination in the nodeList - so work backwards from the destination as you store the path; 
            //start at the last nodeList entry - which should be the destination
            currentPath.Add(nodeList[nodeList.Count - 1].nodeLocation);
            //insert the previous Node as it is either the origin(2 step path) or the next point to review
            currentPath.Insert(0, nodeList[nodeList.Count - 1].nodePreviousLocation);
            //everytime the next part of the path is find insert it at 0 into currentPath
            for(int i = nodeList.Count - 2; i >= 0; i--)
            {
                if (nodeList[i].nodeLocation.Equals(currentPath[0]))
                {
                    //if we found a hit, add the node listed as previous
                    currentPath.Insert(0, nodeList[i].nodePreviousLocation);
                }
            }
            //check that we ended at the origin
            if(currentPath[0].X == elementToMove.getWorldPositionX() && currentPath[0].Y == elementToMove.getWorldPositionY())
            {
                //path was found successfully
                pathFound = true;
            }
            else
            {
                //an error has occured - fail the path
                elementToMove.SetStatusMessage("Hmm, something blocked my path.");
                noPathExists = true;
                this.SetComplete();
            }
        }

        public Vector2 ReturnDestination()
        {
            return destination;
        }

        private void DevelopFlowMap()
        {
            if(nodeList.Count == 0)
            {
                //seed list
                //elementToMove.SetStatusMessage("Looking for path");
                MapNode newNode;
                newNode.nodeLocation = new Vector2(elementToMove.getWorldPositionX(), elementToMove.getWorldPositionY());
                newNode.nodePreviousLocation = new Vector2(elementToMove.getWorldPositionX(), elementToMove.getWorldPositionY());
                nodeList.Add(newNode);
                nextListTarget = 0;
                checkedMap[elementToMove.getWorldPositionX(), elementToMove.getWorldPositionY()] = true;
            }
            else
            {
                //grow list
                if(nextListTarget >= nodeList.Count)
                {
                    //No further nodes to check, end search
                    noPathExists = true;
                    elementToMove.SetStatusMessage("I can't get there!" + "(" + destination.X + "," + destination.Y + ")");
                    this.SetComplete();
                }
                else
                {
                    //still nodes to check
                    //check 4 cardinal directions
                    for(int i = 0; i < 4; i++)
                    {
                        Vector2 newNodeLocation  = new Vector2(nodeList[nextListTarget].nodeLocation.X, nodeList[nextListTarget].nodeLocation.Y);
                        if (i == 0 && newNodeLocation.Y > 0) newNodeLocation.Y--; //North
                        if (i == 1 && newNodeLocation.X < associatedMap.getNumberTilesX()- 1) newNodeLocation.X++; //East
                        if (i == 2 && newNodeLocation.Y < associatedMap.getNumberTilesY() - 1) newNodeLocation.Y++; //South
                        if (i == 3 && newNodeLocation.X > 0) newNodeLocation.X--; //West

                        //if they are already checked - ignore
                        if (checkedMap[(int)newNodeLocation.X, (int)newNodeLocation.Y] == false)
                        {
                            checkedMap[(int)newNodeLocation.X, (int)newNodeLocation.Y] = true;
                            //if they are already occupied - ignore
                            if (!associatedMap.getOccupied(newNodeLocation))
                            {
                                //at this point it is safe to assume this is a usable node and add it to the list
                                MapNode nodeToAdd;
                                nodeToAdd.nodeLocation = newNodeLocation;
                                nodeToAdd.nodePreviousLocation = new Vector2(nodeList[nextListTarget].nodeLocation.X, nodeList[nextListTarget].nodeLocation.Y);
                                nodeList.Add(nodeToAdd);
                                //If this new point was our destination - complete this step
                                if (nodeToAdd.nodeLocation.Equals(destination))
                                {
                                    elementToMove.SetStatusMessage("Found Path");
                                    flowMapProduced = true; //start next step
                                    i = 4; //exit for loop
                                }
                            }
                            else
                            {
                                //if they are occupied - see if the person occupying the space can move
                                if (associatedMap.getOccupyingElement(newNodeLocation).GetMovable())
                                {                                    
                                    //at this point it is safe to assume this is a usable node and add it to the list
                                    MapNode nodeToAdd;
                                    nodeToAdd.nodeLocation = newNodeLocation;
                                    nodeToAdd.nodePreviousLocation = new Vector2(nodeList[nextListTarget].nodeLocation.X, nodeList[nextListTarget].nodeLocation.Y);
                                    nodeList.Add(nodeToAdd);
                                    //If this new point was our destination - complete this step
                                    if (nodeToAdd.nodeLocation.Equals(destination))
                                    {
                                        elementToMove.SetStatusMessage("Found Path");
                                        flowMapProduced = true; //start next step
                                        i = 4; //exit for loop
                                    }
                                }
                            }
                        }
                    }
                    //increment next list target for next cycle
                    nextListTarget++;
                }
            }
        }
    }
}
