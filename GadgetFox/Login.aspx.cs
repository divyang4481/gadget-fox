using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Web.SessionState;

namespace GadgetFox
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblLoginError.Text = "";
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);
            try
            {
                myConnection.Open();
                SqlCommand cmd = new SqlCommand("Select FirstName,LastName,RoleID from [GadgetFox].[dbo].[Users] where EmailID=@EmailID and Password=@Password", myConnection);
                cmd.Parameters.AddWithValue("@EmailID", txtEmailID.Text);
                cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    Session["user"] = dr["FirstName"] + " " + dr["LastName"];
                    Session["userID"] = txtEmailID.Text;
                    Session["userRole"] = dr["RoleID"];
                    Response.Redirect("~/Home.aspx");
                }
                else
                    lblLoginError.Text = "Invalid username password combination";
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