namespace GridBlueprint.Model;

/// <summary>
/// Agent With High Risk, Medium Speed and High Aggression
/// </summary>
public class AgentType24: ComplexAgent
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