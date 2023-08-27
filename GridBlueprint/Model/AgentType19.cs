using System;

namespace GridBlueprint.Model;

/// <summary>
/// Agent With High Risk, Low Speed and Low Aggression
/// </summary>
public class AgentType19: ComplexAgent
{
    #region Init

    public override void Init(GridLayer layer)
    {
        Layer = layer;
        Position = Layer.FindRandomPosition();
        Directions = CreateMovementDirectionsList(); 
        Layer.ComplexAgentEnvironment.Insert(this);
        RiskLevel = Behaviour.HighRisk();
        Speed = Behaviour.LowSpeed();
        Pushiness = 0; 
    }
    
    #endregion

    #region Tick

    public override void Tick()
    {
        if (Layer.Ring)
        {
            var i = Random.Next(0, 2);
            Stairs = Layer.Stairs[i];
            Exit = FindNearestExit(Layer.Exits);
            var distStairs = CalculateDistance(Position, Stairs);
            var distExit = CalculateDistance(Position, Exit);
            Console.WriteLine("Agents moving towards exit");
            if (RiskLevel > TickCount)
            {
                if (TickCount % Speed != 0) return;
                if (distExit < distStairs)
                {
                    MoveTowardsGoalLow();
                }
                else
                {
                    MoveStraightToExitLow();
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