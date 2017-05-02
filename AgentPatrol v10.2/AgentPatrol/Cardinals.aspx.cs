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
    //Error = 0
    //Free Form = 1
    //Constrained-3 = 2
    //Constrained-4 = 3
    static int Height;
    static int Width;
    static int Region;
    static Point[][] regions;
    static List<Agent> agents = new List<Agent>();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //Set GUI functions to visible or not
            disableNewBack();
            disableSave();
            enableAlgorithmSelect();
            disableUpload();
            disableMaxSteps();
            //end GUI
            Session["isBlock"] = false;
            Session["isGraph"] = false;
            Session["AlgorithmNumber"] = 0;
            Session["Algorithm"] = "";
            Session["MaxSteps"] = 0;
            ClientScript.RegisterStartupScript(GetType(), "Javascript", "javascript:getPageSize();", true);
        }
        else
        {
            //We are currently viewing the Block view
            //Update the block view controls so it is clickable
            if ((bool)Session["isBlock"])
            {
                disableUpload();
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
                disableUpload();
                WorldView = (Map)Session["Map"];
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


    #region Buttons

    /*
     * Will run a single step of the algorithm and update 
     */
    protected void btnOneStep_Click(object sender, EventArgs e)
    {
        int maxStep = int.Parse(Session["MaxSteps"].ToString());
        if (maxStep == 0)
        {
            if (!WorldView.isAlgorithmDone())
            {
                WorldView.oneStepAlgorithm();
                if ((bool)Session["isGraph"])
                    WorldView.updateGraphView();
            }
            if (WorldView.isAlgorithmDone())
            {
                disableControls();
                enableSave();
                lblError.Text = "Algorithm Finished! Please save if you would like.";
                buttonSide.Attributes["class"] = "AlgorithmFinished";

                lblError.CssClass = "successMessage";
            }
            lblStepCount.Text = "Steps: " + WorldView.getStepCount();//Update the step counter
        }
        else
        {
            if (!WorldView.isAlgorithmDone() && WorldView.getStepCount() < maxStep)
            {
                WorldView.oneStepAlgorithm();
                if ((bool)Session["isGraph"])
                    WorldView.updateGraphView();
            }
            if (WorldView.isAlgorithmDone() || WorldView.getStepCount() == maxStep)
            {
                disableControls();
                enableSave();
                lblError.Text = "Algorithm Finished! Please save if you would like.";
                buttonSide.Attributes["class"] = "AlgorithmFinished";

                lblError.CssClass = "successMessage";
            }
            lblStepCount.Text = "Steps: " + WorldView.getStepCount() + " / " + maxStep;//Update the step counter
        }
    }

    /*
     * Will run 'n' steps of the algorithm
     */
    protected void btnNStep_Click(object sender, EventArgs e)
    {
        int steps = 0;
        if (int.TryParse(txtStepCount.Text, out steps) && steps >= 1 && steps <= 500)
            for (int x = 0; x < steps; x++)
                btnOneStep_Click(this, EventArgs.Empty);
    }

    /*
     * Validates the save file name and then passes it tobe saved
     */
    protected void btnSaveFile_Click(object sender, EventArgs e)
    {
        string fileName = txtFileName.Text;

        if (fileName.Equals(""))
        {
            lblError.Text = "No file name entered";
            lblError.CssClass = "errorMessage";
            return;
        }
        if (fileName.Length >= 25)
        {
            lblError.Text = "File name is too long";
            lblError.CssClass = "errorMessage";
            return;
        }

        bool saveWorked = WorldView.saveResults(fileName);
        if (saveWorked)
        {
            lblError.Text = "Saved sucessfully";
            lblError.CssClass = "successMessage";
            disableSave();
        }
        else
        {
            lblError.Text = "Error in saving file";
            lblError.CssClass = "errorMessage";
        }
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
     * User selects the algorithm they want to use,
     * else gets an error message
     */
    protected void btnAlgorithmSelect_Click(object sender, EventArgs e)
    {
        if (int.Parse(ddlAlgorithmSelect.SelectedValue) != 0)
        {
            Session["Algorithm"] = ddlAlgorithmSelect.SelectedItem;
            Session["AlgorithmNumber"] = int.Parse(ddlAlgorithmSelect.SelectedValue);
            enableMaxSteps();
            disableAlgorithmSelect();
            lblAlgorithmSelected.Text = "Algorithm: " + Session["Algorithm"].ToString();
            lblInputError.Text = "";
        }
        else
        {
            lblInputError.Text = "No algorithm selected";
            lblInputError.CssClass = "errorMessage";
        }
    }

    /*
     * Decides if there is a maximum number of steps or not
     */
    protected void btnMaxSteps_Click(object sender, EventArgs e)
    {
        int maxSteps;

        bool result = int.TryParse(txtMaxSteps.Text, out maxSteps);
        if (result)
        {
            if (maxSteps > 0)
            {
                Session["MaxSteps"] = maxSteps;
                enableUpload();
                disableMaxSteps();
                lblInputError.Text = "";
            }
            else
            {
                lblInputError.Text = "Value must be greater than 0";
                lblInputError.CssClass = "errorMessage";
            }
        }
        else
        {
            if (txtMaxSteps.Text.Equals(""))
            {
                Session["MaxSteps"] = 0;
                enableUpload();
                disableMaxSteps();
                lblInputError.Text = "";
            }
            else
            {
                lblInputError.Text = "Invalid value";
                lblInputError.CssClass = "errorMessage";
            }
        }
    }

    /*
     * Redirects the user to the history page
     */
    protected void btnViewOldRuns_Click(object sender, EventArgs e)
    {
        Response.Redirect("History.aspx");
    }

    /*
     * Starts a new run by refreshing the page and all controls
     */
    protected void btnNewRun_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.RawUrl);
    }

    #endregion Buttons

    #region Enable&DisableButtons

    /*
     * Helps to reset the controls when changing maps
     */
    private void resetControls()
    {
        viewContainer.Controls.Clear();
        UDPViews.Update();
    }

    /*
     * Enables the new run button 
     */
    protected void enableNewBack()
    {
        btnNewRun.CssClass = "button-negative";
    }

    /*
     * disables the new run and back to block view button 
     */
    protected void disableNewBack()
    {
        btnNewRun.CssClass = "hidden";
        btnViewBlock.CssClass = "hidden";
    }

    /*
     * Enables the buttons to allow file upload
     */
    protected void enableUpload()
    {
        file.CssClass = "visible";
        btnUploadMap.CssClass = "button-positive";
        lblMapSelect.CssClass = "visible";
    }

    /*
     * Disables the buttons to not allow file upload
     */
    protected void disableUpload()
    {
        lblMapSelect.CssClass = "hidden";
        file.CssClass = "hidden";
        btnUploadMap.CssClass = "hidden";
    }

    /*
     * Enables the buttons to allow saving of the file
     */
    protected void enableSave()
    {
        txtFileName.CssClass = "txtSaveFile";
        btnSaveFile.CssClass = "button-positive";
    }

    /*
     * Disables the buttons to not allow saving
     */
    protected void disableSave()
    {
        txtFileName.CssClass = "hidden";
        btnSaveFile.CssClass = "hidden";
    }

    /*
     * Enables selection of maximum steps
     */
    protected void enableMaxSteps()
    {
        lblMaxSteps.CssClass = "visible";
        txtMaxSteps.CssClass = "visible";
        btnMaxSteps.CssClass = "button-positive";
    }

    /*
     * Enables selection of maximum steps
     */
    protected void disableMaxSteps()
    {
        lblMaxSteps.CssClass = "hidden";
        txtMaxSteps.CssClass = "hidden";
        btnMaxSteps.CssClass = "hidden";
    }

    /*
     * Enables the buttons to allow selecting an algorithm
     */
    protected void enableAlgorithmSelect()
    {
        lblAlgorithmSelect.CssClass = "visible";
        btnAlgorithmSelect.CssClass = "button-positive";
        ddlAlgorithmSelect.CssClass = "visible";
    }

    /*
     * Disables the buttons for selecting an algorithm
     */
    protected void disableAlgorithmSelect()
    {
        lblAlgorithmSelect.CssClass = "hidden";
        btnAlgorithmSelect.CssClass = "hidden";
        ddlAlgorithmSelect.CssClass = "hidden";
    }

    /*
     * Disables the step buttons and textbox
     */
    private void disableControls()
    {
        btnOneStep.Enabled = false;
        btnNStep.Enabled = false;
        txtStepCount.Enabled = false;
        btnOneStep.CssClass = "button-Disabled";
        btnNStep.CssClass = "button-Disabled";
    }

    /*
     * Enables the step buttons and textbox
     */
    private void enableControls()
    {
        btnOneStep.Enabled = true;
        btnNStep.Enabled = true;
        txtStepCount.Enabled = true;
        btnOneStep.CssClass = "button";
        btnNStep.CssClass = "button";
    }

    #endregion Enable&DisableButtons

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
    protected void btnUploadMap_Click(object sender, EventArgs e)
    {
        disableSave();
        agents.Clear();//everytime upload a new map file, agent list must be cleared
        Session["isBlock"] = false;
        Session["isGraph"] = false;
        int algorithmNumber = int.Parse(Session["AlgorithmNumber"].ToString());

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
                    String message = "";
                    message = checkFile(arr, algorithmNumber);
                    if (message.Equals(""))
                    {
                        Response.Write("<script>alert('Upload successfully!')</script>");
                        //This is the code to generate the block view when you upload a map
                        resetControls();
                        enableControls();
                        WorldView = new Map(Height, Width, Region, regions, agents, algorithmNumber, int.Parse(Session["MaxSteps"].ToString()));
                        Session["isBlock"] = true;
                        Session["Map"] = WorldView;
                        lblStepCount.Text = "Steps: ";
                        updateBlock();
                        takeInput.Visible = false;
                        enableNewBack();
                        ClientScript.RegisterStartupScript(GetType(), "Javascript", "javascript:getPageSize();", true);
                        //Code ends here
                    }
                    else
                        // Label1.Text=message;
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
            Response.Write("<script>alert('error')</script>");
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
    public string checkFile(ArrayList arr, int algorithmNumber)
    {
        string message = "";
        if (arr.Count >= 3)//height,width,region
        {
            try
            {
                try { Height = Convert.ToInt16(arr[0]); }
                catch (FormatException) { return "Invalid format of HEIGHT!"; }
                try { Width = Convert.ToInt16(arr[1]); }
                catch (FormatException) { return "Invalid format of WIDTH!"; }
                try { Region = Convert.ToInt16(arr[2]); }
                catch (FormatException) { return "Invalid format of NUMBER OF REGIONS!"; }
                regions = new Point[Region][];
                if (Height < 8 || Height > 15 || Width < 8 || Width > 15)
                    return "Size of environment：Minimum 8 X 8, Maximum 15 X 15！";
            }
            catch
            {
                return "Invalid input of HEIGHT/WIDTH/NUMBER OF REGIONS!";
            }
        }
        else
            return "Input file incomplete! (must include height/width/number of regions/coordinates and agents of each regions)";

        //  if (Region == (arr.Count - 3) / 2 && (arr.Count - 3) % 2 == 0)
        {
            message = regionsAndAgents(arr, algorithmNumber);
            if (!message.Equals(""))
                return message;
        }
        // else
        //    return "Input file incomplete! (miss agents or coordinates in one region )";
        //     return "NUMBER OF REGIONS does not matches the actual number of regions in file!";

        return message;
    }

    //interpreter information of each region and agent 
    public string regionsAndAgents(ArrayList arr, int algorithmNumber)
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
                string[] agentInfo;
                try { agentInfo = (arr[i + 3].ToString()).Split(' '); }
                catch { return "No agents in region " + (i + 1) / 2 + " !"; }

                if ((algorithmNumber == 1 && agentInfo.Length / 2 >= 1 && agentInfo.Length / 2 <= Math.Ceiling((double)regionNum / 2)) ||
                    (algorithmNumber == 2 && agentInfo.Length / 2 >= 1 && agentInfo.Length / 2 <= Math.Ceiling((double)regionNum / 3)) ||
                    (algorithmNumber == 3 && agentInfo.Length / 2 >= 1 && agentInfo.Length / 2 <= Math.Ceiling((double)regionNum / 4))
                    )
                {
                    message = saveAgent(i, agentInfo, algorithmNumber);
                }
                else
                {
                    return "Invalid number of agents in region " + (i + 1) / 2 + " !";
                }
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
        message = checkAdjacentRegion();
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
                    return "The No." + (j + 1) + " coordinate in region " + (i / 2 + 1) + " is invalid!";

                try
                {
                    tile.X = Convert.ToInt16(numbers[0]);
                    tile.Y = Convert.ToInt16(numbers[1]);
                }
                catch (FormatException) { return "The No." + (j + 1) + " coordinate in region " + (i / 2 + 1) + " is invalid!"; }

                if (tile.Y < 0 || tile.Y >= Width || tile.X < 0 || tile.X >= Height)
                    return "The coordinate (" + (tile.Y + 1) + "," + (tile.X + 1) + ") in region " + (i / 2 + 1) + " is overstep the boundary!";
                regions[i / 2][j] = tile;
            }
            catch
            {
                return "Invalid coordinate in region " + (i / 2 + 1) + "!";
            }
        }
        return message;
    }

    //save each Agent object in a list
    public string saveAgent(int i, string[] agentInfo, int algorithmNumber)
    {
        string message = "";
        Point tile = new Point();
        List<Point> agentList = new List<Point>();
        if (agentInfo.Length % 2 != 0)
            return "Invalid agents format in region " + (i + 1) / 2 + " !";

        int id = 0;
        for (int m = 0; m < agentInfo.Length; m++)
        {
            if (m % 2 == 0)//agent id
                try
                {
                    id = Convert.ToInt16(agentInfo[m]);
                }
                catch (Exception)
                {
                    return "The ID of No." + (m / 2 + 1) + " agent in region " + (i + 1) / 2 + " is invalid!";
                }
            else//agent coordinate
                try
                {
                    string n = agentInfo[m].Replace("(", "");
                    n = n.Replace(")", "");
                    string[] numbers = n.Split(',');
                    if (numbers.Length != 2)
                        return "The No." + (m + 1) / 2 + " agent in region " + (i + 1) / 2 + " is invalid!";
                    try
                    {
                        tile.X = Convert.ToInt16(numbers[0]);
                        tile.Y = Convert.ToInt16(numbers[1]);
                    }
                    catch (FormatException) { return "The No." + (m + 1) / 2 + " agent in region " + (i + 1) / 2 + " is invalid!"; }
                    if (checkAgentCoordinate(tile, i))
                    {
                        Agent thisAgent = new Agent(id);
                        thisAgent.setStartLocation(tile);
                        agentList.Add(tile);
                        agents.Add(thisAgent);
                    }
                    else
                        return "Agent coordinate (" + (tile.Y + 1) + "," + (tile.X + 1) + ") is not in region " + (i + 1) / 2 + " !";
                }
                catch (Exception e)
                {
                    return "Invalid agent coordinate in region " + (i + 1) / 2 + " !";
                }
            if (algorithmNumber == 3)
            {
                message = checkDeadEnd(agentList, regions[(i - 1) / 2], (i + 1) / 2);
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
                Point p = new Point(agents[i].getLocation().X, agents[i].getLocation().Y);
                message = "The ID of agent (" + (p.Y + 1) + "," + (p.X + 1) + ") is duplicate with another agent!";
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
            message = checkConnectivity(thisRegion, i);
            if (!message.Equals(""))
                return message;
            for (int j = 0; j < thisRegion.Length; j++)
                if (h.Contains(regions[i][j]))
                    return "The coordinate (" + (regions[i][j].Y + 1) + "," + (regions[i][j].X + 1) + ") in region " + (i + 1) + " is duplicate with another coordinate!";
                else
                    h.Add(regions[i][j]);
        }
        return message;
    }

    public string checkConnectivity(Point[] thisRegion, int i)
    {
        List<Point> pointsInRegion = new List<Point>(thisRegion);
        List<Point> visit = new List<Point>();
        string message = "";
        if (thisRegion.Length == 1)
            return message;
        isAdjacentRegion(pointsInRegion, visit, thisRegion[0]);
        if (visit.Count != thisRegion.Length)
            message = "Coordinates are not connected in region " + (i + 1) + " !";
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

    public String checkDeadEnd(List<Point> agentList, Point[] region, int regionIndex)
    {
        String message = "";
        List<Point> deadEnds = new List<Point>();
        for (int i = 0; i < region.Length; i++)
        {
            Point up = new Point(region[i].X, region[i].Y - 1);
            Point down = new Point(region[i].X, region[i].Y + 1);
            Point left = new Point(region[i].X - 1, region[i].Y);
            Point right = new Point(region[i].X + 1, region[i].Y);
            int num = 0;
            for (int j = 0; j < region.Length; j++)
            {
                if (region[j] == up || region[j] == down || region[j] == right || region[j] == left)
                    num++;
            }
            if (num == 1)
                deadEnds.Add(region[i]);
        }
        if (deadEnds.Count == 0)
        {
            //if (agentList.Count > 1)
            //  return "Only can put one agent if there is no dead end in region "+regionIndex+" !";
            //else
            return message;
        }
        for (int j = 0; j < agentList.Count; j++)
        {
            bool flag = false;
            foreach (Point d in deadEnds)
            {
                if (agentList[j] == d)
                {
                    flag = true;
                }
            }
            if (!flag)

            {
                return "Agents must in dead end in region " + regionIndex + " !";
            }
        }
        return message;
    }

    public String checkAdjacentRegion()
    {
        String message = "";

        int[,] regionsMarks = new int[Height + 2, Width + 2];
        for (int row = 0; row < Height + 2; row++)
        {
            for (int col = 0; col < Width + 2; col++)
            {
                regionsMarks[row, col] = regionNum(row, col);
                //if(regionsMarks[row,col]!=-1)
                //  message += " " + regionsMarks[row, col] + " "+row+","+col+" ";           
            }
        }

        //return message;
        for (int a = 0; a < regions.Length; a++)
        {
            for (int b = 0; b < regions[a].Length; b++)
            {
                int x = (regions[a][b].X) + 1;
                int y = (regions[a][b].Y) + 1;
                if (regionsMarks[x, y - 1] == (a + 1) || regionsMarks[x, y - 1] == -1)
                {
                    if (regionsMarks[x, y + 1] == (a + 1) || regionsMarks[x, y + 1] == -1)
                    {
                        if (regionsMarks[x - 1, y] == (a + 1) || regionsMarks[x - 1, y] == -1)
                        {
                            if (regionsMarks[x + 1, y] == (a + 1) || regionsMarks[x + 1, y] == -1)
                            {

                            }
                            else
                            {
                                message = "Invaild coordinate (" + (regions[a][b].Y + 1) + "," + (regions[a][b].X + 1) + ") in region " + (a + 1) + ", which makes region " + (a + 1) + " and region " + regionsMarks[x + 1, y] + " to be adjacent!";
                                return message;
                            }
                        }
                        else
                        {
                            message = "Invaild coordinate (" + (regions[a][b].Y + 1) + "," + (regions[a][b].X + 1) + ") in region " + (a + 1) + ", which makes region " + (a + 1) + " and region " + regionsMarks[x - 1, y] + " to be adjacent!";
                            return message;
                        }

                    }
                    else
                    {
                        message = "Invaild coordinate (" + (regions[a][b].Y + 1) + "," + (regions[a][b].X + 1) + ") in region " + (a + 1) + ", which makes region " + (a + 1) + " and region " + regionsMarks[x, y + 1] + " to be adjacent!";
                        return message;
                    }
                }
                else
                {
                    message = "Invaild coordinate (" + (regions[a][b].Y + 1) + "," + (regions[a][b].X + 1) + ") in region " + (a + 1) + ", which makes region " + (a + 1) + " and region " + regionsMarks[x, y - 1] + " to be adjacent!";
                    return message;
                }

            }
        }

        return message;
    }

    public int regionNum(int row, int col)
    {
        for (int i = 0; i < regions.Length; i++)
        {
            for (int j = 0; j < regions[i].Length; j++)
            {
                if (regions[i][j].X == row - 1 && regions[i][j].Y == col - 1)
                {
                    return (i + 1);
                }
            }
        }
        return -1;
    }

    #endregion FileRead

}

