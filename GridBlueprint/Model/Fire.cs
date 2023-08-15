using System;
using System.Collections.Generic;
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
        Position = _layer.FindRandomPosition();
        _startSpread = 5;
        _layer.FireEnvironment.Insert(this);
    }


    #endregion

    #region Tick

  
    public void Tick()
    {
        if (_startSpread < _layer.GetCurrentTick())
        {
            Spread(); 
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
        _directions = CreateMovementDirectionsList();
        var currPos = Position;

        foreach (Position cell in _directions)
        {
            var newX = currPos.X + cell.X;
            var newY = currPos.Y + cell.Y;
            if (0 <= newX && newX < _layer.Width && 0 <= newY && newY < _layer.Height)
            {
                if (_layer.IsRoutable(newX, newY)){
                    _layer.FireEnvironment.Insert(new Fire
                    {
                        Position = new Position(newX, newY)
                    });

                    Console.WriteLine("Fire spread to: {0}", Position);
                }
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
    private int _startSpread; 


    #endregion
}