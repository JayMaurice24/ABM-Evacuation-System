using System;
using System.Collections.Generic;

namespace GridBlueprint.Model;

/// <summary>
/// Agent With Low Risk, Low Speed and Low Aggression
/// </summary>
public class AgentType1 : ComplexAgent
{
    #region Init

    public override void Init(GridLayer layer)
    {
        Layer = layer;
        Position = Layer.FindRandomPosition();
        Directions = CreateMovementDirectionsList();
        RiskLevel = Behaviour.LowRisk();
        Speed = Behaviour.LowSpeed();
        Pushiness = 0;
        FirstAct = false;
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
        Layer.ComplexAgentEnvironment.Insert(this);
    }

    #endregion

    #region Tick

    public override void Tick()
    {
        if (Layer.Ring)
        {
            if (RiskLevel > TickCount)
            {
                if (IsConscious)
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
                            MoveTowardsGoalLow();
                            Console.WriteLine(Group.Count > 1
                                ? $"Agent {ID} is leading group"
                                : $"Agent {ID} can lead group");
                        }
                        else if (IsInGroup && !IsLeader)
                        {
                            MoveTowardsGroupLeader();
                            Console.WriteLine($"Agent {ID} moving in group");
                        }

                        else
                        {
                            MoveTowardsGoalLow();
                            Console.WriteLine($"Agent {ID} is moving alone");

                        }

                        HelpAgent();
                    }
                    else if (Helping)
                    {
                        MoveTowardsGoalLow();
                        Console.WriteLine($"Agent {ID} is carrying agent {Helped.ID}");
                    }
                    else
                    {
                        if (ReachedDistressedAgent)
                        {
                            GetExit();
                        }
                        else
                        {
                            MoveTowardsGoalLow();
                        }
                    }

                }
                else
                {
                    if (Position.Equals(Helper.Position))
                    {
                        FoundHelp = true;
                    }
                    else if (FoundHelp)
                    {
                        MovingWithHelp();
                    }
                }

                {
                    MoveRandomly();
                }
            }
            else
            {
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
}