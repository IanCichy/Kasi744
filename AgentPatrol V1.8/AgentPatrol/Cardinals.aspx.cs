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
            Session["isBlock"] = false;
            Session["isGraph"] = false;
            ClientScript.RegisterStartupScript(GetType(), "Javascript", "javascript:getPageSize();", true);
        }
        else
        {
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
            if ((bool)Session["isGraph"])
            {
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
        }
    }

    protected void updateBlock()
    {
        if ((bool)Session["isBlock"])
        {
            //Find the correct place to put the world on the webpage and add it to the controls
            blockViewContainer.Controls.Clear();
            blockViewContainer.Controls.Add(((Map)Session["Map"]).getBlockViewDisplay());
            WorldView.setBlockViewMapSize(int.Parse(PageWidth.Value), int.Parse(PageHeight.Value));
            UDPblockView.Update();
        }
    }

    protected void updateGraph()
    {
        if ((bool)Session["isGraph"])
        {
            //Find the correct place to put the world on the webpage and add it to the controls
            graphViewContainer.Controls.Clear();
            graphViewContainer.Controls.Add(((Map)Session["Map"]).getGraphViewDisplay());
            WorldView.setGraphViewMapSize(int.Parse(PageWidth.Value), int.Parse(PageHeight.Value));
            UDPgraphView.Update();
        }
    }

    protected void btnOneStep_Click(object sender, EventArgs e)
    {
        if ((bool)Session["isBlock"])
        {
            WorldView.oneStepAlgorithm();
            updateBlock();
        }
        if ((bool)Session["isGraph"])
        {
            WorldView.updateGraphView();
            updateGraph();
        }
    }

    protected void Block_button_Click(object sender, EventArgs e)
    {
        Button button = sender as Button;
        // identify which button was clicked and perform necessary actions
        string[] s = button.ID.Split(',');
        int x = int.Parse(s[0]);
        int y = int.Parse(s[1]);
        Point p = new Point(x, y);
        WorldView.makeGraphView(p);
        Session["isGraph"] = true;
        updateGraph();
    }

    protected void Graph_button_Click(object sender, EventArgs e)
    {
        Button button = sender as Button;
        // identify which button was clicked and perform necessary actions
        string[] s = button.ID.Split(',');
        int x = int.Parse(s[0].Substring(1));
        int y = int.Parse(s[1]);
        Point p = new Point(x, y);
        string info = WorldView.getAgentInfo(p);
        lblInfo.Text = info;
        mpe.Show();
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
                        Label1.Text = "Upload successfully!";

                        WorldView = new Map(Height, Width, Region, regions);
                        Session["isBlock"] = true;
                        Session["Map"] = WorldView;
                        updateBlock();
                        updateGraph();
                        ClientScript.RegisterStartupScript(GetType(), "Javascript", "javascript:getPageSize();", true);

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
