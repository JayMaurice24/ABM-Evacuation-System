using System;

namespace GridBlueprint.Model;

public class Behaviour
{
    public int Risk;
    public int Push;
    public int Movement;


    public Behaviour()
    {
        
    }

        public static int LowRisk()
        {
            var rand = new Random();
            return rand.Next(1, 4);
        }
        public static int MediumRisk()
        {
            var rand = new Random();
            return rand.Next(4, 7);
        }

        public static int HighRisk()
        {
            var rand = new Random();
            return rand.Next(7, 10);
        }

        public static int LowSpeed()
        {
            var rand = new Random();
            return rand.Next(1, 4);
        }
        
        public static int MediumSpeed()
        {
            var rand = new Random();
            return rand.Next(4, 7);
        }

        public static int HighSpeed()
        {
            var rand = new Random();
            return rand.Next(7, 10);
        }
    }