using System;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using Mars.Numerics;
namespace GridBlueprint.Model;

public class Alarm : IAgent<GridLayer>, IPositionable
{
    
    #region Init
    public void Init(GridLayer layer){
        _layer = layer;
        _state = AgentState.ExploreAgents;  // Initial state of the agent. Is overwritten eventually in Tick()
        Position = _layer.FindRandomPosition();
        _layer.AlarmEnvironment.Insert(this);
    }

    #endregion

    #region Tick
        public void Tick()
        {
            On = DetectFire();
        }
        

    #endregion

    #region Methods

    
    private bool DetectFire()
    {
            var agents = _layer.FireEnvironment.Explore(Position, radius: 10);

            foreach (var agent in agents)
            {
                if (Distance.Chebyshev(new []{Position.X, Position.Y}, new []{agent.Position.X, agent.Position.Y}) <= 10)
                {
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
    private AgentState _state;

    #endregion
    

}