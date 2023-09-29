using System;
using System.Collections.Generic;
using Mars.Components.Agents;

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
                    Console.WriteLine($"{GetType().Name} {ID} is Unconscious");
                }
            }
            else
            {
                if (DelayTime < Layer.GetCurrentTick() && !ForgotOnce)
                {
                    if (Rand.NextDouble() > 0.7)
                    {
                        Console.WriteLine($"{GetType().Name} {ID} Has forgotten an item and is heading back");
                        AgentForgotItem = true;
                        ForgotOnce = true;
                        if(IsInGroup)ReturningWithGroupForItem = UpdateGroupStatus();
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
                                Console.WriteLine(
                                    $"{GetType().Name}  {ID} has moved to cell {Position} (Is returning for item and group is following)");
                            }
                            else if (IsInGroup && !IsLeader)
                            {
                                MoveTowardsGroupLeader();
                                Console.WriteLine(
                                    $" {GetType().Name} {ID} has moved to cell {Position} (Is returning for item with group)");
                            }
                        }
                        else
                        {
                            ReturnForItem();
                            Console.WriteLine(
                                $"{GetType().Name} {ID} has moved to cell {Position}(Is returning for item)");
                        }
                    }
                    else if (ReturningWithGroupForItem)
                    {
                        if (IsLeader)
                        {
                            EvacuateLow();
                            Console.WriteLine(
                                $"{GetType().Name}  {ID} has moved to cell {Position} (Group member forgot Item & Leader is returning with them)");
                        }
                        else if (IsInGroup && !IsLeader)
                        {
                            MoveTowardsGroupLeader();
                            Console.WriteLine(
                                $" {GetType().Name} {ID} has moved to cell {Position} (Group member forgot Item & Member is returning with them)");
                        }

                    }
                    else
                    {
                        if (IsLeader)
                        {
                            FormGroup(this);
                            EvacuateLow();
                            Console.WriteLine(Group.Count > 0
                                ? $"{GetType().Name}  {ID} has moved to cell {Position} (Is leading group)"
                                : $"{GetType().Name}  {ID} has moved to cell {Position} (Can lead group)");
                        }
                        else if (IsInGroup && !IsLeader)
                        {
                            MoveTowardsGroupLeader();
                            Console.WriteLine(
                                $" {GetType().Name} {ID} has moved to cell {Position} (Is moving in group)");
                        }

                        else
                        {
                            EvacuateLow();
                            Console.WriteLine($"{GetType().Name} {ID} has moved to cell {Position}  (Is moving alone)");

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
                                    Console.WriteLine(
                                    $"{GetType().Name}  {ID} has moved to cell {Position} (Is returning for item and group is following)");
                            }
                            else if (ReachedDistressedAgent)
                            {
                                OfferHelp();
                                Console.WriteLine(
                                    $"{GetType().Name}  {ID} has moved to cell {Position} (Is carrying agent  {Helped.ID} )");
                            }
                            else
                            {
                                EvacuateLow();
                                Console.WriteLine(
                                    $"{GetType().Name}  {ID} has moved to cell {Position} (Is returning to help {Helped.ID} With group)");
                            }
                        }
                        else
                        {
                            MoveTowardsGroupLeader();
                            Console.WriteLine($" {GetType().Name} {ID} has moved to cell {Position} (Is Helping {Helped.GetType().Name}  {Helped.ID} with group)");
                        }
                    }
                    else
                    {
                        if (Helping)
                        {
                            EvacuateLow();
                            Console.WriteLine($"{GetType().Name}{ID} has moved to cell {Position} (is carrying agent {Helped.ID})");
                        }
                        else
                        {
                            if (ReachedDistressedAgent)
                            {
                                OfferHelp();
                                Console.WriteLine($"{GetType().Name} {ID} Has reached at cell {Position} {Helped.GetType().Name} {Helped.ID} and is now heading exit");
                                Goal = FindNearestExit(Layer.PossibleGoal);
                            }
                            else
                            {
                                EvacuateLow();
                            }
                        }
                       
                    }
                }
                Consciousness();
            }
        }
    }

    #endregion
}



           

