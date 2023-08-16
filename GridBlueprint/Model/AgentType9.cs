namespace GridBlueprint.Model;


/// <summary>
/// Agent With High Risk and High Speed 
/// </summary>
/// 
public class AgentType9 : ComplexAgent
{
    public new void Init(GridLayer layer)
    {
        base.Init(layer);
        RiskLevel = Behaviour.HighRisk();
        Speed = Behaviour.HighSpeed();
    }

    public new void Tick()
    {
        base.Tick();
    }
}