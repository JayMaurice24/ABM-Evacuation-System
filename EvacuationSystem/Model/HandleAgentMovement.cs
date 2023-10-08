using System;

namespace EvacuationSystem.Model;

public class HandleAgentMovement
{
    private readonly Evacuee _evacuee;
    private readonly Evacuate _agent;
    private readonly HandleGroup _group;
    private readonly GridLayer _layer;
    private readonly Random _rand = new Random();

    public HandleAgentMovement(Evacuee evacuee, GridLayer layer)
    {
        _evacuee = evacuee;
        _layer = layer;
        _agent = new Evacuate(evacuee, layer);
        _group = new HandleGroup(evacuee, layer);
        
    }
     /// <summary>
    /// Determines if An Agent can start moving
    /// </summary>
    /// <returns></returns>
    private bool ToMoveOrNotToMove()
    {
        return _layer.GetCurrentTick() >= 2
               && _layer.Ring
               && (_evacuee.RiskLevel >= _layer.GetCurrentTick() || _evacuee.Perception(_evacuee.Position, _layer.FireLocations[0]));
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
            _evacuee.DelayTime = _rand.Next(20, 60);
            _evacuee.ForgotAnItem = true;
        }
        SetExit();
        _evacuee.DetermineLeader();
    }

    private void SetExit()
    {
        _evacuee.Goal = _evacuee.FindNearestExit(_layer.Exits);
        Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} has started evacuating");
        _evacuee.FoundExit = true;
    }

    private void ChanceOfReturningForItem()
    {
        if (!(_rand.NextDouble() < 0.5)) return;
        _evacuee.Goal = _evacuee.OriginalPosition;
        if(_evacuee.IsInGroup) _group.Update();
        Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} has forgotten an item and is heading back");
    }

    private void UnconsciousEvacuee()
    {
        if (_evacuee.FoundHelp)
        {
            _agent.MoveWithHelp();
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
            if (!_evacuee.FoundDistressedAgent)
            {
                Evacuation();
            }
            else if (_evacuee.ReturningWithGroupToHelp)
            {
                HelpingWithGroup();
            }
            else
            {
                HelpingAlone();
            }
            
            _evacuee.UpdateHealthStatus();
        }
    }

    private void Evacuation()
    {
        if (_evacuee.IsLeader)
        {
            if(!_evacuee.ReturningWithGroupForItem) _evacuee.FormGroup();
            _agent.Move();
        }
        else if (_evacuee.IsInGroup)
        {
            if (!_evacuee.LeaderHasReachedExit)
            {
                _agent.MoveTowardsLeader();
            }
            else
            {
                _agent.Move();
            }
        }
        else
        {
            _agent.Move();
        }
        _evacuee.HelpAgent();
    }

    private void HelpingWithGroup()
    {
        if (_evacuee.IsLeader)
        {
            if (_evacuee.Helping)
            {
                _agent.Move();
            }
            else if (_evacuee.ReachedDistressedAgent)
            {
                _evacuee.OfferHelp();
            }
            else
            {
                _agent.Move();
            }
        }
        else
        {
            _agent.MoveTowardsLeader();
        }
    }

    private void HelpingAlone()
    {
        if (_evacuee.Helping)
        {
            _agent.Move();
        }
        else
        {
            if (_evacuee.ReachedDistressedAgent)
            {
                _evacuee.OfferHelp();
                _evacuee.Goal = _evacuee.FindNearestExit(_layer.Exits);
            }
            else
            {
                _agent.Move();
            }
        }
    }
    

}