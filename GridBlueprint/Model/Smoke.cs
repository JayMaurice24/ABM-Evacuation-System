using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mars.Common.Core.Collections;
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

    public void Tick()
    {
        if (!_layer.FireStarted || (_layer.GetCurrentTick() % 3) != 0) return;
        if (!_layer.SmokeSpread)
        {
            var fire = _layer.FireEnvironment.Explore().Take(1).First();
            var cell = Directions[_rand.Next(Directions.Count)];
            Position = new Position(fire.Position.X + cell.X, fire.Position.Y + cell.Y);
            Spread();
            _layer.SmokeSpread = true; 
        }
        else
        {
            Spread();
        }
        Damage();
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
            if (_layer.IsRoutable(newX, newY)){
                ExpandSmoke(new Position(newX, newY));
            }
        }
    }

    private void ExpandSmoke(Position position)
    {
        if (_layer != null)
        {
            Debug.Assert(_layer != null, nameof(_layer) + " != null");
            var first = _layer.AgentManager.Spawn<Smoke, GridLayer>(null, agent => { agent.Position = position; })
                .Take(1).First();
            _layer.SmokeEnvironment.Insert(first); 
        }

        Console.WriteLine("Smoke spread to: {0}", position);
    }
    
    /// <summary>
    /// Causes Health Damage to agents, everytime they inhale smoke; 
    /// </summary>
    private void Damage()
    {
        var agent = _layer.ComplexAgentEnvironment.Entities.MinBy(agent =>
            Distance.Chebyshev(Position.PositionArray, agent.Position.PositionArray));
        if (agent == null) return;
        var targetDistance = (int) Distance.Chebyshev(Position.PositionArray, agent.Position.PositionArray);
        if (targetDistance <= 1 && Position.Equals(agent.Position)) agent.Health--;
        
    }
    

    #endregion
    #region Fields & Properties

    
    public Guid ID { get; set; }
    public Position Position { get; set; }
    private float Density { get; set; }
    private float Speed { get; set; }
    private readonly Random _rand = new Random();
    private List<Position> Directions { get; set; }
    private GridLayer _layer;

    #endregion




}