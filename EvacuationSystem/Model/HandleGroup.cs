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
    
    private void DisbandGroup()
    {
        foreach (var agent in _evacuee.Group)
        {
            agent.IsInGroup = false;
            agent.Leader = null;
            agent.Goal = agent.FindNearestExit(_layer.Exits);
            Console.WriteLine($"{agent.GetType().Name} {agent.ID} Has left the group and is now moving alone");
        }

        _evacuee.Group.Clear();
        _evacuee.IsInGroup = false;
        _evacuee.IsLeader = false;

        if (_evacuee.FoundDistressedAgent)
        {
            _evacuee.ReturningWithGroupToHelp = false;
            Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} is no longer leading the group and is returning to help an agent");
        }
        else
        {
            _evacuee.ReturningWithGroupForItem = false;
            Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} is no longer leading the group and is returning to find an item");
        }
        ModelOutput.NumGroupLeave++;
    }
    private void LeaderChange()
    {
        var newLeader = _evacuee.Group.OrderByDescending(agent => agent.Leadership).First();
        newLeader.IsLeader = true;

        foreach (var a in _evacuee.Group)
        {
            a.Leader = newLeader;
            newLeader.Group.Add(a);
        }
        newLeader.Goal = newLeader.FindNearestExit(_layer.Exits);
        _evacuee.Group.Clear();
        
        _evacuee.IsLeader = false;
        _evacuee.IsInGroup = false;
        

        if (_evacuee.FoundDistressedAgent)
        {
            _evacuee.ReturningWithGroupToHelp = false;
            Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} is no longer leading the group and is returning to help an agent");
        }
        else
        {
            _evacuee.ReturningWithGroupForItem = false;
            Console.WriteLine($"Group has been split, {newLeader.GetType().Name} {newLeader.ID} is the new leader");
        }
        ModelOutput.NumGroupLeave++;
    }

    private void CreateNewGroup(List<Evacuee> newGroup)
    {
        switch (newGroup.Count)
        {
            case >= 1 when newGroup.Count == 1:
            {
                var agent = newGroup[0];
                agent.IsInGroup = false;
                agent.Leader = null;
                agent.Goal = agent.FindNearestExit(_layer.Exits);
                Console.WriteLine($"{agent.GetType().Name} {agent.ID} Has left group and is moving alone");
                ModelOutput.NumGroupLeave++;
                break;
            }
            case >= 1:
            {
                var newLead = newGroup.OrderByDescending(agent => agent.Leadership).First();
                newLead.IsLeader = true;
                newLead.Leader = null;
                newLead.Goal = newLead.FindNearestExit(_layer.Exits);

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

        _evacuee.Group.Clear();
        _evacuee.Group = remainingMembers;
        if(newGroup.Count < 1) return;
        CreateNewGroup(newGroup);
    }
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
    
    private void LeaveGroup()
    {
        _evacuee.Leader.Group.Remove(_evacuee);
        _evacuee.Leader = null;
        _evacuee.IsInGroup = false;
        Console.WriteLine($"{_evacuee.GetType().Name} {_evacuee.ID} Has left group and is returning for item alone");
    }

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

        _evacuee.Leader.Group.Clear();
        _evacuee.Leader.Group = remainingMembers;

        if (_evacuee.Leader == null) return;

        if (_evacuee.FoundDistressedAgent)
        {
            _evacuee.Leader.ReturningWithGroupToHelp = true;
            _evacuee.Leader.FoundDistressedAgent = true;
            _evacuee.ReturningWithGroupToHelp = true;
        }
        else
        {
            _evacuee.Leader.ReturningWithGroupForItem = true;
            _evacuee.Leader.Goal = _evacuee.OriginalPosition;
        }
        if (newGroup.Count<1) return;
        CreateNewGroup(newGroup);
    }
}