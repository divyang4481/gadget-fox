using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
namespace GadgetFox
{
    public partial class EditShippingAddress : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userID"] != null)
                {
                   String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                    SqlConnection myConnection = new SqlConnection(myConnectionString);
         
                    try
                    {
                        myConnection.Open();
                        SqlCommand cmd = new SqlCommand("Select * from [GadgetFox].[dbo].[Users] where EmailID=@EmailID", myConnection);
                        cmd.Parameters.AddWithValue("@EmailID", Session["userID"].ToString());
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            
                            address1TB.Text = dr["Address Line 1"].ToString();
                            address2TB.Text = dr["Address Line 2"].ToString();
                            if (dr["Zip"] != null)
                                zipcodeTB.Text = dr["Zip"].ToString();
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

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}