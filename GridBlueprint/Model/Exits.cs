using System;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;
namespace GridBlueprint.Model;

public class Exits : IEntity, IPositionable
{
    public Position Position { get; set; }
    public bool IsOpen { get; private set;}
    private bool IsLocked { get; set;}
    //private int state; 
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
