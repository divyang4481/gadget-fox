using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace GadgetFox
{
    public partial class EditDefaultCard : System.Web.UI.Page
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
                        SqlCommand cmd = new SqlCommand("Select * from [GadgetFox].[dbo].[Addresses], [GadgetFox].[dbo].[ZipCodes] where Addresses.Zip = ZipCodes.Zip and EmailID=@EmailID and IsProfileAddress=@IsProfileAddress", myConnection);
                        cmd.Parameters.AddWithValue("@EmailID", Session["userID"].ToString());
                        cmd.Parameters.AddWithValue("@IsProfileAddress", true);
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {

                            address1TB.Text = dr["Address Line1"].ToString();
                            address2TB.Text = dr["Address Line2"].ToString();
                            cityTB.Text = dr["City"].ToString();
                           // countryDL.Text = dr["Country"].ToString();
                            stateDL.Text = dr["State"].ToString();
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

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void saveButton_Clicked(object sender, EventArgs e)
        {
            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);
            try
            {
                myConnection.Open();
                SqlCommand cmd1 = new SqlCommand("Select COUNT(*) from ZipCodes where Zip=@Zip", myConnection);
                cmd1.Parameters.AddWithValue("@Zip", zipcodeTB.Text);
                int ziprows = (int)cmd1.ExecuteScalar();
                if (ziprows == 0)
                {
                    SqlCommand cmd2 = new SqlCommand("INSERT INTO [GadgetFox].[dbo].[ZipCodes] ([Zip],[City],[State],[Country])" +
            " VALUES(@Zip,@City,@State,@Country)", myConnection);
                    cmd2.Parameters.AddWithValue("@City", cityTB.Text);
                    cmd2.Parameters.AddWithValue("@State", stateDL.Text);
                    cmd2.Parameters.AddWithValue("@Country", countryDL.Text);
                    cmd2.Parameters.AddWithValue("@Zip", zipcodeTB.Text);
                    cmd2.ExecuteNonQuery();
                }
                // Update command
                SqlCommand cmd = new SqlCommand("Update Addresses set [Address Line1]=@AddressLine1, [Address Line2]=@AddressLine2, Zip=@Zip where " +
                    "EmailID=@EmailID", myConnection);
                cmd.Parameters.AddWithValue("@AddressLine1", address1TB.Text);
                cmd.Parameters.AddWithValue("@AddressLine2", address2TB.Text);
                cmd.Parameters.AddWithValue("@Zip", zipcodeTB.Text);
                cmd.Parameters.AddWithValue("@EmailID", Session["userID"]);
                int rows = cmd.ExecuteNonQuery();
                if (rows == 1)
                {
                    Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('Information Saved successfully')</SCRIPT>");
                    Response.Redirect("~/Home.aspx");
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