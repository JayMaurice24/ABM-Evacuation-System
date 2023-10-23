using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Common;
using Mars.Common.IO;
using Mars.Interfaces.Environments;
using Mars.Numerics;

namespace EvacuationSystem.Model;

public class Evacuate
{
    public bool ReachedItem;
    private readonly Evacuee _evacuee;
    private readonly GridLayer _layer;
    public bool TripInProgress;
    public List<Position>.Enumerator _path;

    public Evacuate(Evacuee evacuee, GridLayer layer)
    {
        _evacuee = evacuee;
        _layer = layer;
    }

    #region Solo Movement or Leader Movement

    public void Move()
    {
        _evacuee.Goal ??= _evacuee.Movement.ExitWithNoFire();
        if (!TripInProgress)
        {
            FindRoute();
        }
        if (!_path.MoveNext())
        {
            TripInProgress = false; return;}
        if (_evacuee.AvoidFire(_path.Current))
        {
            FireAvoidance();
        }
        else if (IsCellOccupied(_path.Current))
        {
            if (!_evacuee.Helping)
            {
                ToPushOrNotToPush();
            }
            MoveToCell(_path.Current);
        }
        else
        {
            MoveToCell(_path.Current);
        }
        PositionOutput();
        if (_evacuee.Position.Equals(_evacuee.Goal))
        {
            HandleGoalReached();
        }
    }

    #endregion

    #region GroupMovement
    public void MoveTowardsLeader()
    {
        if (_evacuee.Leader == null || !_evacuee.Leader.Group.Contains(_evacuee))
        {
            _evacuee.Goal = _evacuee.Movement.ExitWithNoFire();
            _evacuee.Leader = null;
            _evacuee.IsInGroup = false;
            TripInProgress = false;
        }
        if (!TripInProgress)
        {
            if (_evacuee.Leader != null) _evacuee.Goal = _evacuee.Leader.Position;
            FindRoute();
        }
        if (!_path.MoveNext())
        {
            TripInProgress = false; return;}
        UpdatePosition(_path.Current);
        if (!_evacuee.Position.Equals(_evacuee.Goal)) return;
        if (_layer.Exits.Contains(_evacuee.Goal))
        {
            ExitReached();
        }
        else
        {
            TripInProgress = false;
        }
    }
    private Position AvoidCollision(Position target)
    {
        var group = GetNearByObstacles(); 
        if(group == null) return target;
        foreach (var evacuee in group)
        {
            if (!Equals(target, evacuee.Position)) return target;
            if (_evacuee.Leader != null && !_evacuee.Leader.Group.Contains(_evacuee))
            {
                ToPushOrNotToPush();
            }
            else
            {
                target = evacuee.ChangeDirection(target); 
            }
        }
        return target;
    }
    private void UpdatePosition(Position target)
    {
        var newPosition = AvoidCollision(target);
        if (_evacuee.AvoidFire(newPosition))
        {
            FireAvoidance();
        }
        else
        {
            MoveToCell(newPosition);
        }
        PositionOutput();
    }
    

    #endregion
   
    #region Actions 
    /*public void MoveWithHelp()
    {
        if(_evacuee.Helper == null) return;
        var target =  _evacuee.Helper.Position;
        var directionToMove = (int)PositionHelper.CalculateBearingCartesian(_evacuee.Position.X, _evacuee.Position.Y, target.X, target.Y);
        _layer.EvacueeEnvironment.MoveTowards(_evacuee, directionToMove, 1);
        Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} has been moved to {_evacuee.Position} (is being carried by {_evacuee.Helper.GetType().Name} {_evacuee.Helper.ID} ");
    }*/

    public void MoveWithHelp()
    {
        if (!TripInProgress)
        {
            _evacuee.Goal = _evacuee.Helper.Position; 
            FindRoute();
        }
        if (!_path.MoveNext()) {TripInProgress = false; return;}
        MoveToCell(_path.Current);
        if (_layer.Exits.Contains(_evacuee.Goal))
        {
         
            ExitReached();
        }
        else
        {
            TripInProgress = false;
        }
    }
    

    private void FindRoute()
    {
        _path = _layer.FindPath(_evacuee.Position, _evacuee.Goal).GetEnumerator();
        TripInProgress = true;
    }

    private void FireAvoidance()
    {
        var next = _evacuee.ChangeDirection(_path.Current);
        MoveToCell(next);
    }

    public void HandleReturnForItem()
    {
        _evacuee.AgentReturningForItem = true;
        _evacuee.Goal = _evacuee.Movement.OriginalPosition();
        TripInProgress = false; 
    }
    public void HandleLeaderReturningForItem(Position target)
    {
        _evacuee.AgentReturningForItem = true;
        _evacuee.Goal = target;
        TripInProgress = false; 
    }
    public void HandleReturnForHelp()
    {
        Position nearestCell = null;
        foreach (var direction in _layer.Directions)
        {
            var newX = _evacuee.Helped.Position.X + direction.X;
            var newY = _evacuee.Helped.Position.Y + direction.Y;
            nearestCell = new Position(newX, newY);

            if (!_layer.IsRoutable(newX, newY)) continue;
            _evacuee.Goal = nearestCell;
        }

        if (nearestCell == null) _evacuee.Goal = _evacuee.Helped.Position;
        TripInProgress = false;
    }
    private void ToPushOrNotToPush()
    {
        var otherAgent = GetAgentAt(_path.Current);

        switch (_evacuee.Aggression)
        {
            case 1 when otherAgent != null && otherAgent.Aggression <= _evacuee.Aggression && otherAgent.Strength < _evacuee.Strength:
                _evacuee.PushAgent(otherAgent);
                Console.WriteLine($"{otherAgent.GetType().Name} {otherAgent.ID} has been pushed by {_evacuee.GetType().Name} {_evacuee.ID}");
                break;

            case 2 when otherAgent != null:
                _evacuee.PushAgent(otherAgent);
                Console.WriteLine($"{otherAgent.GetType().Name} {otherAgent.ID} has been pushed by {_evacuee.GetType().Name} {_evacuee.ID}");
                break;
        }
    }
    
    private Evacuee GetAgentAt(Position position)
    {
        return _layer.EvacueeEnvironment.Entities.FirstOrDefault(agent => agent != _evacuee && agent.Position.Equals(position));
    }
    
    private void MoveToCell(Position cell)
    {
        _layer.EvacueeEnvironment.MoveTo(_evacuee, cell, 1);
    }
      
    private void HandleGoalReached()
    {
        if (_layer.Exits.Contains(_evacuee.Goal))
        {
            ExitReached();
        }
        else if (_evacuee.FoundDistressedAgent)
        {
            ReachedHelplessEvacuee();
        }
        else if (_evacuee.AgentReturningForItem)
        {
            ItemFound();
        }
        
    }
    
    private void ItemFound()
    {
        Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} has Found Item and is heading to the exit");
        switch (_evacuee.IsInGroup)
        { 
            case true:
                GroupReturn();
                break;
            default:
                _evacuee.AgentReturningForItem = false;
                _evacuee.Goal = _evacuee.Movement.ExitWithNoFire();
                TripInProgress = false;
                break;
        }
        ReachedItem = true;
    }

    private void ReachedHelplessEvacuee()
    {
        Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} has Reached {_evacuee.Helped.GetType().Name} {_evacuee.Helped.ID} is heading to the exit");
        _evacuee.ReachedDistressedAgent = true; 
        _evacuee.OfferHelp();
        switch (_evacuee.ReturningWithGroupToHelp)
        {
            case true:
                GroupReturn();
                break;
            default:
                _evacuee.Goal = _evacuee.Movement.ExitWithNoFire();
                TripInProgress = false;
                break;
        }
        
    }

    public void HandleGroupLeave()
    {
        _evacuee.Goal = _evacuee.Movement.ExitWithNoFire();
        TripInProgress = false;
    }

    private void GroupReturn()
    {
        if (!_evacuee.IsLeader) return;
        if (_evacuee.ReturningWithGroupToHelp)
        {
            if (_evacuee.Helping)
            {
                foreach (var agent in _evacuee.Group)
                {
                    agent.Helping = true;
                }
            }
            else
            {
                foreach (var agent in _evacuee.Group)
                {
                    agent.ReturningWithGroupToHelp = false;
                    agent.FoundDistressedAgent = false;
                    agent.Helped = null;
                }

            }
        }
        else
        {
            foreach (var agent in _evacuee.Group)
            {
                agent.ReturningWithGroupForItem = false;
            }
            _evacuee.ReturningWithGroupForItem = false;
            _evacuee.AgentReturningForItem = false; 
        }
        _evacuee.Goal = _evacuee.FindNearestExit(_layer.Exits);
        TripInProgress = false;
    }

    private void ExitReached()
    {
        Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} has reached exit {_evacuee.Goal}");
        if (_evacuee.IsLeader && _evacuee.IsInGroup)
        {
            foreach (var agent in _evacuee.Group)
            {
                agent.LeaderHasReachedExit = true;
                agent.Goal = _evacuee.Goal;
                agent.Movement.Agent.TripInProgress = false;
                Console.WriteLine($"{agent.GetType().Name} {agent.ID}'s group leader has reached exit {_evacuee.Goal} and is heading for exit too");
            }
        }
        ModelOutput.NumReachExit++;
        _evacuee.RemoveFromSimulation();
    }
    private IEnumerable<Evacuee> GetNearByObstacles()
    {
        return _layer.EvacueeEnvironment.Explore(_evacuee.Position, 1);
    }

   

    private bool IsCellOccupied(Position targetPosition)
    {
        var agents = _layer.EvacueeEnvironment.Entities.MinBy(agent =>
            Distance.Chebyshev(new[] { _evacuee.Position.X, _evacuee.Position.Y }, new[] { agent.Position.X, agent.Position.Y }));

        return agents != null && targetPosition.Equals(agents.Position);
    }
    #endregion

    #region Outputs
    private void PositionOutput()
    {
        var status = !_evacuee.IsInGroup ? SoloOutput() : _evacuee.IsLeader ? LeaderOutput() : NonLeaderOutput();

        Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} has moved to cell {_evacuee.Position} {status}");
    }
    
    private string LeaderOutput()
    {
        string status;

        if (_evacuee.ReturningWithGroupForItem)
        {
            status = _evacuee.AgentReturningForItem
                ? "(Is returning for item and group is following)"
                : "(Group member forgot Item & Leader is returning with them)";
        }
        else if (_evacuee.ReturningWithGroupToHelp)
        {
            status = _evacuee.Helping
                ? $"(Is carrying agent {_evacuee.Helped.ID})" : $"(Is returning to help {_evacuee.Helped.GetType().Name} {_evacuee.Helped.ID} With group)";
        }
        else 
        {
            status = _evacuee.Group.Count == 0
                ? "(Is Moving alone)"
                : "(Is leading group)";
        }
        return status;
    }
    private string SoloOutput()
    {
        string status;

        if (_evacuee.AgentReturningForItem)
        {
            status = "(Is returning for item)";
        }
        else if (_evacuee.FoundDistressedAgent)
        {
            status = _evacuee.Helping
                ? $"(Is helping {_evacuee.Helped.GetType()} {_evacuee.Helped.ID})"
                : _evacuee.ReachedDistressedAgent
                    ? $"(Has reached {_evacuee.Helped.GetType().Name} {_evacuee.Helped.ID} and is now heading exit)"
                    : $"is going to help {_evacuee.Helped.GetType().Name} {_evacuee.Helped.ID})";
        }
        else
        {
            status = "(Is moving alone)";
        }

        return status;
    }
    
    private string NonLeaderOutput()
    {
        string status;

        if (_evacuee.ReturningWithGroupForItem)
        {
            status = "(Is returning for item with group)";
        }
        else if (_evacuee.ReturningWithGroupToHelp)
        {
            status = _evacuee.Helping
                ? $"(Is Helping {_evacuee.Helped.GetType().Name}  {_evacuee.Helped.ID} with group)"
                : $"(Is returning to help {_evacuee.Helped.GetType().Name}  {_evacuee.Helped.ID} with group)";
        }
        else
        {
            status = "(Is moving in group)";
        }

        return status;
    }
    #endregion

}