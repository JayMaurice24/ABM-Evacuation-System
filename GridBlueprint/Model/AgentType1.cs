using System;

namespace GridBlueprint.Model;

/// <summary>
/// Agent With Low Risk, Low Speed and Low Aggression
/// </summary>
public class AgentType1 : ComplexAgent
{
    public new void Init(GridLayer layer)
    {
        base.Init(layer);
        RiskLevel = Behaviour.LowRisk();
        Speed = Behaviour.LowSpeed();
    }

    public new void Tick()
    {
        if (_layer.Ring)
        {
            var i = _random.Next(0, 2);
            _stairs = _layer.Stairs[i];
            _exit = FindNearestExit(_layer.Exits);
            var distStairs = CalculateDistance(Position, _stairs);
            var distExit = CalculateDistance(Position, _exit);
            Console.WriteLine("Agents moving towards exit");

            if (distExit < distStairs)
            {
                MoveTowardsGoal();
            }
            else
            {
                MoveStraightToExit();
            }
        }
        else
        {
            MoveRandomly();
        }
    }

}