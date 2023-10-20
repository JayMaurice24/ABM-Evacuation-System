using System;
using System.Collections.Generic;
using System.Linq;


namespace EvacuationSystem.Model;

public class HandleGroup
{
    private readonly Evacuee _evacuee;
    private readonly Random _rand = new Random();
    private readonly GridLayer _layer;
    public HandleGroup(Evacuee evacuee, GridLayer layer)
    {
        _evacuee = evacuee;
        _layer = layer; 
    }

    public void Update()
    {
        if (_evacuee.IsLeader)
        {
            LeaderUpdate();
        }
        else
        {
            FollowerUpdate();
        }
    }
    /// <summary>
    /// Updates a leader's group status 
    /// </summary>
    private void LeaderUpdate()
    {
        if (_rand.NextDouble() < 0.5)
        {
            switch (_evacuee.Group.Count)
            {
                case 1:
                    DisbandGroup();
                    break;
                case > 1:
                    LeaderChange();
                    break;
                default:
                    _evacuee.IsLeader = false;
                    _evacuee.IsInGroup = false;
                    break;
            }
        }
        else
        {
            ToSplitOrNotToSplit();
        }
    }
    /// <summary>
    /// disbands group
    /// </summary>
    private void DisbandGroup()
    {
        var agent = _evacuee.Group[0]; 
        agent.IsInGroup = false;
        agent.Leader = null;
        agent.Movement.Agent.HandleGroupLeave();
        Console.WriteLine($"{agent.GetType().Name} {agent.ID} Has left the group and is now moving alone");

        _evacuee.Group.Clear();
        _evacuee.IsInGroup = false;
        _evacuee.IsLeader = false;
        if (_evacuee.FoundDistressedAgent)
        {
            _evacuee.Movement.Agent.HandleReturnForHelp();
        }
        else
        {
            _evacuee.Movement.Agent.HandleReturnForItem();
        }

        Console.WriteLine(_evacuee.FoundDistressedAgent
            ? $"{_evacuee.GetType().Name} {_evacuee.ID} is no longer leading the group and is returning to help an agent"
            : $"{_evacuee.GetType().Name} {_evacuee.ID} is no longer leading the group and is returning to find an item");
        ModelOutput.NumGroupLeave++;
    }
    /// <summary>
    /// Changes leader 
    /// </summary>
    private void LeaderChange()
    {
        var group = _evacuee.Group;
        var newLeader = group.OrderByDescending(agent => agent.Leadership).First();
        newLeader.IsLeader = true;
        newLeader.Leader = null;

        foreach (var agent in _evacuee.Group.Where(agent => agent != newLeader))
        {
            agent.Leader = newLeader;
            newLeader.Group.Add(agent);
        }
        newLeader.Goal = newLeader.Movement.ExitWithNoFire();
        _evacuee.Group.Clear();
        
        _evacuee.IsLeader = false;
        _evacuee.IsInGroup = false;


        Console.WriteLine(_evacuee.FoundDistressedAgent
            ? $"{_evacuee.GetType().Name} {_evacuee.ID} is no longer leading the group and is returning to help an agent"
            : $"Group has been split, {newLeader.GetType().Name} {newLeader.ID} is the new leader");
        ModelOutput.NumGroupLeave++;
    }
    /// <summary>
    /// Creates a new group
    /// </summary>
/// <param name="newGroup"></param>
    private void CreateNewGroup(List<Evacuee> newGroup)
    {
        switch (newGroup.Count)
        {
            case >= 1 when newGroup.Count == 1:
            {
                var agent = newGroup[0];
                agent.IsInGroup = false;
                agent.Leader = null;
                agent.Movement.Agent.HandleGroupLeave();
                Console.WriteLine($"{agent.GetType().Name} {agent.ID} Has left group and is moving alone");
                ModelOutput.NumGroupLeave++;
                break;
            }
            case > 1:
            {
                var newLead = newGroup.OrderByDescending(agent => agent.Leadership).First();
                newLead.IsLeader = true;
                newLead.Leader = null;
                newLead.Movement.Agent.HandleGroupLeave();

                foreach (var agent in newGroup)
                {
                    newLead.Group.Add(agent);
                    agent.Leader = newLead;
                }

                Console.WriteLine(
                    $"Group has been split into two, {newLead.GetType().Name} {newLead.ID} is the second group leader");
                ModelOutput.NumGroupSplits++;
                break;
            }
        }
    }
    
    /// <summary>
    /// Decides if a group should be split 
    /// </summary>
    private void ToSplitOrNotToSplit()
    {
        var newGroup = new List<Evacuee>();
        var remainingMembers = new List<Evacuee>();

        foreach (var agent in _evacuee.Group)
        {
            if (agent.Empathy > 0.5)
            {
                if (_evacuee.FoundDistressedAgent)
                {
                    agent.ReturningWithGroupToHelp = true;
                    agent.FoundDistressedAgent = true;
                    agent.Helped = _evacuee.Helped;
                }
                else
                {
                    agent.ReturningWithGroupForItem = true;
                }

                remainingMembers.Add(agent);
            }
            else
            {
                newGroup.Add(agent);
            }
        }

        if (remainingMembers.Count > 0)
        {  _evacuee.Group.Clear();
            _evacuee.Group = remainingMembers;
            ModelOutput.NumGroupReturns++;
        }
        if (_evacuee.FoundDistressedAgent)
        {
            _evacuee.Movement.Agent.HandleReturnForHelp();
        }
        else
        {
            _evacuee.Movement.Agent.HandleReturnForItem();
            ModelOutput.NumberRet++;
        }
        if(newGroup.Count < 1) return;
        CreateNewGroup(newGroup);
    }
    
    /// <summary>
    /// Update group status for followers
    /// </summary>
    private void FollowerUpdate()
    {
        if (_rand.NextDouble() > 0.5)
        { 
            LeaveGroup();
        }
        else
        {
            ToGoAloneOrNotToGoALone();
        }
    }
    
    /// <summary>
    /// takes an agent our of a group
    /// </summary>
    private void LeaveGroup()
    {
        _evacuee.Leader.Group.Remove(_evacuee);
        _evacuee.Leader = null;
        _evacuee.IsInGroup = false;
        if (_evacuee.FoundDistressedAgent)
        {
            _evacuee.Movement.Agent.HandleReturnForHelp();
            Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} Has left group and is returning to assist an agent");
        }
        else
        {
            _evacuee.Movement.Agent.HandleReturnForItem();
            Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} Has left group and is returning for item alone");
        }
    }
    /// <summary>
    /// Same as split, from a follower's perspective
    /// </summary>
    private void ToGoAloneOrNotToGoALone()
    {
        var newGroup = new List<Evacuee>();
        var remainingMembers = new List<Evacuee>();

        foreach (var agent in _evacuee.Leader.Group.Where(agent => agent != _evacuee))
        {
            if (agent.Empathy > 0.5)
            {
                if (_evacuee.FoundDistressedAgent)
                {
                    agent.ReturningWithGroupToHelp = true;
                    agent.FoundDistressedAgent = true;
                    agent.Helped = _evacuee.Helped;
                }
                else
                {
                    agent.ReturningWithGroupForItem = true;
                }

                remainingMembers.Add(agent);
            }
            else
            {
                newGroup.Add(agent);
            }
        }
        if (_evacuee.Leader == null) return;
        if (remainingMembers.Count > 0)
        {   _evacuee.Leader.Group.Clear();
            _evacuee.Leader.Group = remainingMembers;
            ModelOutput.NumGroupReturns++;
        }
        if (_evacuee.FoundDistressedAgent)
        {
            _evacuee.Leader.ReturningWithGroupToHelp = true;
            _evacuee.Leader.FoundDistressedAgent = true;
            _evacuee.Leader.Helped = _evacuee.Helped;
            _evacuee.ReturningWithGroupToHelp = true;
            _evacuee.Helped.Helper = _evacuee.Leader;
            _evacuee.Leader.Movement.Agent.HandleReturnForHelp();
        }
        else
        {
            _evacuee.Leader.ReturningWithGroupForItem = true;
            _evacuee.Leader.Movement.Agent.HandleLeaderReturningForItem(_evacuee.Movement.OriginalPosition());
            ModelOutput.NumberRet++;
        }
        if (newGroup.Count<1) return;
        CreateNewGroup(newGroup);
    }
}