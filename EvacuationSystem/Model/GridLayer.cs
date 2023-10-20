using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Common.Core.Random;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;

namespace EvacuationSystem.Model;

public class GridLayer : RasterLayer, ISteppedActiveLayer
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
    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle,
        UnregisterAgent unregisterAgentHandle)
    {
        var initLayer = base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
        AgentManager = layerInitData.Container.Resolve<IAgentManager>();
        FireEnvironment = new SpatialHashEnvironment<Fire>(Width, Height);
        Fires = AgentManager.Spawn<Fire, GridLayer>().ToList();
        EvacueeEnvironment = new SpatialHashEnvironment<Evacuee>(Width, Height);
        AlarmEnvironment = new SpatialHashEnvironment<Alarm>(Width, Height);
        SmokeEnvironment = new SpatialHashEnvironment<Smoke>(Width, Height);
        Alarms = AgentManager.Spawn<Alarm, GridLayer>().ToList();
        
        var stairF = new List<Position>()
        {
            new(58, 75),
            new(57, 75),
            new(56, 75)
            
        };
        var stairB = new List<Position>()
        {
            new(74, 30) ,
            new(74, 29)
        };
        Agent1 = AgentManager.Spawn<EvacueeType1, GridLayer>().ToList();
        Agent2 = AgentManager.Spawn<EvacueeType2, GridLayer>().ToList();
        Agent3 = AgentManager.Spawn<EvacueeType3, GridLayer>().ToList();
        Agent4 = AgentManager.Spawn<EvacueeType4, GridLayer>().ToList();
        Agent5 = AgentManager.Spawn<EvacueeType5, GridLayer>().ToList();
        Agent6 = AgentManager.Spawn<EvacueeType6, GridLayer>().ToList();
        Agent7 = AgentManager.Spawn<EvacueeType7, GridLayer>().ToList();
        Agent8 = AgentManager.Spawn<EvacueeType8, GridLayer>().ToList();
        Agent9 = AgentManager.Spawn<EvacueeType9, GridLayer>().ToList();
        Agent10 = AgentManager.Spawn<EvacueeType10, GridLayer>().ToList();
        Agent11 = AgentManager.Spawn<EvacueeType11, GridLayer>().ToList();
        Agent12 = AgentManager.Spawn<EvacueeType12, GridLayer>().ToList();
        Agent13 = AgentManager.Spawn<EvacueeType13, GridLayer>().ToList();
        Agent14 = AgentManager.Spawn<EvacueeType14, GridLayer>().ToList();
        Agent15 = AgentManager.Spawn<EvacueeType15, GridLayer>().ToList();
        Agent16 = AgentManager.Spawn<EvacueeType16, GridLayer>().ToList();
        Agent17 = AgentManager.Spawn<EvacueeType17, GridLayer>().ToList();
        Agent18 = AgentManager.Spawn<EvacueeType18, GridLayer>().ToList();

        ModelOutput.NumberOfAgents = Agent1.Count + Agent2.Count + Agent3.Count + Agent4.Count + Agent5.Count + Agent6.Count
                         + Agent7.Count + Agent8.Count + Agent9.Count + Agent10.Count + Agent11.Count + Agent12.Count
                         + Agent13.Count + Agent14.Count + Agent15.Count + Agent16.Count + Agent17.Count + Agent18.Count;
        ModelOutput.NumAgentsLeft = ModelOutput.NumberOfAgents;
        FrontStairs = stairF;
        BackStairs = stairB;
        Exits = FrontStairs;
        Exits.AddRange(BackStairs);
        Directions = MovementDirections.CreateMovementDirectionsList();
        _output = new ModelOutput(this);
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
    /// <summary>
    /// Areas where agents should not be spawned 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool NoSpawn(int x, int y)
    {
        switch (x)
        {
            case > 1 and < 14 when y is < 50 and > 45:
            case > 77 and < 86 when y is < 50 and > 36:
            case > 73 and < 94 when y is < 25 and > 19:
            case > 56 and < 65 when y is > 68 and < 88:
            case > 73 and < 95 when y is > 25 and < 31:
                return true;
            default:
                return false;
        }
    }
    
    /// <summary>
    /// finds a random position to spawn the agent
    /// </summary>
    /// <returns></returns>
    public Position FindRandomPosition()
    {
        var random = RandomHelper.Random;
        var check = true;
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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
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

    private bool ViewRange(Position agent1, Position agent2)
    {
        switch (agent1.X)
        {
            case > 1 and < 19
                when agent1.Y is > 50 and < 87 && agent2.X is > 1 and < 19 && agent2.Y is > 50 and < 87: //Coral
            case > 21 and < 54
                when agent1.Y is > 52 and < 84 && agent2.X is > 21 and < 54 && agent2.Y is > 52 and < 84: //West 
            case > 65 and < 100
                when agent1.Y is > 52 and < 84 && agent2.X is > 65 and < 100 && agent2.Y is > 52 and < 84: //East
            case > 102 and < 120
                when agent1.Y is > 42 and < 87 && agent2.X is > 102 and < 120 && agent2.Y is > 42 and < 87: //Braae
            case > 44 and < 120
                when agent1.Y is > 1 and < 15 && agent2.X is > 44 and < 120 && agent2.Y is > 1 and < 15: //CSHons 
            case > 77 and < 86
                when agent1.Y is > 41 and < 45 && agent2.X is > 77 and < 86 && agent2.Y is > 41 and < 45: //FBathroom
            case > 86 and < 94
                when agent1.Y is > 38 and < 45 && agent2.X is > 86 and < 94 && agent2.Y is > 38 and < 45: //FBathroom2
            case > 77 and < 86
                when agent1.Y is > 32 and < 36 && agent2.X is > 77 and < 86 && agent2.Y is > 32 and < 36: //MBathroom 
            case > 86 and < 94
                when agent1.Y is > 32 and < 38 && agent2.X is > 86 and < 94 && agent2.Y is > 32 and < 38: //MBathroom2 
            case > 44 and < 63
                when agent1.Y is > 15 and < 32 && agent2.X is > 44 and < 63 && agent2.Y is > 15 and < 32: //Atrium
            case > 96 and < 120
                when agent1.Y is > 16 and < 32 && agent2.X is > 96 and < 120 && agent2.Y is > 16 and < 32: //SysDev
            case > 14 and < 102
                when agent1.Y is > 46 and < 50 && agent2.X is > 14 and < 102 && agent2.Y is > 46 and < 50: //Passage1
            case > 50 and < 53
                when agent1.Y is > 51 and < 70 && agent2.X is > 50 and < 53 && agent2.Y is > 51 and < 70: //Passage2
            case > 56 and < 65
                when agent1.Y is > 45 and < 68 && agent2.X is > 56 and < 65 && agent2.Y is > 45 and < 68: //Passage3
            case > 63 and < 94
                when agent1.Y is > 26 and < 31 && agent2.X is > 63 and < 94 && agent2.Y is > 26 and < 31: //Passage4
            case > 63 and < 94
                when agent1.Y is > 15 and < 19 && agent2.X is > 63 and < 94 && agent2.Y is > 15 and < 19: //Passage5
            case > 63 and < 73
                when agent1.Y is > 19 and < 31 && agent2.X is > 63 and < 94 && agent2.Y is > 19 and < 31: //Passage6
                return true;
            default:
                return false;
        }
    }

    private void SpreadFireAndSmoke()
    {
        Smokes.AddRange(AgentManager.Spawn<Smoke, GridLayer>().ToList()); 
        if (GetCurrentTick() % SpreadRate != 0) return;
        Fires.AddRange(AgentManager.Spawn<Fire, GridLayer>().ToList());
    }
    
    public Position SetSmokeOrFirePosition(int x)
    {
        Position nearest; 
        switch (x)
        {
            case 1:
                var nextFireDirection = Rand.Next(FireLocations.Count);
                nearest = FireLocations[nextFireDirection];
                break;
            default:
                if (SmokeLocations.Count > 0)
                {
                    var nextSmokeDirection = Rand.Next(SmokeLocations.Count);
                    nearest = SmokeLocations[nextSmokeDirection];
                }
                else
                {
                    nearest = FireLocations[0];
                }
                break;
        }
        double newX = 0, newY = 0;
        var check = true;
        while (check)
        {
            var randomDirection = Rand.Next(0, Directions.Count);
            var nextDirection = Directions[randomDirection];
            newX = nearest.X + nextDirection.X;
            newY = nearest.Y + nextDirection.Y;
            if (ViewRange(nearest,new Position(newX,newY)))
            {
                check = false;
            }
        }

        var position = Position.CreatePosition(newX, newY);
        if(x == 0) SmokeLocations.Add(position);
        else{FireLocations.Add(position);}
        return position;
    }

    #endregion
    
   
    #region Fields and Properties

    public List<Position> Exits { get; private set; }
    public List<Position> FrontStairs { get; private set; }
    public List<Position> BackStairs { get; private set; }
    public SpatialHashEnvironment<Evacuee> EvacueeEnvironment { get; private set; }
    public SpatialHashEnvironment<Fire> FireEnvironment { get; private set; }
    public SpatialHashEnvironment<Smoke> SmokeEnvironment { get; set; }
    public SpatialHashEnvironment<Alarm> AlarmEnvironment { get; set; }
    public List<Fire> Fires { get; private set; }
   
    public List<Smoke> Smokes { get; private set; }
    public List<Alarm> Alarms { get; private set; }
    internal List<EvacueeType1> Agent1 { get; set; }
    internal List<EvacueeType2> Agent2 { get; set; }
    internal List<EvacueeType3> Agent3 { get; set; }
    internal List<EvacueeType4> Agent4 { get; set; }
    public List<EvacueeType5> Agent5 { get; set; }
    public List<EvacueeType6> Agent6 { get; set; }
    public List<EvacueeType7> Agent7 { get; set; }
    public List<EvacueeType8> Agent8 { get; set; }
    public List<EvacueeType9> Agent9 { get; set; }
    internal List<EvacueeType10> Agent10 { get; set; }
    internal List<EvacueeType11> Agent11 { get; set; }
    internal List<EvacueeType12> Agent12 { get; set; }
    internal List<EvacueeType13> Agent13 { get; set; }
    internal List<EvacueeType14> Agent14 { get; set; }
    internal List<EvacueeType15> Agent15 { get; set; }
    internal List<EvacueeType16> Agent16 { get; set; }
    internal List<EvacueeType17> Agent17 { get; set; }
    internal List<EvacueeType18> Agent18 { get; set; }
    public List<Position> Directions { get; private set; }
    public List<Position> FireLocations { get; set; } = new List<Position>();
    public List<Position> SmokeLocations { get; set; } = new List<Position>();
    protected UnregisterAgent UnregisterAgentHandle { get; set; }
    public RegisterAgent RegisterAgentHandle { get; set; }
    private static readonly Random Rand = new Random();
    private IAgentManager AgentManager { get; set; }
    private ModelOutput _output;
    public int SpreadRate { get; set;}
    public bool FireStarted { get; set; }
    public bool SmokeSpread { get; set; }
    public bool Ring { get; set; }
    
    #endregion

    public void Tick()
    {
        Console.WriteLine($"Tick {GetCurrentTick()}");
        Console.WriteLine($"Number of total agents: {ModelOutput.NumberOfAgents}");
        Console.WriteLine($"Number of agents who reached exit: {ModelOutput.NumReachExit}");
        Console.WriteLine($"Number of agents still in simulation: {EvacueeEnvironment.Entities.Count()}");
        _output.Write();
        switch (FireStarted)
        {
            case true when !SmokeSpread:
                Smokes = AgentManager.Spawn<Smoke, GridLayer>().ToList();
                break;
            case true when SmokeSpread:
                SpreadFireAndSmoke();
                break;
        }
        if(EvacueeEnvironment.Entities.Count()< 10){ModelOutput.WriteRemDetails();}
        if (EvacueeEnvironment.Entities.Any()) return;
        SmokeEnvironment.Reset();FireEnvironment.Reset();AlarmEnvironment.Reset();
    }

    public void PostTick()
    {
        //do nothing
    }
    public void PreTick()
    {
        SpreadRate = Rand.Next(1,5);
    }
}

