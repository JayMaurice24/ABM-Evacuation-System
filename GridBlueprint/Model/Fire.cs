using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Mars.Interfaces.Agents;
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
        _startSpread = _rand.Next(10, 20);
        _directions = CreateMovementDirectionsList(); 
        _expand = _rand.Next(20, 30);
        _spreadCounter = 0; 
    }


    #endregion

    #region Tick

  
    public void Tick()
    {
        if (_startSpread < _layer.GetCurrentTick()) return;
        if(!_layer.FireStarted)
        {
            Position = _layer.FindRandomPosition(); 
            _layer.FireEnvironment.Insert(this);
            _layer.FireStarted = true; 
        }
        else
        {
            if (_spreadCounter == _expand)
            {
                Spread();
                Kill();
                _expand = _rand.Next(20, 30);
                _spreadCounter = 0; 
            }
            else
            {
                _spreadCounter++; 
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
        var randomDirection = _rand.Next(1, _directions.Count);
        for (var i = 0; i < randomDirection; i++ ){
            var numMovements = _rand.Next(0, _directions.Count - 1);
            var cell = _directions[numMovements];
            var newX = Position.X + cell.X;
            var newY = Position.Y + cell.Y;
            if (!(0 <= newX) || !(newX < _layer.Width) || !(0 <= newY) || !(newY < _layer.Height)) continue;
            if (_layer.IsRoutable(newX, newY)){
                SpreadFromPosition(new Position(newX, newY));
            }
        }
    }


    [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
    private void SpreadFromPosition(Position position)
    {
        _layer.AgentManager.Spawn<Fire, GridLayer>(null, agent =>
        {
            agent.Position = position;
        }).Take(1).First();

        Console.WriteLine("Fire spread to: {0}", position);
    }

    private void Kill ()
    {
        var agent = _layer.ComplexAgentEnvironment.Entities.MinBy(agent =>
            Distance.Chebyshev(Position.PositionArray, agent.Position.PositionArray));
        if (agent == null) return;
        var targetDistance = (int) Distance.Chebyshev(Position.PositionArray, agent.Position.PositionArray);
        if (targetDistance <= 1)
        {
            agent.RemoveFromSimulation();
        }
    }

    #endregion

    #region Fields and Properties
    
  
    public Position Position { get; set; }
    public Guid ID { get; set; }
    public UnregisterAgent UnregisterAgentHandle { get; set; }
    private GridLayer _layer;
    private List<Position> _directions;
    private int _startSpread;
    private int _expand;
    private int _spreadCounter; 
    private readonly Random _rand = new Random();
    #endregion
}