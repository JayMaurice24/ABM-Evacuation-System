using System;
using System.Collections.Generic;
using System.Linq;
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
            _path = Layer.FindPath(Position, Exit).GetEnumerator();
            _tripInProgress = true;
            
        }

        if (!_path.MoveNext()) return;
        Layer.ComplexAgentEnvironment.MoveTo(this, AvoidFire() ? ChangeDirection(_path.Current) : _path.Current, 1);

        if (!IsCellOccupied(_path.Current))
        {
            Layer.ComplexAgentEnvironment.MoveTo(this, _path.Current, Speed);  
        }

        if (!Position.Equals(Exit)) return;
        _path = Layer.FindPath(Position, Stairs).GetEnumerator();
        _tripInProgress = true;
        if (!_path.MoveNext()) return;
        Layer.ComplexAgentEnvironment.MoveTo(this, _path.Current, Speed);
        if (!Position.Equals(Stairs)) return;
        Console.WriteLine($"ComplexAgent {ID} reached goal {Stairs}");
        RemoveFromSimulation();
        _tripInProgress = false;
    }
    
   /// <summary>
   /// Agent Moves towards goal with Medium Aggression 
   /// </summary>
    protected void MoveTowardsGoalMedium()
    {
        if (!_tripInProgress)
        {
            // Finds closest exit and moves towards exit 
            _path = Layer.FindPath(Position, Exit).GetEnumerator();
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

        if (!Position.Equals(Exit)) return;
        _path = Layer.FindPath(Position, Stairs).GetEnumerator();
        _tripInProgress = true;
        if (!_path.MoveNext()) return;
        Layer.ComplexAgentEnvironment.MoveTo(this, _path.Current, Speed);
        if (!Position.Equals(Stairs)) return;
        Console.WriteLine($"ComplexAgent {ID} reached goal {Stairs}");
        RemoveFromSimulation();
        _tripInProgress = false;
    }
   
   /// <summary>
   /// Agent moves towards goal with high aggression 
   /// </summary>
    protected void MoveTowardsGoalHigh()
    {
        if (!_tripInProgress)
        {
            // Finds closest exit and moves towards exit 
            _path = Layer.FindPath(Position, Exit).GetEnumerator();
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

        if (!Position.Equals(Exit)) return;
        _path = Layer.FindPath(Position, Stairs).GetEnumerator();
        _tripInProgress = true;
        if (!_path.MoveNext()) return;
        Layer.ComplexAgentEnvironment.MoveTo(this, _path.Current, Speed);
        if (Position.Equals(Stairs))
        {
            Console.WriteLine($"ComplexAgent {ID} reached goal {Stairs}");
            RemoveFromSimulation();
            _tripInProgress = false;
        }
    }
/// <summary>
/// Moves the agent straight towards the stairs if closer to the stairs
/// </summary>
protected void MoveStraightToExitLow()
    {
        if (!_tripInProgress)
        {
            // Finds closest exit and moves towards exit 
            _path = Layer.FindPath(Position, Stairs).GetEnumerator();
            _tripInProgress = true;
        }

        if (!_path.MoveNext()) return;
        Layer.ComplexAgentEnvironment.MoveTo(this, AvoidFire() ? ChangeDirection(_path.Current) : _path.Current, 1);
        if (!IsCellOccupied(_path.Current))
        {
            Layer.ComplexAgentEnvironment.MoveTo(this, _path.Current, Speed);  
        }

        if (!Position.Equals(Stairs)) return;
        Console.WriteLine($"ComplexAgent {ID} reached goal {Stairs}");
        RemoveFromSimulation();
        _tripInProgress = false;
    }
protected void MoveStraightToExitMedium()
{
    if (!_tripInProgress)
    {
        // Finds closest exit and moves towards exit 
        _path = Layer.FindPath(Position, Stairs).GetEnumerator();
        _tripInProgress = true;
    }

    if (!_path.MoveNext()) return;
    Layer.ComplexAgentEnvironment.MoveTo(this, AvoidFire() ? ChangeDirection(_path.Current) : _path.Current, 1);
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

    if (!Position.Equals(Stairs)) return;
    Console.WriteLine($"ComplexAgent {ID} reached goal {Stairs}");
    RemoveFromSimulation();
    _tripInProgress = false;
}
protected void MoveStraightToExitHigh()
{
    if (!_tripInProgress)
    {
        // Finds closest exit and moves towards exit 
        _path = Layer.FindPath(Position, Stairs).GetEnumerator();
        _tripInProgress = true;
    }

    if (!_path.MoveNext()) return;
    Layer.ComplexAgentEnvironment.MoveTo(this, AvoidFire() ? ChangeDirection(_path.Current) : _path.Current, Speed);
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

    if (!Position.Equals(Stairs)) return;
    Console.WriteLine($"ComplexAgent {ID} reached goal {Stairs}");
    RemoveFromSimulation();
    _tripInProgress = false;
}
    /// <summary>
     /// Finds the exit closest to the agent  
     /// </summary>
    protected Position FindNearestExit(List<Exits> targets)
    {
        Position nearestExit = null; 
        double shortestDistance = double.MaxValue;
        foreach (var p in targets)
        {
            double distance = CalculateDistance(Position, p.Position); 
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestExit = p.Position; 
            }
        }

        return nearestExit;
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
    ///     Explores the environment for agents of another type and increments their counter if they are nearby.
    /// </summary>
    private void ExploreAgents()
    {
        var agents = Layer.ComplexAgentEnvironment.Explore(Position, radius: 5);

        foreach (var agent in agents)
        {
            if (Distance.Chebyshev(new []{Position.X, Position.Y}, new []{agent.Position.X, agent.Position.Y}) <= 1.0)
            {
                agent.IncrementCounter();
            }
        }
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
/// <summary>
/// Agents Move Randomly before tick
/// </summary>
    protected void MoveRandomly()
    {
        var nextDirection = Directions[Random.Next(Directions.Count)];
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
    public void IncrementCounter()
    {
        MeetingCounter += 1;
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
    private bool _tripInProgress;
    protected readonly Random Random = new();
    private List<Position>.Enumerator _path;
    protected int MeetingCounter { get; private set; }
    protected int RiskLevel { get; set;}
    protected int Pushiness { get; set; }
    protected int Speed { get; set; }
    protected int TickCount; 

    #endregion
}