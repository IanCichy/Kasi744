using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System;
using System.Web.UI;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;

public class Map
{
    private Tile[,] Tiles;
    private Tile[,] GraphTiles;
    private Region[] Regions;
    private int width, height,
        regionWidth, regionHeight,
        currentRegionInGraph,
        openTiles, regionCount,
        agentCount, stepCounter, maxSteps;
    private bool randomAgents = false;
    //Free Form = 1
    //Constrained-3 = 2
    //Constrained-4 = 3
    private int algorithmNumber = 0;
    private Panel containerBlockView, containerGraphView, graphViewLeft, graphViewRight;
    private Random rnd = new Random();

    /**
	 * Map Constructor - takes care of creating all views for the map
	 * @param int h - the height of the map
	 * @param int w - the width of the map
     * @param int numRegions - the number of regions
	 * @param Point[][] tileRegions - the 2d array (jagged) of points in each region
	 */
    public Map(int h, int w, int numRegions, Point[][] tileRegions, List<Agent> Agents, int algNumber, int maxStep)
    {
        openTiles = 0;
        regionCount = numRegions;
        height = h;
        width = w;
        regionWidth = 0;
        regionHeight = 0;
        currentRegionInGraph = 0;
        stepCounter = 0;
        maxSteps = maxStep;

        algorithmNumber = algNumber;
        Regions = new Region[numRegions];
        Tiles = new Tile[height, width];
        containerBlockView = new Panel();
        containerGraphView = new Panel();
        graphViewLeft = new Panel();
        graphViewRight = new Panel();

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
            agentCount = Agents.Count;
            foreach (Agent A in Agents)
                foreach (Region R in Regions)
                    if (R.getLocations().Contains(A.getLocation()))
                        R.addAgent(A, Tiles[A.getLocation().X, A.getLocation().Y]);

            foreach (Region reg in Regions)
                foreach (Agent agt in reg.getAgents())
                    if (!agt.hasTarget() && algorithmNumber == 1)
                    {
                        Point P = findTargetFreeForm(reg, agt);
                        agt.setTarget(P);
                    }
                    else if (!agt.hasTarget() && algorithmNumber == 2)
                    {
                        Point P = findTargetForC3(reg, agt);
                        agt.setTarget(P);
                    }
                    else if (!agt.hasTarget() && algorithmNumber == 3)
                    {
                        Point P = findTargetForC4(reg, agt);
                        agt.setTarget(P);
                    }
        }

        //Color the blockview to show wall,open,and agents
        colorMap();
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
        foreach (Control c in graphViewLeft.Controls)
        {
            c.Dispose();
        }
        graphViewLeft.Controls.Clear();

        foreach (Control c in graphViewRight.Controls)
        {
            c.Dispose();
        }
        graphViewRight.Controls.Clear();

        //Find the region the location is from
        foreach (Region r in Regions)
            if (r.getLocations().Contains(loc))
                currentRegionInGraph = r.getID();

        //Get region variables
        regionWidth = (Regions[currentRegionInGraph].getWidth() * 2) - 1;
        regionHeight = (Regions[currentRegionInGraph].getHeight() * 2) - 1;
        int Hadj = Regions[currentRegionInGraph].getHeightAdjust();
        int Wadj = Regions[currentRegionInGraph].getWidthAdjust();

        //Get the tiles in the region and make a 'dummy' array to fit the region
        GraphTiles = new Tile[regionHeight, regionWidth];

        //Loop through the tiles and put them into the 'dummy' array
        foreach (Tile t in Regions[currentRegionInGraph].getTiles())
            GraphTiles[(t.getxPos() - Hadj) * 2, (t.getyPos() - Wadj) * 2] = new Tile(t.getType(), t.getxPos(), t.getyPos(), "G", false);

        //Loop through the array and add spacer tiles for walls
        for (int i = 0; i < regionHeight; i++)
        {
            for (int t = 0; t < regionWidth; t++)
            {
                if (i % 2 == 0 && t % 2 == 0)
                {
                    if (GraphTiles[i, t] == null)
                        GraphTiles[i, t] = new Tile(1, 100 + i, 100 + t, "S", true);
                }
                else if (i % 2 != 0 && t % 2 == 0)
                {
                    //VERTICAL LINES
                    if ((GraphTiles[i - 1, t] != null && (GraphTiles[i - 1, t].getType() == 0 || GraphTiles[i - 1, t].getType() == 2)) &&
                        (GraphTiles[i + 1, t] != null && (GraphTiles[i + 1, t].getType() == 0 || GraphTiles[i + 1, t].getType() == 2)))
                        GraphTiles[i, t] = new Tile(4, 100 + i, 100 + t, "L", true);
                    else
                        GraphTiles[i, t] = new Tile(1, 100 + i, 100 + t, "S", true);
                }
                else if (i % 2 == 0 && t % 2 != 0)
                {
                    //HORIZONTAL LINES
                    if ((GraphTiles[i, t - 1] != null && (GraphTiles[i, t - 1].getType() == 0 || GraphTiles[i, t - 1].getType() == 2)) &&
                        (GraphTiles[i, t + 1] != null && (GraphTiles[i, t + 1].getType() == 0 || GraphTiles[i, t + 1].getType() == 2)))
                        GraphTiles[i, t] = new Tile(5, 100 + i, 100 + t, "L", true);
                    else
                        GraphTiles[i, t] = new Tile(1, 100 + i, 100 + t, "S", true);
                }
                else
                {
                    //EMPTY SPACES
                    GraphTiles[i, t] = new Tile(1, 100 + i, 100 + t, "S", true);
                }
            }
        }

        Table tbl = new Table();
        tbl.CssClass = "tableClear";

        //Loop through the final 'dummy' array and add the button to the screen to form the graph view
        for (int i = 0; i < regionHeight; i++)
        {
            TableRow r = new TableRow();

            for (int j = 0; j < regionWidth; j++)
            {
                Tile t = GraphTiles[i, j];

                if (t.getType() == 1)
                {
                    t.getDisplayLnkBtn().CssClass = "graphViewSpacer";
                    TableCell c = new TableCell();
                    c.Controls.Add(t.getDisplayLnkBtn());
                    r.Cells.Add(c);
                }
                else if (t.getType() == 2)
                {
                    t.getLocLabel().Text = "(" + t.getxPos() + ", " + t.getyPos() + ")";
                    t.getLocLabel().CssClass = "graphViewLabel";

                    t.getAgentLabel().CssClass = "graphViewAgentNumber";
                    var txt = Tiles[t.getxPos(), t.getyPos()].getDisplay().Text;
                    if ((txt).Equals(""))
                        t.getAgentLabel().Text = "0";
                    else
                        t.getAgentLabel().Text = txt;

                    t.getDisplayPnl().CssClass = "graphViewNodeVisited";
                    TableCell c = new TableCell();

                    t.getDisplayPnl().Controls.Add(t.getLocLabel());
                    t.getDisplayPnl().Controls.Add(t.getAgentLabel());
                    c.Controls.Add(t.getDisplayPnl());
                    r.Cells.Add(c);
                }
                else if (t.getType() == 0)
                {
                    t.getLocLabel().Text = "(" + t.getxPos() + ", " + t.getyPos() + ")";
                    t.getLocLabel().CssClass = "graphViewLabel";

                    t.getAgentLabel().CssClass = "graphViewAgentNumber";
                    var txt = Tiles[t.getxPos(), t.getyPos()].getDisplay().Text;
                    if ((txt).Equals(""))
                        t.getAgentLabel().Text = "0";
                    else
                        t.getAgentLabel().Text = txt;

                    t.getDisplayPnl().CssClass = "graphViewNode";
                    TableCell c = new TableCell();
                    t.getDisplayPnl().Controls.Add(t.getLocLabel());
                    t.getDisplayPnl().Controls.Add(t.getAgentLabel());
                    c.Controls.Add(t.getDisplayPnl());

                    r.Cells.Add(c);
                }
                else if (t.getType() == 4)
                {
                    t.getDisplayLnkBtn().CssClass = "graphViewVerticalConnector";
                    TableCell c = new TableCell();
                    c.Controls.Add(t.getDisplayLnkBtn());
                    r.Cells.Add(c);
                }
                else if (t.getType() == 5)
                {
                    t.getDisplayLnkBtn().CssClass = "graphViewHorizontalConnector";
                    TableCell c = new TableCell();
                    c.Controls.Add(t.getDisplayLnkBtn());
                    r.Cells.Add(c);
                }
            }
            tbl.Rows.Add(r);
        }
        graphViewLeft.Controls.Add(tbl);


        //RIGHT HALF
        Label lbltrglst = new Label();
        lbltrglst.Text = "Target List";
        graphViewRight.Controls.Add(lbltrglst);
        TextBox txttrglst = new TextBox();
        List<Point> trglst = Regions[currentRegionInGraph].getTargetList();
        string trg = "";
        foreach (Point p in trglst)
        {
            trg += "(" + p.X + "," + p.Y + ")";
        }
        txttrglst.Text = trg;
        txttrglst.TextMode = TextBoxMode.MultiLine;
        txttrglst.Width = 250;
        txttrglst.Height = 100;
        txttrglst.CssClass = "trgLst";
        txttrglst.Enabled = false;
        graphViewRight.Controls.Add(txttrglst);
        foreach (Agent A in Regions[currentRegionInGraph].getAgents())
        {
            TextBox txtb = new TextBox();
            txtb.Text += "ID: " + A.getID();
            txtb.Text += " - Pos: " + "(" + A.getLocation().X + "," + A.getLocation().Y + ")";
            txtb.Text += " - Tgt: " + "(" + A.getTarget().X + "," + A.getTarget().Y + ")";
            txtb.Width = 250;
            txtb.Enabled = false;
            graphViewRight.Controls.Add(txtb);

        }



        // ADD LEFT AND RIGHT TO CONTAINER
        graphViewLeft.CssClass = "graphViewLeft";
        graphViewRight.CssClass = "graphViewRight";
        containerGraphView.Controls.Add(graphViewLeft);
        containerGraphView.Controls.Add(graphViewRight);
    }

    /**
    *  Update the graph view when we take a step in the algorithm
    */
    public void updateGraphView()
    {
        //Loop through the final 'dummy' array and add the button to the screen to form the graph view
        foreach (Tile t in GraphTiles)
        {
            if (t.getType() != 1 && t.getType() != 4 && t.getType() != 5)
            {
                t.setType(Tiles[t.getxPos(), t.getyPos()].getType());
                if (t.getType() == 2)
                {
                    var txt = Tiles[t.getxPos(), t.getyPos()].getDisplay().Text;
                    if (txt.Equals(""))
                        t.getAgentLabel().Text = "0";
                    else
                        t.getAgentLabel().Text = txt;
                    t.getDisplayPnl().CssClass = "graphViewNodeVisited";
                }
                else
                {
                    var txt = Tiles[t.getxPos(), t.getyPos()].getDisplay().Text;
                    if (txt.Equals(""))
                        t.getAgentLabel().Text = "0";
                    else
                        t.getAgentLabel().Text = txt;
                    t.getDisplayPnl().CssClass = "graphViewNode";
                }
            }
        }


        foreach (Control c in graphViewRight.Controls)
        {
            c.Dispose();
        }
        graphViewRight.Controls.Clear();

        //RIGHT HALF
        Label lbltrglst = new Label();
        lbltrglst.Text = "Target List:";
        graphViewRight.Controls.Add(lbltrglst);
        TextBox txttrglst = new TextBox();
        List<Point> trglst = Regions[currentRegionInGraph].getTargetList();
        string trg = "";
        foreach (Point p in trglst)
        {
            trg += "(" + p.X + "," + p.Y + ")";
        }
        txttrglst.Text = trg;
        txttrglst.TextMode = TextBoxMode.MultiLine;
        txttrglst.Width = 250;
        txttrglst.Height = 100;
        txttrglst.CssClass = "trgLst";
        txttrglst.Enabled = false;
        graphViewRight.Controls.Add(txttrglst);
        foreach (Agent A in Regions[currentRegionInGraph].getAgents())
        {
            TextBox txtb = new TextBox();
            txtb.Text += "ID: " + A.getID();
            txtb.Text += " - Pos: " + "(" + A.getLocation().X + "," + A.getLocation().Y + ")";
            txtb.Text += " - Tgt: " + "(" + A.getTarget().X + "," + A.getTarget().Y + ")";
            txtb.Width = 250;
            txtb.Enabled = false;
            graphViewRight.Controls.Add(txtb);

        }
    }

    /**
     * Computes one step of the algorithm
    //Free Form = 1
    //Constrained-3 = 2
    //Constrained-4 = 3
     */
    public void oneStepAlgorithm()
    {
        if (algorithmNumber == 1)
            oneStepAlgorithmFreeForm();
        else if (algorithmNumber == 2)
            oneStepAlgorithmForC3();
        else if (algorithmNumber == 3)
            oneStepAlgorithmForC4();
    }

    #region FreeForm
    /*
     * 
     * Free Form
     * 
     */
    private void oneStepAlgorithmFreeForm()
    {
        foreach (Region R in Regions)
            R.updateNStepTargetList(stepCounter);

        if (!isAlgorithmDone())
        {
            stepCounter++;
            foreach (Region R in Regions)
            {
                if (!R.isRegionComplete())
                {
                    foreach (Agent A in R.getAgents())
                    {
                        if (A.hasTarget())
                            R.setAgentLocation(A, R.findShortestPathMove(A));
                    }
                    foreach (Agent A in R.getAgents())
                    {
                        if (A.getTarget().Equals(A.getLocation()))
                            A.setHasTarget(false);

                        if (!A.hasTarget())
                        {
                            Point P = findTargetFreeForm(R, A);
                            if (P.X != -1)
                                A.setTarget(P);
                        }
                    }

                }
            }
        }
        //We color the map when we are done moving agents around
        colorMap();
    }

    private Point findTargetFreeForm(Region r, Agent a)
    {
        Point target = new Point(-1, -1);
        double max = 0;
        foreach (Point p in r.getTargetList())
        {
            double length = Math.Abs(Math.Pow((a.getLocation().X - p.X), 2) + Math.Pow((a.getLocation().Y - p.Y), 2));
            if (length > max)
            {
                max = length;
                target = p;
            }
        }
        r.removeTargetFromList(target);
        return target;
    }
    #endregion FreeForm

    #region Constrained3
    /*
     * 
     * Constrained-3
     * 
     */
    private void oneStepAlgorithmForC3()
    {
        foreach (Region R in Regions)
            R.updateNStepTargetList(stepCounter);

        if (!isAlgorithmDone())
        {
            stepCounter++;
            foreach (Region R in Regions)
            {
                if (!R.isRegionComplete())
                {
                    foreach (Agent A in R.getAgents())
                    {
                        if (A.hasTarget())
                            R.setAgentLocation(A, R.findShortestPathMove(A));
                    }
                    foreach (Agent A in R.getAgents())
                    {
                        if (A.getTarget().Equals(A.getLocation()))
                            A.setHasTarget(false);

                        if (!A.hasTarget())
                        {
                            Point P = findTargetForC3(R, A);
                            if (P.X != -1)
                                A.setTarget(P);
                        }
                    }
                }
            }
        }
        //We color the map when we are done moving agents around
        colorMap();
    }

    //This part is changed into the Depth-First Traversal code
    //find the furest and in-targetListed tile as target
    public Point findTargetForC3(Region r, Agent a)
    {
        Point start = a.getLocation();
        List<Point> targetPts = new List<Point>();
        foreach (Point p in r.getTargetList())
            targetPts.Add(new Point(p.X, p.Y));

        List<Point> allPts = new List<Point>();
        foreach (Point p in r.getLocations())
            allPts.Add(new Point(p.X, p.Y));

        var previous = new Dictionary<Point, Point>();
        var queue = new Queue<Point>();

        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            Point pt = queue.Dequeue();
            foreach (var neighbor in getAdjacentPoints(pt, allPts))
            {
                if (previous.ContainsKey(neighbor))
                    continue;
                previous[neighbor] = pt;
                queue.Enqueue(neighbor);
            }
        }
        //Now we have a dictionary of all the connections available to us
        //We will use this to find the longest path to a viable target
        List<Point> longestPath = new List<Point>();

        foreach (Point p in targetPts)
        {
            var path = new List<Point>();
            var current = p;
            while (!current.Equals(start))
            {
                path.Add(current);
                current = previous[current];
            };

            if (path.Count > longestPath.Count)
                longestPath = path;
        }

        Point target = new Point(-1, -1);
        if (longestPath != null && longestPath.Count > 0)
            target = longestPath[0];

        return target;
    }

    public List<Point> getAdjacentPoints(Point p1, List<Point> pts)
    {
        var adj = new List<Point>();
        foreach (Point p2 in pts)
            if ((p1.X == p2.X && p1.Y == p2.Y + 1) || (p1.X == p2.X && p1.Y == p2.Y - 1) ||
                (p1.X == p2.X + 1 && p1.Y == p2.Y) || (p1.X == p2.X - 1 && p1.Y == p2.Y))
                adj.Add(p2);
        return adj;
    }

    #endregion Constrained3

    #region Constrained4
    /*
     * 
     * Constrained-4
     * 
     */
    private void oneStepAlgorithmForC4()
    {
        foreach (Region R in Regions)
            R.updateNStepTargetList(stepCounter);

        if (!isAlgorithmDone())
        {
            stepCounter++;
            foreach (Region R in Regions)
            {
                if (!R.isRegionComplete())
                {
                    foreach (Agent A in R.getAgents())
                    {
                        if (A.hasTarget())
                            R.setAgentLocation(A, R.findShortestPathMove(A));
                    }

                    foreach (Agent A in R.getAgents())
                    {
                        if (A.getTarget().Equals(A.getLocation()))
                            A.setHasTarget(false);

                        if (!A.hasTarget())
                        {
                            Point P = findTargetForC4(R, A);
                            if (P.X != -1)
                                A.setTarget(P);
                        }
                    }


                }
            }
        }
        //We color the map when we are done moving agents around
        colorMap();
    }

    public Point findTargetForC4(Region r, Agent a)
    {
        Point target = new Point(-1, -1);
        if (r.getTargetList().Count > 0)
            target = r.getTargetList()[0];
        return target;
    }
    #endregion Constrained4


    /**
     * Runs the oneStepAlgorithm method n times
     */
    public void nStepAlgorithm(int n)
    {
        for (int x = 0; x < n; x++)
            oneStepAlgorithm();
    }

    /**
    * Adjusts the size of the block view depending on the page width and height
    * @param int w - the width of the browser
    * @param int h - the height of the browser
    */
    public void setBlockViewMapSize(int w, int h)
    {
        int adjW = (int)(((w * .95) * .725) / width);
        int adjH = (int)(((h * .95) * .85) / height);
        // int size = Math.Max(Math.Min(adjW, adjH), 65);
        int size = Math.Min(adjW, adjH);
        foreach (Tile t in Tiles)
            t.setSize(size, size);
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
        //int adjW = (int)(((w * .95) * .75) / width);
        //int adjH = (int)(((h * .95) * .85) / height);
        // int size = Math.Min(adjW, adjH);
        int size = 32;
        foreach (Tile t in GraphTiles)
        {
            if (t.getType() != 1 && t.getType() != 4 && t.getType() != 5)
                t.setSize(size, size);
            else if (t.getType() == 4)
            {
                t.setSize(2, size - 15);
            }
            else if (t.getType() == 5)
            {
                t.setSize(size - 15, 2);
            }
        }
        graphViewLeft.Width = (size * regionWidth);
        graphViewLeft.Height = (size * regionHeight);
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
                info += "Agent ID: " + a.getID() + "<br/>Target: " + a.getTarget().ToString() + "<br/>Steps: ";
                int x = 0;
                foreach (Point pt in a.getVisitedList())
                {
                    if (x == 0)
                    {
                        info += "(" + pt.X + "," + pt.Y + ")";
                        x++;
                    }
                    else
                        info += " -> (" + pt.X + "," + pt.Y + ")";
                }
                info += "<br/>";
            }
        }
        return info;
    }

    public bool isAlgorithmDone()
    {
        foreach (Region R in Regions)
            if (!R.isRegionComplete())
                return false;
        return true;
    }

    public bool saveResults(string fName)
    {
        try
        {
            //Variables to save
            string fname = fName;
            DateTime ts = DateTime.Now;
            string MpSze = height + "x" + width;
            string numReg = regionCount.ToString();
            string numAgnts = agentCount.ToString();
            string numStps = stepCounter.ToString();
            string numStpsLmt = maxSteps.ToString();
            //Free Form = 1
            //Constrained-3 = 2
            //Constrained-4 = 3
            string algUsed = "";
            if (algorithmNumber == 1)
                algUsed = "Free Form";
            else if (algorithmNumber == 2)
                algUsed = "Constrained-3";
            else
                algUsed = "Constrained-4";
            //Variables to save

            string information = "";
            int x = 0;
            information += "Run Name: " + fName + "\n";
            information += "Map Size: " + MpSze + "\n";
            information += "Total Steps: " + stepCounter + "\n";
            information += "Step Limit: " + numStpsLmt + "\n";
            information += "Time Stamp: " + ts + "\n";

            foreach (Region R in Regions)
            {
                x = 0;
                information += "Region " + R.getID() + ": ";
                foreach (Point pt in R.getLocations())
                {
                    if (x == 0)
                    {
                        information += "(" + pt.X + "," + pt.Y + ")";
                        x++;
                    }
                    else
                        information += ", (" + pt.X + "," + pt.Y + ")";
                }

                information += "\n -- Target List -- ";
                List<int> numbering = R.getNStepTargetListCount();
                int count = 0;
                foreach (List<Point> LP in R.getNStepTargetList())
                {
                    x = 0;
                    information += "\n";
                    information += "step: " + numbering[count] + " : ";
                    count++;
                    foreach (Point lpt in LP)
                    {
                        if (x == 0)
                        {
                            information += "(" + lpt.X + "," + lpt.Y + ")";
                            x++;
                        }
                        else
                            information += ", (" + lpt.X + "," + lpt.Y + ")";
                    }
                }

                information += "\n -- Agents -- ";
                foreach (Agent A in R.getAgents())
                {
                    x = 0;
                    information += "\nAgent " + A.getID() + ": ";
                    foreach (Point pt in A.getVisitedList())
                    {
                        if (x == 0)
                        {
                            information += "(" + pt.X + "," + pt.Y + ")";
                            x++;
                        }
                        else
                            information += " -> (" + pt.X + "," + pt.Y + ")";
                    }
                }
                information += "\n - - - - - - - - - - - - - - \n\n";
            }

            // Create an insert command
            var command = new SqlCommand(
                "INSERT INTO RunData VALUES (@fileName, @fileData, @timeStamp, @mapSize, @regions, @agents, @stepsTaken, @stepLimit, @algorithm)")
            {
                CommandType = CommandType.Text,
                Connection = Connections.Connection()
            };
            command.Parameters.Add(new SqlParameter("fileName", fname));
            command.Parameters.Add(new SqlParameter("fileData", information));
            command.Parameters.Add(new SqlParameter("timeStamp", ts));
            command.Parameters.Add(new SqlParameter("mapSize", MpSze));
            command.Parameters.Add(new SqlParameter("regions", numReg));
            command.Parameters.Add(new SqlParameter("agents", numAgnts));
            command.Parameters.Add(new SqlParameter("stepsTaken", numStps));
            command.Parameters.Add(new SqlParameter("stepLimit", numStpsLmt));
            command.Parameters.Add(new SqlParameter("algorithm", algUsed));

            // Execute command
            command.Connection.Open();
            command.ExecuteNonQuery();
            command.Connection.Close();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
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
