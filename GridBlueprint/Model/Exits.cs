using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Common;
using Mars.Components.Agents;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using Mars.Numerics;
namespace GridBlueprint.Model;

public class Exits : IEntity, IPositionable
{
    public Position Position { get; set; }
    public bool IsOpen { get; private set;}
    private bool IsLocked { get; set;}
    private int state; 
    public Exits()
    {
        IsOpen = false;
        IsLocked = true; 
    }

    public void Initialize(Position position, bool isLocked = true, bool isOpen = true)
    {
        Position = position;
        IsLocked = isLocked;
        IsOpen = isOpen;
    }

    public void Open()
    {
        if (!IsLocked)
        {
            IsOpen = true;
        }
    }

    public void Close()
    {
        IsOpen = false; 
    }

    public Guid ID { get; set; }
}
