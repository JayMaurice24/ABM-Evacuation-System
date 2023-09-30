using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Mars.Common.Core.Collections;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using Mars.Numerics;

namespace EvacuationSystem.Model;


public class Fire : IAgent<GridLayer>, IPositionable
{
    #region Init
    
    public void Init(GridLayer layer)
    {
        _layer = layer;
        _directions = MovementDirections.CreateMovementDirectionsList();
        _startSpread = _rand.Next(10, 30); 
        if (!_layer.FireStarted)
        {
            Position = _layer.FindRandomPosition();
        }
        _layer.FireEnvironment.Insert(this);
    }


    #endregion

    #region Tick

    public void Tick()
    {
        if (!_layer.FireStarted)
        {
            Console.WriteLine($"Fire started in the {_layer.Room(Position)}");
            _layer.FireStarted = true;
            _layer.FireLocations.Add(Position);

        }
        else
        {
            if (_layer.GetCurrentTick()%_startSpread == 0)
            {
                if (_rand.NextDouble()> 0.7)
                {
                    Spread();
                }
                
            }

            HurtAgent();

        }
    }

    #endregion
        #region Methods
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
    
        private void SpreadFromPosition(Position position)
        {
            _layer.FireLocations.Add(position);
            _layer.AgentManager.Spawn<Fire, GridLayer>(null, agent => { agent.Position = position; });
            Console.WriteLine("Fire spread to: {0}", position);
        }

        private void HurtAgent ()
        {
            var agent = _layer.EvacueeEnvironment.Entities.MinBy(agent =>
                Distance.Chebyshev(Position.PositionArray, agent.Position.PositionArray));
            if (agent == null) return;
            var targetDistance = (int) Distance.Chebyshev(Position.PositionArray, agent.Position.PositionArray);
            if (targetDistance <= 2)
            {
                agent.Health -= 10;
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
    private readonly Random _rand = new Random();
    #endregion
}