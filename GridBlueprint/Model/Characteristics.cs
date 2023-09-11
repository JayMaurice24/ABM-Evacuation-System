using System;

namespace GridBlueprint.Model;

public abstract class Characteristics
{
        public static int LowRisk()
        {
            return Rand.Next(1, 4);
        }
        public static int MediumRisk()
        {
            return Rand.Next(4, 7);
        }

        public static int HighRisk()
        {
            return Rand.Next(7, 10);
        }

        public static int LowSpeed()
        {
            return Rand.Next(6, 10);
        }

        public static int HighSpeed()
        {
            return Rand.Next(1, 5);
        }
        

        private static readonly Random Rand = new Random(); 
}