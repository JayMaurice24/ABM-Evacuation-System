using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Common;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using Mars.Numerics;
namespace GridBlueprint.Model;

public class Exits 
{
    public Position Position { get; }
    public bool IsOpen { get; private set;}
    public bool IsLocked { get; private set;}

    public Exits(Position position, bool isLocked = true, bool isOPen = true)
    {
        Position = position;
        IsLocked = isLocked;
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
    
}