namespace GridBlueprint.Model;

/// <summary>
/// Agent With Low Risk, Medium Speed and Low Aggression 
/// </summary>
public class AgentType4 : ComplexAgent
{
    public new void Init(GridLayer layer)
    {
        base.Init(layer);
        RiskLevel = Behaviour.MediumRisk();
        Speed = Behaviour.MediumSpeed();
    }

    public new void Tick()
    {
        base.Tick();
    }
}