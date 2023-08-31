using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using Mars.Numerics;

namespace GridBlueprint.Model;

public class ComplexAgent : IAgent<GridLayer>, IPositionable
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
        TickCount = (int)layer.GetCurrentTick();
        Group = null;
        Leader = null;

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
    ///     Generates a list of eight movement directions that the agent uses for random movement.
    /// </summary>
    /// <returns>The list of movement directions</returns>
    protected static List<Position> CreateMovementDirectionsList()
    {
        return new List<Position>
        {
            MovementDirections.North,
            MovementDirections.Northeast,
            MovementDirections.East,
            MovementDirections.Southeast,
            MovementDirections.South,
            MovementDirections.Southwest,
            MovementDirections.West,
            MovementDirections.Northwest
        };
    }



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
        Layer.ComplexAgentEnvironment.MoveTo(this, AvoidFire() ? ChangeDirection(_path.Current) : _path.Current, 1);

        if (!IsCellOccupied(_path.Current))
        {
            Layer.ComplexAgentEnvironment.MoveTo(this, _path.Current, Speed);  
        }

        if (!Position.Equals(Goal)) return;
        if (Layer.Stairs.Contains(Goal))
        {
            Console.WriteLine($"ComplexAgent {ID} reached goal {Goal}");
            RemoveFromSimulation();
            _tripInProgress = false;
        }
        else
        {
            Goal = ClosestStairs(Layer.Stairs); 
            _path = Layer.FindPath(Position, Goal).GetEnumerator();
            _tripInProgress = true;
            if (!_path.MoveNext()) return;
            Layer.ComplexAgentEnvironment.MoveTo(this, _path.Current, Speed);
            if (!Position.Equals(Goal)) return;
            Console.WriteLine($"ComplexAgent {ID} reached goal {Goal}");
            RemoveFromSimulation();
            _tripInProgress = false;
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
        Layer.ComplexAgentEnvironment.MoveTo(this, AvoidFire() ? ChangeDirection(_path.Current) : _path.Current,
            1);

        if (IsCellOccupied(_path.Current))
        {
            var otherAgent = GetAgentAt(Position);
            if (otherAgent != null)
            {
                if (otherAgent.Pushiness < Pushiness)
                {
                    PushAgent(otherAgent);
                }
            }
        }
        else{
            Layer.ComplexAgentEnvironment.MoveTo(this, _path.Current, Speed);  
        }

        if (Layer.Stairs.Contains(Goal))
        {
            Console.WriteLine($"ComplexAgent {ID} reached goal {Goal}");
            RemoveFromSimulation();
            _tripInProgress = false;
        }
        else
        {
            Goal = ClosestStairs(Layer.Stairs); 
            _path = Layer.FindPath(Position, Goal).GetEnumerator();
            _tripInProgress = true;
            if (!_path.MoveNext()) return;
            Layer.ComplexAgentEnvironment.MoveTo(this, _path.Current, Speed);
            if (!Position.Equals(Goal)) return;
            Console.WriteLine($"ComplexAgent {ID} reached goal {Goal}");
            RemoveFromSimulation();
            _tripInProgress = false;
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
        Layer.ComplexAgentEnvironment.MoveTo(this, AvoidFire() ? ChangeDirection(_path.Current) : _path.Current, 1);

        if (IsCellOccupied(_path.Current))
        {
            var otherAgent = GetAgentAt(Position);
            if (otherAgent != null)
            {
                PushAgent(otherAgent);
            }
        }
        else{
            Layer.ComplexAgentEnvironment.MoveTo(this, _path.Current, Speed);  
        }

        if (Layer.Stairs.Contains(Goal))
        {
            Console.WriteLine($"ComplexAgent {ID} reached goal {Goal}");
            RemoveFromSimulation();
            _tripInProgress = false;
        }
        else
        {
            Goal = ClosestStairs(Layer.Stairs); 
            _path = Layer.FindPath(Position, Goal).GetEnumerator();
            _tripInProgress = true;
            if (!_path.MoveNext()) return;
            Layer.ComplexAgentEnvironment.MoveTo(this, _path.Current, Speed);
            if (!Position.Equals(Goal)) return;
            Console.WriteLine($"ComplexAgent {ID} reached goal {Goal}");
            RemoveFromSimulation();
            _tripInProgress = false;
        }
    }
/// <summary>
/// Moves the agent straight towards the stairs if closer to the stairs
/// </summary>

    /// <summary>
     /// Finds the exit closest to the agent  
     /// </summary>
    protected Position FindNearestExit(List<Exits> targets)
    {
        Position nearestExit = null; 
        var shortestDistance = double.MaxValue;
        foreach (var p in targets)
        {
            var distance = CalculateDistance(Position, p.Position);
            if (!(distance < shortestDistance)) continue;
            shortestDistance = distance;
            nearestExit = p.Position;
        }

        return nearestExit;
    }
    protected Position ClosestStairs(List<Position> targets)
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

    protected void FormGroup()
    {
        var potentialGroupMembers = Layer.ComplexAgentEnvironment.Explore(Position, radius: 5).ToList();

        if (potentialGroupMembers.Count > 0)
        {
            Group = potentialGroupMembers;
            foreach (var agent in Group)
            {
                agent.IsInGroup = true; 
            }
            SelectLeader();
        }
    }
    private void SelectLeader()
    {
        // Find the closest agent to the exit as the leader for each group
        var leader = this;
        foreach (var agent in Group)
        {
            if (agent.IsLeader) continue;

            var minDistance = CalculateDistance(agent.Position, agent.Goal);
            foreach (var otherAgent in Group)
            {
                if (otherAgent == this || otherAgent.IsLeader) return;

                double distance = CalculateDistance(otherAgent.Position, otherAgent.Goal);
                if (distance < minDistance && otherAgent.Pushiness >= 0 ){
                    minDistance = distance;
                    leader = otherAgent;
                }
                else
                {
                    leader = this;
                }
            }
        }
        leader.IsLeader = true;
    }
    private ComplexAgent FindGroupLeader()
    {
        // Find the leader of the group that the agent belongs to
        foreach (var otherAgent in Group)
        {
            if (otherAgent.IsLeader)
                return otherAgent;
        }
        // If no leader is found, return the agent itself as a fallback
        return this;
    }
    private Vector2 CalculateSocialForce(ComplexAgent leader)
    {
        Vector2 totalForce = Vector2.Zero;
        if (IsInGroup)
        {
            Vector2 attractiveForce = Vector2.Normalize(new Vector2((float)(leader.Position.X - Position.X),(float)(leader.Position.Y - Position.Y) )) * (float)_attractiveForceMultiplier;

            totalForce += attractiveForce;

            foreach (var groupMember in Group)
            {
                if (groupMember != this)
                {
                    Vector2 repulsiveForce = Vector2.Normalize(new Vector2((float)(Position.X - groupMember.Position.X),(float)(Position.Y - groupMember.Position.Y) ))
                                            * _repulsiveForceMultiplier / (float)CalculateDistance(Position, groupMember.Position);
                    totalForce += repulsiveForce;
                }
            }
        }

        return totalForce;
    }

    private Vector2 CalculateObstacleAvoidanceForce()
    {
        Vector2 obstacleAvoidanceForce = Vector2.Zero;

        var nearestObstacle = GetNearestObstacle();

        if (nearestObstacle != null)
        {
            Vector2 awayFromObstacle = Vector2.Normalize(new Vector2((float)(Position.X - nearestObstacle.Position.X), (float)(Position.Y - nearestObstacle.Position.Y)))
                                       * _obstacleAvoidanceMultiplier
                                       / (float)CalculateDistance(Position, nearestObstacle.Position);
            obstacleAvoidanceForce += awayFromObstacle;
        }

        return obstacleAvoidanceForce;
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
    protected void MoveTowardsGroupLeader()
    {
        var socialForce = CalculateSocialForce(FindGroupLeader());
        var obstacleAvoidanceForce = CalculateObstacleAvoidanceForce();
        var netForce = socialForce + obstacleAvoidanceForce;

        UpdateVelocity(netForce);

        // Update position based on new velocity
        var newX = Position.X + _currentVelocity.X;
        var newY = Position.Y + _currentVelocity.Y;

            if (0 <= newX && newX < Layer.Width && 0 <= newY && newY < Layer.Height)
            {
                if (Layer.IsRoutable(newX, newY) && !IsCellOccupied(new Position(newX, newY)))
                {
                    Position = new Position(newX, newY);
                    Layer.ComplexAgentEnvironment.MoveTo(this, new Position(newX, newY));
                }
            }
        }
   /*
    
    private void MoveTogether(List<ComplexAgent> group)
    {
        if (group.Count > 0)
        {
            Vector2 centerOfMass = new Vector2(0, 0);

            // Calculate the center of mass of the group
            foreach (ComplexAgent agent in group)
            {
                centerOfMass += new Vector2(agent.Position.X, agent.Position.Y);
            }
            centerOfMass /= group.Count;

            // Calculate the vector towards the center of mass
            Vector2 cohesionVector = centerOfMass - new Vector2(Position.X, Position.Y);
            cohesionVector = cohesionVector.Normalized(); // Normalize for consistent movement speed

            // Update agent's position to move towards the center of mass
            Vector2 newPosition = new Vector2(Position.X, Position.Y) + cohesionVector * Speed;
            if (IsPositionValid(newPosition)) // Check if the new position is valid
            {
                Position = new Position((int)newPosition.X, (int)newPosition.Y);
                _layer.ComplexAgentEnvironment.MoveTo(this, Position);
                Console.WriteLine($"{GetType().Name} moved to a new cell: {Position}");
            }
        }
    }

    private bool IsPositionValid(Vector2 position)
    {
        return 0 <= position.X && position.X < _layer.Width &&
               0 <= position.Y && position.Y < _layer.Height &&
               _layer.IsRoutable((int)position.X, (int)position.Y);
    }

    public void Tick()
    {
        // ... (existing code)

        if (_layer.Ring)
        {
            if (_layer.GetCurrentTick() > RiskLevel)
            {
                // ... (existing code)

                // Perform group interactions
                List<ComplexAgent> group = GetAgentsInProximity(); // Get agents within interaction radius
                MoveTogether(group); // Move together with agents in the same group
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

    // ... (existing code)
}

private List<ComplexAgent> GetAgentsInProximity()
{
    List<ComplexAgent> agentsInProximity = new List<ComplexAgent>();

    foreach (var agent in _layer.ComplexAgents)
    {
        if (agent != this) // Exclude the current agent
        {
            double distance = CalculateDistance(Position, agent.Position);
            if (distance <= InteractionRadius) // Adjust InteractionRadius to your needs
            {
                agentsInProximity.Add(agent);
            }
        }
    }

    return agentsInProximity;
}

     // Calculate the center of mass
        Vector2 centerOfMass = CalculateCenterOfMass(group);

        // Calculate cohesion vector
        Vector2 cohesionVector = centerOfMass - new Vector2(Position.X, Position.Y);
        cohesionVector = cohesionVector.Normalize(); // Normalize to maintain direction

        // Calculate desired velocity
        Vector2 desiredVelocity = cohesionVector * Speed; // Calculate desired velocity

        // Calculate steering force
        Vector2 steering = desiredVelocity - currentVelocity; // Calculate steering force

        // Calculate new velocity
        Vector2 newVelocity = currentVelocity + steering; // Calculate new velocity

        // Update agent's position using new velocity
        Vector2 newPosition = new Vector2(Position.X, Position.Y) + newVelocity;
        if (IsPositionValid(newPosition)) // Check if the new position is valid
        {
            Position = new Position((int)newPosition.X, (int)newPosition.Y);
            _layer.ComplexAgentEnvironment.MoveTo(this, Position);
            Console.WriteLine($"{GetType().Name} moved to a new cell: {Position}");
            
    */
    private ComplexAgent GetNearestObstacle()
    {
        ComplexAgent nearestObstacle = null;
        double shortestDistance = double.MaxValue;

        foreach (var agent in Layer.ComplexAgentEnvironment.Entities)
        {
            if (agent != this) // Assuming you have an IsObstacle property in ComplexAgent
            {
                double distance = CalculateDistance(Position, agent.Position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestObstacle = agent;
                }
            }
        }

        return nearestObstacle;
    }
    
    /// <summary>
    /// Calculates the distance between two coordinates 
    /// </summary>
    protected double CalculateDistance(Position coords1, Position coords2)
    {
        return Distance.Chebyshev(new[] { coords1.X, coords1.Y }, new[] { coords2.X, coords2.Y }); 

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

        if (fire != null)
        {
            if (position.Equals(fire.Position))
            {
                foreach (var next in Directions)
                {
                    var newX = Position.X + next.X;
                    var newY = Position.Y + next.Y;

                    if (0 <= newX && newX < Layer.Width && 0 <= newY && newY < Layer.Height &&
                        Layer.IsRoutable(newX, newY) && AvoidFire() == false)
                    {
                        var nextCell = Layer.FireEnvironment.Entities.FirstOrDefault(flame =>
                            Distance.Chebyshev(new[] { newX, newY }, new[] { flame.Position.X, flame.Position.Y }) <=
                            1.0);

                        if (nextCell == null)
                        {
                            return new Position(newX, newY);

                        }
                    }
                }
            }

           
        }
        return position;
    }

    
    
    /// <summary>
    /// Checks if the agent is in close proximity to the fire
    /// </summary>
    /// <returns></returns>
    private bool AvoidFire()
    {
        var fire = Layer.FireEnvironment.Explore(Position, radius: 1); 
        
        foreach (var flame in fire)
        {
            if (Distance.Chebyshev(new []{Position.X, Position.Y}, new []{flame.Position.X, flame.Position.Y}) <= 2.0)
            {
                return true;
            }
        }

        return false; 
    }

    protected bool Perception()
    {
        var fire = Layer.FireEnvironment.Entities.MinBy(flame =>
            Distance.Chebyshev(new[] { Position.X, Position.Y }, new[] { flame.Position.X, flame.Position.Y }));
        
        return  ViewRange(Position.X, Position.Y, fire.Position.X, fire.Position.Y);
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
        var nextDirection = Directions[Rand.Next(Directions.Count)];
        var newX = Position.X + nextDirection.X;
        var newY = Position.Y + nextDirection.Y;
        
        // Check if chosen move is within the bounds of the grid
        if (0 <= newX && newX < Layer.Width && 0 <= newY && newY < Layer.Height)
        {
            // Check if chosen move goes to a cell that is routable
            if (Layer.IsRoutable(newX, newY))
            {
                Position = new Position(newX, newY);
                Layer.ComplexAgentEnvironment.MoveTo(this, new Position(newX, newY));
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

    /// <summary>
    /// Checks if the next cell is occupied by an agent
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    private bool IsCellOccupied(Position targetPosition)
    {
        var agents = Layer.ComplexAgentEnvironment.Entities.MinBy(agent =>
            Distance.Chebyshev(new[] { Position.X, Position.Y }, new[] { agent.Position.X, agent.Position.Y }));

        if (agents != null)
        {
            if (targetPosition.Equals(agents.Position))
            {
                return true; 
            }
        }
        
        return false;
    }
    

    /// <summary>
    ///     Removes this agent from the simulation and, by extension, from the visualization.
    /// </summary>
    public void RemoveFromSimulation()
    {
        Console.WriteLine($"ComplexAgent {ID} is removing itself from the simulation.");
        Layer.ComplexAgentEnvironment.Remove(this);
        UnregisterAgentHandle.Invoke(Layer, this);
    }

    /// <summary>
    /// Push other agents out the way
    /// </summary>
    /// <param name="otherAgent"></param>
    private void PushAgent(ComplexAgent otherAgent)
        {
            // Calculate the direction to push the other agent
            var pushDirectionX = otherAgent.Position.X - Position.X;
            var pushDirectionY = otherAgent.Position.Y - Position.Y;

            // Calculate the new position for the other agent after the push
            var newAgentX = otherAgent.Position.X + pushDirectionX;
            var newAgentY = otherAgent.Position.Y + pushDirectionY;

            // Check if the new position is within the bounds of the grid
            if (0 <= newAgentX && newAgentX < Layer.Width &&
                0 <= newAgentY && newAgentY < Layer.Height)
            {
                // Check if the new position is routable
                if (Layer.IsRoutable(newAgentX, newAgentY))
                {
                    // Move the other agent to the new position
                    otherAgent.Position = new Position(newAgentX, newAgentY);
                    Layer.ComplexAgentEnvironment.MoveTo(otherAgent, new Position(newAgentX, newAgentY));
                }
            }
        }

    /// <summary>
    /// Retrieves an agent based off proximity
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private ComplexAgent GetAgentAt(Position position)
        {
            // Iterate through the list of agents in the environment
            foreach (var agent in Layer.ComplexAgentEnvironment.Entities)
            {
                if (agent != this && agent.Position.Equals(position))
                {
                    return agent; // Return the agent found at the specified position
                }
            }
    
            return null; // No agent found at the specified position
        }
    

    #endregion

    #region Fields and Properties

    public Guid ID { get; set; }
   
    public Position Position { get; set; }
    
    [PropertyDescription(Name = "MaxTripDistance")]
    public double MaxTripDistance { get; set; }
    
    [PropertyDescription(Name = "AgentExploreRadius")]

    public UnregisterAgent UnregisterAgentHandle { get; set; }

    protected static GridLayer Layer;
    protected List<Position> Directions;
    protected Position Exit;
    protected Position Stairs;
    protected Position Goal; 
    private bool _tripInProgress;
    private static readonly Random Rand = new();
    private List<Position>.Enumerator _path;
    protected int RiskLevel { get; set;}
    private List<ComplexAgent> Group { get; set; }
    protected int Pushiness { get; set; }
    protected int Health;
    protected bool IsLeader { get; set; }
    protected int Speed { get; set; }
    private ComplexAgent Leader { get; set; }
    protected int TickCount;
    protected bool IsInGroup;
    protected bool FirstAct;
    private const int MaxSpeed = 10;
    protected int Collaboration;
    private readonly double _attractiveForceMultiplier = Rand.NextDouble();
    private readonly int _repulsiveForceMultiplier = Rand.Next(1, 10);
    private readonly int _obstacleAvoidanceMultiplier = Rand.Next(1, 10);
    private Vector2 _currentVelocity = Vector2.Zero;
    #endregion
}