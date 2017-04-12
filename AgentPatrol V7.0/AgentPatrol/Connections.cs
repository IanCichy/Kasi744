using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AgentPatrol;
using System.Data.SqlClient;

public static class Connections
{
    // connection strings
    private const String ConnectionString =
        "Data Source=MURCSP16\\CardinalDB;Initial Catalog=Cardinal;User Id=sa;Password=Kasi744#%744";

    public static SqlConnection Connection()
    {
        return new SqlConnection(ConnectionString);
    }
}


