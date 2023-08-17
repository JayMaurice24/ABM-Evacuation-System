namespace GridBlueprint.Model;

/// <summary>
/// Agent With Low Risk and Low Speed and Low Aggression
/// </summary>
public class AgentType1 : ComplexAgent
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