using System;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;

namespace EvacuationSystem.Model;

public class Smoke : IAgent<GridLayer>, IPositionable
{

    #region Init

    public void Init(GridLayer layer)
    {
        _layer = layer;
        Density = _rand.NextDouble();
        Position = _layer.SetSmokeOrFirePosition(0);
        _layer.SmokeEnvironment.Insert(this);
    }

    
    #endregion

    #region Tick
    public void Tick()
    {
        if (!_layer.SmokeSpread)
        { 
            _layer.SmokeLocations.Add(Position);
            _layer.SmokeSpread = true;
            Damage();
        }
        else
        {
            Damage(); 
        }
        
    }

    #endregion

    #region Methods

    private void Damage()
    {
       var nearbyAgents = _layer.EvacueeEnvironment.Explore(Position ,1);
       if (nearbyAgents is null) return;
       foreach (var agent in nearbyAgents)
       { 
           agent.Health -= Density switch
           {
               < 0.3 => 1,
               < 0.7 => 3,
               _ => 5
           };
       }
    }


    #endregion
    #region Fields & Properties

    
    public Guid ID { get; set; }
    public Position Position { get; set; }
    private double Density { get; set; }
    private readonly Random _rand = new Random();
    private GridLayer _layer;

    #endregion




}