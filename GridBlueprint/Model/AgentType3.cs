namespace GridBlueprint.Model;


/// <summary>
/// Agent With Low Risk, Low Speed and High Aggression 
/// </summary>

public class AgentType3 : ComplexAgent
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