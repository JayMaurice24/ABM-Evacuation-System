namespace GridBlueprint.Model;


/// <summary>
/// Agent With Low Risk and High Speed 
/// </summary>

public class AgentType3 : ComplexAgent
{
    public new void Init(GridLayer layer)
    {
        base.Init(layer);
        RiskLevel = Behaviour.LowRisk();
        Speed = Behaviour.HighSpeed();
    }

    public new void Tick()
    {
        base.Tick();
    }
}