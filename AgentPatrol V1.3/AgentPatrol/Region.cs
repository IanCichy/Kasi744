using System;
using AgentPatrol;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

public class Region
{
    private int RegionID;
    private List<Tile> Tiles;
    private List<Agent> Agents;
    int minAgents, maxAgents, curAgents, regWidth, regHeight;
    private Random rnd = new Random();

    public Region(int r)
    {
        RegionID = r;
        minAgents = 1;
        maxAgents = 0;
        curAgents = 0;
        regWidth = 0;
        regHeight = 0;
        Tiles = new List<Tile>();
        Agents = new List<Agent>();
    }
    public void addTileToRegion(Tile t)
    {
        Tiles.Add(t);
        maxAgents = (int)Math.Ceiling((Tiles.Count) / 2.0);
    }

    public List<Tile> getTiles()
    {
        return Tiles;
    }

    public List<Point> getLocations()
    {
        List<Point> p = new List<Point>();
        foreach (Tile t in Tiles)
        {
            Point pt = t.getPointLocation();
            p.Add(pt);
        }
        return p;
    }

    public void addAgent(Agent a)
    {
        Point p = Tiles.ElementAt((rnd.Next(0, Tiles.Count))).getPointLocation();
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
