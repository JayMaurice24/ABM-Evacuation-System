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


public class Fire : IAgent<GridLayer>, IPositionable
{
    #region Init
    
    public void Init(GridLayer layer)
    {
        _layer = layer;
        Position = _layer.FindRandomPosition();
        _state = AgentState.MoveRandomly;  // Initial state of the agent. Is overwritten eventually in Tick()
        _layer.FireEnvironment.Insert(this);
    }


    #endregion

    #region Tick

    public void Tick()
    {
        Spread();
    }

    #endregion
    #region Methods
    /// <summary>
    /// Creates list of movement directions
    /// </summary>
    /// <returns></returns>
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
    /// Spreads fire across a radius 
    /// </summary>
    private void Spread()
    {
        _directions = CreateMovementDirectionsList();

        foreach (Position cell in _directions)
        {
            if (_layer.IsRoutable(cell.X, cell.Y))
            {
                _layer.FireEnvironment.Insert(new Fire
                {
                    Position = cell
                });

                Console.WriteLine("Fire spread to: {0}", cell);
            }
        }
    }

    #endregion

    #region Fields and Properties
    
  
    public Position Position { get; set; }
    public Guid ID { get; set; }
    public UnregisterAgent UnregisterAgentHandle { get; set; }
    private GridLayer _layer;
    private List<Position> _directions;
    private readonly Random _random = new();
    private AgentState _state;
    private List<Position>.Enumerator _path;

    #endregion
}