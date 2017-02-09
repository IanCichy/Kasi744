using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System;

public class Map
{
    private Tile[,] Tiles;
    //private List<Agent> Agents;
    private Region[] Regions;
    private int width, height, size, openTiles, regionCount, agentCount;
    private Panel container;
    private Random rnd = new Random();


    /*
     * Inputs: 
     *      int[,] M  - A map file for input
     *      int s - the size of each tile in the view
     */
    public Map(int w, int h, int numRegions, Region[] tileRegions, int[,] M, int s)
    {
        openTiles = 0;
        regionCount = numRegions;
        width = M.GetLength(0);
        height = M.GetLength(1);
        size = s;

        Regions = tileRegions;
        //Agents = new List<Agent>();
        Tiles = new Tile[width, height];

        container = new Panel();
        container.Width = width * size;
        container.BorderColor = Color.FromArgb(19, 80 , 147);
        container.BorderWidth = 5;

        for (int i = 0; i < width; i++)
        {
            for (int t = 0; t < height; t++)
            {
                if (M[i, t] == 0)
                    openTiles++;
                Tile tle = new Tile(M[i, t], i, t, size);
                Tiles[i, t] = tle;
                container.Controls.Add(tle.getDisplay());
            }
        }

        agentCount = rnd.Next(regionCount, openTiles / 2);
        makeAgents();
        colorMap();
    }

    private void colorMap()
    {
        //Color the Basic Tiles
        foreach (Tile t in Tiles)
        {
            Color col = Color.White;
            if (t.getType() == 0)
            {
                if (t.isSeen() == true)
                    col = Color.FromArgb(215, 55, 55);//Red
                else
                    col = Color.White;
            }
            else if (t.getType() == 1)
                col = Color.FromArgb(30, 30, 30);//Black

            t.getTile().getDisplay().BackColor = col;
        }

        //Color the Agents Blue
        foreach (Region r in Regions)
        {
            foreach (Agent a in r.getAgents())
            {
                int xl = a.getLocation().X;
                int yl = a.getLocation().Y;
                Tiles[xl, yl].getTile().getDisplay().BackColor = Color.FromArgb(55, 125, 215);//Blue
                Tiles[xl, yl].setSeen(true);
            }
        }
    }

    private void makeAgents()
    {
        int currentAgents = 0;
        foreach (Region r in Regions)
        {
            Agent a = new Agent(currentAgents);
            currentAgents++;
            r.addAgent(a);
        }

        while (currentAgents < agentCount)
        {
            Agent a = new Agent(currentAgents);
            int t = rnd.Next(0, regionCount);

            if (Regions[t].getCurAgents() < Regions[t].getMaxAgents())
            {
                Regions[t].addAgent(a);
                currentAgents++;
            }
        }
    }

    public void oneStepAlgorithm()
    {
        int[] rowNbr = new int[] { -1, 1, 0, 0 };
        int[] colNbr = new int[] { 0, 0, -1, 1 };
        foreach (Region r in Regions)
        {
            foreach (Agent a in r.getAgents())
            {
                int k = rnd.Next(0, 4);
                Point p = new Point(a.getLocation().X + rowNbr[k], a.getLocation().Y + colNbr[k]);
                if (r.getLocations().Contains(p))
                {
                    a.setLocation(p);
                }
            }
        }
        colorMap();
    }





    public void setTileType(int x, int y, int type)
    {
        Tiles[x, y].setType(type);
        colorMap();
    }

    #region get/set Methods
    public Map getMap()
    {
        return this;
    }

    public Tile[,] getTiles()
    {
        return Tiles;
    }

    public Panel getDisplay()
    {
        return container;
    }

    public int getWidth()
    {
        return width;
    }

    public int getHeight()
    {
        return height;
    }

    public int getRegionCount()
    {
        return regionCount;
    }
    #endregion get/set Methods

}
