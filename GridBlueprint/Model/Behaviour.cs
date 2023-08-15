using System;

namespace GridBlueprint.Model;

public class Behaviour
{
    public int Risk;
    public int Push;
    public int Movement;


    public Behaviour()
    {
        Risk = Riskiness();
        Push = Pushiness();
        Movement = Speed();
    }

        private int Riskiness()
        {
            var rand = new Random();
            return rand.Next(1, 10);
        }

        private int Pushiness()
        {
            var rand = new Random();
            return rand.Next(1, 10);
        }

        private int Speed()
        {
            var rand = new Random();
            return rand.Next(1, 10);
        }
    }