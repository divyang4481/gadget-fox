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
    public partial class Checkout : System.Web.UI.Page
    {
        private int addressId = -1;
        private Double purchaseTotal = 0.00;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userID"] == null)
            {
                // Redirect user to login before doing anything else
                Response.Redirect("~/Login.aspx?redirect=ShoppingCart.aspx");
            }

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
                            String ccId = dr3["CCID"].ToString();
                            String ccType = dr3["CCType"].ToString();
                            String ccNum = dr3["CCNum"].ToString();
                            cardDL.Items.Add(new ListItem(ccType + " **" + ccNum.Substring(ccNum.Length-4, 4), ccId));
                        }
                        dr3.Close();  

                        // Find all products in shopping cart for this user
                        SqlCommand cmd4 = new SqlCommand("select * from viewCart where EmailID=@EmailID", myConnection); // Define SQL query
                        cmd4.Parameters.AddWithValue("@EmailID", Session["userID"]);
                        SqlDataReader dr4 = cmd4.ExecuteReader();

                        while (dr4.Read())
                        {
                            String pid = dr4["ProductID"].ToString();
                            int qty = int.Parse(dr4["Quantity"].ToString());
                            Double price = Double.Parse(dr4["Price"].ToString());

                            // Calculate purchase total
                            Double salePrice = 0.00;
                            if (dr4["SalePrice"].ToString().Length > 0 && Double.Parse(dr4["SalePrice"].ToString()) > 0 && dr4["SalePrice"].ToString() != "&nbsp;")
                            {
                                salePrice = Double.Parse(dr4["SalePrice"].ToString());
                                purchaseTotal = purchaseTotal + salePrice * qty;
                            }
                            else
                            {
                                purchaseTotal = purchaseTotal + price * qty;
                            }
                        }
                        dr4.Close();

                        subTotalLB.Text = string.Format("{0:$#,###.##}", purchaseTotal);
                        //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Alert", "<script>alert('subtotal = " + purchaseTotal + "')</script>");
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

        protected void checkoutBtn_Clicked(object sender, EventArgs e)
        {
            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);
            try
            {
                myConnection.Open();

                // Find all products in shopping cart for this user
                SqlCommand cmd = new SqlCommand("select * from viewCart where EmailID=@EmailID", myConnection); // Define SQL query
                cmd.Parameters.AddWithValue("@EmailID", Session["userID"]);
                SqlDataReader dr = cmd.ExecuteReader();

                System.Collections.Hashtable products = new System.Collections.Hashtable();
                
                int orderId = getNextOrderId();
                Double purchaseTotal = 0.00;
                while (dr.Read())
                {
                    String pid = dr["ProductID"].ToString();
                    int qty = int.Parse(dr["Quantity"].ToString());
                    Double price = Double.Parse(dr["Price"].ToString());

                    products.Add(pid, qty);

                    // Calculate purchase total
                    Double salePrice = 0.00;
                    if (dr["SalePrice"].ToString().Length > 0 && Double.Parse(dr["SalePrice"].ToString()) > 0 && dr["SalePrice"].ToString() != "&nbsp;")
                    {
                        salePrice = Double.Parse(dr["SalePrice"].ToString());
                        purchaseTotal = purchaseTotal + salePrice * qty;
                    }
                    else
                    {
                        purchaseTotal = purchaseTotal + price * qty;
                    }
                }  // End of dr.Read
                dr.Close();

                if (!(purchaseTotal > 0))
                {
                    returnLabel.Text = "You have no items to checkout";
                    return;
                }
                
                /**************************************************************
                 * Create purchase order
                 **************************************************************/
                String ccId = cardDL.SelectedValue;

                String[] ship = shippingDL.Text.Split(':');
                String shipType = ship[0];
                String shipAmount = ship[1].Replace('$', ' ').Trim();

                String[] card = cardDL.Text.Split(' ');
                String cardType = card[0].Trim();

                // OrderID, EmailID, Status, PurchaseDate, OrderTotal, TaxAmount, ShipAmount, ShipType, ShipAddress1, ShipAddress2, ShipCity, ShipState, ShipCountry, TrackingID, CCID, PaymentType 
                SqlCommand cmd3 = new SqlCommand("INSERT INTO [GadgetFox].[dbo].[Orders] " + 
                        "(OrderID, EmailID, Status, PurchaseDate, OrderTotal, TaxAmount, ShipAmount, ShipType, FirstName, LastName, ShipAddress1, ShipAddress2, ShipCity, ShipState, ShipCountry, ShipZip, TrackingID, CCID, PaymentType)" +
                        " VALUES(@OrderID, @EmailID, @Status, @PurchaseDate, @OrderTotal, @TaxAmount, @ShipAmount, @ShipType, @FirstName, @LastName, @ShipAddress1, @ShipAddress2, @ShipCity, @ShipState, @ShipCountry, @ShipZip, @TrackingID, @CCID, @PaymentType)", myConnection);
                cmd3.Parameters.AddWithValue("@OrderID", orderId);
                cmd3.Parameters.AddWithValue("@EmailID", Session["userID"].ToString());
                cmd3.Parameters.AddWithValue("@Status", "Processing");
                cmd3.Parameters.AddWithValue("@PurchaseDate", DateTime.Today.Date);
                cmd3.Parameters.AddWithValue("@OrderTotal", purchaseTotal);
                cmd3.Parameters.AddWithValue("@TaxAmount", Double.Parse("0.00"));
                cmd3.Parameters.AddWithValue("@ShipAmount", Double.Parse(shipAmount));
                cmd3.Parameters.AddWithValue("@ShipType", shipType);
                cmd3.Parameters.AddWithValue("@FirstName", firstNameTB.Text.Trim());
                cmd3.Parameters.AddWithValue("@LastName", lastNameTB.Text.Trim());
                cmd3.Parameters.AddWithValue("@ShipAddress1", address1TB.Text.Trim());
                cmd3.Parameters.AddWithValue("@ShipAddress2", address2TB.Text.Trim());
                cmd3.Parameters.AddWithValue("@ShipCity", cityTB.Text.Trim());
                cmd3.Parameters.AddWithValue("@ShipState", stateDL.Text.Trim());
                cmd3.Parameters.AddWithValue("@ShipCountry", countryDL.Text);
                cmd3.Parameters.AddWithValue("@ShipZip", zipcodeTB.Text);
                cmd3.Parameters.AddWithValue("@TrackingID", "");
                cmd3.Parameters.AddWithValue("@CCID", ccId);
                cmd3.Parameters.AddWithValue("@PaymentType", cardType);
                int rc3 = cmd3.ExecuteNonQuery();
                if (rc3 > 0)
                {
                    returnLabel.Text = "Your order is being processed";
                    checkoutBtn.Visible = false;
                    cancelBtn.Text = "Close";
                }
                else
                {
                    returnLabel.Text = "Your order could not be processed. Please try again later!";
                    return;
                }

                /**************************************************************
                 * Create orders
                 **************************************************************/
                if (purchaseTotal > 0)
                {
                    foreach (String pid in products.Keys)
                    {
                        /**************************************************************
                         * Update product quantity
                         **************************************************************/
                        // Update if exists
                        SqlCommand cmd7 = new SqlCommand("Update Products set Quantity=(Quantity - @Quantity) where " +
                            "ProductID=@ProductID", myConnection);
                        cmd7.Parameters.AddWithValue("@Quantity", products[pid]);
                        cmd7.Parameters.AddWithValue("@ProductID", pid);
                        int rc7 = cmd7.ExecuteNonQuery();
                        if (rc7 != 1)  // One row should have been updated
                        {
                            // Handle error?
                        }

                        SqlCommand cmd2 = new SqlCommand("Select COUNT(*) from [GadgetFox].[dbo].[OrderedProducts] where " +
                            "OrderID=@OrderID and ProductID=@ProductID", myConnection);
                        cmd2.Parameters.AddWithValue("@OrderID", orderId);
                        cmd2.Parameters.AddWithValue("@ProductID", pid);
                        int idRows = (int)cmd2.ExecuteScalar();
                        if (idRows != 0)
                        {
                            // Update if exists
                            SqlCommand cmd4 = new SqlCommand("Update viewCart set Quantity=@Quantity where " +
                                "OrderID=@OrderID and ProductID=@ProductID", myConnection);
                            cmd4.Parameters.AddWithValue("@Quantity", products[pid]);
                            cmd4.Parameters.AddWithValue("@OrderID", orderId);
                            cmd4.Parameters.AddWithValue("@ProductID", pid);
                            int rc4 = cmd4.ExecuteNonQuery();
                            if (rc4 == 1)  // One row should have been updated
                            {
                                returnLabel.Text = "Your items were updated in the cart";
                            }
                        }
                        else
                        {
                            // Insert otherwise
                            SqlCommand cmd5 = new SqlCommand("INSERT INTO [GadgetFox].[dbo].[OrderedProducts] (OrderID,ProductID,Quantity)" +
                                " VALUES(@OrderID,@ProductID,@Quantity)", myConnection);
                            cmd5.Parameters.AddWithValue("@OrderID", orderId);
                            cmd5.Parameters.AddWithValue("@ProductID", pid);
                            cmd5.Parameters.AddWithValue("@Quantity", products[pid]);
                            int rc5 = cmd5.ExecuteNonQuery();
                            if (rc5 != 1)  // One row should have been inserted
                            {
                                returnLabel.Text = "Failed to checkout. Please try again later!";
                                checkoutBtn.Visible = false;
                                cancelBtn.Text = "Close";
                                return;
                            }
                        }
                    }
                }  // End of if purchaseTotal

                /**************************************************************
                 * Delete item in cart since it is on order
                 **************************************************************/
                SqlCommand cmd6 = new SqlCommand("Delete from [GadgetFox].[dbo].[Carts] where " +
                    "EmailID=@EmailID", myConnection);
                cmd6.Parameters.AddWithValue("@EmailID", Session["userID"].ToString());
                int rc = cmd6.ExecuteNonQuery();

                totalLB.Text = string.Format("{0:$#,###.##}", purchaseTotal + Double.Parse(shipAmount));
                
                // Send to invoice page based on order
                Response.Redirect("~/Invoice.aspx?oid=" + orderId);
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

        /**
         * Generate the next order Id
         */
        public int getNextOrderId()
        {
            int nextId = 0;
            int id;
            String conStr = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection con = new SqlConnection(conStr);

            SqlCommand cmd = new SqlCommand("Select OrderID from [GadgetFox].[dbo].[Orders]", con);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                id = Convert.ToInt16(dr["OrderID"].ToString());
                if (id > nextId)
                {
                    nextId = id;
                }
            }
            con.Close();

            return nextId + 1;
        }

        /**
         * Update total after shipping is selected
         */
        protected void shippingDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            String[] ship = shippingDL.Text.Split(':');
            if (ship.Length < 1)
                return;

            String shipType = ship[0];
            String shipAmount = ship[1].Replace('$', ' ').Trim();

            if (subTotalLB.Text.Length > 0)
                purchaseTotal = Double.Parse(subTotalLB.Text.Replace('$', ' ').Trim()); 
            
            Double total = purchaseTotal + Double.Parse(shipAmount);
            
            totalLB.Text = string.Format("{0:$#,###.##}", total);
        }
    }
}