using System.Collections.Generic;
using Mars.Interfaces.Environments;

namespace GridBlueprint.Model;

public abstract class MovementDirections
{
    public static readonly Position North = new(0, 1);
    public static readonly Position Northeast = new(1, 1);
    public static readonly Position East = new(1, 0);
    public static readonly Position Southeast = new(1, -1);
    public static readonly Position South = new(0, -1);
    public static readonly Position Southwest = new(-1, -1);
    public static readonly Position West = new(-1, 0);
    public static readonly Position Northwest = new(-1, 1);
    
    public static List<Position> CreateMovementDirectionsList()
    {
        return new List<Position>
        {
            MovementDirections.North,
            MovementDirections.Northeast,
            MovementDirections.East,
            MovementDirections.Southeast,
            MovementDirections.South,
            MovementDirections.Southwest,
            MovementDirections.West,
            MovementDirections.Northwest
        };
    }
}