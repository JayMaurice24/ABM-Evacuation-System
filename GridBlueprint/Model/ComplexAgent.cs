using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Common;
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
        
        
        
        MoveTowardsGoal();

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
            // Explore nearby grid cells based on their values
            Position exit = FindNearestExit(Position, _layer.Exits);
            _path = _layer.FindPath(Position, exit).GetEnumerator();
            _tripInProgress = true;
        }
        
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
    
    /// <summary>
     /// Finds the exit closest to the agent  
     /// </summary>
     /// <param name="currPos"></param>
     /// <param name="targets"></param>
    private Position FindNearestExit(Position currPos, List<Position> targets)
    {
        Position nearestExit = null; 
        double shortestDistance = double.MaxValue;
        foreach (var p in targets)
        {
            double distance = CalculateDistance(currPos,p);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestExit = p; 
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
        // Explore nearby other SimpleAgent instances
        var agents = _layer.ComplexAgentEnvironment.Explore(Position, radius: AgentExploreRadius);

        foreach (var agent in agents)
        {
            if (Distance.Chebyshev(new []{Position.X, Position.Y}, new []{agent.Position.X, agent.Position.Y}) <= 1.0)
            {
                agent.IncrementCounter();
            }
        }
    }

    /// <summary>
    ///     Selects a new state from the AgentState enumeration to guide for subsequent behavior.
    ///     Will return the current state if a route is still in progress.
    /// </summary>
    /// <returns>The selected state</returns>
    private AgentState RandomlySelectNewState()
    {
        if (_state == AgentState.MoveTowardsGoal && _tripInProgress)
        {
            Console.WriteLine("Trip still in progress, so no state change.");
            return AgentState.MoveTowardsGoal;
        }

        var agentStates = Enum.GetValues(typeof(AgentState));
        var newState = (AgentState) agentStates.GetValue(_random.Next(agentStates.Length))!;
        Console.WriteLine($"New state: {newState}");
        return newState;
    }

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
    public double AgentExploreRadius { get; set; }
    
    public UnregisterAgent UnregisterAgentHandle { get; set; }
    
    private GridLayer _layer;
    private List<Position> _directions;
    private readonly Random _random = new();
    private Position _goal;
    private bool _tripInProgress;
    private AgentState _state;
    private List<Position>.Enumerator _path;
    public int MeetingCounter { get; private set; }

    #endregion
}