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

namespace EvacuationSystem.Model;

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
        if (registerAgentHandle == null) throw new ArgumentNullException(nameof(registerAgentHandle));
        var initLayer = base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
        
        EvacueeEnvironment = new SpatialHashEnvironment<Evacuee>(Width, Height);
        FireEnvironment = new SpatialHashEnvironment<Fire>(Width, Height);
        AlarmEnvironment = new SpatialHashEnvironment<Alarm>(Width, Height);
        SmokeEnvironment = new SpatialHashEnvironment<Smoke>(Width, Height);
        
        AgentManager = layerInitData.Container.Resolve<IAgentManager>();
        Fires = AgentManager.Spawn<Fire, GridLayer>().ToList();
        Smokes = AgentManager.Spawn<Smoke, GridLayer>().ToList();
        Alarms = AgentManager.Spawn<Alarm, GridLayer>().ToList();

        var exitLocations = new List<Position>
        {
            new(58, 68),
            new(55, 70),
            new(64, 70)
        };

        Exits = exitLocations;
        FireLocations = new List<Position>();
        
        
        var stairF = new List<Position>()
        {
            new(58, 75),
            new(57, 75),
            new(56, 75),
            
        };
        var stairB = new List<Position>()
        {
            new(74, 30) ,
            new(74, 29)
        };
        FrontStairs = stairF;
        BackStairs = stairB;
        Stairs = FrontStairs;
        Stairs.AddRange(BackStairs);
        PossibleGoal = Exits;
        PossibleGoal.AddRange(Stairs);
        SpawnAgents();
        Directions = MovementDirections.CreateMovementDirectionsList();
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
            var numAgents = Rand.Next(1, 5);
            for (var i = 0; i <= numAgents; i++)
            {
                switch (x)
                {
                    case 1:
                        Agent1.AddRange(AgentManager.Spawn<EvacueeType1, GridLayer>().ToList());
                        break;
                    case 2:
                        Agent2.AddRange(AgentManager.Spawn<EvacueeType2, GridLayer>().ToList());
                        break;
                    case 3:
                        Agent3.AddRange(AgentManager.Spawn<EvacueeType3, GridLayer>().ToList());
                        break;
                    case 4:
                        Agent4.AddRange(AgentManager.Spawn<EvacueeType4, GridLayer>().ToList());
                        break;
                    case 5:
                        Agent5.AddRange(AgentManager.Spawn<EvacueeType5, GridLayer>().ToList());
                        break;
                    case 6:
                        Agent6.AddRange(AgentManager.Spawn<EvacueeType6, GridLayer>().ToList());
                        break;
                    case 7:
                        Agent7.AddRange(AgentManager.Spawn<EvacueeType7, GridLayer>().ToList());
                        break;
                    case 8:
                        Agent8.AddRange(AgentManager.Spawn<EvacueeType8, GridLayer>().ToList());
                        break;
                    case 9:
                        Agent9.AddRange(AgentManager.Spawn<EvacueeType9, GridLayer>().ToList());
                        break;
                    case 10:
                        Agent10.AddRange(AgentManager.Spawn<EvacueeType10, GridLayer>().ToList());
                        break;
                    case 11:
                        Agent11.AddRange(AgentManager.Spawn<EvacueeType11, GridLayer>().ToList());
                        break;
                    case 12:
                        Agent12.AddRange(AgentManager.Spawn<EvacueeType12, GridLayer>().ToList());
                        break;
                    case 13:
                        Agent13.AddRange(AgentManager.Spawn<EvacueeType13, GridLayer>().ToList());
                        break;
                    case 14:
                        Agent14.AddRange(AgentManager.Spawn<EvacueeType14, GridLayer>().ToList());
                        break;
                    case 15:
                        Agent15.AddRange(AgentManager.Spawn<EvacueeType15, GridLayer>().ToList());
                        break;
                    case 16:
                        Agent16.AddRange(AgentManager.Spawn<EvacueeType16, GridLayer>().ToList());
                        break;
                    case 17:
                        Agent17.AddRange(AgentManager.Spawn<EvacueeType17, GridLayer>().ToList());
                        break;
                    case 18:
                        Agent18.AddRange(AgentManager.Spawn<EvacueeType18, GridLayer>().ToList());
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
        return coordinate.X switch
        {
            > 1 and < 19 when coordinate.Y is > 50 and < 87 => //Coral
                "Coral Seminar room",
            > 21 and < 54 when coordinate.Y is > 52 and < 84 => //West
                "Undergraduate West labs",
            > 65 and < 100 when coordinate.Y is > 52 and < 84 => //East
                "Undergraduate East labs",
            > 102 and < 120 when coordinate.Y is > 42 and < 87 => //Braae
                "Braae labs",
            > 44 and < 120 when coordinate.Y is > 1 and < 15 => //CSHons
                "CS Hons lab",
            > 77 and < 86 when coordinate.Y is > 41 and < 45 => //FBathroom
                "Female Bathroom",
            > 86 and < 94 when coordinate.Y is > 38 and < 45 => //FBathroom2
                "Female Bathroom",
            > 77 and < 86 when coordinate.Y is > 32 and < 36 => //MBathroom 
                "Male Bathroom",
            > 86 and < 94 when coordinate.Y is > 32 and < 38 => //MBathroom2 
                "Male Bathroom",
            > 44 and < 63 when coordinate.Y is > 15 and < 32 => //Atrium
                "Atrium Seminar room",
            > 96 and < 120 when coordinate.Y is > 16 and < 32 => //SysDev
                "SysDev Seminar room",
            > 14 and < 102 when coordinate.Y is > 46 and < 50 => //Passage1
                "Corridor",
            > 50 and < 53 when coordinate.Y is > 51 and < 70 => //Passage2
                "Corridor",
            > 56 and < 65 when coordinate.Y is > 45 and < 68 => //Passage3
                "Corridor",
            > 63 and < 94 when coordinate.Y is > 26 and < 31 => //Passage4
                "Corridor",
            > 63 and < 94 when coordinate.Y is > 15 and < 19 => //Passage5
                "Corridor",
            > 63 and < 73 when coordinate.Y is > 19 and < 31 => //Passage6
                "Corridor",
            _ => "In Passage"
        };
    }
    

    #endregion
    
   
    #region Fields and Properties

    /// <summary>
    ///     The environment of the SimpleAgent agents
    /// </summary>

    public List<Position> Exits { get; private set; }
    public List<Position> Stairs { get; private set; }
    public List<Position> FrontStairs { get; private set; }
    public List<Position> BackStairs { get; private set; }

    /// <summary>
    ///     The environment of the ComplexAgent agents
    /// </summary>
    public SpatialHashEnvironment<Evacuee> EvacueeEnvironment { get; set; }
    public SpatialHashEnvironment<Fire> FireEnvironment { get; set; }
    public SpatialHashEnvironment<Smoke> SmokeEnvironment { get; set; }
    public SpatialHashEnvironment<Alarm> AlarmEnvironment { get; set; }
 
    public List<Fire> Fires { get; private set; }

    public List<Position> Directions { get; private set; }
    public List<Smoke> Smokes { get; private set; }
    public List<Alarm> Alarms { get; private set; }
    public List<Position> PossibleGoal {get; private set; }

  
    private List<EvacueeType1> Agent1 { get; set; } = new List<EvacueeType1>();
    private List<EvacueeType2> Agent2 { get; set; } = new List<EvacueeType2>();
    private List<EvacueeType3> Agent3 { get; set; } = new List<EvacueeType3>();
    private List<EvacueeType4> Agent4 { get; set; } = new List<EvacueeType4>();
    private List<EvacueeType5> Agent5 { get; set; } = new List<EvacueeType5>();
    private List<EvacueeType6> Agent6 { get; set; } = new List<EvacueeType6>();
    private List<EvacueeType7> Agent7 { get; set; } = new List<EvacueeType7>();
    private List<EvacueeType8> Agent8 { get; set; } = new List<EvacueeType8>();
    private List<EvacueeType9> Agent9 { get; set; } = new List<EvacueeType9>();
    private List<EvacueeType10> Agent10 { get; set; } = new List<EvacueeType10>();
    private List<EvacueeType11> Agent11 { get; set; } = new List<EvacueeType11>();
    private List<EvacueeType12> Agent12 { get; set; } = new List<EvacueeType12>();
    private List<EvacueeType13> Agent13 { get; set; } = new List<EvacueeType13>();
    private List<EvacueeType14> Agent14 { get; set; } = new List<EvacueeType14>();
    private List<EvacueeType15> Agent15 { get; set; } = new List<EvacueeType15>();
    private List<EvacueeType16> Agent16 { get; set; } = new List<EvacueeType16>();
    private List<EvacueeType17> Agent17 { get; set; } = new List<EvacueeType17>();
    private List<EvacueeType18> Agent18 { get; set; } = new List<EvacueeType18>(); 
    
    public List<Position> FireLocations { get; set; }
    private int _numAgents;
    private static readonly Random Rand = new Random();
    public IAgentManager AgentManager { get; private set; }
    public bool FireStarted { get; set; }
    public bool SmokeSpread { get; set; }
    public bool Ring { get; set; }
    
    #endregion
}