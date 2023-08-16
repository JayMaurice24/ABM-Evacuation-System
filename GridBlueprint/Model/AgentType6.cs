namespace GridBlueprint.Model;


/// <summary>
/// Agent With Medium Risk and High Speed 
/// </summary>
public class AgentType6 : ComplexAgent
{
    public new void Init(GridLayer layer)
    {
        base.Init(layer);
        RiskLevel = Behaviour.MediumRisk();
        Speed = Behaviour.HighSpeed();
    }

    public new void Tick()
    {
        base.Tick();
    }
}