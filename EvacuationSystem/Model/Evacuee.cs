using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Common;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;
using Mars.Numerics;

namespace EvacuationSystem.Model;

public class Evacuee : IAgent<GridLayer>, IPositionable
{
    #region Init

    /// <summary>
    ///     The initialization method of the ComplexAgent which is executed once at the beginning of a simulation.
    ///     It sets an initial Position and an initial State and generates a list of movement directions.
    /// </summary>
    /// <param name="layer">The GridLayer that manages the agents</param>
    public virtual void Init(GridLayer layer)
    { 
        Layer = layer;

    }

    #endregion

    #region Tick

    /// <summary>
    ///     The tick method of the ComplexAgent which is executed during each time step of the simulation.
    ///     A ComplexAgent can move randomly along straight lines. It must stay within the bounds of the GridLayer
    ///     and cannot move onto grid cells that are not routable.
    /// </summary>
    public virtual void Tick()
    {
        
    }

    #endregion

    #region Methods
    /// <summary>
     /// Finds the exit closest to the agent  
     /// </summary>
   public Position FindNearestExit(List<Position> targets)
    {
        Position nearestExit = null; 
        var shortestDistance = double.MaxValue;
        foreach (var p in targets)
        {
            var distance = CalculateDistance(Position, p);
            if (!(distance < shortestDistance)) continue;
            shortestDistance = distance;
            nearestExit = p;
        }

        return nearestExit;
    }
    /// <summary>
    /// agent with leadership skills forms a group
    /// </summary>
    public void FormGroup()
    {
        if (!IsLeader) return;
        var potentialGroupMembers = Layer.EvacueeEnvironment.Explore(Position, radius: 3).ToList();
        if (potentialGroupMembers.Count <= 0) return;
        foreach (var agent in potentialGroupMembers.Where(agent => agent.CollaborationFactor > 0.5 && !agent.IsInGroup && !agent.IsLeader && agent!=this && !Group.Contains(agent)))
        {
            agent.IsInGroup = true;
            agent.Leader = this;
            Group.Add(agent);
            ModelOutput.NumInGroup++;
            Console.WriteLine($"{agent.GetType().Name} {agent.ID} has joined {GetType().Name} {ID}'s Group");
        }

        if (!GroupFormed && Group.Count > 1)
        {
            ModelOutput.NumberOfGroups++;
            GroupFormed = true;
        }
    }
    /// <summary>
    /// Identifies if an agent can lead a group
    /// </summary>
    public void DetermineLeader()
    {
        var surroundingAgents = Layer.EvacueeEnvironment.Explore(Position, radius: 5).ToList();
        if(surroundingAgents.Count < 2) return;
        var potentialLeaders = surroundingAgents.Where(agent => !agent.IsInGroup || !agent.IsLeader).ToList();
        if (!potentialLeaders.Any()) return;
        var leader = potentialLeaders.OrderByDescending(agent => agent.Leadership).First();
        if (leader != this) return;
        IsLeader = true;
        IsInGroup = true;
        ModelOutput.NumInGroup++;
    }

    /// <summary>
    /// Calculates the distance between two coordinates 
    /// </summary>
    private double CalculateDistance(Position coords1, Position coords2)
    {
        return Distance.Chebyshev(new[] { coords1.X, coords1.Y }, new[] { coords2.X, coords2.Y }); 

    }

    /// <summary>
    /// Changes the agent's travel direction when they're in close proximity to the fire 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Position ChangeDirection(Position position)
    {
        foreach (var next in Layer.Directions)
        {
            var newX = Position.X + next.X;
            var newY = Position.Y + next.Y;
            var nextCell = new Position(newX, newY);

            if (!Layer.IsRoutable(newX, newY) || AvoidFire(nextCell)) continue;
            if (IsInGroup && !IsLeader) return nextCell;
            // If the new direction is opposite to the original heading (180-degree turn), change goals to the south exit
            if ((int)PositionHelper.CalculateBearingCartesian(Position.X, Position.Y, newX, newY) != 180)
                return nextCell;
            if (Layer.FrontStairs.Contains(Goal))
            {
                Goal = FindNearestExit(Layer.BackStairs);
            }
            else if (Layer.BackStairs.Contains(Goal))
            {
                Goal = FindNearestExit(Layer.FrontStairs);
            }
            else
            {
                Goal = FindNearestExit(Layer.Exits);
            }

            return nextCell;
        }

        return position;
    }
    /// <summary>
    /// Checks if the agent is in close proximity to the fire
    /// </summary>
    /// <returns></returns>
    public bool AvoidFire(Position target)
    {
        return Layer.FireLocations.Contains(target);
    }
    

    public bool Perception(Position agent1, Position agent2)
    {
        switch (agent1.X)
        {
            case > 1 and < 19 when agent1.Y is > 50 and < 87 && agent2.X is > 1 and < 19 && agent2.Y is > 50 and < 87: //Coral
            case > 21 and < 54 when agent1.Y is > 52 and < 84 && agent2.X is > 21 and < 54 && agent2.Y is > 52 and < 84: //West 
            case > 65 and < 100 when agent1.Y is > 52 and < 84 && agent2.X is > 65 and < 100 && agent2.Y is > 52 and < 84: //East
            case > 102 and < 120 when agent1.Y is > 42 and < 87 && agent2.X is > 102 and < 120 && agent2.Y is > 42 and < 87: //Braae
            case > 44 and < 120 when agent1.Y is > 1 and < 15 && agent2.X is > 44 and < 120  && agent2.Y is > 1 and < 15: //CSHons 
            case > 77 and < 86 when agent1.Y is > 41 and < 45 && agent2.X is > 77 and < 86 && agent2.Y is > 41 and < 45: //FBathroom
            case > 86 and < 94 when agent1.Y is > 38 and < 45 && agent2.X is > 86 and < 94 && agent2.Y is > 38 and < 45: //FBathroom2
            case > 77 and < 86 when agent1.Y is > 32 and < 36 && agent2.X is > 77 and < 86 && agent2.Y is > 32 and < 36: //MBathroom 
            case > 86 and < 94 when agent1.Y is > 32 and < 38 && agent2.X is > 86 and < 94 && agent2.Y is > 32 and < 38: //MBathroom2 
            case > 44 and < 63 when agent1.Y is > 15 and < 32 && agent2.X is > 44 and < 63 && agent2.Y is > 15 and < 32: //Atrium
            case > 96 and < 120 when agent1.Y is > 16 and < 32 && agent2.X is > 96 and < 120 && agent2.Y is > 16 and < 32: //SysDev
            case > 14 and < 102 when agent1.Y is > 46 and < 50 && agent2.X is > 14 and < 102 && agent2.Y is > 46 and < 50: //Passage1
            case > 50 and < 53 when agent1.Y is > 51 and < 70 && agent2.X is > 50 and < 53 && agent2.Y is > 51 and < 70: //Passage2
            case > 56 and < 65 when agent1.Y is > 45 and < 68 && agent2.X is > 56 and < 65 && agent2.Y is > 45 and < 68: //Passage3
            case > 63 and < 94 when agent1.Y is > 26 and < 31 && agent2.X is > 63 and < 94 && agent2.Y is > 26 and < 31: //Passage4
            case > 63 and < 94 when agent1.Y is > 15 and < 19 && agent2.X is > 63 and < 94 && agent2.Y is > 15 and < 19: //Passage5
            case > 63 and < 73 when agent1.Y is > 19 and < 31 && agent2.X is > 63 and < 94 && agent2.Y is > 19 and < 31: //Passage6
                if (Layer.SeeFire) return true;
                Console.WriteLine($"{GetType().Name} {ID} has spotted the fire and alerted everyone else");
                Layer.SeeFire = true;
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Push other agents out the way
    /// </summary>
    /// <param name="otherAgent"></param>
    public void PushAgent(Evacuee otherAgent)
        {
            // Calculate the direction to push the other agent
            var pushDirectionX = otherAgent.Position.X - Position.X;
            var pushDirectionY = otherAgent.Position.Y - Position.Y;

            // Calculate the new position for the other agent after the push
            var newAgentX = otherAgent.Position.X + pushDirectionX;
            var newAgentY = otherAgent.Position.Y + pushDirectionY;

            // Check if the new position is within the bounds of the grid
            if (!(0 <= newAgentX) || !(newAgentX < Layer.Width) ||
                !(0 <= newAgentY) || !(newAgentY < Layer.Height)) return;
            // Check if the new position is routable
            if (!Layer.IsRoutable(newAgentX, newAgentY)) return;
            // Move the other agent to the new position
            otherAgent.Position = new Position(newAgentX, newAgentY);
            Layer.EvacueeEnvironment.MoveTo(otherAgent, new Position(newAgentX, newAgentY));
            ModelOutput.NumberOfAgentsPushed++;
        }

    /// <summary>
    /// Retrieves an agent based off proximity
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
  

    private Evacuee FindAgentsInNeed()
    {
        var nearbyAgents = Layer.EvacueeEnvironment.Explore(Position, radius: 10).ToList();
        return nearbyAgents.FirstOrDefault(agent => !agent.IsConscious && Perception(Position, agent.Position));
    }
    public void HelpAgent()
    {
        if (Health <= 50 || !(Empathy > 0.5) || !(Strength > 0.5)) return;
        var helped = FindAgentsInNeed();
        if (helped is not { Helper: null }) return;
        if (Layer.IsRoutable(helped.Position.X + 1, helped.Position.Y))
        {
            Goal = new Position(helped.Position.X + 1, helped.Position.Y);
        }
        else if (Layer.IsRoutable(helped.Position.X - 1, helped.Position.Y))
        {
            Goal = new Position(helped.Position.X + 1, helped.Position.Y);
        }
        else if (Layer.IsRoutable(helped.Position.X, helped.Position.Y + 1))
        {
            Goal = new Position(helped.Position.X, helped.Position.Y + 1);
        }
        else
        {
            Goal = new Position(helped.Position.X, helped.Position.Y - 1);
        }
        FoundDistressedAgent = true;
        if (IsInGroup)
        {
            var group = new HandleGroup(this, Layer);
            group.Update();
            switch (ReturningWithGroupToHelp)
            {
                case true when IsLeader:
                    Helped = helped;
                    helped.Helper = this;
                    foreach (var agent in Group)
                    {
                        agent.Helped = helped;
                    }
                    break;
                case true:
                {
                    foreach (var agent in Leader.Group)
                    {
                        agent.Helped = helped;
                    }

                    break;
                }
                default:
                    Helped = helped;
                    helped.Helper = this;
                    break;
            }
        }
        else
        {
            Helped = helped;
            helped.Helper = this;
        }

    }
    public void OfferHelp()
    {
        if (Layer.EvacueeEnvironment.Entities.Contains(Helped))
        {
            Helped.FoundHelp = true;
            Helping = true;
            ModelOutput.NumFoundHelp++;
        }
        else
        {
            Console.WriteLine($"{Helped.GetType().Name} {Helped.ID} Has been found dead");
            FoundDistressedAgent = false;
            Helped = null;
        }
    }

    public void UpdateHealthStatus()
    {
        if (Health <= 5 && IsConscious)
        {
            IsConscious = false;
            ModelOutput.NumUnconscious++;
        }

        if (Health > 0) return;
        Console.WriteLine($"{GetType().Name} {ID} Has been killed in the simulation");
        Layer.RemoveFromSimulation(this);
        ModelOutput.NumDeaths++;
        ModelOutput.NumAgentsLeft--;

    }

    #endregion

    #region Fields and Properties
    public Guid ID { get; set; }
    public Position Position { get; set; }
    public bool ForgotAnItem { get; set; }
    protected static GridLayer Layer;
    public Position Goal { get; set; }
    protected static readonly Random Rand = new();
    public int RiskLevel { get; set;}
    public List<Evacuee> Group { get; set; }
    public int Aggression { get; set; }
    public int Health { get; set; }
    public bool IsLeader { get; set; }
    public int Speed { get; set; }
    public bool Helping { get; set; }
    public int DelayTime { get; set; }
    public bool LeaderHasReachedExit { get; set; }
    public Position OriginalPosition { get; set;}
    public bool FoundHelp { get; set; }
    public Evacuee Leader { get; set; }
    public Evacuee Helper { get; set; }
    public Evacuee Helped { get; set; }
    public bool IsInGroup;
    public bool FoundExit;
    public bool AgentReturningForItem { get; set; }
    public bool ReturningWithGroupForItem { get; set; }
    public bool ReturningWithGroupToHelp { get; set; }
    public bool ReachedDistressedAgent { get; set; }
    public bool FoundDistressedAgent { get; set; }
    protected double CollaborationFactor { get; set; }
    public double Leadership { get; set; }
    private bool GroupFormed { get; set; }
    public bool IsConscious { get; set; }
    protected HandleAgentMovement Movement; 
    public bool EvacueeHasStartedMoving { get; set; }
    public double Strength { get; set; }
    public double Empathy { get; set; }

    #endregion
}