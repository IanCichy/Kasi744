using System;
using AgentPatrol;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections.Generic;

public class Tile
{
    private Button btn;
    private int type;
    private Point location;
    private int score;
    private bool visited;
    private LinkButton lnkbtn;
    private Panel pnl;
    private Label lbl1;
    private Label lbl2;

    /*
     * Creates a new tile with a
     * type
     * xlocation
     * ylocation
     * and also creates the buttons that are displayed
     */
    public Tile(int t, int x, int y)
    {
        type = t;
        location = new Point(x, y);
        btn = new Button();
        btn.ID = "" + x + "," + y;
        score = 0;
        visited = false;
    }

    /*
     * Creates a new tile with a
     * type
     * xlocation
     * ylocation
     * and also creates the buttons that are displayed
     * additional string input to modify id
     */
    public Tile(int t, int x, int y, string s)
    {
        type = t;
        location = new Point(x, y);
        btn = new Button();
        btn.ID = s +  x + "," + y;
    }

    /*
     * Creates a new tile with a
     * type
     * xlocation
     * ylocation
     * and also creates the buttons that are displayed
     * additional string input to modify id
     * Also a bool to use a panel instead of a button
     */
    public Tile(int t, int x, int y, string s, bool b)
    {
        if (b)
        {
            type = t;
            location = new Point(x, y);
            lnkbtn = new LinkButton();
        }
        else
        {
            type = t;
            location = new Point(x, y);
            pnl = new Panel();
            lbl1 = new Label();
            lbl2 = new Label();
        }
    }

    public void setSize(int x, int y)
    {
        if (btn != null)
        {
            btn.Width = x;
            btn.Height = y;
        }
        else if (lnkbtn != null)
        {
            lnkbtn.Width = x;
            lnkbtn.Height = y;
        }
        else
        {
            pnl.Width = x;
            pnl.Height = y;
        }
    }

    public void setType(int t)
    {
        type = t;
    }

    public void setScore(int s)
    {
        score = s;
    }

    public int getScore()
    {
        return score;
    }

    public Tile getTile()
    {
        return this;
    }

    public int getxPos()
    {
        return location.X;
    }

    public int getyPos()
    {
        return location.Y;
    }

    public Point getPointLocation()
    {
        return location;
    }

    public Button getDisplay()
    {
        return btn;
    }

    public LinkButton getDisplayLnkBtn()
    {
        return lnkbtn;
    }

    public Panel getDisplayPnl()
    {
        return pnl;
    }

    public Label getLocLabel()
    {
        return lbl1;
    }

    public Label getAgentLabel()
    {
        return lbl2;
    }

    public int getType()
    {
        return type;
    }

    public Tile Clone()
    {
        return new Tile(this.getType(), this.getxPos(), this.getyPos());
    }

    public bool beenVisited()
    {
        return visited;
    }

    public void setVisited(bool b)
    {
        visited = b;
    }
}
