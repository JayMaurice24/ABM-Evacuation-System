using System;
using System.Collections.Generic;

namespace GridBlueprint.Model;

/// <summary>
/// Agent With Medium Risk,  Medium Aggression and Low Speed
/// </summary>
/// 
public class AgentType8 : ComplexAgent
{
    #region Init

     public override void Init(GridLayer layer)
    {
        Layer = layer;
        Position = Layer.FindRandomPosition();
        Directions = CreateMovementDirectionsList();
        RiskLevel = Characteristics.MediumRisk();
        Speed = Characteristics.LowSpeed();
        Aggression = 1;
        FoundExit = false;
        Leadership = Rand.NextDouble();
        Empathy = Rand.NextDouble();
        CollaborationFactor = Rand.NextDouble();
        Health = Rand.Next(30, 100);
        Strength = Rand.NextDouble();
        IsConscious = true;
        IsLeader = false;
        IsInGroup = false;
        Group = new List<ComplexAgent>();
        Leader = null;
        Helper = null;
        Helped = null; 
        Layer.ComplexAgentEnvironment.Insert(this);
    }

    #endregion

    #region Tick

    public override void Tick()
    {
        if (Layer.Ring)
        {
            if (RiskLevel <= TickCount) return;
            switch (IsConscious)
            {
                case true:
                {
                    if (!FoundExit)
                    {
                        GetExit();
                        if (!IsLeader && !IsInGroup) DetermineLeader();
                    }

                    if (!FoundDistressedAgent)
                    {
                        if (IsLeader)
                        {
                            FormGroup(this);
                            MoveTowardsGoalMedium();
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
                            MoveTowardsGoalMedium();
                            Console.WriteLine($" {GetType().Name} Agent {ID} is moving alone");

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
                            GetExit();
                        }
                        else
                        {
                            MoveTowardsGoalMedium();
                        }
                    }

                    Consciousness();
                    break;
                }
                case false when FoundHelp:
                    MovingWithHelp();
                    break;
                case false:
                    Console.WriteLine($"{GetType().Name} {ID} is Unconscious");
                    break;
            }
        }
        else {
                    switch (IsLeader)
                    {
                        case false when !IsInGroup:
                            DetermineLeader();
                            ;
                            MoveRandomly();
                            break;
                        case true:
                            MoveRandomly();
                            FormGroup(this);
                            break;
                        default:
                        {
                            if (IsInGroup && !IsLeader)
                            {
                                MoveTowardsGroupLeader();
                            }

                            break;
                        }
                    }

                    Console.WriteLine($"Agent {GetType().GUID} moving randomly");
        }

    }


            #endregion

}