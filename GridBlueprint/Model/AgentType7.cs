namespace GridBlueprint.Model;


/// <summary>
/// Agent With High Risk and Low Speed 
/// </summary>
/// 
public class AgentType7 : ComplexAgent
{
    public new void Init(GridLayer layer)
    {
        base.Init(layer);
        RiskLevel = Behaviour.HighRisk();
        Speed = Behaviour.LowSpeed();
    }

    public new void Tick()
    {
        base.Tick();
    }
}