using System.Collections.Generic;

namespace EvacuationSystem.Model;

/// <summary>
/// Agent With High Risk, Medium Aggression and High Speed 
/// </summary>
public class EvacueeType17: Evacuee
{
    #region Init

    /// <summary>
    /// Initialisation Method of the evacuee type, this is where the properties are defined before as the instance is spawned
    /// </summary>
     public override void Init(GridLayer layer)
    {
        Layer = layer;
        Position =  Layer.FindRandomPosition();
        RiskLevel = Characteristics.HighRisk();
        Mobility = Characteristics.HighSpeed();
        Aggression = 1;
        var x = Position.X;
        var y = Position.Y;
        Leadership = Rand.NextDouble();
        Empathy = Rand.NextDouble();
        CollaborationFactor = Rand.NextDouble();
        Health = Rand.Next(50, 100);
        Strength = Rand.NextDouble();
        IsConscious = true;
        Group = new List<Evacuee>();
        Leader = null;
        Helped = null;
        Helper = null;
        Movement = new HandleAgentMovement(this, layer, x, y);
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