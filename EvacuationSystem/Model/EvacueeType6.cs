using System.Collections.Generic;

namespace EvacuationSystem.Model;


/// <summary>
/// Agent With Low Risk, High Aggression and High Speed 
/// </summary>
public class EvacueeType6 : Evacuee
{
    #region Init

    /// <summary>
    /// Initialisation Method of the evacuee type, this is where the properties are defined before as the instance is spawned
    /// </summary>
    public override void Init(GridLayer layer)
    {
        Layer = layer;
        Position =  Layer.FindRandomPosition();
        RiskLevel = Characteristics.LowRisk();
        Mobility = Characteristics.HighSpeed();
        Aggression = 2;
        var x = Position.X;
        var y = Position.Y;
        Leadership = Rand.NextDouble();
        Empathy = Rand.NextDouble();
        CollaborationFactor = Rand.NextDouble();
        Health = Rand.Next(50, 100);
        Strength = Rand.NextDouble();
        Group = new List<Evacuee>();
        Leader = null;
        Helped = null;
        Helper = null;
        IsConscious = true;
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