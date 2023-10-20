using System;
using System.Collections.Generic;
using Mars.Interfaces.Environments;

namespace EvacuationSystem.Model;

public class HandleAgentMovement
{
    private readonly Evacuee _evacuee;
    public readonly Evacuate Agent;
    public readonly HandleGroup Group;
    private readonly GridLayer _layer;
    private readonly Random _rand = new();
    private readonly List<double> _coords = new();

    public HandleAgentMovement(Evacuee evacuee, GridLayer layer, double x, double y)
    {
        _evacuee = evacuee;
        _layer = layer;
        Agent = new Evacuate(evacuee, layer);
        Group = new HandleGroup(evacuee, layer);
        _coords.Add(x);
        _coords.Add(y);

    }
     /// <summary>
    /// Determines if An Agent can start moving
    /// </summary>
    /// <returns></returns>
    private bool ToMoveOrNotToMove()
    {
        return  _layer.Ring
               && (_evacuee.RiskLevel < (int)_layer.GetCurrentTick() || _evacuee.Perception(_evacuee.Position, _layer.FireLocations[0]));
    }

     /// <summary>
     /// If Agent has not started moving, determine whether or not they should move, if yes, Determine 
     /// </summary>
     public void HasNotStartedMoving()
    {
        if (!ToMoveOrNotToMove()) return;
        _evacuee.EvacueeHasStartedMoving = true;
        StartMoving();
    }

    /// <summary>
    /// Determines if an agent will forget an item, finds an exit and determines leaders
    /// </summary>
    private void StartMoving()
    {
        if (_rand.NextDouble() > 0.5)
        {
            _evacuee.DelayTime = _evacuee.RiskLevel + _rand.Next(20, 60);
            _evacuee.ForgotAnItem = true;
        }
        SetExit();
        _evacuee.DetermineLeader();
    }
    /// <summary>
    /// Sets the 
    /// </summary>
    private void SetExit()
    {
        _evacuee.Goal = ExitWithNoFire();
        Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} has started evacuating");
    }
/// <summary>
/// Sets an agent's position to an exit with no fire
/// </summary>
/// <returns></returns>
    public Position ExitWithNoFire()
    {
        var fireStartLocation = _layer.FireLocations[0];
        return fireStartLocation.X switch
        {
            > 56 and < 65 when fireStartLocation.Y is > 68 and < 88 => _evacuee.FindNearestExit(_layer.BackStairs),
            > 73 and < 95 when fireStartLocation.Y is > 25 and < 31 => _evacuee.FindNearestExit(_layer.FrontStairs),
            _ => _evacuee.FindNearestExit(_layer.Exits)
        };
    }
    /// <summary>
    /// Determines the evacuee's chance of returning for an item
    /// </summary>
    private void ChanceOfReturningForItem()
    {
        if (_evacuee.ProbabilityOfSuccess(OriginalPosition()) < 0.5 || _evacuee.FoundDistressedAgent) return;
        switch (_evacuee.IsInGroup)
        {
            case true:
                Group.Update();
                break;
            default:
                Agent.HandleReturnForItem();
                break;
        }
        ModelOutput.NumForget++;
    }
    /// <summary>
    /// Determines the flow of an unconscious evacuee
    /// </summary>
    private void UnconsciousEvacuee()
    {
        if (_evacuee.FoundHelp)
        {
            Agent.MoveWithHelp();
        }
        else
        { 
            Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} is Unconscious at cell {_evacuee.Position}");
        }
    }
    /// <summary>
    /// Handles how the evacuee should move
    /// </summary>
    public void IsMoving()
    {
        if ((int)_layer.GetCurrentTick() % _evacuee.Mobility != 0) return;
        if (!_evacuee.IsConscious)
        {
            UnconsciousEvacuee();
        }
        else
        {
            if (_evacuee.DelayTime == _layer.GetCurrentTick() && _evacuee.ForgotAnItem)
            {
                ChanceOfReturningForItem();
            }
            else 
            {
                Evacuation();
            }
            
        }
        _evacuee.UpdateHealthStatus();
    }
    /// <summary>
    /// The evacuation process 
    /// </summary>
    private void Evacuation()
    {
        if (_evacuee.IsLeader)
        {
            if(!_evacuee.ReturningWithGroupForItem || !_evacuee.ReturningWithGroupToHelp) _evacuee.FormGroup();
            Agent.Move();
        }
        else if (_evacuee.IsInGroup)
        {
            if (_evacuee.LeaderHasReachedExit)
            {
                Agent.Move();
            }
            else
            {
                Agent.MoveTowardsLeader();
            }
        }
        else
        {
            Agent.Move();
        }

        if (!_evacuee.FoundDistressedAgent)
        {
            _evacuee.HelpAgent();
        }
    }

    public Position OriginalPosition()
    {
        return new Position(_coords[0], _coords[1]); 
    }
}