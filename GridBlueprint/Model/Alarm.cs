using System;
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
        Position = new Position(locX, locY);
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

            foreach (var agent in agents)
            {
                if (Distance.Chebyshev(new []{Position.X, Position.Y}, new []{agent.Position.X, agent.Position.Y}) <= 15)
                {
                    Console.WriteLine("Fire detected");
                    return true; 
                }
            }

            return false; 
    }
    

    #endregion

    #region  Fields and properties
    public Position Position { get; set; }
    public Guid ID { get; set; }
    public bool On; 
    public UnregisterAgent UnregisterAgentHandle { get; set; }
    private GridLayer _layer;
    
    [PropertyDescription(Name = "locX")]
    public int locX { get; set; }
    
    [PropertyDescription(Name = "locY")]
    public int locY { get; set; }


    #endregion
    

}