using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;

namespace EvacuationSystem.Model;

public class Smoke : IAgent<GridLayer>, IPositionable
{

    #region Init

    public void Init(GridLayer layer)
    {
        _layer = layer;
        Directions = MovementDirections.CreateMovementDirectionsList();
        Density = _rand.NextDouble();
    }

    
    #endregion

    #region Tick
    public void Tick()
    {
        if (_layer.FireStarted && !_layer.SmokeSpread)
        {
            var firstFire = _layer.FireLocations[0];
            var cell = Directions[_rand.Next(Directions.Count)];
            Position = new Position(firstFire.X + cell.X, firstFire.Y + cell.Y);
            _layer.SmokeSpread = true;
            Spread();
        }
        else if (_layer.SmokeSpread)
        {
            if (_rand.NextDouble() <= 0.4)Spread();
        }
    }

    #endregion

    #region Methods
    
    private void Spread()
    {
        var randomDirection = _rand.Next(1, Directions.Count);
        for (var i = 0; i < randomDirection; i++ ){
            var numMovements = _rand.Next(0, Directions.Count - 1);
            var cell = Directions[numMovements];
            var newX = Position.X + cell.X;
            var newY = Position.Y + cell.Y;
            if (!(0 <= newX) || !(newX < _layer.Width) || !(0 <= newY) || !(newY < _layer.Height)) continue;
            if (!_layer.IsRoutable(newX, newY)) continue;
            _layer.AgentManager.Spawn<Smoke, GridLayer>(null, agent =>
            {
                agent.Position = new Position(newX, newY);
            }).Take(1).First();
        }
    }

    #endregion
    #region Fields & Properties

    
    public Guid ID { get; set; }
    public Position Position { get; set; }
    private double Density { get; set; }
    private readonly Random _rand = new Random();
    private List<Position> Directions { get; set; }
    private GridLayer _layer;

    #endregion




}