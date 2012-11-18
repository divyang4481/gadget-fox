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
    public partial class Invoice : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userID"] == null)
            {
                // Redirect user to login before doing anything else
                Response.Redirect("~/Login.aspx?redirect=" + HttpContext.Current.Request.Url.AbsoluteUri);
            }

            // Get order ID to get invoice for
            String reqOid = Request.QueryString["oid"];
            if (reqOid == null)
            {
                // Do not continue if no order Id is given
                return;
            }

            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);

            try
            {
                myConnection.Open();

                /**
                 * Find product orders 
                 */
                orderIdLB.Text = "Order ID: " + reqOid;

                DataTable Table1 = new DataTable("MyOrders");
                DataRow Row1 = Table1.NewRow();
                SqlCommand cmd4 = new SqlCommand("select * from vw_CustomerOrders where EmailID=@EmailID and OrderID=@OrderID", myConnection); //define SQL query
                cmd4.Parameters.AddWithValue("@EmailID", Session["userID"]);
                cmd4.Parameters.AddWithValue("@OrderID", reqOid);

                SqlDataReader dr4 = cmd4.ExecuteReader();

                // Add colums for each field into the table
                DataColumn name = new DataColumn("Name");
                DataColumn quantity = new DataColumn("Quantity");
                DataColumn price = new DataColumn("Price");                

                name.DataType = System.Type.GetType("System.String");
                quantity.DataType = System.Type.GetType("System.String");
                price.DataType = System.Type.GetType("System.String");
                
                Table1.Columns.Add(name);
                Table1.Columns.Add(quantity);
                Table1.Columns.Add(price);

                String iShipAddress = null;
                String iPurchaseDate = null;
                String iShipType = null;
                String iPaymentType = null;

                // Order total breakdown
                Double iSubTotal = 0.00;
                String iTax = null;
                String iShippingTotal = null;
                String iTotal = null;

                while (dr4.Read())
                {
                    Row1 = Table1.NewRow();
                    Table1.Rows.Add(Row1);

                    // Insert values into the row from the query
                    Row1["Name"] = dr4["Name"].ToString();
                    Row1["Quantity"] = dr4["Quantity"].ToString();
                    Row1["Price"] = "$" + string.Format("{0:$#,###.##}", dr4["Price"].ToString());


                    if (Boolean.Parse(dr4["InSale"].ToString()) && Double.Parse(dr4["SalePrice"].ToString()) > 0)
                    {
                        Row1["Price"] = "$" + string.Format("{0:$#,###.##}", dr4["SalePrice"].ToString());
                    }

                    // Format shipping address
                    if (iShipAddress == null)
                    {
                        iShipAddress = "<br/><br/>" + dr4["FirstName"].ToString() + " " + dr4["LastName"].ToString();
                        iShipAddress += "<br/>" + dr4["ShipAddress1"].ToString();
                        if (dr4["ShipAddress2"].ToString().Length > 0)
                        {
                            iShipAddress += "<br/>" + dr4["ShipAddress2"].ToString();
                        }
                        iShipAddress += "<br/>" + dr4["ShipCity"].ToString() + ", " + dr4["ShipState"].ToString() + " " + dr4["ShipZip"].ToString();
                    }

                    if (iPurchaseDate == null)
                    {
                        iPurchaseDate = dr4["PurchaseDate"].ToString();
                    }

                    if (iShipType == null)
                    {
                        iShipType = dr4["ShipType"].ToString();
                    }

                    if (iPaymentType == null)
                    {
                        iPaymentType = dr4["CCType"].ToString();
                    }

                    if (iTotal == null)
                    {
                        iTax = dr4["TaxAmount"].ToString();
                        iShippingTotal = dr4["ShipAmount"].ToString();
                        iTotal = dr4["OrderTotal"].ToString();
                    }
                }
                // Add last order
                dr4.Close();

                shipLB.Text = iShipAddress;
                purchaseDateLB.Text = iPurchaseDate;
                shipTypeLB.Text = "Shipped via " + iShipType;
                paymentLB.Text = "Paid with " + iPaymentType;

                // What happens when the product was on sale and is now no longer on sale? The sub-total is not accurate!
                subTotalLB.Text = string.Format("{0:$#,###.##}", Double.Parse(iTotal.ToString()));
                taxLB.Text = "$" + iTax;
                shipTotalLB.Text = string.Format("{0:$#,###.##}", Double.Parse(iShippingTotal)); 
                Double total = Double.Parse(iTotal) + Double.Parse(iTax) + Double.Parse(iShippingTotal);
                totalLB.Text = string.Format("{0:$#,###.##}", total);

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

        }
    }
}