using System.IO;

namespace EvacuationSystem.Model;

public class ModelOutput
{
    private readonly GridLayer _layer;
    readonly StreamWriter _output = new StreamWriter("tickOutput.txt");
    public ModelOutput( GridLayer layer)
    {
        _layer = layer; 
    }
    public void Write()
    {
        _output.WriteLine($"Total ticks {_layer.GetCurrentTick()}");
        _output.WriteLine($"Number of total agents: {NumberOfAgents}");
        _output.WriteLine($"Number of Evacuee Type 1 : {_layer.Agent1.Count}");
        _output.WriteLine($"Number of Evacuee Type 2 : {_layer.Agent2.Count}");
        _output.WriteLine($"Number of Evacuee Type 3 : {_layer.Agent3.Count}");
        _output.WriteLine($"Number of Evacuee Type 4: {_layer.Agent4.Count}");
        _output.WriteLine($"Number of Evacuee Type 5 : {_layer.Agent5.Count}");
        _output.WriteLine($"Number of Evacuee Type 6 : {_layer.Agent6.Count}");
        _output.WriteLine($"Number of Evacuee Type 7 : {_layer.Agent7.Count}");
        _output.WriteLine($"Number of Evacuee Type 8 : {_layer.Agent8.Count}");
        _output.WriteLine($"Number of Evacuee Type 9 : {_layer.Agent9.Count}");
        _output.WriteLine($"Number of Evacuee Type 10 : {_layer.Agent10.Count}");
        _output.WriteLine($"Number of Evacuee Type 11 : {_layer.Agent11.Count}");
        _output.WriteLine($"Number of Evacuee Type 12 : {_layer.Agent12.Count}");
        _output.WriteLine($"Number of Evacuee Type 13 : {_layer.Agent13.Count}");
        _output.WriteLine($"Number of Evacuee Type 14 : {_layer.Agent14.Count}");
        _output.WriteLine($"Number of Evacuee Type 15 : {_layer.Agent15.Count}");
        _output.WriteLine($"Number of Evacuee Type 16 : {_layer.Agent16.Count}");
        _output.WriteLine($"Number of Evacuee Type 17 : {_layer.Agent17.Count}");
        _output.WriteLine($"Number of Evacuee Type 18 : {_layer.Agent18.Count}");
        _output.WriteLine($"Number of agents who reached exit: {NumReachExit}");
        _output.WriteLine($"Number of Agents Moving in group : {NumInGroup}");
        _output.WriteLine($"Number of Agents Moving alone : {NumberOfAgents - NumInGroup}");
        _output.WriteLine($"Number of agents still in simulation: {NumAgentsLeft}");
        _output.WriteLine($"Number of groups formed: {NumberOfGroups}");
        _output.WriteLine($"Number of agents who forgotten an item :{NumForget}");
        _output.WriteLine($"Number of agents pushed : {NumberOfAgentsPushed}");
        _output.WriteLine($"Number of unconscious agents :  {NumUnconscious}");
        _output.WriteLine($"Number of agents who found help: {NumFoundHelp}");
        _output.WriteLine($"Number of deaths: {NumDeaths}");
        _output.WriteLine($"Number of Group returns : {NumGroupReturns}");
        _output.WriteLine($"Number of Group Splits : {NumGroupSplits}");
        _output.WriteLine($"Number of Agents who left a group : {NumGroupLeave}");
    }
    public static int NumberOfAgents { get; set; }
    public static int NumberOfGroups { get; set; }
    public static int NumberOfAgentsPushed { get; set; }
    public static int NumGroupReturns { get; set; }
    public static int NumForget { get; set; }
    public static int NumAgentsLeft { get; set; }
    public static int NumInGroup { get; set; }
    public static int NumReachExit { get; set; }
    public static int NumUnconscious { get; set; }
    public static int NumDeaths { get; set; }
    public static int NumFoundHelp { get; set; }
    public static int NumGroupSplits { get; set; }
    public static int NumGroupLeave { get; set; }
   
}