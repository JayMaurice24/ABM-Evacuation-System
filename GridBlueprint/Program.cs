using System;
using System.IO;
using GridBlueprint.Model;
using Mars.Components.Starter;
using Mars.Interfaces.Model;

namespace GridBlueprint;

internal static class Program
{
    private static void Main()
    {
        // Create a new model description and add model components to it
        var description = new ModelDescription();
        description.AddLayer<GridLayer>();
        description.AddAgent<Fire, GridLayer>();
        description.AddAgent<Alarm, GridLayer>();
        description.AddAgent<AgentType1, GridLayer>();
        description.AddAgent<AgentType2, GridLayer>();
        description.AddAgent<AgentType3, GridLayer>();
        description.AddAgent<AgentType4, GridLayer>();
        description.AddAgent<AgentType5, GridLayer>();
        description.AddAgent<AgentType6, GridLayer>();
        description.AddAgent<AgentType7, GridLayer>();
        description.AddAgent<AgentType8, GridLayer>();
        description.AddAgent<AgentType9, GridLayer>();
        description.AddAgent<AgentType10, GridLayer>();
        description.AddAgent<AgentType11, GridLayer>();
        description.AddAgent<AgentType12, GridLayer>();
        description.AddAgent<AgentType13, GridLayer>();
        description.AddAgent<AgentType14, GridLayer>();
        description.AddAgent<AgentType15, GridLayer>();
        description.AddAgent<AgentType16, GridLayer>();
        description.AddAgent<AgentType17, GridLayer>();
        description.AddAgent<AgentType18, GridLayer>();
        description.AddAgent<AgentType19, GridLayer>();
        description.AddAgent<AgentType20, GridLayer>();
        description.AddAgent<AgentType21, GridLayer>();
        description.AddAgent<AgentType22, GridLayer>();
        description.AddAgent<AgentType23, GridLayer>();
        description.AddAgent<AgentType24, GridLayer>();
        description.AddAgent<AgentType25, GridLayer>();
        description.AddAgent<AgentType26, GridLayer>();
        description.AddAgent<AgentType27, GridLayer>();
        
        
        
        description.AddAgent<HelperAgent, GridLayer>();
        description.AddEntity<Exits>();

        // Load the simulation configuration from a JSON configuration file
        var file = File.ReadAllText("config.json");
        var config = SimulationConfig.Deserialize(file);

        // Couple model description and simulation configuration
        var starter = SimulationStarter.Start(description, config);

        // Run the simulation
        var handle = starter.Run();
        
        // Close the program
        Console.WriteLine("Successfully executed iterations: " + handle.Iterations);
        starter.Dispose();
    }
}