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

}
