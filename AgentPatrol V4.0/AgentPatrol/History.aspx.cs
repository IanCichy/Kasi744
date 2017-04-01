using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AgentPatrol;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

public partial class History : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtSearch.Focus();
        }
        else
        {

        }
        sqlRuns.ConnectionString = Connections.Connection().ConnectionString;
    }

    protected void btnReturn_Click(object sender, EventArgs e)
    {
        Response.Redirect("Cardinals.aspx");
    }

    protected void gvRuns_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("ViewRunHistory"))
        {
            GridViewRow gvr = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            int rowIndex = gvr.RowIndex;

            var fileID = int.Parse(gvRuns.DataKeys[rowIndex]["ID"].ToString());

            txtResults.Text = getData(fileID);
            txtResults.Width = 600;
            txtResults.Height = 750;
        }
    }

    public String getData(int id)
    {
        // Set up update command
        var command =
            new SqlCommand("SELECT fileData from RunData where ID=@ID")
            {
                CommandType = CommandType.Text,
                Connection = Connections.Connection()
            };

        command.Parameters.Add(new SqlParameter("ID", id));

        // Execute command
        command.Connection.Open();
        String message = (String)command.ExecuteScalar();
        command.Connection.Close();

        return message;
    }

    protected void gvRuns_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            foreach (TableCell tc in e.Row.Cells)
            {
                if (tc.HasControls())
                {
                    // search for the header link
                    LinkButton lnk = (LinkButton)tc.Controls[0];
                    if (lnk != null)
                    {
                        // inizialize a new image
                        System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                        // setting the dynamically URL of the image
                        img.ImageUrl = "Assets/Images/" + (gvRuns.SortDirection == SortDirection.Ascending ? "arrow_up" : "arrow_down") + ".png";
                        img.Width = 16;
                        img.Height = 8;
                        // checking if the header link is the user's choice
                        if (gvRuns.SortExpression == lnk.CommandArgument)
                        {
                            // adding a space and the image to the header link
                            tc.Controls.Add(img);
                        }
                    }
                }
            }
        }
    }

}
