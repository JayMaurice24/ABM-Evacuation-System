using System;
using System.Linq;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
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
            if (!_layer.Ring)
            {
                _layer.Ring = DetectFire();
            }
        }
        

    #endregion

    #region Methods

    
    private bool DetectFire()
    {

        var fire = _layer.FireEnvironment.Entities.MinBy(flame =>
            Distance.Chebyshev(new[] { Position.X, Position.Y }, new[] { flame.Position.X, flame.Position.Y }));

        if (fire != null &&
            !(Distance.Chebyshev(new[] { Position.X, Position.Y }, new[] { fire.Position.X, fire.Position.Y }) <=
              10.0)) return false;
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