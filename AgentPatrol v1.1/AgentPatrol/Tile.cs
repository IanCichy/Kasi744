using System;
using AgentPatrol;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections.Generic;

public class Tile
{
    private Button btn;
    private int type, size;
    private Point location;
    private bool seen = false;

    public Tile(int t, int x, int y, int s)
    {
        type = t;
        location = new Point(x, y);
        size = s;
        btn = new Button();

        btn.Width = size;
        btn.Height = size;
        btn.BorderStyle = BorderStyle.Solid;
        btn.BorderColor = Color.Gray;
        btn.BorderWidth = 1;
    }

    public bool isSeen()
    {
        return seen;
    }

    public void setSeen(bool b)
    {
        seen = b;
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

    public Button getDisplay()
    {
        return btn;
    }

    public int getType()
    {
        return type;
    }
}
