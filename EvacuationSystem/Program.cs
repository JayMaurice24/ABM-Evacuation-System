using System;
using System.IO;
using EvacuationSystem.Model;
using Mars.Components.Starter;
using Mars.Interfaces.Model;

namespace EvacuationSystem;

internal static class Program
{
    private static void Main()
    {
        // Create a new model description and add model components to it
        var description = new ModelDescription();
        description.AddLayer<GridLayer>();
        description.AddAgent<Fire, GridLayer>();
        description.AddAgent<EvacueeType1, GridLayer>();
        description.AddAgent<EvacueeType2, GridLayer>();
        description.AddAgent<EvacueeType3, GridLayer>();
        description.AddAgent<EvacueeType4, GridLayer>();
        description.AddAgent<EvacueeType5, GridLayer>();
        description.AddAgent<EvacueeType6, GridLayer>();
        description.AddAgent<EvacueeType7, GridLayer>();
        description.AddAgent<EvacueeType8, GridLayer>();
        description.AddAgent<EvacueeType9, GridLayer>();
        description.AddAgent<EvacueeType10, GridLayer>();
        description.AddAgent<EvacueeType11, GridLayer>();
        description.AddAgent<EvacueeType12, GridLayer>();
        description.AddAgent<EvacueeType13, GridLayer>();
        description.AddAgent<EvacueeType14, GridLayer>();
        description.AddAgent<EvacueeType15, GridLayer>();
        description.AddAgent<EvacueeType16, GridLayer>();
        description.AddAgent<EvacueeType17, GridLayer>();
        description.AddAgent<EvacueeType18, GridLayer>();
        description.AddAgent<Alarm, GridLayer>();
        description.AddAgent<Smoke, GridLayer>();
        
        
        
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