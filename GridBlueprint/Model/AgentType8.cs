namespace GridBlueprint.Model;

/// <summary>
/// Agent With High Risk and Medium Speed 
/// </summary>
/// 
public class AgentType8 : ComplexAgent
{
    public new void Init(GridLayer layer)
    {
        base.Init(layer);
        RiskLevel = Behaviour.HighRisk();
        Speed = Behaviour.MediumSpeed();
    }

    public new void Tick()
    {
        base.Tick();
    }
}