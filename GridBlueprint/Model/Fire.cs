using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
namespace GridBlueprint.Model;


public class Fire : IAgent<GridLayer>, IPositionable
{
    #region Init
    
    public void Init(GridLayer layer)
    {
        _layer = layer;
        _startSpread = 5;
        _directions = CreateMovementDirectionsList();
    }


    #endregion

    #region Tick

  
    public void Tick()
    {
        if(_startSpread < _layer.GetCurrentTick() && _layer.GetCurrentTick()%10 == 0)
        {
            if(!_layer.FireStarted)
            {
                Position = _layer.FindRandomPosition(); 
                _layer.FireEnvironment.Insert(this);
                _layer.FireStarted = true; 
            }
            else
            {
                Spread();
            }
        }
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
        foreach (Position cell in _directions)
        {
            var newX = Position.X + cell.X;
            var newY = Position.Y + cell.Y;
            if (0 <= newX && newX < _layer.Width && 0 <= newY && newY < _layer.Height)
            {
                if (_layer.IsRoutable(newX, newY)){
                    SpreadFromPosition(new Position(newX, newY));
                }
            }
        }
    }


    private void SpreadFromPosition(Position position)
    {
        _layer.AgentManager.Spawn<Fire, GridLayer>(null, agent =>
        {
            agent.Position = position;
        }).Take(1).First();

        Console.WriteLine("Fire spread to: {0}", position);
    }
    

    #endregion

    #region Fields and Properties
    
  
    public Position Position { get; set; }
    public Guid ID { get; set; }
    public UnregisterAgent UnregisterAgentHandle { get; set; }
    private GridLayer _layer;
    private List<Position> _directions;
    private readonly Random _random = new();
    private int _startSpread;
    private bool _fireStarted = false; 

    #endregion
}