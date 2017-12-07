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
        private ActorElement elementToMove;
        private EventManager callingEventManager;

        //Target
        private Vector2 destination;

        //Boolean gates
        private bool flowMapProduced;
        private bool pathFound;
        private bool noPathExists;
        private bool movementWasPossible;

        //Current path
        private List<Vector2> currentPath;
        private int reattemptCounter;
        private const int reattemptLimit = 20;
        private Vector2 originalDestination;

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
            elementToMove = (ActorElement)elementToMoveIn;
            destination = destinationIn;
            gameTime = gameTimeIn;
            pathFound = false;
            flowMapProduced = false;
            noPathExists = false;
            movementWasPossible = false;
            nextListTarget = 0;
            reattemptCounter = 0;
            originalDestination = destination;
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
                if (moveGoingOn || !associatedMap.getOccupied(destination))
                {
                    //In this case we either only have to move one step - or this is a component of a larger path
                    this.MoveToDestination();
                }
                else
                {
                    elementToMove.SetStuck(true);
                    //kill user event if this wasn't a child
                    if (elementToMove.GetFinalDestination() == destination)
                    {
                        this.SetComplete();
                        elementToMove.KillLinkedMovement();
                    }
                    else
                    {
                        this.SetComplete();
                    }
                }            
            }
            else
            {
                if (reattemptCounter > reattemptLimit)
                {
                    elementToMove.SetStatusMessage("Ran out of attempts");
                    this.ShutdownSmoothly();
                    elementToMove.KillLinkedMovement();
                }
                if (!this.GetShutdownSmoothly())
                {
                    if (pathFound)
                    {
                        movementWasPossible = true;
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
                    elementToMove.KillLinkedMovement();
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
                elementToMove.SetStuck(false);
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
                elementToMove.SetStuck(true);
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
                        if (elementToMove.GetStuck())
                        {
                            //already stuck, stop movement
                            elementToMove.KillLinkedMovement();
                            this.ShutdownSmoothly();
                        }
                        else
                        {
                            elementToMove.SetStuck(true);
                        }                        
                        elementToMove.SetStatusMessage("Someone is in my spot!");
                        this.ShutdownSmoothly(); //instead of kill
                        canContinue = false;
                        if (associatedMap.getOccupied(elementToMove.GetFinalDestination())){
                            if (!associatedMap.getOccupyingElement(elementToMove.GetFinalDestination()).Moving())
                            {
                                reattemptCounter++;
                                EventMoveTo reattempt = new EventMoveTo(associatedGame, associatedMap, elementToMove, associatedMap.FindNearest(originalDestination, "Empty"), this.gameTime);
                                //transfer the calling event to the new event as we are giving up control
                                reattempt.setCallingEvent(this.GetCallingEvent());
                                reattempt.SetReattemptCounter(reattemptCounter);
                                reattempt.SetOriginalDestination(originalDestination);
                                elementToMove.ReplaceLinkedMovement(reattempt);
                                callingEventManager.AddEvent(reattempt);
                            }
                            else
                            {
                                reattemptCounter++;
                                EventMoveTo reattempt = new EventMoveTo(associatedGame, associatedMap, elementToMove, destination, this.gameTime);
                                //transfer the calling event to the new event as we are giving up control
                                reattempt.setCallingEvent(this.GetCallingEvent());
                                reattempt.SetReattemptCounter(reattemptCounter);
                                elementToMove.ReplaceLinkedMovement(reattempt);
                                callingEventManager.AddEvent(reattempt);
                            }
                        }
                        else
                        {
                            reattemptCounter++;
                            EventMoveTo reattempt = new EventMoveTo(associatedGame, associatedMap, elementToMove, destination, this.gameTime);
                            //transfer the calling event to the new event as we are giving up control
                            reattempt.setCallingEvent(this.GetCallingEvent());
                            reattempt.SetReattemptCounter(reattemptCounter);
                            elementToMove.ReplaceLinkedMovement(reattempt);
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
                elementToMove.SetStuck(true);
                noPathExists = true;
                this.SetComplete();
            }
        }

        public void SetReattemptCounter(int counter)
        {
            reattemptCounter = counter;
        }

        public Vector2 ReturnDestination()
        {
            return destination;
        }

        public void SetOriginalDestination(Vector2 destinationIn)
        {
            originalDestination = destinationIn;
        }

        private void DevelopFlowMap()
        {
            if(nodeList.Count == 0)
            {
                //seed list
                elementToMove.SetStatusMessage("Looking for path");
                MapNode newNode;
                newNode.nodeLocation = new Vector2(elementToMove.getWorldPositionX(), elementToMove.getWorldPositionY());
                newNode.nodePreviousLocation = new Vector2(elementToMove.getWorldPositionX(), elementToMove.getWorldPositionY());
                nodeList.Add(newNode);
                nextListTarget = 0;
                checkedMap[elementToMove.getWorldPositionX(), elementToMove.getWorldPositionY()] = true;
                //check to see if my end target is populated already, and if I should shift my target
                if (associatedMap.getOccupied(this.destination))
                {
                    if(associatedMap.getOccupyingElement(this.destination).Idle() || (associatedMap.getOccupyingElement(this.destination).GetStuck()))
                    {
                        this.destination = associatedMap.FindNearest(originalDestination, "Empty");
                    }
                }
            }
            else
            {
                //check if target was populated
                if (associatedMap.getOccupied(destination))
                {
                    if (associatedMap.getOccupyingElement(destination).GetMovable() && !associatedMap.getOccupyingElement(destination).Idle() && !associatedMap.getOccupyingElement(destination).GetStuck())
                    {
                        nodeList.Clear();
                    }
                }
                //grow list
                if (nextListTarget >= nodeList.Count)
                {
                    reattemptCounter++;
                    EventMoveTo reattempt = new EventMoveTo(associatedGame, associatedMap, elementToMove, associatedMap.FindNearest(originalDestination, "Empty"), this.gameTime);
                    //transfer the calling event to the new event as we are giving up control
                    reattempt.setCallingEvent(this.GetCallingEvent());
                    reattempt.SetReattemptCounter(reattemptCounter);
                    reattempt.SetOriginalDestination(originalDestination);
                    elementToMove.ReplaceLinkedMovement(reattempt);
                    this.SetComplete();
                    callingEventManager.AddEvent(reattempt);
                }
                else
                {
                    //still nodes to check
                    //check 4 cardinal directions
                    for (int i = 0; i < 4; i++)
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
                                if (associatedMap.getOccupyingElement(newNodeLocation).GetMovable() && !associatedMap.getOccupyingElement(newNodeLocation).Idle() && !associatedMap.getOccupyingElement(newNodeLocation).GetStuck())
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
