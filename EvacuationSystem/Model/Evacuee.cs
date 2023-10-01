using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Mars.Common;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;
using Mars.Numerics;
using Microsoft.CodeAnalysis.Operations;

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
   /// Agents Move towards goal with Low aggression 
   /// </summary>
    protected void Evacuate()
    {
        if (!_tripInProgress)
        {
            // Finds closest exit and moves towards exit 
            _path = Layer.FindPath(Position, Goal).GetEnumerator();
            _tripInProgress = true;
            
        }
        if (!_path.MoveNext()) return;
        if (AvoidFire(_path.Current))
        {
            var next = ChangeDirection(_path.Current);
            Layer.EvacueeEnvironment.MoveTo(this, next, 1);
            _path = Layer.FindPath(Position, Goal).GetEnumerator();
            return;
        }
        if (IsCellOccupied(_path.Current)){
            var otherAgent = GetAgentAt(Position);
            switch (Aggression)
            { 
                case 1:
                    
                    if (otherAgent != null)
                    { if (otherAgent.Aggression <= Aggression && otherAgent.Strength < Strength)
                        {
                            PushAgent(otherAgent);
                            Console.WriteLine($"Agent {otherAgent.ID} has been pushed by {ID}");
                        }
                    }
                    break;

                case 2:
                    if (otherAgent != null)
                    {
                        PushAgent(otherAgent);
                        Console.WriteLine($"Agent {otherAgent.ID} has been pushed by {ID}");
                    }
                    break;

                default:
                    return; 
            }
            Layer.EvacueeEnvironment.MoveTo(this, _path.Current,1);
        }
        else
        {
            Layer.EvacueeEnvironment.MoveTo(this, _path.Current,1);
        }
        if (IsLeader)
        {
            switch (ReturningWithGroupForItem)
            {
                case true when AgentForgotItem:
                    Console.WriteLine($"{GetType().Name}  {ID} has moved to cell {Position} (Is returning for item and group is following)");
                    break;
                case true:
                    Console.WriteLine($"{GetType().Name}  {ID} has moved to cell {Position} (Group member forgot Item & Leader is returning with them)");
                    break;
                default:
                {
                    if(ReturningWithGroupToHelp)
                    {
                        if (Helping)
                        {
                            Console.WriteLine($"{GetType().Name}  {ID} has moved to cell {Position} (Is returning for item and group is following)");
                        }
                        else if (ReachedDistressedAgent)
                        {
                            Console.WriteLine($"{GetType().Name}  {ID} has moved to cell {Position} (Is carrying agent  {Helped.ID} )");
                        }
                        else
                        {
                            Console.WriteLine($"{GetType().Name}  {ID} has moved to cell {Position} (Is returning to help {Helped.ID} With group)");
                        }
                    }
                    else
                    { if (Group.Count == 0)
                        {
                            Console.WriteLine($"{GetType().Name}  {ID} has moved to cell {Position} (Is Moving alone)");
                            IsLeader = false;
                            IsInGroup = false;
                        }
                        else
                        {
                            Console.WriteLine($"{GetType().Name}  {ID} has moved to cell {Position} (is leading group)");
                        }
                    }

                    break;
                }
            }
        }
        else
        {
            if (AgentForgotItem)
            {
                Console.WriteLine($"{GetType().Name} {ID} has moved to cell {Position}(Is returning for item)");
            }
            else if (FoundDistressedAgent)
            {
                if (Helping)
                {
                    Console.WriteLine($"{GetType().Name}{ID} has moved to cell {Position} (is carrying agent {Helped.ID})");
                }
                else if (ReachedDistressedAgent)
                {
                    Console.WriteLine($"{GetType().Name} {ID} Has reached at cell {Position} {Helped.GetType().Name} {Helped.ID} and is now heading exit");
                }
                else
                {
                    Console.WriteLine($"{GetType().Name} {ID} Has moved to cell {Position} and is going to help {Helped.GetType().Name} {Helped.ID}");
                }
            }
            else
            {
                Console.WriteLine($"{GetType().Name} {ID} has moved to cell {Position}  (Is moving alone)");
            }
        }
        if (!Position.Equals(Goal)) return;
        if (AgentForgotItem && Goal.Equals(OriginalPosition))
        {
            Console.WriteLine($"{this.GetType().Name} {ID} has Found Item and is heading to the exit");
            AgentForgotItem = false;
            _tripInProgress = false;
            Goal = FindNearestExit(Layer.PossibleGoal);
            _path = Layer.FindPath(Position, Goal).GetEnumerator();
        }
        else if (Layer.Stairs.Contains(Goal))
        {
            Console.WriteLine($"{this.GetType().Name} {ID} has reached exit {Goal}");
            Layer.RemoveFromSimulation(this);
        }
        else
        {
            Goal = FindNearestExit(Layer.Stairs); 
            _path = Layer.FindPath(Position, Goal).GetEnumerator();
           
        }

    }
   /// <summary>
     /// Finds the exit closest to the agent  
     /// </summary>

protected Position FindNearestExit(List<Position> targets)
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
    /// <param name="leader"></param>
    protected void FormGroup()
    {
        if (!IsLeader) return;
        var potentialGroupMembers = Layer.EvacueeEnvironment.Explore(Position, radius: 5).ToList();
        if (potentialGroupMembers.Count <= 0) return;
        foreach (var agent in potentialGroupMembers.Where(agent => agent.CollaborationFactor > 0.5 && !agent.IsInGroup && !agent.IsLeader && agent!=this))
        {
            agent.IsInGroup = true;
            agent.Leader = this;
            Group.Add(agent);
        }

    }
    /// <summary>
    /// Identifies if an agent can lead a group
    /// </summary>
    protected void DetermineLeader()
    {
        var surroundingAgents = Layer.EvacueeEnvironment.Explore(Position, radius: 5).ToList();
        if(!(surroundingAgents.Count > 1)) return;
        var leader = surroundingAgents.OrderByDescending(agent => agent.Leadership).First();
        if (leader != this) return;
        IsLeader = true;
        IsInGroup = true;
    }
 

    private void UpdateVelocity(Vector2 netForce)
    {
        // Update velocity using net force
        _currentVelocity += netForce;

        // Limit the velocity to a maximum value (MaxSpeed)
        if (_currentVelocity.Length() > MaxSpeed)
        {
            _currentVelocity = Vector2.Normalize(_currentVelocity) * MaxSpeed;
        }
    }
    /// <summary>
    /// Agent follows group leader
    /// </summary>
    protected void MoveTowardsGroupLeader()
    {
        if (!IsInGroup || IsLeader) return;
        var socialForce = SocialForceModel.CalculateSocialForce(Leader, this, Leader.Group);
        var nearestObstacle = GetNearestObstacle();
        var obstacleAvoidanceForce = SocialForceModel.CalculateObstacleAvoidanceForce(this, nearestObstacle);
        var netForce = socialForce + obstacleAvoidanceForce;

        UpdateVelocity(netForce);

        // Update position based on new velocity
        var newX = Math.Round(Position.X + _currentVelocity.X, MidpointRounding.AwayFromZero);
        var newY = Math.Round(Position.Y + _currentVelocity.Y, MidpointRounding.AwayFromZero);

        if (!(0 <= newX) || !(newX < Layer.Width) || !(0 <= newY) || !(newY < Layer.Height)) return;
        if (!Layer.IsRoutable(newX, newY) || IsCellOccupied(new Position(newX, newY))) return;
        if (AvoidFire(_path.Current))
        {
            var target = ChangeDirection(new Position(newX, newY));
            Layer.EvacueeEnvironment.MoveTo(this, target, 1);
            Position = target;
        }
        else
        {
            var target = new Position(newX, newY);
            Layer.EvacueeEnvironment.MoveTo(this, target, 1);
            Position = target;
        }

        if (ReturningWithGroupForItem)
        {
            Console.WriteLine(AgentForgotItem
                ? $" {GetType().Name} {ID} has moved to cell {Position} (Is returning for item with group)"
                : $"{GetType().Name} {ID} has moved to cell {Position} (Group member forgot Item & Member is returning with them)");
        }
        else if (ReturningWithGroupToHelp)
        {
            Console.WriteLine($"{GetType().Name} {ID} has moved to cell {Position} (Is Helping {Helped.GetType().Name}  {Helped.ID} with group)");
        }
        else
        {
            Console.WriteLine($"{GetType().Name} {ID} has moved to cell {Position} (Is moving in group)");
        }
     
    }
    
    /// <summary>
    /// Calculates the distance between two coordinates 
    /// </summary>
    private double CalculateDistance(Position coords1, Position coords2)
    {
        return Distance.Chebyshev(new[] { coords1.X, coords1.Y }, new[] { coords2.X, coords2.Y }); 

    }
    private Evacuee GetNearestObstacle()
    {
        var nearest = Layer.EvacueeEnvironment.Entities.MinBy(agent =>
            Distance.Chebyshev(Position.PositionArray, agent.Position.PositionArray));

        if (nearest != this && nearest != null) return nearest;
        return null;
    }
    /// <summary>
    /// Changes the agent's travel direction when they're in close proximity to the fire 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private Position ChangeDirection(Position position)
    {
        var fire = Layer.FireEnvironment.Entities.MinBy(flame =>
            Distance.Chebyshev(new[] { Position.X, Position.Y }, new[] { flame.Position.X, flame.Position.Y }));

        if (fire == null) return position;

        if (!position.Equals(fire.Position))
        {
            return position;
        }
        foreach (var next in Layer.Directions)
        {
            var newX = Position.X + next.X;
            var newY = Position.Y + next.Y;
            var nextCell = new Position(newX, newY);

            if (!(0 <= newX) || !(newX < Layer.Width) || !(0 <= newY) || !(newY < Layer.Height) ||
                !Layer.IsRoutable(newX, newY) || !Layer.FireLocations.Contains(nextCell)) continue;
            // If the new direction is opposite to the original heading (180-degree turn), change goals to the south exit
            if ((int)PositionHelper.CalculateBearingCartesian(Position.X, Position.Y, newX, newY) == 180)
            {
                if (Layer.Exits.Contains(Goal))
                {
                    foreach (var ex in Layer.Exits.Where(ex => !Goal.Equals(ex)))
                    {
                        Goal = ex;
                        break;
                    }
                }
                else if (Layer.FrontStairs.Contains(Goal))
                {
                    Goal = FindNearestExit(Layer.BackStairs);
                }
                else if (Layer.BackStairs.Contains(Goal))
                {
                    Goal = FindNearestExit(Layer.FrontStairs);
                }
                else
                {
                    Goal = FindNearestExit(Layer.PossibleGoal);
                }
                
            }
            return nextCell;
        }
        return position;
    }
    /// <summary>
    /// Checks if the agent is in close proximity to the fire
    /// </summary>
    /// <returns></returns>
    private bool AvoidFire(Position target)
    {
        return Layer.FireLocations.Contains(target);
    }
    
    
    /// <summary>
    /// Checks if the next cell is occupied by an agent
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    private bool IsCellOccupied(Position targetPosition)
    {
        var agents = Layer.EvacueeEnvironment.Entities.MinBy(agent =>
            Distance.Chebyshev(new[] { Position.X, Position.Y }, new[] { agent.Position.X, agent.Position.Y }));

        return agents != null && targetPosition.Equals(agents.Position);
    }
    
    protected bool Perception(Position agent1, Position agent2)
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

protected void ReturnForItem()
{
    if (!Returning){
        Goal = OriginalPosition;
        Returning = true;
    }
    Evacuate();
    }

    /// <summary>
    /// Push other agents out the way
    /// </summary>
    /// <param name="otherAgent"></param>
    private void PushAgent(Evacuee otherAgent)
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
        }

    /// <summary>
    /// Retrieves an agent based off proximity
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private Evacuee GetAgentAt(Position position)
    {
        return Layer.EvacueeEnvironment.Entities.FirstOrDefault(agent => agent != this && agent.Position.Equals(position));
    }

    private Evacuee FindAgentsInNeed()
    {
        var nearbyAgents = Layer.EvacueeEnvironment.Explore(Position, radius: 10).ToList();
        foreach(var agent in nearbyAgents)
        {
            if (!agent.IsConscious && Perception(Position, agent.Position))
            {
                return agent; 
            }
        }

        return null; 
    }

    protected void MovingWithHelp()
    {
        Position = Helper.Position;
        Layer.EvacueeEnvironment.MoveTo(this, new Position(Helper.Position.X, Helper.Position.Y));
    }
    protected void HelpAgent()
    {
        if (Health <= 50 || !(Empathy > 0.5) || !(Strength > 0.5)) return;
        var helped = FindAgentsInNeed();
        if (helped is not { Helper: null }) return;
        Goal = helped.Position;
        FoundDistressedAgent = true;
        if (IsInGroup)
        {
            UpdateGroupStatus();
            if (ReturningWithGroupToHelp)
            {
                ReturningWithGroupToHelp = true;
                if (IsLeader)
                {
                    Helped = helped;
                    helped.Helper = this; 
                    Console.WriteLine($"{GetType().Name}  {ID} is going to help Agent {helped.ID} with group");
                }
                else
                {
                    foreach (var agent in Leader.Group)
                    {
                        agent.Helped = helped;
                        Console.WriteLine($"{GetType().Name}  {ID}is going to help Agent {helped.ID} with group");
                    }
                    
                }
            }
            else
            {
                Helped = helped;
                helped.Helper = this;
                Console.WriteLine($"{GetType().Name}  {ID} is going to help Agent {helped.ID}");
            }
        }
        else
        {
            Helped = helped;
            helped.Helper = this;
            Console.WriteLine($"{GetType().Name}  {ID}is going to help Agent {helped.ID}");
        }

        helped.FoundHelp = true;

    }

    protected void UpdateGroupStatus()
    {
        if (IsLeader)
        {
            if (Rand.NextDouble() > 0.5)
            {
                if (Group.Count == 1)
                {
                    foreach (var agent in Group)
                    {
                        agent.IsInGroup = false;
                        agent.Leader = null;
                    }
                    Group.Clear();
                }
                else
                {
                    var newLeader = Group.OrderByDescending(agent => agent.Leadership).First();
                    newLeader.IsLeader = true;
                    if (Group != null) 
                        foreach (var a in Group)
                        { a.Leader = newLeader;
                            newLeader.Group.Add(a);
                        }
                    IsInGroup = false;
                    Group.Clear();
                    IsLeader = false;
                    newLeader.Goal = FindNearestExit(Layer.PossibleGoal);
                    if (FoundDistressedAgent)
                    {
                        ReturningWithGroupToHelp = false;
                        Console.WriteLine($"{GetType().Name} {ID} is no longer leading the group and is returning to help an agent");
                    }
                    else
                    {
                        ReturningWithGroupForItem = false;
                        Console.WriteLine($"{GetType().Name} {ID} is no longer leading the group and is returning to find an item");
                    }
                    
                }
            }
            else
            {
                if (Group.Count<1) return;
                var newGroup = new List<Evacuee>();
                var remainingMembers = new List<Evacuee>();
                foreach (var agent in Group)
                {
                    if (agent.Empathy> 0.5)
                    {
                        if (FoundDistressedAgent)
                        {
                            agent.ReturningWithGroupToHelp = true;
                            agent.FoundDistressedAgent = true;
                        }
                        else
                        {
                            agent.ReturningWithGroupForItem = true;
                        }
                        remainingMembers.Add(agent);
                    }
                    else
                    {
                        newGroup.Add(agent);
                    }
                }
                Group.Clear();
                Group = remainingMembers;
                switch (newGroup.Count)
                {
                    case < 1:
                        return;
                    case 1:
                    {
                        var agent = newGroup[0];
                        agent.IsInGroup = false;
                        agent.Leader = null;
                        agent.Goal = FindNearestExit(Layer.PossibleGoal);
                        Console.WriteLine($"{agent.GetType().Name} {ID} Has left group and is moving alone");
                        break;
                    }
                    default:
                    {
                        Evacuee newLead = null;
                        foreach (var agent in from agent in newGroup
                                 let agentLead = agent.Leadership
                                 where agentLead > Leadership
                                 select agent)
                        {
                            newLead = agent;
                        }

                        if (newLead == null) return;
                        {
                            newLead.IsLeader = true;
                            newLead.Goal = Goal;
                            foreach (var agent in newGroup)
                            {
                                newLead.Group.Add(agent);
                                agent.Leader = newLead;
                            }

                            newLead.Goal = FindNearestExit(Layer.PossibleGoal);
                            Console.WriteLine($"Group has been split");
                        }
                        break;
                    }
                }
            }

        }
        else 
        {
            if (Rand.NextDouble() > 0.5) 
            {
                Leader.Group.Remove(this);
                Leader = null;
                IsInGroup = false;
            }
            else 
            {
                var newGroup = new List<Evacuee>();
                var remainingMembers = new List<Evacuee>();
                foreach (var agent in  from agent in Leader.Group where agent!= this select agent)
                {
                    if (agent.Empathy> 0.5)
                    {
                        if (FoundDistressedAgent)
                        {
                            agent.ReturningWithGroupToHelp = true;   
                            agent.FoundDistressedAgent = true; 
                        }
                        else
                        {
                            agent.ReturningWithGroupForItem = true;   
                        }
                        remainingMembers.Add(agent);
                    }
                    else
                    {
                        newGroup.Add(agent);
                    }
                }
                Leader.Group.Clear();
                Leader.Group = remainingMembers; 
                
                switch (newGroup.Count)
                {
                    case < 1:
                        return;
                    case 1:
                    {
                        var agent = newGroup[0];
                        agent.IsInGroup = false;
                        agent.Leader = null;
                        agent.Goal = FindNearestExit(Layer.PossibleGoal);
                        Console.WriteLine($"{agent.GetType().Name} {ID} Has left group and is moving alone");
                        break;
                    }
                    default:
                    {
                        var newLead = newGroup.OrderByDescending(agent => agent.Leadership).First();
                        newLead.IsLeader = true;
                        newLead.Leader = null;
                        newLead.Goal = FindNearestExit(Layer.PossibleGoal);
                        foreach (var agent in newGroup)
                        {
                            newLead.Group.Add(agent);
                            agent.Leader = newLead;
                        }

                        Console.WriteLine($"Group has been split");
                        break;
                    }
                }

                if (Leader == null) return;
                if (FoundDistressedAgent)
                {
                    Leader.ReturningWithGroupToHelp = true;
                    Leader.FoundDistressedAgent = true;
                        
                }
                else
                {
                    Leader.ReturningWithGroupForItem = true;
                    Leader.Goal = OriginalPosition;
                }
            }
        }
    }
    

    protected void OfferHelp()
    {
        if (Helped.Health != 0)
        {
            Helped.FoundHelp = true;
            Helped.Helper = this;
            Helping = true;
        }
        else
        {
            Console.WriteLine($"{Helped.GetType().Name} {Helped.ID} Has been found dead");
            FoundDistressedAgent = false;
            Helped = null;
        }
       
    }

    protected void UpdateHealthStatus()
    {
        if (Health <= 5)
        {
            IsConscious = false; 
        }
        if (Health != 0) return;
        Console.WriteLine($"{GetType().Name} {ID} Has been killed in the simulation");
        Layer.RemoveFromSimulation(this);
    }

    #endregion

    #region Fields and Properties

    public Guid ID { get; set; }
   
    public Position Position { get; set; }


    protected bool ForgotOnce { get; set; }

    protected static GridLayer Layer;
    protected Position Goal; 
    private bool _tripInProgress;
    protected static readonly Random Rand = new();
    private List<Position>.Enumerator _path;
    protected int RiskLevel { get; set;}
    protected List<Evacuee> Group { get; set; }
    protected int Aggression { get; set; }
    public int Health;
    protected bool IsLeader { get; set; }
    protected int Speed { get; set; } 
    protected bool Helping { get; set; }
    protected int DelayTime { get; set; }
    protected Position OriginalPosition { get; set;}
    protected bool FoundHelp { get; set; }
    protected Evacuee Leader { get; set; }
    protected Evacuee Helper { get; set; }
    protected Evacuee Helped { get; set; }
    protected bool IsInGroup;
    protected bool FoundExit;
    private const int MaxSpeed = 2;
    protected bool AgentForgotItem { get; set; }
    protected bool ReturningWithGroupForItem { get; set; }
    protected bool ReturningWithGroupToHelp { get; set; }
    protected bool ReachedDistressedAgent { get; set; }
    protected bool FoundDistressedAgent { get; set; }
    protected double CollaborationFactor { get; set; }
    protected double Leadership { get; set; }
    private bool Returning { set; get; }
    protected bool IsConscious { get; set; }
    protected bool EvacueeHasStartedMoving { get; set; }
    protected double Strength { get; set; }
    protected double Empathy { get; set; }

    private Vector2 _currentVelocity = Vector2.Zero;
    #endregion
}