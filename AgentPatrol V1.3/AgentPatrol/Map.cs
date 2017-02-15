using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System;

public class Map
{
    private Tile[,] Tiles;
    private Tile[,] GraphTiles;
    private Region[] Regions;
    private int width, height, openTiles, regionCount, agentCount;
    private Panel containerBlockView, containerGraphView;
    private Random rnd = new Random();

    /**
	 * Map Constructor - takes care of creating all views for the map
	 * @param int h - the height of the map
	 * @param int w - the width of the map
     * @param int numRegions - the number of regions
	 * @param Point[][] tileRegions - the 2d array (jagged) of points in each region
	 */
    public Map(int h, int w, int numRegions, Point[][] tileRegions)
    {
        openTiles = 0;
        regionCount = numRegions;
        height = h;
        width = w;

        Regions = new Region[numRegions];
        Tiles = new Tile[height, width];
        containerBlockView = new Panel();
        containerGraphView = new Panel();

        //Loop through each dimension of the map and create wall tiles for every space
        for (int i = 0; i < height; i++)
        {
            for (int t = 0; t < width; t++)
            {
                Tile tle = new Tile(1, i, t);
                Tiles[i, t] = tle;
                containerBlockView.Controls.Add(tle.getDisplay());
            }
        }

        //loop through each region and set each space in that region to be open
        //also, create the Regions[] with the correct region data
        for (int m = 0; m < tileRegions.Length; m++)
        {
            Point[] arrp = tileRegions[m];
            Region r = new Region(m);
            foreach (Point p in arrp)
            {
                openTiles++;
                Tiles[p.X, p.Y].setType(0);
                r.addTileToRegion(Tiles[p.X, p.Y]);
            }
            Regions[m] = r;
        }

        //Randomly select an amount of agents to create and make them
        agentCount = rnd.Next(regionCount, openTiles / 2);
        makeAgents();
        //Color the blockview to show wall,open,and agents
        colorMap();
    }

    /**
    * Will color the blockview to show the different types of tiles
    */
    private void colorMap()
    {
        //Color the Basic Tiles using CssClasses for the colors and set the text to be empty
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

        //Color the Agents Blue and count the number on each space
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

    /**
     *  MAKE THE GRAPH VIEW
     * 
     * 
     */
    public void makeGraphView(Point loc)
    {
        containerGraphView.Controls.Clear();
        //HEIGHT WIDTH
        GraphTiles = new Tile[4, 3];

        int regionID = -1;
        foreach (Region r in Regions)
        {
            if (r.getLocations().Contains(loc))
                regionID = r.getID();
        }

        List<Tile> tls = Regions[regionID].getTiles();

        foreach (Tile t in tls)
        {
            Button b = new Button();
            b.Text = t.getxPos() + ", " + t.getyPos();
            b.CssClass = "graphViewNode";
            containerGraphView.Controls.Add(b);
        }

    }

    /**
	 * Creates the agents by randomly placing them into available regions
	 */
    private void makeAgents()
    {
        //Counter for how many agents we have made
        int currentAgents = 0;
        //loop through each region and add ONE agent so there is a minimun of one in each
        foreach (Region r in Regions)
        {
            Agent a = new Agent(currentAgents);
            currentAgents++;
            r.addAgent(a);
        }

        //While we can still place more agents,
        //we put an agent into a random region ONLY IF
        //that region can accept more agents and has not reached its maximum
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



    /**
     * Computes one step of the algorithm
     */
    //----------------------------------------------NOT DONE YET----------------------------------------
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
        //We color the map when we are done moving agents around
        colorMap();
    }
    //----------------------------------------------NOT DONE YET----------------------------------------


    /**
     * Runs the oneStepAlgorithm method n times
     */
    public void nStepAlgorithm(int n)
    {
        for (int x = 0; x < n; x++)
        {
            oneStepAlgorithm();
        }
    }

    /**
    * Adjusts the size of the block view depending on the page width and height
    * @param int w - the width of the browser
    * @param int h - the height of the browser
    */
    public void setBlockViewMapSize(int w, int h)
    {
        int adjW = (int)((w * .45) / width);
        int adjH = (int)((h * .6) / height);
        int size = Math.Min(adjW, adjH);
        foreach (Tile t in Tiles)
        {
            t.setSize(size, size);
        }
        containerBlockView.Width = size * width;
        containerBlockView.Height = size * height;
    }

    /**
    * Adjusts the size of the graph view depending on the page width and height
    * @param int w - the width of the browser
    * @param int h - the height of the browser
    */
    public void setGraphViewMapSize(int w, int h)
    {

        //NEED CODE
    }

    //SET AND GET METHODS
    #region get/set Methods

    public Map getMap()
    {
        return this;
    }

    public Tile[,] getTiles()
    {
        return Tiles;
    }

    public Panel getBlockViewDisplay()
    {
        return containerBlockView;
    }

    public Panel getGraphViewDisplay()
    {
        return containerGraphView;
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

    public Region[] getRegions()
    {
        return Regions;
    }

    #endregion get/set Methods

}
