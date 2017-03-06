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
    private List<List<Point>> NStepTargetList;
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
        NStepTargetList = new List<List<Point>>();
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

        foreach (Agent a in Agents)
            if (a.hasTarget())
                return false;

        return true;
    }

    public void addRandomAgent(Agent a)
    {
        Tile t = Tiles.ElementAt(rnd.Next(0, Tiles.Count));
        Agents.Add(a);
        curAgents++;
        setAgentLocation(a, t);
    }

    public void addAgent(Agent a, Tile t)
    {
        Agents.Add(a);
        curAgents++;
        setAgentLocation(a, t);
    }

    public void updateNStepTargetList()
    {
        NStepTargetList.Add(TargetList);
    }

    public List<List<Point>> getNStepTargetList()
    {
        return NStepTargetList;
    }



    /*
     * MAIN METHOD USED FOR ALGORITHM
     */
    public Tile findShortestPathMove(Agent a)
    {
        var previous = new Dictionary<Tile, Tile>();
        var queue = new Queue<Tile>();

        Tile MyTarget = null;
        Tile MyLocation = null;
        foreach (Tile t in Tiles)
        {
            if (t.getPointLocation().Equals(a.getTarget()))
                MyTarget = t;

            if (t.getPointLocation().Equals(a.getLocation()))
                MyLocation = t;
        }
        queue.Enqueue(MyLocation);


        while (queue.Count > 0)
        {
            Tile tle = queue.Dequeue();
            foreach (var neighbor in getAdjacentTiles(tle))
            {
                if (previous.ContainsKey(neighbor))
                    continue;

                previous[neighbor] = tle;
                queue.Enqueue(neighbor);
            }
        }

        var path = new List<Tile>();
        var current = MyTarget;
        while (!current.Equals(MyLocation))
        {
            path.Add(current);
            current = previous[current];
        };

        path.Reverse();

        return path.ElementAt(0);
    }

    public List<Tile> getAdjacentTiles(Tile CurrentTile)
    {
        var adj = new List<Tile>();

        foreach (Tile tle in Tiles)
        {
            if (tle.getPointLocation().Equals(new Point(CurrentTile.getxPos() + 1, CurrentTile.getyPos())) ||
                tle.getPointLocation().Equals(new Point(CurrentTile.getxPos() - 1, CurrentTile.getyPos())) ||
                tle.getPointLocation().Equals(new Point(CurrentTile.getxPos(), CurrentTile.getyPos() + 1)) ||
                tle.getPointLocation().Equals(new Point(CurrentTile.getxPos(), CurrentTile.getyPos() - 1)))
            {
                adj.Add(tle);
            }
        }

        return adj;
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
