using System;

namespace GridBlueprint.Model;


/// <summary>
/// Agent With Low Risk, High Speed and High Aggression 
/// </summary>
/// 
public class AgentType9 : ComplexAgent
{
    #region Init

    public override void Init(GridLayer layer)
    {
        Layer = layer;
        Position = Layer.FindRandomPosition();
        Directions = CreateMovementDirectionsList(); 
        Layer.ComplexAgentEnvironment.Insert(this);
        RiskLevel = Behaviour.LowRisk();
        Speed = Behaviour.HighSpeed();
        Pushiness = 0; 
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
                    Console.WriteLine($"Agent moving towards exit");
                    FirstAct = true;
                }
                if (IsInGroup)
                {
                    if (IsLeader)
                    {
                        MoveTowardsGoalLow();
                    }
                    else
                    {
                        MoveTowardsGroupLeader();
                    }
                    
                    Console.WriteLine($"Agent moving in group");
                }
                else
                {
                    if (TickCount % Speed != 0) return;
                    MoveTowardsGoalLow();
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