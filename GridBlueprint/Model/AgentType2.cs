namespace GridBlueprint.Model;

/// <summary>
/// Agent With Low Risk and Medium Speed 
/// </summary>
public class AgentType2 : ComplexAgent
{
    public new void Init(GridLayer layer)
    {
        base.Init(layer);
        RiskLevel = Behaviour.LowRisk();
        Speed = Behaviour.MediumSpeed();
    }

    public new void Tick()
    {
        base.Tick();
    }
}