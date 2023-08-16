using System;
using System.Collections.Generic;
using Mars.Components.Agents;
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
        _state = AgentState.MoveTowardsGoal;  // Initial state of the agent. Is overwritten eventually in Tick()
        _directions = CreateMovementDirectionsList();
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
            var rand = new Random();
            var i = rand.Next(0, 2);
            _stairs = _layer.Stairs[i];
            _exit = FindNearestExit(Position, _layer.Exits);
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
    private void MoveTowardsGoal()
    {
        if (!_tripInProgress)
        {
            // Finds closest exit and moves towards exit 
            var rand = new Random();
            var i = rand.Next(0, 2);
            _exit = FindNearestExit(Position, _layer.Exits);
            _goal = _layer.Stairs[i];
            _path = _layer.FindPath(Position, _exit).GetEnumerator();
            _tripInProgress = true;
            
        }

        if (_path.MoveNext() &&  !AvoidFire())
        {
            _layer.ComplexAgentEnvironment.MoveTo(this, _path.Current, 1);
            if (Position.Equals(_exit))
            {
                _path = _layer.FindPath(Position, _goal).GetEnumerator();
                _tripInProgress = true;
                if (_path.MoveNext())
                {
                    _layer.ComplexAgentEnvironment.MoveTo(this, _path.Current, 1);
                    if (Position.Equals(_goal))
                    {
                        Console.WriteLine($"ComplexAgent {ID} reached goal {_goal}");
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
    private void MoveStraightToExit()
    {
        if (!_tripInProgress)
        {
            // Finds closest exit and moves towards exit 
            var rand = new Random();
            var i = rand.Next(0, 2);
            _goal = _layer.Stairs[i];
            _path = _layer.FindPath(Position, _exit).GetEnumerator();
            _tripInProgress = true;
        }

        if (_path.MoveNext() && !AvoidFire())
        {
            _layer.ComplexAgentEnvironment.MoveTo(this, _path.Current, 1);
            if (Position.Equals(_goal))
            {
                Console.WriteLine($"ComplexAgent {ID} reached goal {_goal}");
                RemoveFromSimulation();
                _tripInProgress = false;
                    }
                }
    }
    /// <summary>
     /// Finds the exit closest to the agent  
     /// </summary>
     /// <param name="currPos"></param>
     /// <param name="targets"></param>
    private Position FindNearestExit(Position currPos, List<Exits> targets)
    {
        Position nearestExit = null; 
        double shortestDistance = double.MaxValue;
        foreach (var p in targets)
        {
            double distance = CalculateDistance(currPos,p.Position);
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
    /// <param name="agentCoord"></param>
    /// <param name="exitCoord"></param>
    /// <returns></returns>
    private double CalculateDistance(Position agentCoord, Position exitCoord)
    {
        double dx = agentCoord.X - exitCoord.X; 
        double dy = agentCoord.Y - exitCoord.Y;
        return Math.Sqrt(dx * dx + dy * dy);

    }

    /// <summary>
    ///     Explores the environment for agents of another type and increments their counter if they are nearby.
    /// </summary>
    private void ExploreAgents()
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

    private bool AvoidFire()
    {
        var fire = _layer.FireEnvironment.Explore(Position, radius: 1); 
        
        foreach (var flame in fire)
        {
            if (Distance.Chebyshev(new []{Position.X, Position.Y}, new []{flame.Position.X, flame.Position.Y}) <= 1.0)
            {
                return true;
            }
        }

        return false; 
    }
    
    private void MoveRandomly()
    {
        var nextDirection = _directions[_random.Next(_directions.Count)];
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

    private bool IsCellOccupied(Position targetPosition)
    {
        foreach (var agent in _layer.ComplexAgents)
        {
            if (agent!= this && agent.Position.Equals(targetPosition)){ 
                return true;  //Cell is occupied
            }
        }
        return false; // The cell is not occupied
    }
/*
    private void Riskiness()
    {
        
    }

    private void Pushiness()
    {
        
    }

    private void Speed()
    {
        
    }*/
    /// <summary>
    ///     Removes this agent from the simulation and, by extension, from the visualization.
    /// </summary>
    private void RemoveFromSimulation()
    {
        Console.WriteLine($"ComplexAgent {ID} is removing itself from the simulation.");
        _layer.ComplexAgentEnvironment.Remove(this);
        UnregisterAgentHandle.Invoke(_layer, this);
    }
    public void IncrementCounter()
    {
        MeetingCounter += 1;
    }

    #endregion

    #region Fields and Properties

    public Guid ID { get; set; }
   
    public Position Position { get; set; }
    
    [PropertyDescription(Name = "MaxTripDistance")]
    public double MaxTripDistance { get; set; }
    
    [PropertyDescription(Name = "AgentExploreRadius")]

    public UnregisterAgent UnregisterAgentHandle { get; set; }
    
    private GridLayer _layer;
    private List<Position> _directions;
    private Position _goal;
    private Position _exit;
    private Position _stairs;
    private bool _tripInProgress;
    private AgentState _state;
    private readonly Random _random = new();
    private List<Position>.Enumerator _path;
    public int MeetingCounter { get; private set; }

    #endregion
}