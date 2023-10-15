using System;
using System.Collections.Generic;
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
        description.AddAgent<Fire, GridLayer>();
        description.AddAgent<Alarm, GridLayer>();
        description.AddAgent<Smoke, GridLayer>();
        
        
        var config = CreateConfig();

        
        // Couple model description and simulation configuration
        var starter = SimulationStarter.Start(description, config);
        // Run the simulation
        var handle = starter.Run();
        
        // Close the program
        Console.WriteLine("Successfully executed iterations: " + handle.Iterations);
        starter.Dispose();
    }
    private static SimulationConfig CreateConfig()
    {
        var rand = new Random();
        const int max = 10;
        var startPoint = DateTime.Now;
        var config = new SimulationConfig
        {
            SimulationIdentifier = "Evacuation System",
            Globals =
            {
                StartPoint = startPoint,
                EndPoint = startPoint + TimeSpan.FromHours(24),
                DeltaTUnit = TimeSpanUnit.Seconds,
                //ShowConsoleProgress = true,
                LiveVisualization = true,
                OutputTarget = OutputTargetType.Csv,
            },
            LayerMappings = {
                new LayerMapping
                {
                    Name = nameof(GridLayer),
                    File = "Resources/HTF.csv"
                }},
            AgentMappings =
            {
                
                new AgentMapping(){
                Name = nameof(EvacueeType1),
                InstanceCount = rand.Next(1, max), 
                },
                new AgentMapping(){
                    Name = nameof(EvacueeType1),
                    InstanceCount = rand.Next(1, max), 
                },
                new AgentMapping(){
                    Name = nameof(EvacueeType2),
                    InstanceCount = rand.Next(1, max), 
                },
                new AgentMapping(){
                    Name = nameof(EvacueeType3),
                    InstanceCount = rand.Next(1, max),  
                },
                new AgentMapping(){
                    Name = nameof(EvacueeType4),
                    InstanceCount = rand.Next(1, max),  
                },
                new AgentMapping(){
                    Name = nameof(EvacueeType5),
                    InstanceCount = rand.Next(1, max), 
                },
                new AgentMapping(){
                    Name = nameof(EvacueeType6),
                    InstanceCount = rand.Next(1, max), 
                },
                new AgentMapping(){
                    Name = nameof(EvacueeType7),
                    InstanceCount = rand.Next(1, max), 
                },
                new AgentMapping(){
                    Name = nameof(EvacueeType8),
                    InstanceCount = rand.Next(1, max), 
                },
                new AgentMapping(){
                    Name = nameof(EvacueeType9),
                    InstanceCount = rand.Next(1, max),  
                },
                new AgentMapping(){
                    Name = nameof(EvacueeType10),
                    InstanceCount = rand.Next(1, max), 
                },
                new AgentMapping(){
                    Name = nameof(EvacueeType11),
                    InstanceCount = rand.Next(1, max),  
                },
                new AgentMapping(){
                    Name = nameof(EvacueeType12),
                    InstanceCount = rand.Next(1, max),  
                },
                new AgentMapping(){
                    Name = nameof(EvacueeType13),
                    InstanceCount = rand.Next(1, max),   
                },
                new AgentMapping(){
                    Name = nameof(EvacueeType14),
                    InstanceCount = rand.Next(1, max), 
                },
                new AgentMapping(){
                    Name = nameof(EvacueeType15),
                    InstanceCount = rand.Next(1, max),  
                },
                new AgentMapping(){
                    Name = nameof(EvacueeType16),
                    InstanceCount = rand.Next(1, max),  
                },
                new AgentMapping(){
                    Name = nameof(EvacueeType17),
                    InstanceCount = rand.Next(1, max),  
                },
                new AgentMapping(){
                    Name = nameof(EvacueeType18),
                    InstanceCount = rand.Next(1, max), 
                },
                new AgentMapping(){
                    Name = nameof(Fire),
                    InstanceCount = 1, 
                },
                new AgentMapping(){
                    Name = nameof(Smoke),
                    InstanceCount = 1, 
                },
                new AgentMapping(){
                    Name = nameof(Alarm),
                    InstanceCount = 22, 
                    File = "Resources/AlarmLocations.csv",
                },
            }
        };
        return config;
    }
}