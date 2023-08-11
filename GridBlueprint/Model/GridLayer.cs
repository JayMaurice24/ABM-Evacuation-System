using System.Collections.Generic;
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
    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle,
        UnregisterAgent unregisterAgentHandle)
    {
        var initLayer = base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);

        SimpleAgentEnvironment = new SpatialHashEnvironment<SimpleAgent>(Width, Height);
        ComplexAgentEnvironment = new SpatialHashEnvironment<ComplexAgent>(Width, Height);

        var agentManager = layerInitData.Container.Resolve<IAgentManager>();

        SimpleAgents = agentManager.Spawn<SimpleAgent, GridLayer>().ToList();
        ComplexAgents = agentManager.Spawn<ComplexAgent, GridLayer>().ToList();
        HelperAgents = agentManager.Spawn<HelperAgent, GridLayer>().ToList();

        List<Position> stairLocation = new List<Position>()
        {
            new Position(61, 75),
            new Position(62, 75),
            new Position(63, 75)
        };

        List<Position> exitLocations = new List<Position>
        {
            new Position(57, 68),
            new Position(58, 68),
            new Position(55, 70),
            new Position(64, 70),
        };

        Exits = exitLocations;
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
                check = false; 
                 
            }
        }
        return Position.CreatePosition(x, y); 
    }
    #endregion
    
   
    #region Fields and Properties

    /// <summary>
    ///     The environment of the SimpleAgent agents
    /// </summary>
    public SpatialHashEnvironment<SimpleAgent> SimpleAgentEnvironment { get; set; }

    public List<Position> Exits { get; private set; }
    public List<Position> Stairs { get; private set; }

    /// <summary>
    ///     The environment of the ComplexAgent agents
    /// </summary>
    public SpatialHashEnvironment<ComplexAgent> ComplexAgentEnvironment { get; set; }

    /// <summary>
    ///     A collection that holds the SimpleAgent instances
    /// </summary>
    public List<SimpleAgent> SimpleAgents { get; private set; }

    /// <summary>
    ///     A collection that holds the ComplexAgent instances
    /// </summary>
    public List<ComplexAgent> ComplexAgents { get; private set; }

    /// <summary>
    ///     A collection that holds the HelperAgent instance
    /// </summary>
    public List<HelperAgent> HelperAgents { get; private set; }

    #endregion
}