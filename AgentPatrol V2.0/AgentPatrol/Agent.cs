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
    private List<Point> Visited, Targets;

    public Agent(int id)
    {
        agentID = id;
        Visited = new List<Point>();
        Targets = new List<Point>();
    }

    public void setLocation(int x, int y)
    {
        Point p = new Point(x, y);
        Location = p;
        Visited.Add(p);
    }

    public void setLocation(Point p)
    {
        Location = p;
        Visited.Add(p);
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
}
