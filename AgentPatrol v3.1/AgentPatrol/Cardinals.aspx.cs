using AgentPatrol;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Collections;

public partial class Cardinals : System.Web.UI.Page
{
    Map WorldView;

    static int Height;
    static int Width;
    static int Region;
    static Point[][] regions;
    static List<Agent> agents = new List<Agent>();
    protected void Page_Load(object sender, EventArgs e)
    {
        //Point[] pts = new Point[8];
        //pts[0] = new Point(0, 1);
        //pts[1] = new Point(0, 4);
        //pts[2] = new Point(0, 2);
        //pts[3] = new Point(0, 3);
        //pts[4] = new Point(1, 3);
        //pts[5] = new Point(2, 3);
        //pts[6] = new Point(0, 5);
        //pts[7] = new Point(7, 2);
        //string s = checkConnectivity(pts);

        if (!IsPostBack)
        {
            //Remove the back button
            btnViewBlock.CssClass = "hidden";
            disallowSave();

            Session["isBlock"] = false;
            Session["isGraph"] = false;
            ClientScript.RegisterStartupScript(GetType(), "Javascript", "javascript:getPageSize();", true);
        }
        else
        {
            //We are currently viewing the Block view
            //Update the block view controls so it is clickable
            if ((bool)Session["isBlock"])
            {
                WorldView = (Map)Session["Map"];
                foreach (Tile tle in WorldView.getTiles())
                {
                    if (tle.getType() != 1)
                    {
                        tle.getDisplay().Click += new EventHandler(Block_button_Click);
                        ScriptManager.GetCurrent(Page).RegisterPostBackControl(tle.getDisplay());
                    }
                }
                updateBlock();
            }
            //We are currently viewing the graph view for a regions
            //update the nodes so they are clickable
            else if ((bool)Session["isGraph"])
            {
                WorldView = (Map)Session["Map"];
                foreach (Tile tle in WorldView.getGraphTiles())
                {
                    if (tle.getType() != 1)
                    {
                        tle.getDisplay().Click += new EventHandler(Graph_button_Click);
                        ScriptManager.GetCurrent(Page).RegisterPostBackControl(tle.getDisplay());
                    }
                }
                updateGraph();
            }
            //No view is currently visisble
            //Disable buttons
            else
            {
                disableControls();
            }
        }
    }

    /*
     * updateBlock -
     *   Checks to see if the algorithm is finished
     *   places the block view on the screen
     */
    protected void updateBlock()
    {
        //Find the correct place to put the world on the webpage and add it to the controls
        viewContainer.Controls.Clear();
        viewContainer.Controls.Add(((Map)Session["Map"]).getBlockViewDisplay());
        WorldView.setBlockViewMapSize(int.Parse(PageWidth.Value), int.Parse(PageHeight.Value));
        //Remove the back button
        btnViewBlock.CssClass = "hidden";
        UDPViews.Update();
    }

    /*
     * updateGraph -
     *   Checks to see if the algorithm is finished
     *   places the graph view on the screen
     */
    protected void updateGraph()
    {
        //Find the correct place to put the world on the webpage and add it to the controls
        viewContainer.Controls.Clear();
        viewContainer.Controls.Add(((Map)Session["Map"]).getGraphViewDisplay());
        WorldView.setGraphViewMapSize(int.Parse(PageWidth.Value), int.Parse(PageHeight.Value));
        //add the back button
        btnViewBlock.CssClass = "button";
        UDPViews.Update();
    }

    /*
     * Will run a single step of the algorithm and update 
     */
    protected void btnOneStep_Click(object sender, EventArgs e)
    {

        if (!WorldView.isSearchDone())
        {
            WorldView.oneStepAlgorithm();
            //if we have any view open
            if ((bool)Session["isBlock"] || (bool)Session["isGraph"])
                lblStepCount.Text = "Steps: " + WorldView.getStepCount();//Update the step counter

            if ((bool)Session["isGraph"])
            {
                WorldView.updateGraphView();
            }
        }

        if (WorldView.isSearchDone())
        {
            disableControls();
            allowSave();

        }
    }
    /*
     * Will run 'n' steps of the algorithm
     */
    protected void btnNStep_Click(object sender, EventArgs e)
    {

        if(!WorldView.isSearchDone())
        {
            int steps = 0;
            if (int.TryParse(txtStepCount.Text, out steps) && steps >= 1 && steps <= 500)
            {
                WorldView.nStepAlgorithm(steps);
            }
            else
            {
                txtStepCount.Text = "";
            }
            //if we have any view open
            if ((bool)Session["isBlock"] || (bool)Session["isGraph"])
                lblStepCount.Text = "Steps: " + WorldView.getStepCount();//Update the step counter

            if ((bool)Session["isGraph"])
            {
                WorldView.updateGraphView();
            }
        }

        if (WorldView.isSearchDone())
        {
            disableControls();
            allowSave();
        }

    }

    protected void btnSaveFile_Click(object sender, EventArgs e)
    {
        string fileName = txtFileName.Text;
        bool saveWorked = WorldView.saveResults(fileName);
        if (saveWorked)
        {
            disallowSave();
            Response.Redirect(Request.RawUrl);
        }
        else
        {
            Response.Redirect(Request.RawUrl);
        }
    }

    protected void allowSave()
    {
        txtFileName.CssClass = "txtEnterText";
        btnSaveFile.CssClass = "button-Upload";
    }

    protected void disallowSave()
    {
        txtFileName.CssClass = "hidden";
        btnSaveFile.CssClass = "hidden";
    }

    /*
     * Allows the user to return from a graph view to the block view
     */
    protected void btnViewBlock_Click(object sender, EventArgs e)
    {
        Session["isGraph"] = false;
        Session["isBlock"] = true;
        updateBlock();
    }

    /*
     * Helps to reset the controls when changing maps
     */
    private void resetControls()
    {
        viewContainer.Controls.Clear();
        UDPViews.Update();
    }

    /*
    * Block_Button_Click
    * Used to display the graph view of a specific region
    */
    protected void Block_button_Click(object sender, EventArgs e)
    {
        Button button = sender as Button;
        // identify which button was clicked and perform necessary actions
        string[] s = button.ID.Split(',');
        Point p = new Point(int.Parse(s[0]), int.Parse(s[1]));
        WorldView.makeGraphView(p);
        Session["isGraph"] = true;
        Session["isBlock"] = false;
        updateGraph();
    }

    /*
     * Graph_Button_Click
     * Used to display the popup with information about agents in a node
     */
    protected void Graph_button_Click(object sender, EventArgs e)
    {
        Button button = sender as Button;
        // identify which button was clicked and perform necessary actions
        string[] s = button.ID.Split(',');
        Point p = new Point(int.Parse(s[0].Substring(1)), int.Parse(s[1]));
        string info = WorldView.getAgentInfo(p);
        lblInfo.Text = info;
        mpe.Show();
    }

    /*
     * Disables some controls on the page
     */
    private void disableControls()
    {
        btnOneStep.Enabled = false;
        btnNStep.Enabled = false;
        btnOneStep.CssClass = "button-Disabled";
        btnNStep.CssClass = "button-Disabled";
    }

    /*
     * Enables some controls on the page
     */
    private void enableControls()
    {
        btnOneStep.Enabled = true;
        btnNStep.Enabled = true;
        btnOneStep.CssClass = "button-OneStep";
        btnNStep.CssClass = "button-NSteps";
    }

    protected void btnViewOldRuns_Click(object sender, EventArgs e)
    {
        Response.Redirect("History.aspx");
    }

    #region FileRead
    //----------Yuxin Liu-------------------------------------------------------------------------------
    //Height
    //Width
    //Region
    //coordinates of region 1(separate by space)
    //id and coordinates of agents in region 1. eg.(1 (0,0) 2 (0,1) separate by space
    //....region 2
    //....agent in region 2
    //....
    protected void uploadMap_Click(object sender, EventArgs e)
    {
        disallowSave();
        agents.Clear();//everytime upload a new map file, agent list must be cleared
        Session["isBlock"] = false;
        Session["isGraph"] = false;
        try
        {
            if (file.HasFile)
            {
                if (IsAllowedExtension(file))
                {
                    string path = Server.MapPath("~/MapFile/") + file.FileName;
                    file.SaveAs(path);
                    ArrayList arr = new ArrayList();
                    foreach (string line in File.ReadAllLines(path))
                        arr.Add(line);

                    if (File.Exists(@path))
                        File.Delete(@path);

                    string message = checkFile(arr);
                    if (message.Equals(""))
                    {
                        Response.Write("<script>alert('Upload successfully!')</script>");
                        //This is the code to generate the block view when you upload a map
                        resetControls();
                        enableControls();
                        WorldView = new Map(Height, Width, Region, regions, agents);
                        Session["isBlock"] = true;
                        Session["Map"] = WorldView;
                        lblStepCount.Text = "Steps: ";
                        updateBlock();
                        ClientScript.RegisterStartupScript(GetType(), "Javascript", "javascript:getPageSize();", true);
                        //Code ends here
                    }
                    else
                        Response.Write("<script>alert('" + message + "')</script>");
                }
                else
                    Response.Write("<script>alert('Please upload .txt file！')</script>");
            }
            else
                Response.Write("<script>alert('No file chosen!')</script>");
        }
        catch (Exception error)
        {
            Response.Write("<script>alert(" + error.ToString() + ")</script>");
        }
    }

    //check if the file is a .txt file
    public bool IsAllowedExtension(FileUpload uploadMap)
    {
        bool flag = false;
        string fileExtension = System.IO.Path.GetExtension(uploadMap.FileName).ToLower();
        if (fileExtension == ".txt")
        {
            flag = true;
        }
        return flag;
    }

    //check format of all information in the map file
    public string checkFile(ArrayList arr)
    {
        string message = "";
        if (arr.Count >= 3)//height,width,region
        {
            try
            {
                Height = Convert.ToInt16(arr[1]);
                Width = Convert.ToInt16(arr[0]);
                Region = Convert.ToInt16(arr[2]);
                regions = new Point[Region][];
                if (Height < 8 || Height > 15 || Width < 8 || Width > 15)
                    return "Size of environment：Minimum 8 X 8, Maximum 15 X 15！";
            }
            catch
            {
                return "Invalid input of height, width and the number of regions!";
            }
        }
        else
            return "Input file must include height, width, number of regions, coordinates and agents of each regions!";

        if (Region == (arr.Count - 3) / 2 && (arr.Count - 3) % 2 == 0)
        {
            message = regionsAndAgents(arr);
            if (!message.Equals(""))
                return message;
        }
        else
            return "Input file must include height, width, number of regions, coordinates and agents of each regions!";

        return message;
    }

    //interpreter information of each region and agent 
    public string regionsAndAgents(ArrayList arr)
    {
        string message = "";
        int regionNum = 0;
        for (int i = 0; i < Region * 2; i++)//for each region
        {
            if (i % 2 == 0)//coordinates of regions
            {
                string[] points = (arr[i + 3].ToString()).Split(' ');
                regionNum = points.Length;
                message = saveRegion(i, points);
                if (!message.Equals(""))
                    return message;
            }
            else // id and coordinate of agents
            {
                string[] agentInfo = (arr[i + 3].ToString()).Split(' ');
                if ((i - 1) >= 0 && (agentInfo.Length / 2 < 1 || agentInfo.Length / 2 > regionNum / 2))
                    return "Invalid number of agents!";
                message = saveAgent(i, agentInfo);
                if (!message.Equals(""))
                    return message;
            }
        }
        message = checkID();
        if (!message.Equals(""))
            return message;
        message = checkRegionsCoordinate();
        if (!message.Equals(""))
            return message;
        return message;
    }

    //save all coordinates of each region
    public string saveRegion(int i, string[] points)
    {
        Point tile = new Point();
        string message = "";
        int num = points.Length;
        regions[i / 2] = new Point[num];
        for (int j = 0; j < points.Length; j++)//for each point of different region
        {
            try
            {
                string nums = points[j].Replace("(", "");
                nums = nums.Replace(")", "");
                string[] numbers = nums.Split(',');
                if (numbers.Length != 2)
                    return "Invalid format of coordinate!";

                tile.X = Convert.ToInt16(numbers[0]);
                tile.Y = Convert.ToInt16(numbers[1]);
                if (tile.Y < 0 || tile.Y >= Width || tile.X < 0 || tile.X >= Height)
                    return "Coordinate overstep the boundary!";
                regions[i / 2][j] = tile;
            }
            catch (Exception error)
            {
                return error.ToString();
            }
        }
        return message;
    }

    //save each Agent object in a list
    public string saveAgent(int i, string[] agentInfo)
    {
        string message = "";
        Point tile = new Point();
        if (agentInfo.Length % 2 != 0)
            return "Invalid format of agents!";

        int id = 0;
        for (int m = 0; m < agentInfo.Length; m++)
        {
            if (m % 2 == 0)//agent id
                try
                {
                    id = Convert.ToInt16(agentInfo[m]);
                }
                catch (Exception e)
                {
                    return e.ToString();
                }
            else//agent coordinate
                try
                {
                    string n = agentInfo[m].Replace("(", "");
                    n = n.Replace(")", "");
                    string[] numbers = n.Split(',');
                    tile.X = Convert.ToInt16(numbers[0]);
                    tile.Y = Convert.ToInt16(numbers[1]);
                    if (checkAgentCoordinate(tile, i))
                    {
                        Agent thisAgent = new Agent(id);
                        thisAgent.setStartLocation(tile);
                        agents.Add(thisAgent);
                    }
                    else
                        return "Invalid coordinate of agent!";
                }
                catch (Exception e)
                {
                    return e.ToString();
                }
        }
        return message;
    }

    //check if the agent is in the region
    public bool checkAgentCoordinate(Point tile, int i)
    {
        bool flag = false;
        Point[] thisRegion = regions[(i - 1) / 2];
        foreach (Point p in thisRegion)
        {
            if (p == tile)
            {
                flag = true;
            }
        }
        return flag;
    }

    //check if the agent ID is unique
    //the ID must be a number and we don't care about the value
    public string checkID()
    {
        string message = "";
        int[] IDs = new int[agents.Count];
        for (int j = 0; j < agents.Count; j++)
        {
            IDs[j] = agents[j].getID();
        }
        Hashtable ht = new Hashtable();
        for (int i = 0; i < IDs.Length; i++)
        {
            if (ht.Contains(IDs[i]))
            {
                message = "Agent ID must be unique!";
                return message;
            }
            else
            {
                ht.Add(IDs[i], IDs[i]);
            }
        }
        return message;
    }

    //1. check if connectivity of each region
    //2. check if all coordinates are unique(different regions cannot have same coordinate)
    public string checkRegionsCoordinate()
    {
        string message = "";
        List<Point> h = new List<Point>();
        Point[] thisRegion;
        for (int i = 0; i < regions.Length; i++)
        {
            thisRegion = regions[i];
            message = checkConnectivity(thisRegion);
            if (!message.Equals(""))
                return message;
            for (int j = 0; j < thisRegion.Length; j++)
                if (h.Contains(regions[i][j]))
                    return "Duplicate coordinates!";
                else
                    h.Add(regions[i][j]);
        }
        return message;
    }

    public string checkConnectivity(Point[] thisRegion)
    {
        List<Point> pointsInRegion = new List<Point>(thisRegion);
        List<Point> visit = new List<Point>();
        string message = "";
        if (thisRegion.Length == 1)
            return message;
        isAdjacentRegion(pointsInRegion, visit, thisRegion[0]);
        if (visit.Count != thisRegion.Length)
            message = "Error";
        return message;
    }

    private void isAdjacentRegion(List<Point> PTS, List<Point> VIST, Point P)
    {
        if (!VIST.Contains(P))
            VIST.Add(P);
        PTS.Remove(P);
        if (PTS.Contains(new Point(P.X, P.Y + 1)))
            isAdjacentRegion(PTS, VIST, new Point(P.X, P.Y + 1));
        if (PTS.Contains(new Point(P.X, P.Y - 1)))
            isAdjacentRegion(PTS, VIST, new Point(P.X, P.Y - 1));
        if (PTS.Contains(new Point(P.X + 1, P.Y)))
            isAdjacentRegion(PTS, VIST, new Point(P.X + 1, P.Y));
        if (PTS.Contains(new Point(P.X - 1, P.Y)))
            isAdjacentRegion(PTS, VIST, new Point(P.X - 1, P.Y));
    }


    #endregion FileRead


}
