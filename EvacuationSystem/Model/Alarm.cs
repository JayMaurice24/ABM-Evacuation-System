using System;
using System.Linq;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using Mars.Numerics;
namespace EvacuationSystem.Model;

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
            if (!_layer.Ring &&  _layer.FireLocations != null)
            {
                _layer.Ring = SmokeDetector();
            }
        }
        

    #endregion

    #region Methods

    
    private bool SmokeDetector()
    {
        if (_layer.SmokeEnvironment == null) return false;
        var smoke = _layer.SmokeEnvironment.Explore(Position,12);
        if (smoke != null);
        Console.WriteLine("Fire detected");
        return true;

    }
    

    #endregion

    #region  Fields and properties
    public Position Position { get; set; }
    public Guid ID { get; set; }
    private GridLayer _layer;
    [PropertyDescription(Name = "locX")]
    public int LocX { get; set; }
    [PropertyDescription(Name = "locY")]
    public int LocY { get; set; }


    #endregion
    

}