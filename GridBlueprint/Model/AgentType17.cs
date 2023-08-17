namespace GridBlueprint.Model;

/// <summary>
/// Agent With Medium Risk, High Speed and Medium Aggression
/// </summary>
public class AgentType17: ComplexAgent
{
    private new void Init(GridLayer layer)
    {
        base.Init(layer);
        RiskLevel = Behaviour.LowRisk();
        Speed = Behaviour.HighSpeed();    }

    private new void Tick()
    {
        base.Tick();
    }

}