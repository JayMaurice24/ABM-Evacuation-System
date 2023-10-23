using System;
using System.Linq;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;
using Mars.Numerics;

namespace EvacuationSystem.Model;


public class Fire : IAgent<GridLayer>, IPositionable
{
    #region Init
    
    public void Init(GridLayer layer)
    {
        _layer = layer;
        Position = !_layer.FireStarted ? _layer.FindRandomPosition() : _layer.SetSmokeOrFirePosition(1);
        _layer.FireLocations.Add(Position);
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
            HurtAgent();
        }
        else
        {
           HurtAgent();
        }
    }

    #endregion
        #region Methods
        /// <summary>
        /// Causes damage to agents 
        /// </summary>
        private void HurtAgent ()
        {
            var nearbyEvacuees = _layer.EvacueeEnvironment.Explore(Position, 1);
            if (nearbyEvacuees== null) return;
            foreach (var evacuee in nearbyEvacuees)
            {
                if (evacuee.Position.Equals(Position))
                {
                    evacuee.RemoveFromSimulation();
                    ModelOutput.NumDeaths++;
                }
                else
                {
                    evacuee.Health -= 10;
                }
            }
        }
    

    #endregion

    #region Fields and Properties
    public Position Position { get; set; }
    public Guid ID { get; set; }
    private GridLayer _layer;
    #endregion
}