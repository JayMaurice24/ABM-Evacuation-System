namespace GridBlueprint.Model;

/// <summary>
/// Agent With Medium Risk and Low Speed 
/// </summary>
public class AgentType4 : ComplexAgent
{
    public new void Init(GridLayer layer)
    {
        base.Init(layer);
        RiskLevel = Behaviour.MediumRisk();
        Speed = Behaviour.LowSpeed();
    }

    public new void Tick()
    {
        base.Tick();
    }
}