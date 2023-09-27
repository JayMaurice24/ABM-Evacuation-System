using System;
using System.Collections.Generic;

namespace EvacuationSystem.Model;

/// <summary>
/// Agent With Medium Risk, High Aggression and High Speed 
/// </summary>
public class EvacueeType12: Evacuee
{
    #region Init

    public override void Init(GridLayer layer)
    {
        Layer = layer;
        OriginalPosition = Layer.FindRandomPosition();
        Position = OriginalPosition;
        RiskLevel = Characteristics.MediumRisk();
        Speed = Characteristics.HighSpeed();
        Aggression = 2;
        FoundExit = false;
        Leadership = Rand.NextDouble();
        Empathy = Rand.NextDouble();
        CollaborationFactor = Rand.NextDouble();
        Health = Rand.Next(30, 100);
        DelayTime = Rand.Next(30, 60);
        Strength = Rand.NextDouble();
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
        if (!Layer.Ring && RiskLevel < Layer.GetCurrentTick()) return;
        if (!FoundExit)
        {
            Goal = FindNearestExit(Layer.PossibleGoal);
            Console.WriteLine($"Agent {ID} moving towards exit");
            FoundExit = true;
            DetermineLeader();
        }
        else
        {
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
                if (DelayTime < Layer.GetCurrentTick())
                {
                    if(Rand.NextDouble() > 0.7){
                        AgentForgotItem = true;
                        Console.WriteLine($"{GetType().Name} {ID} Has forgotten an item and is heading back");
                        
                    }
                    
                }
                if (!FoundDistressedAgent)
                {
                    if (AgentForgotItem)
                    {
                        ReturnForItem();
                    }
                    else
                    {
                        if (IsLeader)
                        {
                            FormGroup(this);
                            MoveTowardsGoalHigh();
                            Console.WriteLine(Group.Count > 1
                                ? $"{GetType().Name}  {ID} is leading group"
                                : $"{GetType().Name}  {ID} can lead group");
                        }
                        else if (IsInGroup && !IsLeader)
                        {
                            MoveTowardsGroupLeader();
                            Console.WriteLine($" {GetType().Name} Agent {ID} moving in group");
                        }

                        else
                        {
                            MoveTowardsGoalHigh();
                            Console.WriteLine($" {GetType().Name} Agent {ID} is moving alone");

                        }
                    }

                    HelpAgent();
                }
                else if (Helping)
                {
                    MoveTowardsGoalLow();
                    Console.WriteLine($"{GetType().Name} t {ID} is carrying agent {Helped.ID}");
                }
                else
                {
                    if (ReachedDistressedAgent)
                    {
                        OfferHelp();
                        Console.WriteLine($"{GetType().Name} {ID} Has reached {Helped.GetType().Name} {Helped.ID} and is moving towards exit");
                        Goal = FindNearestExit(Layer.PossibleGoal);
                    }
                    else
                    {
                        MoveTowardsGoalLow();
                    }
                }

                Consciousness();
            }
        }
    }
    #endregion
}
