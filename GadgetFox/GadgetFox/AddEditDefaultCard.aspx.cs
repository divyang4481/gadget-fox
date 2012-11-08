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
    public partial class AddEditDefaultCard : System.Web.UI.Page
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
                        // Find existing credit card
                        myConnection.Open();
                        SqlCommand cmd = new SqlCommand("Select * from [GadgetFox].[dbo].[CCDetails] where EmailID=@EmailID", myConnection);
                        cmd.Parameters.AddWithValue("@EmailID", Session["userID"].ToString());
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            cardNumberTB.Text = dr["CCNum"].ToString();
                            expMonthTB.Text = dr["ExpMonth"].ToString();
                            expYearTB.Text = dr["ExpYear"].ToString();
                            cvvNumberTB.Text = dr["CVV"].ToString();
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
                // Is there a credit card in the database
                myConnection.Open();
                SqlCommand cmd1 = new SqlCommand("Select COUNT(*) from CCDetails where EmailID=@EmailID", myConnection);
                cmd1.Parameters.AddWithValue("@EmailID", Session["userID"]);
                int idRows = (int)cmd1.ExecuteScalar();
                if (idRows == 0)
                {
                    String firstName = "";
                    String lastName = "";
                    int addressId = -1;

                    // Find address ID
                    SqlCommand cmd2 = new SqlCommand("Select * from [GadgetFox].[dbo].[Addresses] where EmailID=@EmailID", myConnection);
                    cmd2.Parameters.AddWithValue("@EmailID", Session["userID"].ToString());
                    SqlDataReader dr = cmd2.ExecuteReader();
                    if (dr.Read())
                    {
                        addressId = Int16.Parse(dr["AddressID"].ToString());
                    }
                    dr.Close();

                    // Find first and last name
                    SqlCommand cmd3 = new SqlCommand("Select * from [GadgetFox].[dbo].[Users] where EmailID=@EmailID", myConnection);
                    cmd3.Parameters.AddWithValue("@EmailID", Session["userID"].ToString());
                    dr = cmd3.ExecuteReader();
                    if (dr.Read())
                    {
                        firstName = dr["FirstName"].ToString();
                        lastName = dr["LastName"].ToString();
                    }
                    dr.Close();

                    returnLabel.Text = getNextCardId() + " " + Session["userID"] + " " + cardNumberTB.Text +
                        cvvNumberTB.Text + " " + expMonthTB.Text + " " + expYearTB.Text + " " + addressId + " " +
                        firstName + " " + lastName;

                    // Insert new address into database
                    SqlCommand cmd4 = new SqlCommand("INSERT INTO [GadgetFox].[dbo].[CCDetails] ([CCID],[EmailID],[CCNum],[CVV],[ExpMonth],[ExpYear],[AddressID],[FirstName],[LastName],[IsProfileCC])" +
                        " VALUES(@CCId,@EmailId,@CCNum,@CVV,@ExpMonth,@ExpYear,@AddressId,@FirstName,@LastName,@IsProfileCc)", myConnection);
                    cmd4.Parameters.AddWithValue("@CCId", getNextCardId());
                    cmd4.Parameters.AddWithValue("@EmailId", Session["userID"]);
                    cmd4.Parameters.AddWithValue("@CCNum", cardNumberTB.Text);
                    cmd4.Parameters.AddWithValue("@CVV", cvvNumberTB.Text);
                    cmd4.Parameters.AddWithValue("@ExpMonth", Int16.Parse(expMonthTB.Text));
                    cmd4.Parameters.AddWithValue("@ExpYear", Int16.Parse(expYearTB.Text));
                    cmd4.Parameters.AddWithValue("@AddressId", addressId);
                    cmd4.Parameters.AddWithValue("@FirstName", firstName);
                    cmd4.Parameters.AddWithValue("@LastName", lastName);
                    cmd4.Parameters.AddWithValue("@IsProfileCc", 1);
                    
                    int rc = cmd4.ExecuteNonQuery();
                    if (rc > 0)
                    {
                        returnLabel.Text = "Your credit card was successfully saved";
                        saveButton.Visible = false;
                        cancelButton.Text = "Close";
                    }
                    else
                    {
                        returnLabel.Text = "Failed to save your credit card. Please try again later!";
                    }
                }
                else
                {
                    // Update credit card in database
                    SqlCommand cmd = new SqlCommand("Update CCDetails set CCNum=@CCNum, ExpMonth=@ExpMonth, ExpYear=@ExpYear, CVV=@CVV where " +
                        "EmailID=@EmailID", myConnection);
                    cmd.Parameters.AddWithValue("@CCNum", cardNumberTB.Text);
                    cmd.Parameters.AddWithValue("@ExpMonth", expMonthTB.Text);
                    cmd.Parameters.AddWithValue("@ExpYear", expYearTB.Text);
                    cmd.Parameters.AddWithValue("@CVV", cvvNumberTB.Text);
                    cmd.Parameters.AddWithValue("@EmailID", Session["userID"]);
                    int rc = cmd.ExecuteNonQuery();
                    if (rc > 0)
                    {
                        returnLabel.Text = "Your credit card was successfully saved";
                        saveButton.Visible = false;
                        cancelButton.Text = "Close";
                    }
                    else
                    {
                        returnLabel.Text = "Failed to save your credit card. Please try again later!";
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
         * Generate the next credit card Id
         */
        public int getNextCardId()
        {
            int nextId = 0;
            int id;
            String conStr = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection con = new SqlConnection(conStr);

            SqlCommand cmd = new SqlCommand("Select CCID from [GadgetFox].[dbo].[CCDetails]", con);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                id = Convert.ToInt16(dr["CCID"].ToString());
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