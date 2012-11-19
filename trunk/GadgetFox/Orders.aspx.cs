using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace GadgetFox
{
    public partial class Orders : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userID"] == null)
            {
                // Redirect user to login before doing anything else
                Response.Redirect("~/Login.aspx?redirect=Orders.aspx");
            }
            else if (Session["userID"] != null && Session["userRole"].Equals("1"))
            {
                // Redirect user to login before doing anything else
                Response.Redirect("~/Home.aspx");
            }

            // Connection setup
            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);
            
            // Try to connect
            try
            {
                String criteria = Request.QueryString["criteria"];

                DataTable Table1 = new DataTable("MyOrders");
                DataRow Row1 = Table1.NewRow();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = myConnection;

                if (criteria == null)
                {
                    cmd = new SqlCommand("select * from vw_CustomerOrders", myConnection); // Define SQL query
                }
                else
                {
                    cmd = new SqlCommand("select * from vw_CustomerOrders where OrderID='" + criteria + "' or EmailID='" + criteria + "'", myConnection); // Define SQL query
                }

                myConnection.Open();
                SqlDataReader dr = cmd.ExecuteReader(); // Perform SQL query and store it
                
                // Add colums for each field into the table
                // EmailID, ProductID, Name, Price, SalePrice, InSale, ImageID, ImageData 
                DataColumn oid = new DataColumn("Order ID");
                DataColumn emailId = new DataColumn("Email ID");
                DataColumn status = new DataColumn("Status");
                DataColumn purchaseDate = new DataColumn("Purchase Date");
                DataColumn products = new DataColumn("Products");
                DataColumn total = new DataColumn("Total");
                DataColumn tracking = new DataColumn("Tracking #");

                DataColumn actions = new DataColumn("#");

                oid.DataType = System.Type.GetType("System.String");
                emailId.DataType = System.Type.GetType("System.String");
                status.DataType = System.Type.GetType("System.String");
                purchaseDate.DataType = System.Type.GetType("System.String");
                products.DataType = System.Type.GetType("System.String");
                total.DataType = System.Type.GetType("System.String");
                tracking.DataType = System.Type.GetType("System.String");

                actions.DataType = System.Type.GetType("System.String");

                Table1.Columns.Add(oid);
                Table1.Columns.Add(emailId);
                Table1.Columns.Add(status);
                Table1.Columns.Add(purchaseDate);
                Table1.Columns.Add(products);
                Table1.Columns.Add(total);
                Table1.Columns.Add(tracking);

                Table1.Columns.Add(actions);

                int orderId = -1;
                System.Collections.Hashtable orders = new System.Collections.Hashtable();
                Double orderTotal = 0.00;
                while (dr.Read())
                {
                    orderId = int.Parse(dr["OrderID"].ToString());

                    // Add row when a new order is encountered
                    if (!orders.ContainsKey(orderId))
                    {
                        orders[orderId] += dr["Quantity"].ToString() + "x " + dr["Name"].ToString() + ".  ";
                        Row1 = Table1.NewRow();
                        Table1.Rows.Add(Row1);

                        // Insert values into the row from the query
                        Row1["Order ID"] = dr["OrderID"];
                        Row1["Email ID"] = dr["EmailID"];
                        Row1["Status"] = dr["Status"];
                        Row1["Purchase Date"] = DateTime.Parse(dr["PurchaseDate"].ToString()).ToShortDateString();
                        Row1["Products"] = orders[orderId];

                        orderTotal = Double.Parse(dr["OrderTotal"].ToString()) + Double.Parse(dr["TaxAmount"].ToString()) + Double.Parse(dr["ShipAmount"].ToString());
                        Row1["Total"] = string.Format("{0:$#,###.##}", orderTotal);
                        Row1["Tracking #"] = dr["TrackingID"];

                        // Columns to purchase item
                        Row1["#"] = "";  // For cancel order        
                    }
                    else
                    {
                        orders[orderId] += dr["Quantity"].ToString() + "x " + dr["Name"].ToString() + ".  ";
                        Row1["Products"] = orders[orderId];
                    }
                }
                // Add last order
                dr.Close();

                GridView1.DataSource = Table1;
                GridView1.DataBind();
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

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Create drop down list for order status
            DropDownList statusDL = new DropDownList();
            statusDL.Items.Add("Processing");
            statusDL.Items.Add("Shipped");
            statusDL.Items.Add("Canceled");

            // Create cancel order button & update tracking #
            Button cancelBtn = new Button();
            cancelBtn.Style.Add("width", "105px");
            cancelBtn.Style.Add("padding", "5px");
            cancelBtn.Style.Add("margin", "5px");
            cancelBtn.Enabled = false;
            cancelBtn.Text = "Cancel order";
            if (e.Row.RowIndex > -1)
            {
                // Add to quantity drop down list to column
                String status = e.Row.Cells[2].Text;
                e.Row.Cells[2].Controls.Add(statusDL);
                statusDL.SelectedValue = status;

                // Add buttons to column
                String oid = e.Row.Cells[0].Text;
                Literal invoiceLink = new Literal();
                invoiceLink.Text = "<a style='color: blue;' href='Invoice.aspx?oid=" + oid + "'/>" + oid + "</a>";
                e.Row.Cells[0].Controls.Add(invoiceLink);

                // Add textbox for tracking #
                String tracking = e.Row.Cells[6].Text;
                if (tracking == "&nbsp;")
                {
                    tracking = "";
                }

                TextBox trackTB = new TextBox();
                trackTB.Text = tracking;
                e.Row.Cells[6].Controls.Add(trackTB);
                trackTB.AutoPostBack = true;
                trackTB.ID = oid + "T";
                trackTB.TextChanged += new EventHandler(this.trackTB_TextChanged);

                e.Row.Cells[e.Row.Cells.Count - 1].Controls.Add(cancelBtn);

                // Pass order id & row to on-click event
                statusDL.AutoPostBack = true;
                statusDL.ID = oid + "S";
                statusDL.SelectedIndexChanged += new EventHandler(this.statusDL_SelectedIndexChanged);

                //cancelBtn.Click += new EventHandler(this.cancelBtn_Click);
                cancelBtn.CommandArgument = e.Row.RowIndex + "-" + oid;
            }
        }

        /**
         * Order tracking # changed
         */
        protected void trackTB_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            String oid = tb.ID.Replace('T', ' ').Trim();

            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);
            try
            {
                myConnection.Open();

                SqlCommand cmd1 = new SqlCommand("Select COUNT(*) from Orders where OrderID=@OrderID", myConnection);
                cmd1.Parameters.AddWithValue("@OrderID", oid);
                int idRows = (int)cmd1.ExecuteScalar();
                if (idRows == 0)
                {
                    returnLabel.Text = "Failed to update order #" + oid + ". Order does not exist!";
                }
                else
                {
                    // Update existing row
                    SqlCommand cmd2 = new SqlCommand("Update Orders set TrackingID=@TrackingID where " +
                        "OrderID=@OrderID", myConnection);
                    cmd2.Parameters.AddWithValue("@TrackingID", tb.Text);
                    cmd2.Parameters.AddWithValue("@OrderID", oid);
                    int rc = cmd2.ExecuteNonQuery();
                    if (rc == 1)
                    {
                        returnLabel.Text = "Order #" + oid + " status updated to " + tb.Text;
                    }
                }
            }
            catch (SqlException ex)
            {
                Response.Write("<script language='JavaScript'>alert('" + ex.Message + "')</script>");
            }
            finally
            {
                myConnection.Close();
            }
        }

        /**
         * Order status changed
         */
        protected void statusDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList dl = (DropDownList) sender;
            String oid = dl.ID.Replace('S', ' ').Trim();

            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);
            try
            {
                myConnection.Open();

                SqlCommand cmd1 = new SqlCommand("Select COUNT(*) from Orders where OrderID=@OrderID", myConnection);
                cmd1.Parameters.AddWithValue("@OrderID", oid);
                int idRows = (int)cmd1.ExecuteScalar();
                if (idRows == 0)
                {
                    returnLabel.Text = "Failed to update order #" + oid + ". Order does not exist!";
                }
                else
                {
                    // Update existing row
                    SqlCommand cmd2 = new SqlCommand("Update Orders set Status=@Status where " +
                        "OrderID=@OrderID", myConnection);
                    cmd2.Parameters.AddWithValue("@Status", dl.SelectedItem.Value);
                    cmd2.Parameters.AddWithValue("@OrderID", oid);
                    int rc = cmd2.ExecuteNonQuery();
                    if (rc == 1)
                    {
                        returnLabel.Text = "Order #" + oid + " status updated to " + dl.SelectedItem.Value;
                    }                    
                }                
            }
            catch (SqlException ex)
            {
                Response.Write("<script language='JavaScript'>alert('" + ex.Message + "')</script>");
            }
            finally
            {
                myConnection.Close();
            }
        }

        /**
         * Cancel order
         */
        protected void cancelBtn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            String[] args = btn.CommandArgument.ToString().Split('-');
            int row = int.Parse(args[0]);
            String pid = args[1];
        }

        /**
         * Search order
         */
        protected void Search_Click(object sender, EventArgs e)
        {
            if (criteriaTB.Text.Length > 0)
            {
                Response.Redirect("~/Orders.aspx?criteria=" + criteriaTB.Text);
            }
            else
            {
                Response.Redirect("~/Orders.aspx");
            }
        }
    }
}