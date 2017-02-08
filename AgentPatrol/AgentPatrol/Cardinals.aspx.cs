using AgentPatrol;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Cardinals : System.Web.UI.Page
{
    Map world;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            world = new Map(10,10,4,getRegions(),getMapfile(), 50);
            Session["Map"] = world;
            updateGrid(world);
        }
        else
        {
            world = (Map)Session["Map"];
            updateGrid(world);
        }
    }

    protected void updateGrid(Map worldMap)
    {
        //Find the correct place to put the world on the webpage and add it to the controls
        Form.FindControl("MapContainer").Controls.Add(((Map)Session["Map"]).getDisplay());
    }

    protected int[,] getMapfile()
    {

        int[,] array = new int[,]
        {
            {1,1,1,1,1,1,1,1,1,1},
            {1,0,0,0,0,0,1,0,0,0},
            {1,1,1,0,0,0,1,0,0,0},
            {1,0,0,0,0,0,1,0,0,0},
            {1,0,0,1,1,1,1,1,0,0},
            {1,1,1,1,0,0,0,1,0,0},
            {1,0,0,1,0,0,1,1,0,0},
            {1,0,0,1,0,0,0,1,0,0},
            {1,0,0,1,0,0,0,1,0,0},
            {1,1,1,1,1,1,1,1,1,1}
        };


        return array;
    }

    protected Region[] getRegions()
    {
        Region[] reg = new Region[4];
        Region r1 = new Region(1);
        r1.addPointToRegion(new Point(1, 1));
        r1.addPointToRegion(new Point(3, 1));
        r1.addPointToRegion(new Point(4, 1));
        r1.addPointToRegion(new Point(1, 2));
        r1.addPointToRegion(new Point(3, 2));
        r1.addPointToRegion(new Point(4, 2));
        r1.addPointToRegion(new Point(1, 3));
        r1.addPointToRegion(new Point(2, 3));
        r1.addPointToRegion(new Point(3, 3));
        r1.addPointToRegion(new Point(1, 4));
        r1.addPointToRegion(new Point(2, 4));
        r1.addPointToRegion(new Point(3, 4));
        r1.addPointToRegion(new Point(1, 5));
        r1.addPointToRegion(new Point(2, 5));
        r1.addPointToRegion(new Point(3, 5));
        reg[0] = r1;

        Region r2 = new Region(2);
        r2.addPointToRegion(new Point(1, 7));
        r2.addPointToRegion(new Point(2, 7));
        r2.addPointToRegion(new Point(3, 7));
        r2.addPointToRegion(new Point(1, 8));
        r2.addPointToRegion(new Point(2, 8));
        r2.addPointToRegion(new Point(3, 8));
        r2.addPointToRegion(new Point(4, 8));
        r2.addPointToRegion(new Point(5, 8));
        r2.addPointToRegion(new Point(6, 8));
        r2.addPointToRegion(new Point(7, 8));
        r2.addPointToRegion(new Point(8, 8));
        r2.addPointToRegion(new Point(1, 9));
        r2.addPointToRegion(new Point(2, 9));
        r2.addPointToRegion(new Point(3, 9));
        r2.addPointToRegion(new Point(4, 9));
        r2.addPointToRegion(new Point(5, 9));
        r2.addPointToRegion(new Point(6, 9));
        r2.addPointToRegion(new Point(7, 9));
        r2.addPointToRegion(new Point(8, 9));
        reg[1] = r2;

        Region r3 = new Region(3);
        r3.addPointToRegion(new Point(6, 1));
        r3.addPointToRegion(new Point(7, 1));
        r3.addPointToRegion(new Point(8, 1));
        r3.addPointToRegion(new Point(6, 2));
        r3.addPointToRegion(new Point(7, 2));
        r3.addPointToRegion(new Point(8, 2));
        reg[2] = r3;

        Region r4 = new Region(4);
        r4.addPointToRegion(new Point(5, 4));
        r4.addPointToRegion(new Point(6, 4));
        r4.addPointToRegion(new Point(7, 4));
        r4.addPointToRegion(new Point(8, 4));
        r4.addPointToRegion(new Point(5, 5));
        r4.addPointToRegion(new Point(6, 5));
        r4.addPointToRegion(new Point(7, 5));
        r4.addPointToRegion(new Point(8, 5));
        r4.addPointToRegion(new Point(5, 6));
        r4.addPointToRegion(new Point(7, 6));
        r4.addPointToRegion(new Point(8, 6));
        reg[3] = r4;

        return reg;
    }

    protected void btnOneStep_Click(object sender, EventArgs e)
    {
        /*
        Random rnd = new Random();
        int x = rnd.Next(0, world.getWidth());
        int y = rnd.Next(0, world.getHeight());
        int t = rnd.Next(0, 2);

        world.setTileType(x, y, t);

        Session["Map"] = world;
        updateGrid(world);
        */
        world.oneStepAlgorithm();


    }
}
