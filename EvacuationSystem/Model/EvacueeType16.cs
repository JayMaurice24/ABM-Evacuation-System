using System.Collections.Generic;

namespace EvacuationSystem.Model;

/// <summary>
/// Agent With High Risk, Low Aggression and High Speed 
/// </summary>
public class EvacueeType16: Evacuee
{
    #region Init

     public override void Init(GridLayer layer)
    {
        Layer = layer;
        OriginalPosition = Layer.FindRandomPosition();
        Position = OriginalPosition;
        RiskLevel = Characteristics.HighRisk();
        Speed = Characteristics.HighSpeed();
        Aggression = 0;
        Leadership = Rand.NextDouble();
        Empathy = Rand.NextDouble();
        CollaborationFactor = Rand.NextDouble();
        Health = Rand.Next(30, 100);
        Strength = Rand.NextDouble();
        IsConscious = true;
        Group = new List<Evacuee>();
        Leader = null;
        Helped = null;
        Helper = null;
        Movement = new HandleAgentMovement(this, layer);
        Layer.EvacueeEnvironment.Insert(this);
    }

    #endregion

    #region Tick

    public override void Tick()
    {
        if (!EvacueeHasStartedMoving)
        {
            Movement.HasNotStartedMoving();
        }
        else
        {
            Movement.IsMoving();
        }
    }
    #endregion
}