using System;
using AgentPatrol;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

public class Agent
{
    private int agentID;
    private Point Location;
    private Point Target;
    private List<Point> Visited, Targets;

    /*
     * Creates an agent and initalizes the 
     * visited and targets lists
     */
    public Agent(int id)
    {
        agentID = id;
        Visited = new List<Point>();
        Targets = new List<Point>();
    }

    /*
     *  Set the location of the agent
     *  also adds it to the visited list
     */
    public void setLocation(Point p)
    {
        Location = p;
        Visited.Add(p);
    }

    /*
     *  Set the target of the agent
     *  also adds it to the target list
     */
    public void setTarget(Point p)
    {
        Target = p;
        Targets.Add(p);
    }

    public Point getTarget()
    {
        return Target;
    }

    public int getID()
    {
        return agentID;
    }

    public Point getLocation()
    {
        return Location;
    }

    public List<Point> getVisitedList()
    {
        return Visited;
    }

    public List<Point> getTargetList()
    {
        return Targets;
    }
}
