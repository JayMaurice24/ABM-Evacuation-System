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
    public void Init(GridLayer layer)
    { 
        _layer = layer;
        Position = _layer.FindRandomPosition();
        Directions = CreateMovementDirectionsList();
        _layer.ComplexAgentEnvironment.Insert(this);
    }

    #endregion

    #region Tick

    /// <summary>
    ///     The tick method of the ComplexAgent which is executed during each time step of the simulation.
    ///     A ComplexAgent can move randomly along straight lines. It must stay within the bounds of the GridLayer
    ///     and cannot move onto grid cells that are not routable.
    /// </summary>
    public void Tick()
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

    #endregion

    #region Methods

    /// <summary>
    ///     Generates a list of eight movement directions that the agent uses for random movement.
    /// </summary>
    /// <returns>The list of movement directions</returns>
    private static List<Position> CreateMovementDirectionsList()
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
    ///     Moves the agent one step along the shortest routable path towards a fixed goal.
    /// </summary>
    protected void MoveTowardsGoal()
    {
        if (!_tripInProgress)
        {
            // Finds closest exit and moves towards exit 
            _path = _layer.FindPath(Position, _exit).GetEnumerator();
            _tripInProgress = true;
            
        }

        if (_path.MoveNext())
        {
            if (AvoidFire())
            {
                _layer.ComplexAgentEnvironment.MoveTo(this, ChangeDirection(_path.Current), Speed);
            }
            else
            {
                _layer.ComplexAgentEnvironment.MoveTo(this, _path.Current, Speed);  
            }
            if (Position.Equals(_exit))
            {
                _path = _layer.FindPath(Position, _stairs).GetEnumerator();
                _tripInProgress = true;
                if (_path.MoveNext())
                {
                    _layer.ComplexAgentEnvironment.MoveTo(this, _path.Current, Speed);
                    if (Position.Equals(_stairs))
                    {
                        Console.WriteLine($"ComplexAgent {ID} reached goal {_stairs}");
                        RemoveFromSimulation();
                        _tripInProgress = false;
                    }
                }
            }
        }
    }
/// <summary>
/// Moves the agent straight towards the stairs if closer to the stairs
/// </summary>
    protected void MoveStraightToExit()
    {
        if (!_tripInProgress)
        {
            // Finds closest exit and moves towards exit 
            _path = _layer.FindPath(Position, _stairs).GetEnumerator();
            _tripInProgress = true;
        }

        if (_path.MoveNext())
        {
            if (AvoidFire())
            {
                _layer.ComplexAgentEnvironment.MoveTo(this, ChangeDirection(_path.Current), Speed);
            }
            else
            {
                _layer.ComplexAgentEnvironment.MoveTo(this, _path.Current, Speed);  
            }
            if (Position.Equals(_stairs))
            {
                Console.WriteLine($"ComplexAgent {ID} reached goal {_stairs}");
                RemoveFromSimulation();
                _tripInProgress = false;
            }
        }
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
    /// Calculates the distance between the agent's current position and the exit
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
    protected Position ChangeDirection(Position position)
    {
        var fire = _layer.FireEnvironment.Entities.MinBy(flame =>
            Distance.Chebyshev(new[] { Position.X, Position.Y }, new[] { flame.Position.X, flame.Position.Y }));

        if (fire != null)
        {
            if (position.Equals(fire.Position))
            {
                foreach (var next in Directions)
                {
                    var newX = Position.X + next.X;
                    var newY = Position.Y + next.Y;

                    if (0 <= newX && newX < _layer.Width && 0 <= newY && newY < _layer.Height &&
                        _layer.IsRoutable(newX, newY) && AvoidFire() == false)
                    {
                        var nextCell = _layer.FireEnvironment.Entities.FirstOrDefault(flame =>
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
    protected void ExploreAgents()
    {
        var agents = _layer.ComplexAgentEnvironment.Explore(Position, radius: 5);

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
    protected bool AvoidFire()
    {
        var fire = _layer.FireEnvironment.Explore(Position, radius: 1); 
        
        foreach (var flame in fire)
        {
            if (Distance.Chebyshev(new []{Position.X, Position.Y}, new []{flame.Position.X, flame.Position.Y}) <= 2.0)
            {
                return true;
            }
        }

        return false; 
    }
    
    protected void MoveRandomly()
    {
        var nextDirection = Directions[_random.Next(Directions.Count)];
        var newX = Position.X + nextDirection.X;
        var newY = Position.Y + nextDirection.Y;
        
        // Check if chosen move is within the bounds of the grid
        if (0 <= newX && newX < _layer.Width && 0 <= newY && newY < _layer.Height)
        {
            // Check if chosen move goes to a cell that is routable
            if (_layer.IsRoutable(newX, newY))
            {
                Position = new Position(newX, newY);
                _layer.ComplexAgentEnvironment.MoveTo(this, new Position(newX, newY));
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
    protected bool IsCellOccupied(Position targetPosition)
    {
        var agents = _layer.ComplexAgentEnvironment.Entities.MinBy(agent =>
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
    protected void RemoveFromSimulation()
    {
        Console.WriteLine($"ComplexAgent {ID} is removing itself from the simulation.");
        _layer.ComplexAgentEnvironment.Remove(this);
        UnregisterAgentHandle.Invoke(_layer, this);
    }
    public void IncrementCounter()
    {
        MeetingCounter += 1;
    }
    
        protected void Low()
        {
            if (IsCellOccupied(Position))
            {
                // Do nothing and wait for the cell to become unoccupied
            }
            else
            {
                MoveRandomly();
            }
        }

        protected void Medium()
        {
            if (IsCellOccupied(Position))
            {
                ComplexAgent otherAgent = GetAgentAt(Position);
                if (otherAgent != null)
                {
                    if (otherAgent._pushiness < _pushiness)
                    {
                        PushAgent(otherAgent);
                    }
                    else
                    {
                        // Do nothing and wait for the other agent to move
                    }
                }
            }
            else
            {
                MoveRandomly();
            }
        }

        protected void High()
        {
            if (IsCellOccupied(Position))
            {
                ComplexAgent otherAgent = GetAgentAt(Position);
                if (otherAgent != null)
                {
                    PushAgent(otherAgent);
                }
            }
            else
            {
                MoveRandomly();
            }
        }

        protected void PushAgent(ComplexAgent otherAgent)
        {
            // Calculate the direction to push the other agent
            var pushDirectionX = otherAgent.Position.X - Position.X;
            var pushDirectionY = otherAgent.Position.Y - Position.Y;

            // Calculate the new position for the other agent after the push
            var newAgentX = otherAgent.Position.X + pushDirectionX;
            var newAgentY = otherAgent.Position.Y + pushDirectionY;

            // Check if the new position is within the bounds of the grid
            if (0 <= newAgentX && newAgentX < _layer.Width &&
                0 <= newAgentY && newAgentY < _layer.Height)
            {
                // Check if the new position is routable
                if (_layer.IsRoutable(newAgentX, newAgentY))
                {
                    // Move the other agent to the new position
                    otherAgent.Position = new Position(newAgentX, newAgentY);
                    _layer.ComplexAgentEnvironment.MoveTo(otherAgent, new Position(newAgentX, newAgentY));
                }
            }
        }


        protected ComplexAgent GetAgentAt(Position position)
        {
            // Iterate through the list of agents in the environment
            foreach (var agent in _layer.ComplexAgentEnvironment.Entities)
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
    
    protected GridLayer _layer;
    protected List<Position> Directions;
    protected Position _exit;
    protected Position _stairs;
    protected bool _tripInProgress;
    protected Aggression _pushiness;
    protected readonly Random _random = new();
    protected List<Position>.Enumerator _path;
    protected int MeetingCounter { get; private set; }
    protected int RiskLevel { get; set;}
    protected int Speed { get; set; }


    #endregion
}