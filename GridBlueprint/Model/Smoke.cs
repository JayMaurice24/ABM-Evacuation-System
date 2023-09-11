using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;
using Mars.Numerics;

namespace GridBlueprint.Model;

public class Smoke : IAgent<GridLayer>, IPositionable
{

    #region Init

    public void Init(GridLayer layer)
    {
        _layer = layer;
        Directions = MovementDirections.CreateMovementDirectionsList();
    }

    
    #endregion

    #region Tick

    [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
    public void Tick()
    {
        if (_layer.FireStarted && !_layer.SmokeSpread)
        {
            var firstFire = _layer.FireLocations[0];
            var cell = Directions[_rand.Next(Directions.Count)];
            Position = new Position(firstFire.X + cell.X, firstFire.Y + cell.Y);
            Spread();
            _layer.SmokeSpread = true;
        }
        else if (_layer.SmokeSpread)
        {
            if (_rand.NextDouble() >= 0.4) Spread();
            Damage();

        }
    }

    #endregion

    #region Methods
    
    [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH", MessageId = "type: <DoImpl>d__7`1[GridBlueprint.Model.Smoke]")]
    private void Spread()
    {
        var randomDirection = _rand.Next(1, Directions.Count);
        for (var i = 0; i < randomDirection; i++ ){
            var numMovements = _rand.Next(0, Directions.Count - 1);
            var cell = Directions[numMovements];
            var newX = Position.X + cell.X;
            var newY = Position.Y + cell.Y;
            if (!(0 <= newX) || !(newX < _layer.Width) || !(0 <= newY) || !(newY < _layer.Height)) continue;
            if (_layer.IsRoutable(newX, newY)){
                _layer.AgentManager.Spawn<Smoke, GridLayer>(null, agent => { agent.Position = new Position(newX, newY); }).Take(1).First();
            }
        }
    }
    
    
    /// <summary>
    /// Causes Health Damage to agents, everytime they inhale smoke; 
    /// </summary>
    [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
    private void Damage()
    {
        var agent = _layer.ComplexAgentEnvironment.Entities.MinBy(agent =>
            Distance.Chebyshev(Position.PositionArray, agent.Position.PositionArray));
        if (agent == null) return;
        var targetDistance = (int) Distance.Chebyshev(Position.PositionArray, agent.Position.PositionArray);
        if (targetDistance <= 1 && Position.Equals(agent.Position)){ agent.Health--; Console.WriteLine($"{agent.GetType().Name} {agent.ID} Inhaled Smoke");}
        
    }
    

    #endregion
    #region Fields & Properties

    
    public Guid ID { get; set; }
    public Position Position { get; set; }
    private float Density { get; set; }
    private readonly Random _rand = new Random();
    private List<Position> Directions { get; set; }
    private GridLayer _layer;

    #endregion




}