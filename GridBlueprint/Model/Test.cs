namespace GridBlueprint.Model;

public class Test
{
    /*
    public int Id { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double VelocityX { get; set; }
    public double VelocityY { get; set; }
    public bool IsLeader { get; set; }
}

public class EvacuationModel
{
    private List<Agent> agents;

    public EvacuationModel()
    {
        agents = new List<Agent>();
    }

    public void AddAgent(int id, double x, double y)
    {
        var agent = new Agent { Id = id, X = x, Y = y };
        agents.Add(agent);
    }

    public void UpdateModel()
    {
        SelectLeaders();
        MoveGroups();
    }

    private void SelectLeaders()
    {
        // Find the closest agent to the exit as the leader for each group
        foreach (var agent in agents)
        {
            if (agent.IsLeader)
                continue;

            double minDistance = double.MaxValue;
            foreach (var otherAgent in agents)
            {
                if (otherAgent == agent || otherAgent.IsLeader)
                    continue;

                double distance = CalculateDistance(agent, otherAgent);
                if (distance < minDistance)
                    minDistance = distance;
            }

            agent.IsLeader = true;
        }
    }
private void MoveGroups()
{
    const double agentRadius = 0.5; // Radius of each agent (adjust as needed)
    const double relaxationTime = 0.5; // Relaxation time for the Social Force Model (adjust as needed)
    const double repulsionFactor = 100; // Repulsion factor for the Social Force Model (adjust as needed)

    foreach (var agent in agents)
    {
        if (agent.IsLeader)
            continue;

        // Find the leader of the group
        var leader = FindGroupLeader(agent);

        // Calculate desired velocity towards the leader
        double desiredVelocityX = (leader.X - agent.X) / CalculateDistance(agent, leader);
        double desiredVelocityY = (leader.Y - agent.Y) / CalculateDistance(agent, leader);

        // Calculate the forces acting on the agent
        double forceX = 0;
        double forceY = 0;

        // Repulsion forces from other agents
        foreach (var otherAgent in agents)
        {
            if (agent == otherAgent)
                continue;

            double distance = CalculateDistance(agent, otherAgent);
            double overlap = agentRadius - distance;

            if (overlap > 0)
            {
                double forceMagnitude = repulsionFactor * Math.Exp(-overlap / agentRadius) / distance;
                forceX += forceMagnitude * (agent.X - otherAgent.X);
                forceY += forceMagnitude * (agent.Y - otherAgent.Y);
            }
        }

        // Calculate the resulting acceleration
        double accelerationX = (desiredVelocityX - agent.VelocityX) / relaxationTime + forceX;
        double accelerationY = (desiredVelocityY - agent.VelocityY) / relaxationTime + forceY;

        // Adjust agent's velocity towards the desired velocity
        agent.VelocityX += accelerationX;
        agent.VelocityY += accelerationY;
    }
}
    private void MoveGroups()
    {
        foreach (var agent in agents)
        {
            if (agent.IsLeader)
                continue;

            // Find the leader of the group
            var leader = FindGroupLeader(agent);

            // Calculate desired velocity towards the leader
            double desiredVelocityX = (leader.X - agent.X) / CalculateDistance(agent, leader);
            double desiredVelocityY = (leader.Y - agent.Y) / CalculateDistance(agent, leader);

            // Adjust agent's velocity towards the desired velocity
            double velocityDeltaX = desiredVelocityX - agent.VelocityX;
            double velocityDeltaY = desiredVelocityY - agent.VelocityY;
            double accelerationX = velocityDeltaX / 10; // Adjust the acceleration factor as needed
            double accelerationY = velocityDeltaY / 10; // Adjust the acceleration factor as needed
            agent.VelocityX += accelerationX;
            agent.VelocityY += accelerationY;
        }
    }

    private Agent FindGroupLeader(Agent agent)
    {
        // Find the leader of the group that the agent belongs to
        foreach (var otherAgent in agents)
        {
            if (otherAgent.IsLeader && CalculateDistance(agent, otherAgent) < 1)
                return otherAgent;
        }

        // If no leader is found, return the agent itself as a fallback
        return agent;
    }

    private double CalculateDistance(Agent agent1, Agent agent2)
    {
        double deltaX = agent1.X - agent2.X;
        double deltaY = agent1.Y - agent2.Y;
        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }
}

public class Program
{
    public static void Main()
    {
        var model = new EvacuationModel();

        // Adding agents
        model.AddAgent(1, 0, 0);
        model.AddAgent(2, 2, 0);
        model.AddAgent(3, 2, 2);
        model.AddAgent(4, 0, 2);

        // Running the simulation
        for (int i = 0; i < 10; i++)
        {
            model.UpdateModel();

            // Print agent positions
            Console.WriteLine("Time Step: " + i);
            foreach (var agent in model.GetAgents())
            {
                Console.WriteLine("Agent " + agent.Id + " - Position: (" + agent.X + ", " + agent.Y + ")");
            }

            Console.WriteLine();
        }
    }*/
}
