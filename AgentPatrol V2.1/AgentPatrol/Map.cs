using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System;
using System.Web.UI;

public class Map
{
    private Tile[,] Tiles;
    private Tile[,] GraphTiles;
    private Region[] Regions;
    private int width, height,
        regionWidth, regionHeight,
        currentRegionInGraph,
        openTiles, regionCount,
        agentCount, stepCounter;
    private bool randomAgents = false;
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
        regionWidth = 0;
        regionHeight = 0;
        currentRegionInGraph = 0;
        stepCounter = 0;

        Regions = new Region[numRegions];
        Tiles = new Tile[height, width];
        containerBlockView = new Panel();
        containerGraphView = new Panel();

        //Loop through each dimension of the map and create wall tiles for every space
        for (int i = 0; i < height; i++)
        {
            for (int t = 0; t < width; t++)
            {
                //Create each tile as a wall to start type=1
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
                //Change the type to be an open space type=0
                Tiles[p.X, p.Y].setType(0);
                r.addTileToRegion(Tiles[p.X, p.Y]);
            }
            Regions[m] = r;
        }

        //Randomly select an amount of agents to create and randomly set locations
        if (randomAgents)
        {
            agentCount = rnd.Next(regionCount, openTiles / 2);
            makeRandomAgents();
        }
        else
        {
            //Create Agents from the file
        }

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
        GraphTiles = null;
        foreach (Control c in containerGraphView.Controls)
        {
            c.Dispose();
        }
        containerGraphView.Controls.Clear();

        //Find the region the location is from
        foreach (Region r in Regions)
            if (r.getLocations().Contains(loc))
                currentRegionInGraph = r.getID();

        //Get region variables
        regionWidth = Regions[currentRegionInGraph].getWidth();
        regionHeight = Regions[currentRegionInGraph].getHeight();
        int Hadj = Regions[currentRegionInGraph].getHeightAdjust();
        int Wadj = Regions[currentRegionInGraph].getWidthAdjust();

        //Get the tiles in the region and make a 'dummy' array to fit the region
        GraphTiles = new Tile[regionHeight, regionWidth];

        //Loop through the tiles and put them into the 'dummy' array
        foreach (Tile t in Regions[currentRegionInGraph].getTiles())
            GraphTiles[t.getxPos() - Hadj, t.getyPos() - Wadj] = new Tile(t.getType(), t.getxPos(), t.getyPos(), "G"); //t.Clone();

        //Loop through the array and add spacer tiles for walls
        for (int i = 0; i < regionHeight; i++)
            for (int t = 0; t < regionWidth; t++)
                if (GraphTiles[i, t] == null)
                    GraphTiles[i, t] = new Tile(1, 100 + i, 100 + t, "G");

        //Loop through the final 'dummy' array and add the button to the screen to form the graph view
        foreach (Tile t in GraphTiles)
        {
            if (t.getType() == 1)
            {
                t.getDisplay().Text = "Wall\nWall";
                t.getDisplay().CssClass = "graphViewSpacer";
                containerGraphView.Controls.Add(t.getDisplay());
            }
            else if (t.getType() == 2)
            {
                t.getDisplay().Text = t.getxPos() + ", " + t.getyPos() + "\n Agents: " + Tiles[t.getxPos(), t.getyPos()].getDisplay().Text;
                t.getDisplay().CssClass = "graphViewNodeVisited";
                containerGraphView.Controls.Add(t.getDisplay());
            }
            else
            {
                t.getDisplay().Text = t.getxPos() + ", " + t.getyPos() + "\n Agents: " + Tiles[t.getxPos(), t.getyPos()].getDisplay().Text;
                t.getDisplay().CssClass = "graphViewNode";
                containerGraphView.Controls.Add(t.getDisplay());
            }
        }
    }

    /**
    *  Update the graph view when we take a step in the algorithm
    */
    public void updateGraphView()
    {
        //Loop through the final 'dummy' array and add the button to the screen to form the graph view
        foreach (Tile t in GraphTiles)
        {
            if (t.getType() != 1)
            {
                t.setType(Tiles[t.getxPos(), t.getyPos()].getType());
                if (t.getType() == 2)
                {
                    t.getDisplay().Text = t.getxPos() + ", " + t.getyPos() + "\n Agents: " + Tiles[t.getxPos(), t.getyPos()].getDisplay().Text;
                    t.getDisplay().CssClass = "graphViewNodeVisited";
                }
                else if (t.getType() == 0)
                {
                    t.getDisplay().Text = t.getxPos() + ", " + t.getyPos() + "\n Agents: " + Tiles[t.getxPos(), t.getyPos()].getDisplay().Text;
                    t.getDisplay().CssClass = "graphViewNode";
                }
            }
        }
    }

    /**
	 * Creates the agents by randomly placing them into available regions
	 */
    private void makeRandomAgents()
    {
        //Counter for how many agents we have made
        int currentAgents = 0;
        //loop through each region and add ONE agent so there is a minimun of one in each
        foreach (Region r in Regions)
        {
            Agent a = new Agent(currentAgents);
            currentAgents++;
            r.addRandomAgent(a);
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
                Regions[t].addRandomAgent(a);
                currentAgents++;
            }
        }
    }

    /**
     * Computes one step of the algorithm
     */
    public void oneStepAlgorithm()
    {
        if (!isSearchDone())
        {
            stepCounter++;
            foreach (Region r in Regions)
            {
                if (!r.isRegionComplete())
                {
                    foreach (Agent a in r.getAgents())
                    {
                        Tile t = r.getBestNeighboringTiles(a);

                        //set the agents position
                        //this method also updates the tiles and 
                        //the target lists
                        r.setAgentLocation(a,t);

                        //TEMP CODE
                        t.setScore(t.getScore() - 10);
                    }
                }
            }
        }
        //We color the map when we are done moving agents around
        colorMap();
    }

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
        int adjW = (int)((w * .65) / width);
        int adjH = (int)((h * .85) / height);
        int size = Math.Min(Math.Min(adjW, adjH), 65);
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
        int adjW = (int)((w * .65) / regionWidth);
        int adjH = (int)((h * .85) / regionHeight);
        int size = Math.Min(Math.Min(adjW, adjH), 85);
        foreach (Tile t in GraphTiles)
        {
            t.setSize(size, size);
        }
        containerGraphView.Width = size * regionWidth;
        containerGraphView.Height = size * regionHeight;
    }

    /**
    * gathers the correct information about an agent for display
    * @param Point p - the location of the agent
    */
    public string getAgentInfo(Point p)
    {
        string info = "<br/>";
        foreach (Agent a in Regions[currentRegionInGraph].getAgents())
        {
            if (a.getLocation().Equals(p))
            {
                info += "Agent ID: " + a.getID() + "<br/>Steps: ";
                foreach (Point pt in a.getVisitedList())
                {
                    info += "(" + pt.X + "," + pt.Y + ")" + " -> ";
                }
                info.TrimEnd(new char[] { '-', '>' });
                info += "<br/>";
            }
        }
        return info;
    }

    public bool isSearchDone()
    {
        foreach (Region R in Regions)
            if (!R.isRegionComplete())
                return false;
        return true;
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

    public Tile[,] getGraphTiles()
    {
        return GraphTiles;
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

    public int getStepCount()
    {
        return stepCounter;
    }

    #endregion get/set Methods

}
