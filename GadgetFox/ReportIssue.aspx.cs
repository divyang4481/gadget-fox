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
    public partial class ReportIssue : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userID"] == null)
            {
                // Redirect user to login before doing anything else
                Response.Redirect("~/Login.aspx?redirect=ReportIssue.aspx");
            }
            else if (Session["userID"] != null && Session["userRole"].Equals("1"))
            {
                // Redirect user to login before doing anything else
                Response.Redirect("~/Home.aspx");
            }

            if (!IsPostBack)
            {
                // Set customer Id and issue Id
                emailId.Text = Session["userID"].ToString();
                issueId.Text = getNextIssueId().ToString();
            }
        }

        /**
         * Generate the next issue Id
         */
        public int getNextIssueId()
        {
            int nextId = 0;
            int id;
            String conStr = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection con = new SqlConnection(conStr);
            
            SqlCommand cmd = new SqlCommand("Select IssueID from [GadgetFox].[dbo].[Issues]", con);
            
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                id = Convert.ToInt16(dr["IssueID"].ToString());
                if (id > nextId)
                {
                    nextId = id;
                }
            }
            con.Close();

            return nextId + 1;
        }

        /**
         * Verify order Id
         */
        public Boolean isOrderIdValid(int id, string emailId)
        {
            int fId = -1;
            string fEmailId = "";

            String conStr = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection con = new SqlConnection(conStr);

            SqlCommand cmd = new SqlCommand("Select OrderID,EmailID from [GadgetFox].[dbo].[Orders] where OrderID=@OrderID", con);
            cmd.Parameters.AddWithValue("@OrderID", id.ToString());

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                fId = Convert.ToInt16(dr["OrderID"].ToString());
                fEmailId = dr["EmailID"].ToString();
            }
            con.Close();

            if (fId > -1 && fEmailId != "")
            {
                return true;
            }

            return false;
        }

        protected void saveButton_Clicked(object sender, EventArgs e)
        {
            // Verify all fields are entered
            if (emailId.Text == String.Empty ||
                issueId.Text == String.Empty ||
                issueType.Text.Equals("-- Select --") ||
                issueDescription.Text == String.Empty ||
                orderId.Text == String.Empty)
            {
                returnLabel.Text = "Please provide the missing field(s)!";
                return;
            }

            // Do not continue if order is not valid
            if (!isOrderIdValid(Convert.ToInt16(orderId.Text), emailId.Text))
            {
                returnLabel.Text = "No order matching this order Id belongs to you!";
                return;
            }
            
            String conStr = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection con = new SqlConnection(conStr);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO [GadgetFox].[dbo].[Issues] ([EmailID],[IssueID],[IssueType],[IssueDescription],[Status],[OrderID])" +
                    " VALUES(@EmailID,@IssueID,@IssueType,@IssueDescription,@Status,@OrderID)", con);
                cmd.Parameters.AddWithValue("@EmailID", emailId.Text);
                cmd.Parameters.AddWithValue("@IssueID", issueId.Text);
                cmd.Parameters.AddWithValue("@IssueType", issueType.Text);
                cmd.Parameters.AddWithValue("@IssueDescription", issueDescription.Text);
                cmd.Parameters.AddWithValue("@Status", "Pending");  // New issues have status = Pending 
                cmd.Parameters.AddWithValue("@OrderID", orderId.Text);
                int rc = cmd.ExecuteNonQuery();
                if (rc > 0)
                {
                    returnLabel.Text = "Your issue was successfully submitted";
                    saveButton.Visible = false;
                    cancelButton.Text = "Close";
                }
                else
                {
                    returnLabel.Text = "Failed to submit your issue. Please try again later!";
                }
            }
            catch (SqlException ex)
            {                
                Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('" + ex.Message + "')</SCRIPT>");
            }
            finally
            {
                con.Close();
            }
        }

        protected void cancelButton_Clicked(object sender, EventArgs e)
        {
            
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}