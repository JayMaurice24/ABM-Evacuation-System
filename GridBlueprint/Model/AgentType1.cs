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
        Layer.ComplexAgentEnvironment.Insert(this);
        RiskLevel = Behaviour.LowRisk();
        Speed = Behaviour.LowSpeed();
        Pushiness = 0;
        FirstAct = false;
        Leadership = Rand.NextDouble();
        CollaborationFactor = Rand.NextDouble();
        Group = new List<ComplexAgent>();
        Leader = null;
        IsLeader = false;
        IsInGroup = false;
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
                    MakeAgentLead();
                }
                if (IsLeader)
                {
                        FormGroup(this);
                        MoveTowardsGoalLow();
                        Console.WriteLine($"Agent {GetType().GUID} is leading group");
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
            MoveRandomly();
        }
        
    }


    #endregion


}