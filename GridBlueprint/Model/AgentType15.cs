namespace GridBlueprint.Model;

/// <summary>
/// Agent With Medium Risk, Medium Speed and High Aggression
/// </summary>
public class AgentType15: ComplexAgent
{
    private new void Init(GridLayer layer)
    {
        base.Init(layer);
        RiskLevel = Behaviour.LowRisk();
        Speed = Behaviour.MediumSpeed();
        
    }

    private new void Tick()
    {
        base.Tick();
    }

}