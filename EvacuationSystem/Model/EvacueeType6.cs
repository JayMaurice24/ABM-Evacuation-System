using System;
using System.Collections.Generic;

namespace EvacuationSystem.Model;


/// <summary>
/// Agent With Low Risk, High Aggression and High Speed 
/// </summary>
public class EvacueeType6 : Evacuee
{
    #region Init

    public override void Init(GridLayer layer)
    {
        Layer = layer;
        OriginalPosition = Layer.FindRandomPosition();
        Position = OriginalPosition;
        RiskLevel = Characteristics.LowRisk();
        Speed = Characteristics.HighSpeed();
        Aggression = 2;
        FoundExit = false;
        Leadership = Rand.NextDouble();
        Empathy = Rand.NextDouble();
        CollaborationFactor = Rand.NextDouble();
        Health = Rand.Next(30, 100);
        Strength = Rand.NextDouble();
        IsConscious = true;
        FoundExit = false;
        FoundDistressedAgent = false;
        Helping = false; 
        AgentReturningForItem = false;
        IsLeader = false;
        IsInGroup = false;
        Group = new List<Evacuee>();
        Leader = null;
        Helper = null;
        Helped = null; 
        Layer.EvacueeEnvironment.Insert(this);
    }

    #endregion

    #region Tick

 public override void Tick()
    {
        if (!EvacueeHasStartedMoving) 
        {
            if (Layer.GetCurrentTick() < 2) return; 
            if (!Layer.Ring) return;
            if (RiskLevel < Layer.GetCurrentTick() || Perception(Position, Layer.FireLocations[0])) return;
            EvacueeHasStartedMoving = true;
            if (Rand.NextDouble() > 0.5) //Chance of Agent forgetting an item
            {
                DelayTime = Rand.Next(30, 60);
                ForgotAnItem = true;
            }
        }
        if (!FoundExit) //finds nearest exit to the agent
        {
            Goal = FindNearestExit(Layer.PossibleGoal);
            Console.WriteLine($"{GetType().Name}{ID} is heading towards the exit");
            FoundExit = true;
            DetermineLeader();
        }
        else
        {
            if ((int)Layer.GetCurrentTick() % Speed != 0) return; //Agent moves every few seconds
            if (!IsConscious)
            {
                if (FoundHelp)
                {
                    MovingWithHelp();
                }
                else
                { 
                    Console.WriteLine($"{GetType().Name} {ID} is Unconscious at cell {Position}");
                }
            }
            else
            {
                if (DelayTime == Layer.GetCurrentTick() && ForgotAnItem) {
                    if (Rand.NextDouble() > 0.7) //Chance of Returning for Item
                    {
                        AgentReturningForItem = true;
                        if (IsInGroup)
                        {
                            UpdateGroupStatus();
                        }
                        else
                        {
                            Goal = OriginalPosition;
                            Console.WriteLine($"{GetType().Name} {ID} Has forgotten an item and is heading back");
                        }
                        return;
                    }
                }
                if (!FoundDistressedAgent)
                {
                    if (IsLeader)
                    {
                        if(!ReturningWithGroupForItem) FormGroup();
                        Evacuate();
                    }
                    else if (IsInGroup && !IsLeader)
                    {
                        if (!LeaderHasReachedExit)
                        {
                            MoveTowardsGroupLeader();
                        }
                        else
                        {
                            Evacuate();
                        }
                    }

                    else
                    {
                        Evacuate();
                    }
                    HelpAgent();
                }
                else
                {
                    if (ReturningWithGroupToHelp)
                    {
                        if (IsLeader)
                        {
                            if (Helping)
                            {
                                Evacuate();
                            }
                            else if (ReachedDistressedAgent)
                            {
                                OfferHelp();
                            }
                            else
                            {
                                Evacuate();
                            }
                        }
                        else
                        {
                            MoveTowardsGroupLeader();
                        }
                    }
                    else
                    {
                        if (Helping)
                        {
                            Evacuate();
                        }
                        else
                        {
                            if (ReachedDistressedAgent)
                            {
                                OfferHelp();
                                Goal = FindNearestExit(Layer.PossibleGoal);
                            }
                            else
                            {
                                Evacuate();
                            }
                        }
                       
                    }
                }
                DetermineLeader();
                UpdateHealthStatus();
            }
        }
    }

    #endregion
}
