using System;
using AgentPatrol;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

public class Region
{
    private int RegionID;
    private List<Point> Locations;
    private List<Agent> Agents;
    int minAgents, maxAgents, curAgents;
    private Random rnd = new Random();

    public Region(int r)
    {
        RegionID = r;
        minAgents = 1;
        maxAgents = 0;
        curAgents = 0;
        Locations = new List<Point>();
        Agents = new List<Agent>();
    }

    public void addPointToRegion(Point p)
    {
        Locations.Add(p);
        maxAgents = (int)Math.Ceiling((Locations.Count) / 2.0);
    }

    public List<Point> getLocations()
    {
        return Locations;
    }
    
    public void addAgent(Agent a)
    {        
        Point p = Locations.ElementAt((rnd.Next(0, Locations.Count)));
        a.setLocation(p.X, p.Y);
        Agents.Add(a);
        curAgents++;
    }

    public List<Agent> getAgents()
    {
        return Agents;
    }

    public int getMinAgents()
    {
        return minAgents;
    }

    public int getMaxAgents()
    {
        return maxAgents;
    }

    public int getCurAgents()
    {
        return curAgents;
    }

    public void setCurAgents(int c)
    {
        curAgents = c;
    }

    public int getID()
    {
        return RegionID;
    }
}
