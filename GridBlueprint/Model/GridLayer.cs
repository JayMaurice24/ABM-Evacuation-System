using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Mars.Common.Core.Random;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;

namespace GridBlueprint.Model;

public class GridLayer : RasterLayer
{
    #region Init

    /// <summary>
    ///     The initialization method of the GridLayer which spawns and stores the specified number of each agent type
    /// </summary>
    /// <param name="layerInitData"> Initialization data that is passed to an agent manager which spawns the specified
    /// number of each agent type</param>
    /// <param name="registerAgentHandle">A handle for registering agents</param>
    /// <param name="unregisterAgentHandle">A handle for unregistering agents</param>
    /// <returns>A boolean that states if initialization was successful</returns>
    [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle,
        UnregisterAgent unregisterAgentHandle)
    {
        var initLayer = base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
        
        ComplexAgentEnvironment = new SpatialHashEnvironment<ComplexAgent>(Width, Height);
        ExitEnvironment = new SpatialHashEnvironment<Exits>(Width, Height);
        FireEnvironment = new SpatialHashEnvironment<Fire>(Width, Height);
        AlarmEnvironment = new SpatialHashEnvironment<Alarm>(Width, Height);
        
        AgentManager = layerInitData.Container.Resolve<IAgentManager>();
        Agent1 = AgentManager.Spawn<AgentType1, GridLayer>().ToList();
        Agent2 = AgentManager.Spawn<AgentType2, GridLayer>().ToList();
        Agent3 = AgentManager.Spawn<AgentType3, GridLayer>().ToList();
        Agent4 = AgentManager.Spawn<AgentType4, GridLayer>().ToList();
        Agent5 = AgentManager.Spawn<AgentType5, GridLayer>().ToList();
        Agent6 = AgentManager.Spawn<AgentType6, GridLayer>().ToList();
        Agent7 = AgentManager.Spawn<AgentType7, GridLayer>().ToList();
        Agent8 = AgentManager.Spawn<AgentType8, GridLayer>().ToList();
        Agent9 = AgentManager.Spawn<AgentType9, GridLayer>().ToList();
        Agent10 = AgentManager.Spawn<AgentType10, GridLayer>().ToList();
        Agent11 = AgentManager.Spawn<AgentType11, GridLayer>().ToList();
        Agent12 = AgentManager.Spawn<AgentType12, GridLayer>().ToList();
        Agent13 = AgentManager.Spawn<AgentType13, GridLayer>().ToList();
        Agent14 = AgentManager.Spawn<AgentType14, GridLayer>().ToList();
        Agent15 = AgentManager.Spawn<AgentType15, GridLayer>().ToList();
        Agent16 = AgentManager.Spawn<AgentType16, GridLayer>().ToList();
        Agent17 = AgentManager.Spawn<AgentType17, GridLayer>().ToList();
        Agent18 = AgentManager.Spawn<AgentType18, GridLayer>().ToList();
        Agent19 = AgentManager.Spawn<AgentType19, GridLayer>().ToList();
        Agent20 = AgentManager.Spawn<AgentType20, GridLayer>().ToList();
        Agent21 = AgentManager.Spawn<AgentType21, GridLayer>().ToList();
        Agent22 = AgentManager.Spawn<AgentType22, GridLayer>().ToList();
        Agent23 = AgentManager.Spawn<AgentType23, GridLayer>().ToList();
        Agent24 = AgentManager.Spawn<AgentType24, GridLayer>().ToList();
        Agent25 = AgentManager.Spawn<AgentType25, GridLayer>().ToList();
        Agent26 = AgentManager.Spawn<AgentType26, GridLayer>().ToList();
        Agent27 = AgentManager.Spawn<AgentType27, GridLayer>().ToList();
        
       
        var entityManager = Container.Resolve<IEntityManager>();
        HelperAgents = AgentManager.Spawn<HelperAgent, GridLayer>().ToList();
        Fires = AgentManager.Spawn<Fire, GridLayer>().ToList();
        Alarms = AgentManager.Spawn<Alarm, GridLayer>().ToList();

        var westExit = new Exits();
        var eastExit = new Exits();
        var mainExit = new Exits();
    
        mainExit.Initialize(new Position(58, 68), true, false);
        eastExit.Initialize(new Position(55, 70), true, false);
        westExit.Initialize(new Position(64, 70), true, false);

        var exitLocations = new List<Exits>
        {
            mainExit,
            westExit,
            eastExit
        };

        Exits = exitLocations;
        
        
        List<Position> stairLocation = new List<Position>()
        {
            new Position(58, 75),
            new Position(57, 75),
            new Position(56, 75),
            new Position(74, 30) ,
            new Position(74, 29)
        };
        Stairs = stairLocation;
        return initLayer;
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Checks if the grid cell (x,y) is accessible
    /// </summary>
    /// <param name="x">x-coordinate of grid cell</param>
    /// <param name="y">y-coordinate of grid cell</param>
    /// <returns>Boolean representing if (x,y) is accessible</returns>
    public override bool IsRoutable(int x, int y) => this[x, y] == 0;

    private bool NoSpawn(int x, int y)
    {
        switch (x)
        {
            case > 56 and < 64 when y is < 44 and > 33:
            case > 73 and < 94 when y is < 25 and > 19:
            case > 36 and < 41 when y is < 86 and > 77:
                return true;
            default:
                return false;
        }
    }
    public Position FindRandomPosition()
    {
        var random = RandomHelper.Random;
        bool check = true;
        int x = 0, y = 0;
        while (check)
        {   x = random.Next(Width);
            y = random.Next(Height);
            if (IsRoutable(x,y))
            {
                check = NoSpawn(x, y); 
                 
            }
        }
        return Position.CreatePosition(x, y); 
    }
    
    
    #endregion
    
   
    #region Fields and Properties

    /// <summary>
    ///     The environment of the SimpleAgent agents
    /// </summary>

    public List<Exits> Exits { get; private set; }
    public List<Position> Stairs { get; private set; }

    /// <summary>
    ///     The environment of the ComplexAgent agents
    /// </summary>
    public SpatialHashEnvironment<ComplexAgent> ComplexAgentEnvironment { get; set; }
    public SpatialHashEnvironment<Exits> ExitEnvironment { get; set; }
    public SpatialHashEnvironment<Fire> FireEnvironment { get; set; }
    public SpatialHashEnvironment<Alarm> AlarmEnvironment { get; set; }
    
    /// <summary>
    ///     A collection that holds the SimpleAgent instances
    /// </summary>
    public List<Fire> Fires { get; private set; }
    public List<Alarm> Alarms { get; private set; }
    /// <summary>
    ///     A collection that holds the ComplexAgent instances
    /// </summary>

    public List<AgentType1> Agent1 { get; private set; }
    public List<AgentType2> Agent2 { get; private set; }
    public List<AgentType3> Agent3 { get; private set; }
    public List<AgentType4> Agent4 { get; private set; }
    public List<AgentType5> Agent5 { get; private set; }
    public List<AgentType6> Agent6 { get; private set; }
    public List<AgentType7> Agent7 { get; private set; }
    public List<AgentType8> Agent8{ get; private set; }
    public List<AgentType9> Agent9 { get; private set; }
    public List<AgentType10> Agent10 { get; private set; }
    public List<AgentType11> Agent11 { get; private set; }
    public List<AgentType12> Agent12 { get; private set; }
    public List<AgentType13> Agent13 { get; private set; }
    public List<AgentType14> Agent14 { get; private set; }
    public List<AgentType15> Agent15 { get; private set; }
    public List<AgentType16> Agent16 { get; private set; }
    public List<AgentType17> Agent17 { get; private set; }
    public List<AgentType18> Agent18 { get; private set; }
    public List<AgentType19> Agent19 { get; private set; }
    public List<AgentType20> Agent20 { get; private set; }
    
    public List<AgentType21> Agent21 { get; private set; }
    public List<AgentType22> Agent22 { get; private set; }
    public List<AgentType23> Agent23 { get; private set; }
    public List<AgentType24> Agent24 { get; private set; }
    
    public List<AgentType25> Agent25 { get; private set; }
    public List<AgentType26> Agent26 { get; private set; }
    
    public List<AgentType27> Agent27 { get; private set; }
    

    public List<HelperAgent> HelperAgents { get; private set; }

    public IAgentManager AgentManager { get; private set; }
    public bool FireStarted { get; set; } = false;
    public bool Ring { get; set; } = false;
    public IEnumerable<ComplexAgent> ComplexAgents { get; set; }
    public bool FirstAct;

    #endregion
}