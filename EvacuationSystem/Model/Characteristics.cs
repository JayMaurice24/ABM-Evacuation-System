using System;

namespace EvacuationSystem.Model;

public abstract class Characteristics
{
        public static int LowRisk()
        {
            return Rand.Next(1, 6);
        }
        public static int MediumRisk()
        {
            return Rand.Next(5, 11);
        }

        public static int HighRisk()
        {
            return Rand.Next(10, 16);
        }

        public static int LowSpeed()
        {
            return Rand.Next(2, 4);
        }

        public static int HighSpeed()
        {
            return 1;
        }

        private static readonly Random Rand = new Random(); 
}