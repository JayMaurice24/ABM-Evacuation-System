using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Common;
using Mars.Components.Agents;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using Mars.Numerics;
namespace GridBlueprint.Model;

public class Alarm : IAgent<GridLayer>, IPositionable
{
    public Guid ID { get; set; }
    public bool On = false;
   

    #region Init
    public void Init(GridLayer layer){
        _layer = layer;
        Position = _layer.FindRandomPosition();
        _state = AgentState.MoveTowardsGoal;  // Initial state of the agent. Is overwritten eventually in Tick()
        //_layer.ComplexAgentEnvironment.Insert(this);
    }

    #endregion

    #region Tick
        public void Tick()
        {
            throw new NotImplementedException();
        }
        

    #endregion

    #region Methods

    private void detectFire()
    {
        
    }

    #endregion

    #region  Fields and properties

    public Position Position { get; set; }
    public Guid ID { get; set; }
    public UnregisterAgent UnregisterAgentHandle { get; set; }
    private GridLayer _layer;
    private List<Position> _directions;
    private readonly Random _random = new();
    private AgentState _state;
    private List<Position>.Enumerator _path;

    #endregion
    

}