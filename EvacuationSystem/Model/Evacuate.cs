using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        else if (_evacuee.IsCellOccupied(_path.Current))
        {
            ToPushOrNotToPush();
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
    public void MoveWithHelp()
    {
        var target =  _evacuee.Helper.Position;
        _evacuee.Position = target;
        _layer.EvacueeEnvironment.MoveTo(_evacuee, new Position(target.X, target.Y));
    }

    public void MoveTowardsLeader()
    {
        if (!_evacuee.IsInGroup) return;
        var socialForce = SocialForceModel.CalculateSocialForce(_evacuee.Leader, _evacuee, _layer.EvacueeEnvironment.Explore(_evacuee.Position, radius: 5).ToList());
        var nearestObstacle = GetNearestObstacle();
        var obstacleAvoidanceForce = SocialForceModel.CalculateObstacleAvoidanceForce(_evacuee, nearestObstacle);
        var netForce = socialForce + obstacleAvoidanceForce;

        UpdateVelocity(netForce);
        UpdatePosition();
        PrintMovementStatus();
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
        var otherAgent = GetAgentAt(_evacuee.Position);

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
    
    private void PositionOutput()
    {
        var status = _evacuee.IsLeader ? LeaderOutput() : NonLeaderOutput();

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
                ? "(Is returning for item and group is following)"
                : _evacuee.ReachedDistressedAgent
                    ? $"(Is carrying agent {_evacuee.Helped.ID})"
                    : $"(Is returning to help {_evacuee.Helped.GetType().Name} {_evacuee.Helped.ID} With group)";
        }
        else
        {
            status = _evacuee.Group.Count == 0
                ? "(Is Moving alone)"
                : "(Is leading group)";
        }

        return status;
    }
    
    
    private string NonLeaderOutput()
    {
        string status;

        if (_evacuee.AgentReturningForItem)
        {
            status = "(Is returning for item)";
        }
        else if (_evacuee.FoundDistressedAgent)
        {
            status = _evacuee.Helping
                ? $"(Is carrying agent {_evacuee.Helped.ID})"
                : _evacuee.ReachedDistressedAgent
                    ? $"(Has reached at cell {_evacuee.Position} {_evacuee.Helped.GetType().Name} {_evacuee.Helped.ID} and is now heading exit)"
                    : $"(Has moved to cell {_evacuee.Position} and is going to help {_evacuee.Helped.GetType().Name} {_evacuee.Helped.ID})";
        }
        else
        {
            status = "(Is moving alone)";
        }

        return status;
    }
    
    private void HandleGoalReached()
    {
        if (_evacuee.AgentReturningForItem && _evacuee.Goal.Equals(_evacuee.OriginalPosition))
        {
            ItemFound();
        }
        else if (_layer.Stairs.Contains(_evacuee.Goal))
        {
            ExitReached();
        }
        else
        {
            _evacuee.Goal = _evacuee.FindNearestExit(_layer.Stairs);
            _path = _layer.FindPath(_evacuee.Position, _evacuee.Goal).GetEnumerator();
        }
    }
    
    private void ItemFound()
    {
        Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} has Found Item and is heading to the exit");
        _evacuee.AgentReturningForItem = false;
        _tripInProgress = false;
        if(_evacuee.IsInGroup)GroupReturn();
        _evacuee.Goal = _evacuee.FindNearestExit(_layer.PossibleGoal);
        _path = _layer.FindPath(_evacuee.Position, _evacuee.Goal).GetEnumerator();
    }

    private void GroupReturn()
    {
        if (!_evacuee.IsLeader) return;
        foreach (var agent in _evacuee.Group)
        {
            agent.ReturningWithGroupForItem = false;
            _evacuee.ReturningWithGroupForItem = false;
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
                agent.Goal = _evacuee.Goal;
            }
        }

        _layer.RemoveFromSimulation(_evacuee);
    }
    private void UpdateVelocity(Vector2 netForce)
    {
        // Update velocity using net force
        _evacuee._currentVelocity += netForce;

        // Limit the velocity to a maximum value (MaxSpeed)
        if (_evacuee._currentVelocity.Length() > Evacuee.MaxSpeed)
        {
            _evacuee._currentVelocity = Vector2.Normalize(_evacuee._currentVelocity) * Evacuee.MaxSpeed;
        }
    }
    
    private Evacuee GetNearestObstacle()
    {
        var nearest = _layer.EvacueeEnvironment.Entities.MinBy(agent =>
            Distance.Chebyshev(_evacuee.Position.PositionArray, agent.Position.PositionArray));

        if (nearest != _evacuee && nearest != null) return nearest;
        return null;
    }

    private bool IsValidPosition(Position newPosition)
    {
        return 0 <= newPosition.X && newPosition.X < _layer.Width &&
               0 <= newPosition.Y && newPosition.Y < _layer.Height &&
               _layer.IsRoutable(newPosition.X, newPosition.Y);
    }
    private void UpdatePosition()
    {
        var newPosition = CalculateNewPosition();

        if (!IsValidPosition(newPosition)) return;

        if (_evacuee.AvoidFire(_path.Current))
        {
            newPosition = _evacuee.ChangeDirection(newPosition);
        }
        else if(_evacuee.IsCellOccupied(newPosition))
        {
            ToPushOrNotToPush();
        }
       MoveToCell(newPosition);
        _evacuee.Position = newPosition;
    }
    
    private Position CalculateNewPosition()
    {
        // Implement logic to calculate the new position based on velocity
        // Example: return new Position((int)(Position.X + _currentVelocity.X), (int)(Position.Y + _currentVelocity.Y));
        return new Position((int)(_evacuee.Position.X + _evacuee._currentVelocity.X), (int)(_evacuee.Position.Y + _evacuee._currentVelocity.Y));
    }
    private void PrintMovementStatus()
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
            status = $"(Is Helping {_evacuee.Helped.GetType().Name}  {_evacuee.Helped.ID} with group)";
        }
        else
        {
            status = "(Is moving in group)";
        }

        Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} has moved to cell {_evacuee.Position} {status}");
    }


}