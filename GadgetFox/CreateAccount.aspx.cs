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
    public partial class CreateAccount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);
            try
            {
                myConnection.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO [GadgetFox].[dbo].[Users] ([EmailID],[FirstName],[LastName],[DOB],[Password],[PhoneNum],[RoleId])" + 
                "VALUES(@EmailID,@FirstName,@LastName,@DOB,@Password,@PhoneNum,@RoleId)", myConnection);
                cmd.Parameters.AddWithValue("@EmailID", txtEmailID.Text);
                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                if (!String.IsNullOrEmpty(txtDOB.Text) || !txtDOB.Text.Equals("MM/DD/YYYY"))
                    cmd.Parameters.AddWithValue("@DOB", Convert.ToDateTime(txtDOB.Text));
                else
                    cmd.Parameters.AddWithValue("@DOB", null);
                cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
                cmd.Parameters.AddWithValue("@PhoneNum", txtPhoneNum.Text);
                cmd.Parameters.AddWithValue("@RoleID", 1);
                int rows = cmd.ExecuteNonQuery();
                if (rows == 1)
                {
                    Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('Account created successfully')</SCRIPT>");
                    Response.Redirect("~/Login.aspx");
                }
            }
            catch (SqlException ex)
            {
                Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('"+ ex.Message +"')</SCRIPT>");
            }
            finally
            {
                myConnection.Close();
            }
        }
    }
}