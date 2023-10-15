using System;

namespace EvacuationSystem.Model;

public class HandleAgentMovement
{
    private readonly Evacuee _evacuee;
    public readonly Evacuate Agent;
    public readonly HandleGroup Group;
    private readonly GridLayer _layer;
    private readonly Random _rand = new();

    public HandleAgentMovement(Evacuee evacuee, GridLayer layer)
    {
        _evacuee = evacuee;
        _layer = layer;
        Agent = new Evacuate(evacuee, layer);
        Group = new HandleGroup(evacuee, layer);
        
    }
     /// <summary>
    /// Determines if An Agent can start moving
    /// </summary>
    /// <returns></returns>
    private bool ToMoveOrNotToMove()
    {
        return  _layer.Ring
               && (_evacuee.RiskLevel >= (int)_layer.GetCurrentTick() || _evacuee.Perception(_evacuee.Position, _layer.FireLocations[0]));
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

    private void SetExit()
    {
        _evacuee.Goal = _evacuee.FindNearestExit(_layer.Exits);
        Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} has started evacuating");
    }

    private void ChanceOfReturningForItem()
    {
        if (_rand.NextDouble() < 0.5 || _evacuee.FoundDistressedAgent) return;
        
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

    public void IsMoving()
    {
        if ((int)_layer.GetCurrentTick() % _evacuee.Speed != 0) return;
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
}