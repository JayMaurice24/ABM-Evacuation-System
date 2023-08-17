namespace GridBlueprint.Model;

/// <summary>
/// Agent With Medium Risk, Medium Speed and Low Aggression
/// </summary>
public class AgentType13: ComplexAgent
{
    private new void Init(GridLayer layer)
    {
        base.Init(layer);
        RiskLevel = Behaviour.LowRisk();
        Speed = Behaviour.MediumSpeed();    }

    private new void Tick()
    {
        base.Tick();
    }

}