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
    public partial class EditShippingAddress : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userID"] == null)
                {
                    // Redirect user to login before doing anything else
                    Response.Redirect("~/Login.aspx?redirect=EditShippingAddress.aspx");
                }
                else
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
                            countryDL.Text = dr["Country"].ToString();
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

                SqlCommand cmd3 = new SqlCommand("Select COUNT(*) from Addresses where EmailID=@EmailID", myConnection);
                cmd3.Parameters.AddWithValue("@EmailID", Session["userID"]);
                int idRows = (int)cmd3.ExecuteScalar();
                // Insert new address into database
                if (idRows == 0)
                {
                    SqlCommand cmd4 = new SqlCommand("INSERT INTO [GadgetFox].[dbo].[Addresses] ([AddressID],[Address Line1],[Address Line2],[Zip],[EmailID],[IsProfileAddress])" +
                " VALUES(@AddressId,@AddressLine1,@AddressLine2,@Zip,@EmailID,@ProfileAddress)", myConnection);
                    cmd4.Parameters.AddWithValue("@AddressId", getNextAddressId());
                    cmd4.Parameters.AddWithValue("@AddressLine1", address1TB.Text);
                    cmd4.Parameters.AddWithValue("@AddressLine2", address2TB.Text);
                    cmd4.Parameters.AddWithValue("@Zip", zipcodeTB.Text);
                    cmd4.Parameters.AddWithValue("@EmailID", Session["userID"]);
                    cmd4.Parameters.AddWithValue("@ProfileAddress", 1);
                    int rc = cmd4.ExecuteNonQuery();
                    if (rc > 0)
                    {
                        returnLabel.Text = "Your default address was successfully saved";
                        saveButton.Visible = false;
                        cancelButton.Text = "Close";
                    }
                    else
                    {
                        returnLabel.Text = "Failed to save your default address. Please try again later!";
                    }
                }
                else
                {
                    // Update address in database
                    SqlCommand cmd = new SqlCommand("UPDATE Addresses SET [Address Line1]=@AddressLine1, [Address Line2]=@AddressLine2, Zip=@Zip, IsProfileAddress=@ProfileAddress where " +
                        "EmailID=@EmailID", myConnection);
                    cmd.Parameters.AddWithValue("@AddressLine1", address1TB.Text);
                    cmd.Parameters.AddWithValue("@AddressLine2", address2TB.Text);
                    cmd.Parameters.AddWithValue("@Zip", zipcodeTB.Text);
                    cmd.Parameters.AddWithValue("@EmailID", Session["userID"]);
                    cmd.Parameters.AddWithValue("@ProfileAddress", 1);
                    int rc = cmd.ExecuteNonQuery();
                    if (rc > 0)
                    {
                        returnLabel.Text = "Your address was successfully saved";
                        saveButton.Visible = false;
                        cancelButton.Text = "Close";
                    }
                    else
                    {
                        returnLabel.Text = "Failed to save your address. Please try again later!";
                    }
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

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /**
         * Generate the next address Id
         */
        public int getNextAddressId()
        {
            int nextId = 0;
            int id;
            String conStr = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection con = new SqlConnection(conStr);

            SqlCommand cmd = new SqlCommand("Select AddressID from [GadgetFox].[dbo].[Addresses]", con);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                id = Convert.ToInt16(dr["AddressID"].ToString());
                if (id > nextId)
                {
                    nextId = id;
                }
            }
            con.Close();

            return nextId + 1;
        }
    }
}