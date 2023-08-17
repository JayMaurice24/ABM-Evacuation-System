namespace GridBlueprint.Model;

/// <summary>
/// Agent With High Risk, Low Speed and Medium Aggression
/// </summary>
public class AgentType20: ComplexAgent
{
    private new void Init(GridLayer layer)
    {
        base.Init(layer);
        RiskLevel = Behaviour.LowRisk();
        Speed = Behaviour.LowSpeed();
    }

    private new void Tick()
    {
        base.Tick();
    }

}