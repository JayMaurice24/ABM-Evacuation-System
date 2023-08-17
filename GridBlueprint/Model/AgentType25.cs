namespace GridBlueprint.Model;

/// <summary>
/// Agent With High Risk, High Speed and Low Aggression
/// </summary>
public class AgentType25: ComplexAgent
{
    private new void Init(GridLayer layer)
    {
        base.Init(layer);
        RiskLevel = Behaviour.LowRisk();
        Speed = Behaviour.HighSpeed();
        
    }

    private new void Tick()
    {
        base.Tick();
    }

}