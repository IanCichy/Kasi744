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
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //Remove the back button
            btnViewBlock.CssClass = "backButtonHidden";

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
        if (WorldView.isSearchDone())
            disableControls();

        //Find the correct place to put the world on the webpage and add it to the controls
        viewContainer.Controls.Clear();
        viewContainer.Controls.Add(((Map)Session["Map"]).getBlockViewDisplay());
        WorldView.setBlockViewMapSize(int.Parse(PageWidth.Value), int.Parse(PageHeight.Value));
        //Remove the back button
        btnViewBlock.CssClass = "backButtonHidden";
        UDPViews.Update();
    }

    /*
     * updateGraph -
     *   Checks to see if the algorithm is finished
     *   places the graph view on the screen
     */
    protected void updateGraph()
    {
        if (WorldView.isSearchDone())
            disableControls();

        //Find the correct place to put the world on the webpage and add it to the controls
        viewContainer.Controls.Clear();
        viewContainer.Controls.Add(((Map)Session["Map"]).getGraphViewDisplay());
        WorldView.setGraphViewMapSize(int.Parse(PageWidth.Value), int.Parse(PageHeight.Value));
        //add the back button
        btnViewBlock.CssClass = "backButtonVisible";
        UDPViews.Update();
    }

    /*
     * Will run a single step of the algorithm and update 
     */
    protected void btnOneStep_Click(object sender, EventArgs e)
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
    /*
     * Will run 'n' steps of the algorithm
     */
    protected void btnNStep_Click(object sender, EventArgs e)
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
        btnOneStep.CssClass = "buttonDisabled";
        btnNStep.CssClass = "buttonDisabled";
    }

    /*
     * Enables some controls on the page
     */
    private void enableControls()
    {
        btnOneStep.Enabled = true;
        btnNStep.Enabled = true;
        btnOneStep.CssClass = "button";
        btnNStep.CssClass = "button";
    }

    //----------Yuxin Liu-------------------------------------------------------------------------------
    protected void uploadMap_Click(object sender, EventArgs e)
    {
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
                    {
                        arr.Add(line);
                    }
                    if (File.Exists(@path))
                    {
                        File.Delete(@path);
                    }
                    if (checkFile(arr))
                    {
                        Label1.Text = "Upload successful!";
                        //This is the code to generate the block view when you upload a map
                        resetControls();
                        enableControls();
                        WorldView = new Map(Height, Width, Region, regions);
                        Session["isBlock"] = true;
                        Session["Map"] = WorldView;
                        lblStepCount.Text = "Steps: ";
                        updateBlock();
                        ClientScript.RegisterStartupScript(GetType(), "Javascript", "javascript:getPageSize();", true);
                        //Code ends here
                    }
                    else Label1.Text = "Wrong format of file!";
                }
                else
                {
                    Label1.Text = "Please upload .txt file！";
                }
            }
            else
            {
                Label1.Text = "No file chosen!";
            }
        }
        catch (Exception error)
        {
            Label1.Text = error.ToString();
        }
    }

    public static bool IsAllowedExtension(FileUpload uploadMap)
    {
        bool flag = false;
        string fileExtension = System.IO.Path.GetExtension(uploadMap.FileName).ToLower();
        if (fileExtension == ".txt")
        {
            flag = true;
        }
        return flag;
    }

    public static bool checkFile(ArrayList arr)
    {
        bool flag = false;
        if (arr.Count >= 3)//height,width,region
        {
            try
            {
                Height = Convert.ToInt16(arr[0]);
                Width = Convert.ToInt16(arr[1]);
                Region = Convert.ToInt16(arr[2]);
                regions = new Point[Region][];
            }
            catch { return false; }
        }
        if (Region == arr.Count - 3)
        {
            Point tile = new Point();
            for (int i = 0; i < Region; i++)//for each region
            {
                string[] points = (arr[i + 3].ToString()).Split(' ');
                int num = points.Length;
                regions[i] = new Point[num];
                for (int j = 0; j < points.Length; j++)//for each point of different region
                {
                    try
                    {
                        string nums = points[j].Replace("(", "");
                        nums = nums.Replace(")", "");
                        string[] numbers = nums.Split(',');
                        tile.X = Convert.ToInt16(numbers[0]);
                        tile.Y = Convert.ToInt16(numbers[1]);
                        regions[i][j] = tile;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            flag = true;
        }

        return flag;
    }

    //------------------------------------------------------------------------------------
}
