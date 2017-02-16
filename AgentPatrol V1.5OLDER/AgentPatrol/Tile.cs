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

    public Tile(int t, int x, int y)
    {
        type = t;
        location = new Point(x, y);
        btn = new Button();
        btn.ID = "" + x + "," + y;
    }

    public void setSize(int x, int y)
    {
        btn.Width = x;
        btn.Height = y;
    }

    public void setType(int t)
    {
        type = t;
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

    public int getType()
    {
        return type;
    }

    public Tile Clone()
    {
        return new Tile(this.getType(), this.getxPos(), this.getyPos());
    }
}
