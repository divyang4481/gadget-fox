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
    public partial class MyAccount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userID"] == null)
                {
                    // Redirect user to login before doing anything else
                    Response.Redirect("~/Login.aspx?redirect=MyAccount.aspx");
                }
                else
                {
                    String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                    SqlConnection myConnection = new SqlConnection(myConnectionString);
         
                    try
                    {
                        myConnection.Open();

                        /**
                         * Find default address 
                         */
                        SqlCommand cmd = new SqlCommand("Select * from [GadgetFox].[dbo].[Addresses], [GadgetFox].[dbo].[ZipCodes] where Addresses.Zip = ZipCodes.Zip and EmailID=@EmailID and IsProfileAddress=@IsProfileAddress", myConnection);
                        cmd.Parameters.AddWithValue("@EmailID", Session["userID"].ToString());
                        cmd.Parameters.AddWithValue("@IsProfileAddress", true);
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            addressLB.Text = dr["Address Line1"].ToString() + "<br/>";
                            if (dr["Address Line2"].ToString().Length > 0)
                                addressLB.Text += dr["Address Line2"].ToString() + "<br/>";

                            addressLB.Text += dr["City"].ToString() + ", " + dr["State"].ToString() + " " + dr["Zip"].ToString();
                        }
                        dr.Close();

                        /** 
                         * Find first and last name
                         */
                        SqlCommand cmd2 = new SqlCommand("Select * from [GadgetFox].[dbo].[Users] where EmailID=@EmailID", myConnection);
                        cmd2.Parameters.AddWithValue("@EmailID", Session["userID"].ToString());
                        SqlDataReader dr2 = cmd2.ExecuteReader();
                        if (dr2.Read())
                        {                            
                            nameLB.Text = dr2["FirstName"].ToString() + " " + dr2["LastName"].ToString();
                            birthdayLB.Text = DateTime.Parse(dr2["DOB"].ToString()).ToShortDateString();
                        }
                        dr2.Close();  
                      
                        /** 
                         * Find default credit card 
                         */
                        SqlCommand cmd3 = new SqlCommand("Select * from [GadgetFox].[dbo].[CCDetails] where EmailID=@EmailID", myConnection);
                        cmd3.Parameters.AddWithValue("@EmailID", Session["userID"].ToString());
                        SqlDataReader dr3 = cmd3.ExecuteReader();
                        if (dr3.Read())
                        {
                            // Only display last 4 digits
                            String ccType = dr3["CCType"].ToString();
                            String ccNum = dr3["CCNum"].ToString();
                            ccLB.Text = ccType + " ****" + ccNum.Substring(ccNum.Length - 4, 4);
                            ccExpDateLB.Text = dr3["ExpMonth"].ToString() + "/" + dr3["ExpYear"].ToString();
                        }
                        dr3.Close();

                        /**
                         * Find product orders 
                         */
                        DataTable Table1 = new DataTable("MyOrders");
                        DataRow Row1 = Table1.NewRow();
                        SqlCommand cmd4 = new SqlCommand("select * from vw_CustomerOrders where EmailID=@EmailID", myConnection); //define SQL query
                        cmd4.Parameters.AddWithValue("@EmailID", Session["userID"]);

                        SqlDataReader dr4 = cmd4.ExecuteReader();

                        // Add colums for each field into the table
                        // EmailID, ProductID, Name, Price, SalePrice, InSale, ImageID, ImageData 
                        DataColumn oid = new DataColumn("Order ID");
                        DataColumn status = new DataColumn("Status");
                        DataColumn purchaseDate = new DataColumn("Purchase Date");
                        DataColumn products = new DataColumn("Products");
                        // DataColumn shipType = new DataColumn("Shipping");
                        // DataColumn productName = new DataColumn("Product Name");
                        // DataColumn quantity = new DataColumn("Quantity");
                        DataColumn total = new DataColumn("Total");

                        DataColumn actions = new DataColumn("#");

                        oid.DataType = System.Type.GetType("System.String");
                        status.DataType = System.Type.GetType("System.String");
                        purchaseDate.DataType = System.Type.GetType("System.String");
                        products.DataType = System.Type.GetType("System.String");
                        total.DataType = System.Type.GetType("System.String");

                        actions.DataType = System.Type.GetType("System.String");

                        Table1.Columns.Add(oid);
                        Table1.Columns.Add(status);
                        Table1.Columns.Add(purchaseDate);
                        Table1.Columns.Add(products);
                        Table1.Columns.Add(total);

                        Table1.Columns.Add(actions);

                        int orderId = -1;
                        System.Collections.Hashtable orders = new System.Collections.Hashtable();
                        while (dr4.Read())
                        {
                            orderId = int.Parse(dr4["OrderID"].ToString());
                            
                            // Add row when a new order is encountered
                            if (!orders.ContainsKey(orderId))
                            {
                                orders[orderId] += dr4["Quantity"].ToString() + "x " + dr4["Name"].ToString() + ".  ";
                                Row1 = Table1.NewRow();
                                Table1.Rows.Add(Row1);

                                // Insert values into the row from the query
                                Row1["Order ID"] = dr4["OrderID"];
                                Row1["Status"] = dr4["Status"];
                                Row1["Purchase Date"] = DateTime.Parse(dr4["PurchaseDate"].ToString()).ToShortDateString();
                                Row1["Products"] = orders[orderId];
                                Row1["Total"] = "$" + string.Format("{0:$#,###.##}", dr4["OrderTotal"].ToString());

                                // Columns to purchase item
                                Row1["#"] = "";  // For cancel order        
                            }
                            else
                            {
                                orders[orderId] += dr4["Quantity"].ToString() + "x " + dr4["Name"].ToString() + ".  ";
                                Row1["Products"] = orders[orderId];
                            }
                        }
                        // Add last order
                        dr4.Close();

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
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Create cancel order button
            Button cancelBtn = new Button();
            cancelBtn.Style.Add("width", "105px");
            cancelBtn.Style.Add("padding", "5px");
            cancelBtn.Style.Add("margin", "5px");
            cancelBtn.Enabled = false;
            cancelBtn.Text = "Cancel order";
            if (e.Row.RowIndex > -1)
            {
                // Add buttons to column
                String pid = e.Row.Cells[0].Text;
                e.Row.Cells[e.Row.Cells.Count - 1].Controls.Add(cancelBtn);

                // Pass order id & row to on-click event
                //cancelBtn.Click += new EventHandler(this.cancelBtn_Click);
                cancelBtn.CommandArgument = e.Row.RowIndex + "-" + pid;
            }
        }
    }
}