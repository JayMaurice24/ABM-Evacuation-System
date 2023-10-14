using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Common;
using Mars.Interfaces.Environments;
using Mars.Numerics;

namespace EvacuationSystem.Model;

public class Evacuate
{
    private readonly Evacuee _evacuee;
    private readonly GridLayer _layer;
    private bool _tripInProgress;
    private List<Position>.Enumerator _path;

    public Evacuate(Evacuee evacuee, GridLayer layer)
    {
        _evacuee = evacuee;
        _layer = layer;
    }

    #region Solo Movement or Leader Movement

    public void Move()
    {
        if (!_tripInProgress)
        {
            FindRoute();
        }
        if (!_path.MoveNext()) return;
        if (_evacuee.AvoidFire(_path.Current))
        {
            FireAvoidance();
        }
        else if (IsCellOccupied(_path.Current))
        {
            if(!_evacuee.Helping){ToPushOrNotToPush();}
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
        if (!_evacuee.IsInGroup) return;
        if (!_tripInProgress)
        {
            _evacuee.Goal = _evacuee.Leader.Position;
            FindRoute();
        }
        if (!_path.MoveNext()) return;
        UpdatePosition(_path.Current);
        if (_evacuee.LeaderHasReachedExit){ _evacuee.Goal = _evacuee.FindNearestExit(_layer.Exits); return;}
        if (!_evacuee.Position.Equals(_evacuee.Goal)) return;
        if (_layer.Exits.Contains(_evacuee.Goal))
        {
            ExitReached();
        }
        else
        {
            _tripInProgress = false;
        }
    }
    private Position AvoidCollision(Position target)
    {
        var group = GetNearByObstacles(); 
        if(group == null) return target;
        foreach (var evacuee in group)
        {
            if (!Equals(target, evacuee.Position)) return target;
            if (!_evacuee.Leader.Group.Contains(_evacuee))
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
    public void MoveWithHelp()
    {
        var target =  _evacuee.Helper.Position;
        var directionToMove =
            PositionHelper.CalculateBearingCartesian(_evacuee.Position.X, _evacuee.Position.Y, target.X, target.Y);
        _layer.EvacueeEnvironment.MoveTowards(_evacuee, directionToMove, 1);
        Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} has been moved to {_evacuee.Position} (is being carried by {_evacuee.Helper.GetType().Name} {_evacuee.Helper.ID} ");
    }
    

    private void FindRoute()
    {
        _path = _layer.FindPath(_evacuee.Position, _evacuee.Goal).GetEnumerator();
        _tripInProgress = true;
    }

    private void FireAvoidance()
    {
        var next = _evacuee.ChangeDirection(_path.Current);
        MoveToCell(next);
        _path = _layer.FindPath(_evacuee.Position, _evacuee.Goal).GetEnumerator();
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
        if (_evacuee.AgentReturningForItem && _evacuee.Goal.Equals(_evacuee.OriginalPosition))
        {
            ItemFound();
        }
        else if (_evacuee.FoundDistressedAgent)
        {
            ReachedHelplessEvacuee();
        }
        else if (_layer.Exits.Contains(_evacuee.Goal))
        {
            ExitReached();
        }
    }
    
    private void ItemFound()
    {
        Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} has Found Item and is heading to the exit");
        _evacuee.AgentReturningForItem = false;
        if(_evacuee.ReturningWithGroupForItem) GroupReturn();
        _evacuee.Goal = _evacuee.FindNearestExit(_layer.Exits);
        FindRoute();
    }

    private void ReachedHelplessEvacuee()
    {
        Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} has Reached {_evacuee.Helped.GetType().Name} {_evacuee.Helped.ID} is heading to the exit");
        _evacuee.ReachedDistressedAgent = true; 
        _evacuee.OfferHelp();
        if(_evacuee.ReturningWithGroupToHelp) GroupReturn();
        _evacuee.Goal = _evacuee.FindNearestExit(_layer.Exits);
        FindRoute();
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
                _evacuee.ReturningWithGroupForItem = false;
            }
        }
      
    }

    private void ExitReached()
    {
        Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} has reached exit {_evacuee.Goal}");
        if (_evacuee.IsLeader)
        {
            foreach (var agent in _evacuee.Group)
            {
                agent.LeaderHasReachedExit = true;
                Console.WriteLine($"{agent.GetType().Name} {agent.ID}'s group leader has reached exit {_evacuee.Goal} and is heading for exit too");
            }
        }

        ModelOutput.NumReachExit++;
        ModelOutput.NumAgentsLeft--;
        _layer.RemoveFromSimulation(_evacuee);
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
            status = _evacuee.AgentReturningForItem
                ? "(Is returning for item with group)"
                : "(Group member forgot Item & Member is returning with them)";
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