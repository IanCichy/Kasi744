using AgentPatrol;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Cardinals : System.Web.UI.Page
{
    Map WorldView;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            WorldView = new Map(10, 12, 4, getRegions());
            Session["isBlock"] = true;
            Session["isGrid"] = false;
            Session["Map"] = WorldView;
            ClientScript.RegisterStartupScript(GetType(), "Javascript", "javascript:getPageSize();", true);
        }
        else
        {
            WorldView = (Map)Session["Map"];
            foreach (Tile tle in WorldView.getTiles())
            {
                if (tle.getType() != 1)
                    tle.getDisplay().Click += new EventHandler(button_Click);

                ScriptManager.GetCurrent(Page).RegisterPostBackControl(tle.getDisplay());
            }

            if ((bool)Session["isBlock"])
                updateBlock();
            if ((bool)Session["isGrid"])
                updateGrid();
        }
    }

    protected void updateBlock()
    {
        //Find the correct place to put the world on the webpage and add it to the controls
        blockViewContainer.Controls.Add(((Map)Session["Map"]).getBlockViewDisplay());
        WorldView.setBlockViewMapSize(int.Parse(PageWidth.Text), int.Parse(PageHeight.Text));

    }

    protected void updateGrid()
    {
        //Find the correct place to put the world on the webpage and add it to the controls
        graphViewContainer.Controls.Add(((Map)Session["Map"]).getGraphViewDisplay());
        WorldView.setGraphViewMapSize(int.Parse(PageWidth.Text), int.Parse(PageHeight.Text));

    }

    protected Point[][] getRegions()
    {
        Point[][] regions = new Point[4][];

        regions[0] = new Point[15];
        regions[0][0] = new Point(1, 1);
        regions[0][1] = new Point(3, 1);
        regions[0][2] = new Point(4, 1);
        regions[0][3] = new Point(1, 2);
        regions[0][4] = new Point(3, 2);
        regions[0][5] = new Point(4, 2);
        regions[0][6] = new Point(1, 3);
        regions[0][7] = new Point(2, 3);
        regions[0][8] = new Point(3, 3);
        regions[0][9] = new Point(1, 4);
        regions[0][10] = new Point(2, 4);
        regions[0][11] = new Point(3, 4);
        regions[0][12] = new Point(1, 5);
        regions[0][13] = new Point(2, 5);
        regions[0][14] = new Point(3, 5);

        regions[1] = new Point[19];
        regions[1][0] = new Point(1, 7);
        regions[1][1] = new Point(2, 7);
        regions[1][2] = new Point(3, 7);
        regions[1][3] = new Point(1, 8);
        regions[1][4] = new Point(2, 8);
        regions[1][5] = new Point(3, 8);
        regions[1][6] = new Point(4, 8);
        regions[1][7] = new Point(5, 8);
        regions[1][8] = new Point(6, 8);
        regions[1][9] = new Point(7, 8);
        regions[1][10] = new Point(8, 8);
        regions[1][11] = new Point(1, 9);
        regions[1][12] = new Point(2, 9);
        regions[1][13] = new Point(3, 9);
        regions[1][14] = new Point(4, 9);
        regions[1][15] = new Point(5, 9);
        regions[1][16] = new Point(6, 9);
        regions[1][17] = new Point(7, 9);
        regions[1][18] = new Point(8, 9);

        regions[2] = new Point[6];
        regions[2][0] = new Point(6, 1);
        regions[2][1] = new Point(7, 1);
        regions[2][2] = new Point(8, 1);
        regions[2][3] = new Point(6, 2);
        regions[2][4] = new Point(7, 2);
        regions[2][5] = new Point(8, 2);

        regions[3] = new Point[11];
        regions[3][0] = new Point(5, 4);
        regions[3][1] = new Point(6, 4);
        regions[3][2] = new Point(7, 4);
        regions[3][3] = new Point(8, 4);
        regions[3][4] = new Point(5, 5);
        regions[3][5] = new Point(6, 5);
        regions[3][6] = new Point(7, 5);
        regions[3][7] = new Point(8, 5);
        regions[3][8] = new Point(5, 6);
        regions[3][9] = new Point(7, 6);
        regions[3][10] = new Point(8, 6);

        return regions;
    }

    protected void btnOneStep_Click(object sender, EventArgs e)
    {
        WorldView.oneStepAlgorithm();
    }


    protected void button_Click(object sender, EventArgs e)
    {
        Button button = sender as Button;
        // identify which button was clicked and perform necessary actions
        int x = int.Parse(button.ID.Substring(0, 1));
        int y = int.Parse(button.ID.Substring(2));
        Point p = new Point(x, y);
        WorldView.makeGraphView(p);
        Session["isGrid"] = true;
    }


}
