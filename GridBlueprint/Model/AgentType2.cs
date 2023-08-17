namespace GridBlueprint.Model;

/// <summary>
/// Agent With Low Risk, Low Speed and Medium Aggression
/// </summary>
public class AgentType2 : ComplexAgent
{
    public new void Init(GridLayer layer)
    {
        base.Init(layer);
        RiskLevel = Behaviour.LowRisk();
        Speed = Behaviour.LowSpeed();    
    }

    public new void Tick()
    {
        base.Tick();
    }
}