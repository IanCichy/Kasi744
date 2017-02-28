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
    private List<int> Width;
    private List<int> Height;
    private List<Point> TargetList;
    int minAgents, maxAgents, curAgents;
    private Random rnd = new Random();

    /*
     * Creates a Region based on
     * regionID
     * 
     */
    public Region(int r)
    {
        RegionID = r;
        minAgents = 1;
        maxAgents = 0;
        curAgents = 0;
        Tiles = new List<Tile>();
        Agents = new List<Agent>();
        Width = new List<int>();
        Height = new List<int>();
        TargetList = new List<Point>();
    }

    /*
     * Adds Tiles to a region and sets the needed variables 
     * each time a new tile is added
     */
    public void addTileToRegion(Tile t)
    {
        Tiles.Add(t);
        //Add the tile position to the targetlist
        TargetList.Add(t.getPointLocation());
        maxAgents = (int)Math.Floor((Tiles.Count) / 2.0);

        int x = t.getxPos();
        if (!Width.Contains(x))
            Width.Add(x);

        int y = t.getyPos();
        if (!Height.Contains(y))
            Height.Add(y);
    }

    public void setAgentLocation(Agent a, Tile t)
    {
        //make sure this agent and position are valid
        if (Agents.Contains(a) && Tiles.Contains(t))
        {
            //move the agent to the new position
            a.setLocation(t.getPointLocation());
            //Set the tiles type to visited
            if (t.getType() != 2)
                t.setType(2);
            //Remove this location from the target list
            if (TargetList.Contains(t.getPointLocation()))
                TargetList.Remove(t.getPointLocation());
        }
        else
        {
            throw new InvalidOperationException();
        }

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

    public List<Point> getTargetList()
    {
        return TargetList;
    }

    public bool isRegionComplete()
    {
        foreach (Tile t in Tiles)
            if (t.getType() != 2)
                return false;
        return true;
    }

    public void addRandomAgent(Agent a)
    {
        Tile t = Tiles.ElementAt(rnd.Next(0, Tiles.Count));
        setAgentLocation(a,t);
        Agents.Add(a);
        curAgents++;
    }

    public void addAgent(Agent a, Tile t)
    {
        setAgentLocation(a, t);
        Agents.Add(a);
        curAgents++;
    }


    /*
     * TEMP METHOD USED FOR ALGORITHM
     */
    public Tile getBestNeighboringTiles(Agent a)
    {
        int[] rowNbr = new int[] { -1, 1, 0, 0 };
        int[] colNbr = new int[] { 0, 0, -1, 1 };
        List<Tile> options = new List<Tile>();

        //Agent Location
        Point p = a.getLocation();
        Point p1 = new Point(p.X + rowNbr[0], p.Y + colNbr[0]);
        Point p2 = new Point(p.X + rowNbr[1], p.Y + colNbr[1]);
        Point p3 = new Point(p.X + rowNbr[2], p.Y + colNbr[2]);
        Point p4 = new Point(p.X + rowNbr[3], p.Y + colNbr[3]);

        foreach (Tile t in Tiles)
        {
            if (t.getPointLocation().Equals(p1) ||
                t.getPointLocation().Equals(p2) ||
                t.getPointLocation().Equals(p3) ||
                t.getPointLocation().Equals(p4))
            {
                options.Add(t);
            }
        }

        int MaxScore = int.MinValue;
        Tile best = null;
        foreach (Tile t in options)
        {
            if (t.getScore() > MaxScore)
            {
                best = t;
                MaxScore = t.getScore();
            }
        }
        return best;
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

    public int getWidth()
    {
        return Height.Count;
    }

    public int getWidthAdjust()
    {
        return Height.Min();
    }

    public int getHeight()
    {
        return Width.Count;
    }

    public int getHeightAdjust()
    {
        return Width.Min();
    }
}
