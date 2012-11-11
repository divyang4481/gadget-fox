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
        private int addressId = -1;

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

                        // Find default address
                        SqlCommand cmd = new SqlCommand("Select * from [GadgetFox].[dbo].[Addresses], [GadgetFox].[dbo].[ZipCodes] where Addresses.Zip = ZipCodes.Zip and EmailID=@EmailID and IsProfileAddress=@IsProfileAddress", myConnection);
                        cmd.Parameters.AddWithValue("@EmailID", Session["userID"].ToString());
                        cmd.Parameters.AddWithValue("@IsProfileAddress", true);
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            addressId = Int16.Parse(dr["AddressID"].ToString());
                            address1TB.Text = dr["Address Line1"].ToString();
                            address2TB.Text = dr["Address Line2"].ToString();
                            cityTB.Text = dr["City"].ToString();
                            countryDL.Text = dr["Country"].ToString();
                            stateDL.Text = dr["State"].ToString();
                            if (dr["Zip"] != null)
                                zipcodeTB.Text = dr["Zip"].ToString();
                        }
                        dr.Close();

                        // Find first and last name
                        SqlCommand cmd2 = new SqlCommand("Select * from [GadgetFox].[dbo].[Users] where EmailID=@EmailID", myConnection);
                        cmd2.Parameters.AddWithValue("@EmailID", Session["userID"].ToString());
                        SqlDataReader dr2 = cmd2.ExecuteReader();
                        if (dr2.Read())
                        {                            
                            firstNameTB.Text = dr2["FirstName"].ToString();
                            lastNameTB.Text = dr2["LastName"].ToString();
                        }
                        dr2.Close();  
                      
                        // Find default credit card
                        SqlCommand cmd3 = new SqlCommand("Select * from [GadgetFox].[dbo].[CCDetails] where EmailID=@EmailID", myConnection);
                        cmd3.Parameters.AddWithValue("@EmailID", Session["userID"].ToString());
                        SqlDataReader dr3 = cmd3.ExecuteReader();
                        if (dr3.Read())
                        {
                            firstNameTB.Text = dr3["FirstName"].ToString();
                            lastNameTB.Text = dr3["LastName"].ToString();

                            cardTypeDL.Text = dr3["CCType"].ToString();
                            cardNumberTB.Text = dr3["CCNum"].ToString();
                            cvvTB.Text = dr3["CVV"].ToString();
                            expMonthTB.Text = dr3["ExpMonth"].ToString();
                            expYearTB.Text = dr3["ExpYear"].ToString();
                            firstNameTB.Text = dr3["FirstName"].ToString();
                            lastNameTB.Text = dr3["LastName"].ToString();
                            address1TB.Text = dr3["Address1"].ToString();
                            address2TB.Text = dr3["Address2"].ToString();
                            cityTB.Text = dr3["City"].ToString();
                            stateDL.Text = dr3["State"].ToString();
                            countryDL.Text = dr3["Country"].ToString();
                            zipcodeTB.Text = dr3["Zip"].ToString();
                        }
                        dr3.Close();  
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
                    // Insert new credit card into database
                    SqlCommand cmd2 = new SqlCommand("INSERT INTO [GadgetFox].[dbo].[CCDetails] ([CCID],[EmailID],[CCType],[CCNum],[CVV],[ExpMonth],[ExpYear],[FirstName],[LastName],[IsProfileCC],[Address1],[Address2],[City],[State],[Country],[Zip])" +
                        " VALUES(@CCID,@EmailID,@CCType,@CCNum,@CVV,@ExpMonth,@ExpYear,@FirstName,@LastName,@IsProfileCC,@Address1,@Address2,@City,@State,@Country,@Zip)", myConnection);
                    cmd2.Parameters.AddWithValue("@CCID", getNextCardId());
                    cmd2.Parameters.AddWithValue("@EmailID", Session["userID"].ToString());
                    cmd2.Parameters.AddWithValue("@CCType", cardTypeDL.Text);
                    cmd2.Parameters.AddWithValue("@CCNum", cardNumberTB.Text);
                    cmd2.Parameters.AddWithValue("@CVV", cvvTB.Text);
                    cmd2.Parameters.AddWithValue("@ExpMonth", int.Parse(expMonthTB.Text));
                    cmd2.Parameters.AddWithValue("@ExpYear", int.Parse(expYearTB.Text));
                    cmd2.Parameters.AddWithValue("@FirstName", firstNameTB.Text);
                    cmd2.Parameters.AddWithValue("@LastName", lastNameTB.Text);
                    cmd2.Parameters.AddWithValue("@IsProfileCC", true);
                    cmd2.Parameters.AddWithValue("@Address1", address1TB.Text);
                    cmd2.Parameters.AddWithValue("@Address2", address2TB.Text);
                    cmd2.Parameters.AddWithValue("@City", cityTB.Text);
                    cmd2.Parameters.AddWithValue("@State", stateDL.Text);
                    cmd2.Parameters.AddWithValue("@Country", countryDL.Text);
                    cmd2.Parameters.AddWithValue("@Zip", zipcodeTB.Text);
                    int rc = cmd2.ExecuteNonQuery();
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
                    // [CCID],[EmailID],[CCNum],[CVV],[ExpMonth],[ExpYear],[AddressID],[FirstName],[LastName],[IsProfileCC],[Address1],[Address2],[City],[State],[Country],[Zip]
                    SqlCommand cmd = new SqlCommand("Update CCDetails set CCType=@CCType, CCNum=@CCNum, CVV=@CVV, ExpMonth=@ExpMonth, ExpYear=@ExpYear, FirstName=@FirstName, LastName=@LastName, IsProfileCC=@IsProfileCC, Address1=@Address1, Address2=@Address2, City=@City, State=@State, Country=@Country, Zip=@Zip where " +
                        "EmailID=@EmailID", myConnection);
                    cmd.Parameters.AddWithValue("@CCType", cardTypeDL.Text);
                    cmd.Parameters.AddWithValue("@CCNum", cardNumberTB.Text);
                    cmd.Parameters.AddWithValue("@CVV", cvvTB.Text);
                    cmd.Parameters.AddWithValue("@ExpMonth", expMonthTB.Text);
                    cmd.Parameters.AddWithValue("@ExpYear", expYearTB.Text);
                    cmd.Parameters.AddWithValue("@FirstName", firstNameTB.Text);
                    cmd.Parameters.AddWithValue("@LastName", lastNameTB.Text);
                    cmd.Parameters.AddWithValue("@IsProfileCC", true);
                    cmd.Parameters.AddWithValue("@Address1", address1TB.Text);
                    cmd.Parameters.AddWithValue("@Address2", address2TB.Text);
                    cmd.Parameters.AddWithValue("@City", cityTB.Text);
                    cmd.Parameters.AddWithValue("@State", stateDL.Text);
                    cmd.Parameters.AddWithValue("@Country", countryDL.Text);
                    cmd.Parameters.AddWithValue("@Zip", zipcodeTB.Text);
                    cmd.Parameters.AddWithValue("@EmailID", Session["userID"].ToString());
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