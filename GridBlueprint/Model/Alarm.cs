using System;
using System.Linq;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using Mars.Numerics;
namespace GridBlueprint.Model;

public class Alarm : IAgent<GridLayer>, IPositionable
{
    
    #region Init
    public void Init(GridLayer layer){
        _layer = layer;
        Position = new Position(LocX, LocY);
        _layer.AlarmEnvironment.Insert(this);
    }

    #endregion

    #region Tick
        public void Tick()
        {
            if (DetectFire())
            {
                _layer.Ring = true; 
            }
        }
        

    #endregion

    #region Methods

    
    private bool DetectFire()
    {
            var agents = _layer.FireEnvironment.Explore(Position, radius: 15);

            if (!agents.Any(agent =>
                    Distance.Chebyshev(new[] { Position.X, Position.Y },
                        new[] { agent.Position.X, agent.Position.Y }) <= 15)) return false;
            Console.WriteLine("Fire detected");
            return true;

    }
    

    #endregion

    #region  Fields and properties
    public Position Position { get; set; }
    public Guid ID { get; set; }
    public bool On; 
    public UnregisterAgent UnregisterAgentHandle { get; set; }
    private GridLayer _layer;
    
    [PropertyDescription(Name = "locX")]
    public int LocX { get; set; }
    
    [PropertyDescription(Name = "locY")]
    public int LocY { get; set; }


    #endregion
    

}