using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System;

public class Map
{
    private Tile[,] Tiles;
    //private List<Agent> Agents;
    private Region[] Regions;
    private int width, height, openTiles, regionCount, agentCount;
    private Panel container;
    private Random rnd = new Random();

    /*
     * Inputs: 
     *      int[,] M  - A map file for input
     *      int s - the size of each tile in the view
     */
    public Map(int w, int h, int numRegions, Region[] tileRegions, int[,] M)
    {
        openTiles = 0;
        regionCount = numRegions;
        height = h;
        width = w;

        Regions = tileRegions;
        Tiles = new Tile[height, width];
        container = new Panel();
        container.CssClass = "centerMe";


        for (int i = 0; i < height; i++)
        {
            for (int t = 0; t < width; t++)
            {
                if (M[i, t] == 0)
                    openTiles++;
                Tile tle = new Tile(M[i, t], i, t);
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
            if (t.getType() == 0)
                t.getTile().getDisplay().CssClass = "blockViewOpenSpace";
            else if (t.getType() == 1)
                t.getTile().getDisplay().CssClass = "blockViewClosedSpace";
            else if (t.getType() == 2)
                t.getTile().getDisplay().CssClass = "blockViewVisitedSpace";
            t.getTile().getDisplay().Text = "";
        }

        //Color the Agents Blue
        foreach (Region r in Regions)
        {
            foreach (Agent a in r.getAgents())
            {
                int xl = a.getLocation().X;
                int yl = a.getLocation().Y;
                Tiles[xl, yl].getTile().getDisplay().CssClass = "blockViewAgent";

                if (Tiles[xl, yl].getTile().getDisplay().Text.Equals(""))
                    Tiles[xl, yl].getTile().getDisplay().Text = "1";
                else
                    Tiles[xl, yl].getTile().getDisplay().Text = (int.Parse(Tiles[xl, yl].getTile().getDisplay().Text) + 1) + "";
                Tiles[xl, yl].setType(2);
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


    public void nStepAlgorithm(int n)
    {
        for (int x = 0; x < n; x++)
        {
            oneStepAlgorithm();
        }
    }

    public void setTileType(int x, int y, int type)
    {
        Tiles[x, y].setType(type);
        colorMap();
    }

    public void setMapSize(int w, int h)
    {
        int adjW = (int)((w * .45) / width);
        int adjH = (int)((h * .6) / height);
        int size = Math.Min(adjW, adjH);
        foreach (Tile t in Tiles)
        {
            t.setSize(size, size);
        }
        container.Width = size * width;
        container.Height = size * height;
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
