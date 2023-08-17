namespace GridBlueprint.Model;

/// <summary>
/// Agent With Medium Risk, Low Speed and High Aggression
/// </summary>
public class AgentType12: ComplexAgent
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