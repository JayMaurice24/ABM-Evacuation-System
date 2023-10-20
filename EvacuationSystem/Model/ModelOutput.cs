using System.IO;
using Mars.Interfaces.Environments;

namespace EvacuationSystem.Model;

public class ModelOutput
{
    private static GridLayer _layer;
    readonly StreamWriter _output = new("tickOutput.txt");
    static readonly StreamWriter ReturnOutput = new("returnOutput.txt");
    static readonly StreamWriter ExitOutput = new("exitOutput.txt");
    static readonly StreamWriter CasualtyOutput = new("casOutput.txt");
    static readonly StreamWriter RemOutput = new("remOutput.txt");
    public ModelOutput( GridLayer layer)
    {
        _layer = layer; 
    }
    public void Write()
    {
        if ((int)_layer.GetCurrentTick() == 1)
        {
            ReturnOutput.WriteLine("Evacuee Type, ID , Probability Of return");
            ExitOutput.WriteLine("Evacuee Type, ID, Exit Location, tick, Risk level, Mobility, Forgot Item, Found distressed agent, In Group");
            CasualtyOutput.WriteLine("Evacuee Type, ID, Location, Tick, Forgot Item, In Group");
        }
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
        _output.WriteLine($"Number of agents who forgot an item :{NumForget}");
        _output.WriteLine($"Number of agents who forgot an item and is in group:{NumberRet}");
        _output.WriteLine($"Number of agents pushed : {NumberOfAgentsPushed}");
        _output.WriteLine($"Number of unconscious agents :  {NumUnconscious}");
        _output.WriteLine($"Number of agents who found help: {NumFoundHelp}");
        _output.WriteLine($"Number of deaths: {NumDeaths}");
        _output.WriteLine($"Number of Group returns : {NumGroupReturns}");
        _output.WriteLine($"Number of Group Splits : {NumGroupSplits}");
        _output.WriteLine($"Number of Agents who left a group : {NumGroupLeave}");
    }
    public static void WriteExitDetails(Evacuee evacuee)
      {
        
          ExitOutput.WriteLine($"{evacuee.GetType().Name}, {evacuee.ID}, {evacuee.Goal}, {evacuee.RiskLevel}, {evacuee.Mobility}, {evacuee.AgentReturningForItem}, {evacuee.FoundDistressedAgent}, {evacuee.IsInGroup}");
      }
      public static void WriteCasDetails(Evacuee evacuee)
      {
          CasualtyOutput.WriteLine($"{evacuee.GetType().Name}, {evacuee.ID}, {evacuee.Goal}, {evacuee.RiskLevel}, {evacuee.Mobility}, {evacuee.AgentReturningForItem}, {evacuee.FoundDistressedAgent}, {evacuee.IsInGroup}");
      }
      public static void WriteRemDetails()
      {
          RemOutput.WriteLine($"Tick {_layer.GetCurrentTick()}");
          foreach (var evacuee in _layer.EvacueeEnvironment.Entities)
          {
              RemOutput.WriteLine($"{evacuee.GetType().Name}, {evacuee.ID}, {evacuee.Goal}, {evacuee.Position}, {evacuee.Movement.OriginalPosition()}, {evacuee.IsConscious}, {evacuee.EvacueeHasStartedMoving}, {evacuee.RiskLevel}, {evacuee.Mobility}, {evacuee.AgentReturningForItem}, {evacuee.Movement.Agent.ReachedItem}, {evacuee.FoundDistressedAgent}, {evacuee.IsInGroup}, {evacuee.IsLeader}, {evacuee.LeaderHasReachedExit}");
          }
      }
    public static int NumberOfAgents { get; set; }
    public static int NumberOfGroups { get; set; }
    public static int NumberRet { get; set; }
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