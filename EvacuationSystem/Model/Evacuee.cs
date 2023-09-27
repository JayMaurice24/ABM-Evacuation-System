using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
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
          MoveRandomly();
        }

    #endregion

    #region Methods
    

   /// <summary>
   /// Agents Move towards goal with Low aggression 
   /// </summary>
    protected void MoveTowardsGoalLow()
    {
        if (!_tripInProgress)
        {
            // Finds closest exit and moves towards exit 
            _path = Layer.FindPath(Position, Goal).GetEnumerator();
            _tripInProgress = true;
            
        }
        if (!_path.MoveNext()) return;
        if (AvoidFire())
        {
            var next = ChangeDirection(_path.Current);
            Layer.EvacueeEnvironment.MoveTo(this, next, 1);
            _path = Layer.FindPath(Position, Goal).GetEnumerator();
            return;

        }
        if (!IsCellOccupied(_path.Current))
        {
            Layer.EvacueeEnvironment.MoveTo(this, _path.Current,1);
            Console.WriteLine($"{this.GetType().Name} {ID} has moved to cell {_path.Current}");
        }
        if (!Position.Equals(Goal)) return;
        if (Goal.Equals(OriginalPosition))
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
            RemoveFromSimulation();
        }
        else
        {
            Goal = FindNearestExit(Layer.Stairs); 
            _path = Layer.FindPath(Position, Goal).GetEnumerator();
           
        }

    }
   

   /// <summary>
   /// Agent Moves towards goal with Medium Aggression 
   /// </summary>
    protected void MoveTowardsGoalMedium()
    {
        if (!_tripInProgress)
        {
            // Finds closest exit and moves towards exit 
            _path = Layer.FindPath(Position, Goal).GetEnumerator();
            _tripInProgress = true;
            
        }
        if (!_path.MoveNext()) return;
        if (AvoidFire())
        {
            var next = ChangeDirection(_path.Current);
            Layer.EvacueeEnvironment.MoveTo(this, next, 1);
            _path = Layer.FindPath(Position, Goal).GetEnumerator();
            return;

        }
        if (IsCellOccupied(_path.Current))
        {
            var otherAgent = GetAgentAt(Position);
            if (otherAgent != null)
            {
                if (otherAgent.Aggression <= Aggression && otherAgent.Strength < Strength)
                {
                    PushAgent(otherAgent);
                    Console.WriteLine($"Agent {otherAgent.ID} has been pushed by {ID}");
                    
                }
            }
        }
        else{
            Layer.EvacueeEnvironment.MoveTo(this, _path.Current, Speed);  
        }

        if (Goal.Equals(OriginalPosition))
        {
            Console.WriteLine($"{this.GetType().Name} {ID} has Found Item and is heading to the exit");
            Goal = FindNearestExit(Layer.PossibleGoal);
            AgentForgotItem = false;
            _path = Layer.FindPath(Position, Goal).GetEnumerator();
            
        }
        else if (Layer.Stairs.Contains(Goal))
        {
            Console.WriteLine($"Agent {ID} has reached exit {Goal}");
            RemoveFromSimulation();
            _tripInProgress = false;
        }
        else{
            Goal = FindNearestExit(Layer.Stairs); 
            _path = Layer.FindPath(Position, Goal).GetEnumerator();
        }
    }
   
   /// <summary>
   /// Agent moves towards goal with high aggression 
   /// </summary>
    protected void MoveTowardsGoalHigh()
    {
        if (!_tripInProgress)
        {
            // Finds closest exit and moves towards exit 
            _path = Layer.FindPath(Position, Goal).GetEnumerator();
            _tripInProgress = true;
            
        }

        if (!_path.MoveNext()) return;
        if (AvoidFire())
        {
            var next = ChangeDirection(_path.Current);
            Layer.EvacueeEnvironment.MoveTo(this, next, 1);
            _path = Layer.FindPath(Position, Goal).GetEnumerator();
            return;

        }
        if (IsCellOccupied(_path.Current))
        {
            var otherAgent = GetAgentAt(Position);
            if (otherAgent != null)
            {
                PushAgent(otherAgent);
                Console.WriteLine($"Agent {otherAgent.ID} has been pushed by {ID}");
            }
        }
        else{
            Layer.EvacueeEnvironment.MoveTo(this, _path.Current, Speed);  
        }

        if (Goal.Equals(OriginalPosition))
        {
            Console.WriteLine($"{this.GetType().Name} {ID} has Found Item and is heading to the exit");
            Goal = FindNearestExit(Layer.PossibleGoal);
            AgentForgotItem = false;
            _path = Layer.FindPath(Position, Goal).GetEnumerator();
            
        }
        else if (Layer.Stairs.Contains(Goal))
        {
            Console.WriteLine($"{this.GetType().Name} {ID} has reached exit {Goal}");
            RemoveFromSimulation();
            _tripInProgress = false;
        }
        else{
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
    protected void FormGroup(Evacuee leader)
    {
        var potentialGroupMembers = Layer.EvacueeEnvironment.Explore(Position, radius: 5).ToList();
        if (potentialGroupMembers.Count <= 0) return;
        foreach (var agent in potentialGroupMembers.Where(agent => agent.CollaborationFactor > 0.5 && !agent.IsInGroup && !agent.IsLeader))
        {
            agent.IsInGroup = true;
            agent.Leader = leader;
            leader.Group.Add(agent);
        }
    }
    /// <summary>
    /// Identifies if an agent can lead a group
    /// </summary>
    protected void DetermineLeader()
    {
        var agents = Layer.EvacueeEnvironment.Explore(Position, radius: 5).ToList();
        var lead = this.Leadership;
        var closestAgent = this;
        if(!(agents.Count > 1)) return;
        foreach (var agent in from agent in agents let agentLead = agent.Leadership where agentLead > lead select agent)
        {
            closestAgent = agent;
        }

        if (closestAgent != this) return;
        this.IsLeader = true;
        this.IsInGroup = true;
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
        var newX = Position.X + _currentVelocity.X;
        var newY = Position.Y + _currentVelocity.Y;

        if (!(0 <= newX) || !(newX < Layer.Width) || !(0 <= newY) || !(newY < Layer.Height)) return;
        if (!Layer.IsRoutable(newX, newY) || IsCellOccupied(new Position(newX, newY))) return;
        if (AvoidFire())
        {
            Position = ChangeDirection(new Position(newX, newY));
            Layer.EvacueeEnvironment.MoveTo(this, new Position(Position.X, Position.Y));
        }
        else
        {
            Position = new Position(newX, newY);
            Layer.EvacueeEnvironment.MoveTo(this, new Position(newX, newY)); 
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
        return Layer.EvacueeEnvironment.Entities.MinBy(agent =>
            Distance.Chebyshev(Position.PositionArray, agent.Position.PositionArray));
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
        var originalHeading = Layer.Directions.IndexOf(_path.Current);

        foreach (var next in Layer.Directions)
        {
            var newX = Position.X + next.X;
            var newY = Position.Y + next.Y;
            var nextCell = new Position(newX, newY);

            if (!(0 <= newX) || !(newX < Layer.Width) || !(0 <= newY) || !(newY < Layer.Height) ||
                !Layer.IsRoutable(newX, newY) || !Layer.FireLocations.Contains(nextCell)) continue;
            // If the new direction is opposite to the original heading (180-degree turn), change goals to the south exit
            if (Math.Abs(Layer.Directions.IndexOf(next) - originalHeading) == 4)
            {
                if (Layer.Exits.Contains(Goal))
                {
                    foreach (var ex in Layer.Exits.Where(ex => !Goal.Equals(ex)))
                    {
                        Goal = ex;
                        break;
                    }
                    ChangingDirection = true;
                }
                else if (Layer.FrontStairs.Contains(Goal))
                {
                    Goal = FindNearestExit(Layer.BackStairs);
                    ChangingDirection = true;
                }
                else if (Layer.BackStairs.Contains(Goal))
                {
                    Goal = FindNearestExit(Layer.FrontStairs); 
                    ChangingDirection = true;
                }
                else
                {
                    Goal = FindNearestExit(Layer.PossibleGoal); 
                    ChangingDirection = true;
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
    private bool AvoidFire()
    {
        var fire = Layer.FireEnvironment.Explore(Position, radius: 5);
        return fire.Any(flame => Distance.Chebyshev(new[] { Position.X, Position.Y }, new[] { flame.Position.X, flame.Position.Y }) <= 2.0);
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
    
    private bool ViewRange(double agent1X, double agent1Y, double agent2X, double agent2Y)
    {
        switch (agent1X)
        {
            case > 1 and < 19 when agent1Y is > 50 and < 87 && agent2X is > 1 and < 19 && agent2Y is > 50 and < 87: //Coral
            case > 21 and < 54 when agent1Y is > 52 and < 84 && agent2X is > 21 and < 54 && agent2Y is > 52 and < 84: //West 
            case > 65 and < 100 when agent1Y is > 52 and < 84 && agent2X is > 65 and < 100 && agent2Y is > 52 and < 84: //East
            case > 102 and < 120 when agent1Y is > 42 and < 87 && agent2X is > 102 and < 120 && agent2Y is > 42 and < 87: //Braae
            case > 44 and < 120 when agent1Y is > 1 and < 15 && agent2X is > 44 and < 120  && agent2Y is > 1 and < 15: //CSHons 
            case > 77 and < 86 when agent1Y is > 41 and < 45 && agent2X is > 77 and < 86 && agent2Y is > 41 and < 45: //FBathroom
            case > 86 and < 94 when agent1Y is > 38 and < 45 && agent2X is > 86 and < 94 && agent2Y is > 38 and < 45: //FBathroom2
            case > 77 and < 86 when agent1Y is > 32 and < 36 && agent2X is > 77 and < 86 && agent2Y is > 32 and < 36: //MBathroom 
            case > 86 and < 94 when agent1Y is > 32 and < 38 && agent2X is > 86 and < 94 && agent2Y is > 32 and < 38: //MBathroom2 
            case > 44 and < 63 when agent1Y is > 15 and < 32 && agent2X is > 44 and < 63 && agent2Y is > 15 and < 32: //Atrium
            case > 96 and < 120 when agent1Y is > 16 and < 32 && agent2X is > 96 and < 120 && agent2Y is > 16 and < 32: //SysDev
            case > 14 and < 102 when agent1Y is > 46 and < 50 && agent2X is > 14 and < 102 && agent2Y is > 46 and < 50: //Passage1
            case > 50 and < 53 when agent1Y is > 51 and < 70 && agent2X is > 50 and < 53 && agent2Y is > 51 and < 70: //Passage2
            case > 56 and < 65 when agent1Y is > 45 and < 68 && agent2X is > 56 and < 65 && agent2Y is > 45 and < 68: //Passage3
            case > 63 and < 94 when agent1Y is > 26 and < 31 && agent2X is > 63 and < 94 && agent2Y is > 26 and < 31: //Passage4
            case > 63 and < 94 when agent1Y is > 15 and < 19 && agent2X is > 63 and < 94 && agent2Y is > 15 and < 19: //Passage5
            case > 63 and < 73 when agent1Y is > 19 and < 31 && agent2X is > 63 and < 94 && agent2Y is > 19 and < 31: //Passage6
                return true;
            default:
                return false;
        }
    }
/// <summary>
/// Agents Move Randomly before tick
/// </summary>
    protected void MoveRandomly()
    {
        var nextDirection = Layer.Directions[Rand.Next(Layer.Directions.Count)];
        var newX = Position.X + nextDirection.X;
        var newY = Position.Y + nextDirection.Y;
        
        // Check if chosen move is within the bounds of the grid
        if (0 <= newX && newX < Layer.Width && 0 <= newY && newY < Layer.Height)
        {
            // Check if chosen move goes to a cell that is routable
            if (Layer.IsRoutable(newX, newY))
            {
                Position = new Position(newX, newY);
                Layer.EvacueeEnvironment.MoveTo(this, new Position(newX, newY));
                Console.WriteLine($"{GetType().Name} moved to a new cell: {Position}");
            }
            else
            {
                Console.WriteLine($"{GetType().Name} tried to move to a blocked cell: ({newX}, {newY})");
            }
        }
        else
        {
            Console.WriteLine($"{GetType().Name} tried to leave the world: ({newX}, {newY})");
        }
    }

protected void ReturnForItem()
{
    if (!Returning){
        Goal = OriginalPosition;
        Returning = true;
    }
    switch (Aggression)
    {
        case 0: 
            MoveTowardsGoalLow();
            break;
        case 1:
            MoveTowardsGoalMedium();
            break;
        case 2:
            MoveTowardsGoalHigh();
            break;
    }
}

/// <summary>
    ///     Removes this agent from the simulation and, by extension, from the visualization.
    /// </summary>
    public void RemoveFromSimulation()
    {
        Layer.EvacueeEnvironment.Remove(this);
        UnregisterAgentHandle.Invoke(Layer, this);
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
        // Iterate through the list of agents in the environment
        return Layer.EvacueeEnvironment.Entities.FirstOrDefault(agent => agent != this && agent.Position.Equals(position));
    }

    private Evacuee FindAgentsInNeed()
    {
        var nearbyAgents = Layer.EvacueeEnvironment.Explore(Position, radius: 5).ToList();
        foreach(var agent in nearbyAgents)
        {
            if (!agent.IsConscious)
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
        if (helped == null) return;
        Goal = helped.Position;
        FoundDistressedAgent = true;
        Helped = helped;
        Helper.Helped = this; 
        Console.WriteLine($"Agent{ID} is going to help Agent {helped.ID}");
    }

    protected void OfferHelp()
    {
        Helped.FoundHelp = true;
        Helped.Helper = this; 
    }

    protected void Consciousness()
    {
        if (Health <= 5)
        {
            IsConscious = false; 
        }
    }

    #endregion

    #region Fields and Properties

    public Guid ID { get; set; }
   
    public Position Position { get; set; }
    
    [PropertyDescription(Name = "MaxTripDistance")]
    public double MaxTripDistance { get; set; }
    
    [PropertyDescription(Name = "AgentExploreRadius")]

    public UnregisterAgent UnregisterAgentHandle { get; set; }

    private bool ChangingDirection { get; set; }
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
    private const int MaxSpeed = 10;
    protected bool AgentForgotItem;
    protected bool ReachedDistressedAgent { get; set; }
    protected bool FoundDistressedAgent { get; set; }
    protected double CollaborationFactor { get; set; }
    protected double Leadership { get; set; }
    private bool Returning { set; get; }
    protected bool IsConscious { get; set; }
    protected double Strength { get; set; }
    protected double Empathy { get; set; }

    private Vector2 _currentVelocity = Vector2.Zero;
    #endregion
}