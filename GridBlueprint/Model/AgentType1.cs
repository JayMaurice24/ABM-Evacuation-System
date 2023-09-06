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
                if (!FirstAct)
                {
                    Exit = FindNearestExit(Layer.Exits);
                    Stairs = ClosestStairs(Layer.Stairs);
                    var distStairs = CalculateDistance(Position, Stairs);
                    var distExit = CalculateDistance(Position, Exit);
                    Goal = distExit < distStairs ? Exit : Stairs;
                    Console.WriteLine($"Agent {GetType().GUID} moving towards exit");
                    FirstAct = true;
                    if(!IsLeader && !IsInGroup) MakeAgentLead();
                }
                if (IsLeader)
                {
                        FormGroup(this);
                        MoveTowardsGoalLow();
                        Console.WriteLine(Group.Count > 1
                            ? $"Agent {GetType().GUID} is leading group"
                            : $"Agent {GetType().GUID} can lead group");
                }
                else if (IsInGroup && !IsLeader)
                {
                        MoveTowardsGroupLeader();
                        Console.WriteLine($"Agent {GetType().GUID} moving in group");
                }
                    
                else
                {
                    if (TickCount % Speed != 0) return;
                    MoveTowardsGoalLow();
                    Console.WriteLine($"Agent {GetType().GUID} is moving alone");
                    
                }
                
            }
            else
            {
                MoveRandomly();
            }
        }
        else
        {
            switch (IsLeader)
            {
                case false when !IsInGroup:
                    MakeAgentLead();
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