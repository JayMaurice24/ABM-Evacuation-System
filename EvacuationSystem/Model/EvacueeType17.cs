using System;
using System.Collections.Generic;

namespace EvacuationSystem.Model;

/// <summary>
/// Agent With High Risk, Medium Aggression and High Speed 
/// </summary>
public class EvacueeType17: Evacuee
{
    #region Init

     public override void Init(GridLayer layer)
    {
        Layer = layer;
        OriginalPosition = Layer.FindRandomPosition();
        Position = OriginalPosition;
        RiskLevel = Characteristics.HighRisk();
        Speed = Characteristics.HighSpeed();
        Aggression = 1;
        FoundExit = false;
        Leadership = Rand.NextDouble();
        Empathy = Rand.NextDouble();
        CollaborationFactor = Rand.NextDouble();
        Health = Rand.Next(30, 100);
        DelayTime = Rand.Next(30, 60);
        Strength = Rand.NextDouble();
        IsConscious = true;
        IsConscious = true;
        FoundExit = false;
        FoundDistressedAgent = false;
        Helping = false; 
        AgentForgotItem = false;
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
        }

        if (!FoundExit)
        {
            Goal = FindNearestExit(Layer.PossibleGoal);
            Console.WriteLine($"Agent {ID} moving towards exit");
            FoundExit = true;
            DetermineLeader();
        }
        else
        {
            //if ((int)Layer.GetCurrentTick() % Speed != 0) return;
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
                if (DelayTime == Layer.GetCurrentTick() && !ForgotOnce)
                {
                    if (Rand.NextDouble() > 0.7)
                    {
                        Console.WriteLine($"{GetType().Name} {ID} Has forgotten an item and is heading back");
                        AgentForgotItem = true;
                        if(IsInGroup) UpdateGroupStatus();
                        ForgotOnce = true;
                        return;
                    }
                }
                if (!FoundDistressedAgent)
                {
                    if (AgentForgotItem)
                    {
                        if (ReturningWithGroupForItem)
                        {
                            if (IsLeader)
                            {
                                Evacuate();
                            }
                            else if (IsInGroup && !IsLeader)
                            {
                                MoveTowardsGroupLeader();
                            }
                        }
                        else
                        {
                            ReturnForItem();
                        }
                    }
                    else if (ReturningWithGroupForItem)
                    {
                        if (IsLeader)
                        {
                            Evacuate();
                           
                        }
                        else if (IsInGroup && !IsLeader)
                        {
                            MoveTowardsGroupLeader();
                           
                        }

                    }
                    else
                    {
                        if (IsLeader)
                        {
                            FormGroup(this);
                            Evacuate();
                        }
                        else if (IsInGroup && !IsLeader)
                        {
                            MoveTowardsGroupLeader();
                        }

                        else
                        {
                            Evacuate();
                        }
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
                UpdateHealthStatus();
            }
        }
    }

    #endregion
}