using System;
using System.Collections.Generic;
namespace EvacuationSystem.Model;

/// <summary>
/// Agent With Low Risk, Low Aggression and Low Speed
/// </summary>
public class EvacueeType1 : Evacuee
{
    #region Init

    public override void Init(GridLayer layer)
    {
        Layer = layer;
        OriginalPosition = Layer.FindRandomPosition();
        Position = OriginalPosition;
        RiskLevel = Characteristics.LowRisk();
        Speed = Characteristics.LowSpeed();
        Aggression = 0;
        Leadership = Rand.NextDouble();
        Empathy = Rand.NextDouble();
        CollaborationFactor = Rand.NextDouble();
        Health = Rand.Next(30, 100);
        Strength = Rand.NextDouble();
        DelayTime = Rand.Next(30, 60);
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
                        if(IsInGroup) ReturningWithGroupForItem = UpdateGroupStatus();
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
                                EvacuateLow();
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
                            EvacuateLow();
                           
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
                            EvacuateLow();
                        }
                        else if (IsInGroup && !IsLeader)
                        {
                            MoveTowardsGroupLeader();
                        }

                        else
                        {
                            EvacuateLow();
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
                                EvacuateLow();
                            }
                            else if (ReachedDistressedAgent)
                            {
                                OfferHelp();
                            }
                            else
                            {
                                EvacuateLow();
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
                            EvacuateLow();
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
                                EvacuateLow();
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



           

