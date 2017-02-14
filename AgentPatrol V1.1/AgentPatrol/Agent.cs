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

    public Agent(int id)
    {
        agentID = id;
    }

    public void setLocation(int x, int y)
    {
        Location = new Point(x, y);
    }

    public void setLocation(Point p)
    {
        Location = p;
    }

    public Point getLocation()
    {
        return Location;
    }
}
