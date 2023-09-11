using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Mars.Common.Core.Random;
using Mars.Components.Agents;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Interfaces.Agents;
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
        SmokeEnvironment = new SpatialHashEnvironment<Smoke>(Width, Height);
        
        AgentManager = layerInitData.Container.Resolve<IAgentManager>();
        HelperAgents = AgentManager.Spawn<HelperAgent, GridLayer>().ToList();
        Fires = AgentManager.Spawn<Fire, GridLayer>().ToList();
        Smokes = AgentManager.Spawn<Smoke, GridLayer>().ToList();
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
        FireLocations = new List<Position>();
        
        
        List<Position> stairLocation = new List<Position>()
        {
            new Position(58, 75),
            new Position(57, 75),
            new Position(56, 75),
            new Position(74, 30) ,
            new Position(74, 29)
        };
        Stairs = stairLocation;
        SpawnAgents();
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

    
    private void SpawnAgents()
    {
        for (var x = 1; x <= 18; x++)
        {
            var numAgents = Rand.Next(1, 10);
            for (var i = 0; i <= numAgents; i++)
            {
                switch (x)
                {
                    case 1:
                        Agent1.AddRange(AgentManager.Spawn<AgentType1, GridLayer>().ToList());
                        break;
                    case 2:
                        Agent2.AddRange(AgentManager.Spawn<AgentType2, GridLayer>().ToList());
                        break;
                    case 3:
                        Agent3.AddRange(AgentManager.Spawn<AgentType3, GridLayer>().ToList());
                        break;
                    case 4:
                        Agent4.AddRange(AgentManager.Spawn<AgentType4, GridLayer>().ToList());
                        break;
                    case 5:
                        Agent5.AddRange(AgentManager.Spawn<AgentType5, GridLayer>().ToList());
                        break;
                    case 6:
                        Agent6.AddRange(AgentManager.Spawn<AgentType6, GridLayer>().ToList());
                        break;
                    case 7:
                        Agent7.AddRange(AgentManager.Spawn<AgentType7, GridLayer>().ToList());
                        break;
                    case 8:
                        Agent8.AddRange(AgentManager.Spawn<AgentType8, GridLayer>().ToList());
                        break;
                    case 9:
                        Agent9.AddRange(AgentManager.Spawn<AgentType9, GridLayer>().ToList());
                        break;
                    case 10:
                        Agent10.AddRange(AgentManager.Spawn<AgentType10, GridLayer>().ToList());
                        break;
                    case 11:
                        Agent11.AddRange(AgentManager.Spawn<AgentType11, GridLayer>().ToList());
                        break;
                    case 12:
                        Agent12.AddRange(AgentManager.Spawn<AgentType12, GridLayer>().ToList());
                        break;
                    case 13:
                        Agent13.AddRange(AgentManager.Spawn<AgentType13, GridLayer>().ToList());
                        break;
                    case 14:
                        Agent14.AddRange(AgentManager.Spawn<AgentType14, GridLayer>().ToList());
                        break;
                    case 15:
                        Agent15.AddRange(AgentManager.Spawn<AgentType15, GridLayer>().ToList());
                        break;
                    case 16:
                        Agent16.AddRange(AgentManager.Spawn<AgentType16, GridLayer>().ToList());
                        break;
                    case 17:
                        Agent17.AddRange(AgentManager.Spawn<AgentType17, GridLayer>().ToList());
                        break;
                    case 18:
                        Agent18.AddRange(AgentManager.Spawn<AgentType18, GridLayer>().ToList());
                        break;

                }
            }
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

    public string Room(Position coordinate)
    {
        switch (coordinate.X)
        {
            case > 1 and < 19 when coordinate.Y is > 50 and < 87: //Coral
                return "Coral Seminar room";
            case > 21 and < 54 when coordinate.Y is > 52 and < 84: //West
                return "Undergraduate West labs";
            case > 65 and < 100 when coordinate.Y is > 52 and < 84: //East
                return "Undergraduate East labs";
            case > 102 and < 120 when coordinate.Y is > 42 and < 87: //Braae
                return "Braae labs";
            case > 44 and < 120 when coordinate.Y is > 1 and < 15: //CSHons
                return "CS Hons lab";
            case > 77 and < 86 when coordinate.Y is > 41 and < 45: //FBathroom
                return "Female Bathroom"; 
            case > 86 and < 94 when coordinate.Y is > 38 and < 45: //FBathroom2
                return "Female Bathroom"; 
            case > 77 and < 86 when coordinate.Y is > 32 and < 36: //MBathroom 
                return "Male Bathroom";
            case > 86 and < 94 when coordinate.Y is > 32 and < 38: //MBathroom2 
                return "Male Bathroom";
            case > 44 and < 63 when coordinate.Y is > 15 and < 32: //Atrium
                return "Atrium Seminar room";
            case > 96 and < 120 when coordinate.Y is > 16 and < 32: //SysDev
                return "SysDev Seminar room";
            case > 14 and < 102 when coordinate.Y is > 46 and < 50: //Passage1
                return "Corridor";
            case > 50 and < 53 when coordinate.Y is > 51 and < 70: //Passage2
                return "Corridor";
            case > 56 and < 65 when coordinate.Y is > 45 and < 68: //Passage3
                return "Corridor";
            case > 63 and < 94 when coordinate.Y is > 26 and < 31: //Passage4
                return "Corridor";
            case > 63 and < 94 when coordinate.Y is > 15 and < 19: //Passage5
                return "Corridor";
            case > 63 and < 73 when coordinate.Y is > 19 and < 31: //Passage6
                return "Corridor";
            default:
                return "In Passage";
        }
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
    public SpatialHashEnvironment<Smoke> SmokeEnvironment { get; set; }
    public SpatialHashEnvironment<Alarm> AlarmEnvironment { get; set; }
    
    /// <summary>
    ///     A collection that holds the SimpleAgent instances
    /// </summary>
    public List<Fire> Fires { get; private set; }
    public List<Smoke> Smokes { get; private set; }
    public List<Alarm> Alarms { get; private set; }

    /// <summary>
    ///     A collection that holds the ComplexAgent instances
    /// </summary>
    private List<AgentType1> Agent1 { get; set; } = new List<AgentType1>();
    private List<AgentType2> Agent2 { get; set; } = new List<AgentType2>();
    private List<AgentType3> Agent3 { get; set; } = new List<AgentType3>();
    private List<AgentType4> Agent4 { get; set; } = new List<AgentType4>();
    private List<AgentType5> Agent5 { get; set; } = new List<AgentType5>();
    private List<AgentType6> Agent6 { get; set; } = new List<AgentType6>();
    private List<AgentType7> Agent7 { get; set; } = new List<AgentType7>();
    private List<AgentType8> Agent8 { get; set; } = new List<AgentType8>();
    private List<AgentType9> Agent9 { get; set; } = new List<AgentType9>();
    private List<AgentType10> Agent10 { get; set; } = new List<AgentType10>();
    private List<AgentType11> Agent11 { get; set; } = new List<AgentType11>();
    private List<AgentType12> Agent12 { get; set; } = new List<AgentType12>();
    private List<AgentType13> Agent13 { get; set; } = new List<AgentType13>();
    private List<AgentType14> Agent14 { get; set; } = new List<AgentType14>();
    private List<AgentType15> Agent15 { get; set; } = new List<AgentType15>();
    private List<AgentType16> Agent16 { get; set; } = new List<AgentType16>();
    private List<AgentType17> Agent17 { get; set; } = new List<AgentType17>();
    private List<AgentType18> Agent18 { get; set; } = new List<AgentType18>(); 
    public List<Position> FireLocations { get; set; }
    private int _numAgents;
    private static readonly Random Rand = new Random();
    public List<HelperAgent> HelperAgents { get; private set; }

    public IAgentManager AgentManager { get; private set; }
    public bool FireStarted { get; set; } = false;
    public bool SmokeSpread { get; set; } = false;
    public bool Ring { get; set; } = false;
    public IEnumerable<ComplexAgent> ComplexAgents { get; set; }
    public bool FirstAct;

    #endregion
}