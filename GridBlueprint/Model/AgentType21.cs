namespace GridBlueprint.Model;

/// <summary>
/// Agent With High Risk, Low Speed and High Aggression
/// </summary>
public class AgentType21: ComplexAgent
{
    
    #region Init
    private new void Init(GridLayer layer)
    {
        base.Init(layer);
        RiskLevel = Behaviour.LowRisk();
        Speed = Behaviour.LowSpeed();
    }

    #endregion
    
    #region Tick
    private new void Tick()
    {
        base.Tick();
    }
    #endregion

    
 

   
}