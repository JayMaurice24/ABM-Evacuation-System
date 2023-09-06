using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Mars.Interfaces.Environments;
using Mars.Numerics;

namespace GridBlueprint.Model;

public abstract class SocialForceModel
{
    public static Vector2 CalculateSocialForce(ComplexAgent leader, ComplexAgent agent, List <ComplexAgent> group)
    {
        var totalForce = Vector2.Zero;
        var attractiveForce = Vector2.Normalize(new Vector2((float)(leader.Position.X - agent.Position.X),
                    (float)(leader.Position.Y - agent.Position.Y))) * (float)AttractiveForceMultiplier;

        totalForce += attractiveForce;

        return (from groupMember in @group where groupMember != agent select Vector2.Normalize(new Vector2((float)(agent.Position.X - groupMember.Position.X), (float)(agent.Position.Y - groupMember.Position.Y))) * RepulsiveForceMultiplier / (float)CalculateDistance(agent.Position, groupMember.Position)).Aggregate(totalForce, (current, repulsiveForce) => current + repulsiveForce);
    }

    public static Vector2 CalculateObstacleAvoidanceForce(ComplexAgent agent)
    {
        var obstacleAvoidanceForce = Vector2.Zero;

        var nearestObstacle = GetNearestObstacle(agent);

        if (nearestObstacle == null) return obstacleAvoidanceForce;
        var awayFromObstacle = Vector2.Normalize(new Vector2((float)(agent.Position.X - nearestObstacle.Position.X),
                                   (float)(agent.Position.Y - nearestObstacle.Position.Y)))
                               * ObstacleAvoidanceMultiplier
                               / (float)CalculateDistance(agent.Position, nearestObstacle.Position);
        obstacleAvoidanceForce += awayFromObstacle;

        return obstacleAvoidanceForce;
    }

    private static ComplexAgent GetNearestObstacle(ComplexAgent agent)
    {
        ComplexAgent nearestObstacle = null;
        var shortestDistance = double.MaxValue;

        foreach (var otherAgent in Layer.ComplexAgentEnvironment.Entities)
        {
            if (agent == otherAgent) continue; // Assuming you have an IsObstacle property in ComplexAgent
            var distance = CalculateDistance(agent.Position, otherAgent.Position);
            if (!(distance < shortestDistance)) continue;
            shortestDistance = distance;
            nearestObstacle = agent;
        }

        return nearestObstacle;
    }

    private static double CalculateDistance(Position coords1, Position coords2)
    {
        return Distance.Chebyshev(new[] { coords1.X, coords1.Y }, new[] { coords2.X, coords2.Y }); 

    }

    public static GridLayer Layer;
    private static readonly Random Rand = new();
    private static readonly double AttractiveForceMultiplier = Rand.NextDouble();
    private static readonly int RepulsiveForceMultiplier = Rand.Next(1, 10);
    private static readonly int ObstacleAvoidanceMultiplier = Rand.Next(1, 10);
}