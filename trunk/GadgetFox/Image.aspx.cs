using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace GadgetFox
{
    public partial class Image : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);
            try
            {
                myConnection.Open();
                SqlCommand cmd = new SqlCommand("Select * from [GadgetFox].[dbo].[Images] where ImageID=@ImageID", myConnection);
                cmd.Parameters.AddWithValue("@ImageID", Request.QueryString["ImageID"]);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    byte[] _buf = (byte[])dr["ImageData"]; //where your image save as blob in SQL server
                    Response.ContentType = "image/jpg";
                    Response.BinaryWrite(_buf);

                }

            }
            catch (SqlException ex)
            {
                Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('" + ex.Message + "')</SCRIPT>");
            }
            finally
            {
                myConnection.Close();
            }
           
        }
    }
}