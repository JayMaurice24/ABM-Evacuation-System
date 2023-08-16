namespace GridBlueprint.Model;

/// <summary>
/// Agent With Low Risk and Low Speed 
/// </summary>
public class AgentType1 : ComplexAgent
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