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
                        myConnection.Open();
                        SqlCommand cmd = new SqlCommand("Select * from [GadgetFox].[dbo].[CCDetails] where EmailID=@EmailID", myConnection);
                        cmd.Parameters.AddWithValue("@EmailID", Session["userID"].ToString());
                      //  cmd.Parameters.AddWithValue("@IsProfileAddress", true);
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
                myConnection.Open();
                SqlCommand cmd2 = new SqlCommand("INSERT INTO [GadgetFox].[dbo].[CCDetails] ([CCNum],[ExpMonth],[ExpYear],[CVV])" +
                " VALUES(@CCnum,@ExpMonth,@ExpYear)", myConnection);
                cmd2.Parameters.AddWithValue("@CCNum", cardNumberTB.Text);
                cmd2.Parameters.AddWithValue("@ExpMonth", expMonthTB.Text);
                cmd2.Parameters.AddWithValue("@ExpYear", expYearTB.Text);
                cmd2.Parameters.AddWithValue("@CVV", cvvNumberTB.Text);
                SqlCommand cmd = new SqlCommand("Update CCDetails set CCNum=@CCNum, ExpMonth=@ExpMonth, ExpYear=@ExpYear, CVV=@CVV where " +
                    "EmailID=@EmailID", myConnection);
                cmd.Parameters.AddWithValue("@CCNum", cardNumberTB.Text);
                cmd.Parameters.AddWithValue("@ExpMonth", expMonthTB.Text);
                cmd.Parameters.AddWithValue("@ExpYear", expYearTB.Text);
                cmd.Parameters.AddWithValue("@CVV", cvvNumberTB.Text);
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

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}