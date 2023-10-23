using System.Collections.Generic;
using Mars.Interfaces.Environments;

namespace EvacuationSystem.Model;

public abstract class MovementDirections
{
    private static readonly Position North = new(0, 1);
    private static readonly Position Northeast = new(1, 1);
    private static readonly Position East = new(1, 0);
    private static readonly Position Southeast = new(1, -1);
    private static readonly Position South = new(0, -1);
    private static readonly Position Southwest = new(-1, -1);
    private static readonly Position West = new(-1, 0);
    private static readonly Position Northwest = new(-1, 1);
    
    public static List<Position> CreateMovementDirectionsList()
    {
        return new List<Position>
        {
            North,
            Northeast,
            East,
            Southeast,
            South,
            Southwest,
            West,
            Northwest
        };
    }
}