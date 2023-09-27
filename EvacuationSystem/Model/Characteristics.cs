using System;

namespace EvacuationSystem.Model;

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
            return Rand.Next(3, 5);
        }

        public static int HighSpeed()
        {
            return Rand.Next(1, 3);
        }

        private static readonly Random Rand = new Random(); 
}