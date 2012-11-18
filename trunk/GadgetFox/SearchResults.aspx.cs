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
    public partial class SearchResults : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Connection setup
            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);

            // Read the URL parameter
            string subcategory = "";
            String qsubcat = Request.QueryString["subcategory"];

            string category = "";
            String qcat = Request.QueryString["category"];

            // Read the URL parameter
            string searchcriteria = "";
            String qsearchcriteria = Request.QueryString["searchcriteria"];

            string search_criteria = "";
            // Set subquery information
            if (qsubcat != null || qcat != null || qsearchcriteria != null)
            {

                if (qsubcat != null)
                {
                    subcategory = Request.QueryString["subcategory"].ToString(); //read the url parameter
                    search_string.Text = subcategory;
                    search_criteria = "subcategoryname = '" + subcategory + "'";
                }

                if (qcat != null)
                {
                    category = Request.QueryString["category"].ToString(); //read the url parameter
                    search_string.Text = category;
                    search_criteria = "categoryname = '" + category + "'";
                }


                // Declare a table to store rows
                DataTable Table1 = new DataTable("SearchResults");
                //setup a row
                DataRow Row1;

                // Try to connect
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = myConnection;
                    if (qsearchcriteria != null)
                    {
                        searchcriteria = Request.QueryString["searchcriteria"].ToString(); // Read the url parameter

                        cmd.CommandText = "sp_SearchProduct";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@SearchString", searchcriteria);

                    }
                    else
                    {
                        cmd = new SqlCommand("select * from vw_ProductDetails where " + search_criteria, myConnection); // Define SQL query
                    }

                    myConnection.Open();
                    SqlDataReader dr = cmd.ExecuteReader(); // Perform SQL query and store it

                    // Add colums for each field into the table
                    DataColumn pid = new DataColumn("Product ID");
                    DataColumn image = new DataColumn("Image");
                    DataColumn name = new DataColumn("Name");
                    DataColumn description = new DataColumn("Description");
                    DataColumn price = new DataColumn("Price");
                    // DataColumn sale_price = new DataColumn("Sale Price");
                    DataColumn qty = new DataColumn("Quantity");
                    DataColumn actions = new DataColumn("#");

                    pid.DataType = System.Type.GetType("System.String");
                    image.DataType = System.Type.GetType("System.String");
                    name.DataType = System.Type.GetType("System.String");
                    description.DataType = System.Type.GetType("System.String");
                    price.DataType = System.Type.GetType("System.String");
                    // sale_price.DataType = System.Type.GetType("System.Double");

                    qty.DataType = System.Type.GetType("System.String");
                    actions.DataType = System.Type.GetType("System.String");

                    Table1.Columns.Add(pid);
                    Table1.Columns.Add(image);
                    Table1.Columns.Add(name);
                    Table1.Columns.Add(description);
                    Table1.Columns.Add(price);
                    // Table1.Columns.Add(sale_price);

                    Table1.Columns.Add(qty);
                    Table1.Columns.Add(actions);

                    while (dr.Read())
                    {
                        Row1 = Table1.NewRow();

                        // Insert values into the row from the query
                        Row1["Product ID"] = dr["ProductID"];
                        Row1["Image"] = dr["ImageID"];
                        Row1["Name"] = dr["Name"];
                        Row1["Description"] = dr["Description"];
                        Row1["Price"] = "$" + dr["Price"].ToString();

                        // Change price if product is on sale
                        if (Boolean.Parse(dr["InSale"].ToString()) && Double.Parse(dr["SalePrice"].ToString()) > 0)
                        {
                            Row1["Price"] = dr["SalePrice"].ToString() + "/" + dr["Price"].ToString();
                        }

                        // Columns to purchase item
                        Row1["Quantity"] = "";
                        Row1["#"] = "";  // For add to cart and wishlist buttons 

                        Table1.Rows.Add(Row1);
                    }

                    dr.Close();

                }
                catch (SqlException ex)
                {
                    Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('" + ex.Message + "')</SCRIPT>");
                }
                finally
                {
                    myConnection.Close();
                }

                GridView1.DataSource = Table1;
                GridView1.DataBind();
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Create drop down list for product quantity
            DropDownList qtyDL = new DropDownList();
            qtyDL.Items.Add("1");
            qtyDL.Items.Add("2");
            qtyDL.Items.Add("3");
            qtyDL.Items.Add("4");
            qtyDL.Items.Add("5");

            // Create add to cart and add to wishlist buttons
            Button add2CartBtn = new Button();
            add2CartBtn.Style.Add("width", "105px");
            add2CartBtn.Style.Add("padding", "5px");
            add2CartBtn.Style.Add("margin", "5px");
            add2CartBtn.Text = "Add to cart";

            Button add2WishlistBtn = new Button();
            add2WishlistBtn.Style.Add("padding", "5px");
            add2WishlistBtn.Style.Add("margin", "5px");
            add2WishlistBtn.Text = "Add to wishlist";

            // Add edit & remove product info if not customer
            Button editProductInfoBtn = new Button();
            editProductInfoBtn.Style.Add("padding", "5px");
            editProductInfoBtn.Style.Add("margin", "5px");
            editProductInfoBtn.Text = "Edit info";

            Button deleteProductBtn = new Button();
            deleteProductBtn.Style.Add("padding", "5px");
            deleteProductBtn.Style.Add("margin", "5px");
            deleteProductBtn.Text = "Delete product";
            
            if (e.Row.RowIndex > -1)
            {
                // Add buttons to column
                String pid = e.Row.Cells[0].Text;
                if (Session["userID"] != null && !Session["userRole"].Equals("1"))
                {
                    e.Row.Cells[e.Row.Cells.Count - 1].Controls.Add(editProductInfoBtn);
                    e.Row.Cells[e.Row.Cells.Count - 1].Controls.Add(deleteProductBtn);

                    // Add logic to delete product
                    deleteProductBtn.Enabled = false;
                }
                else
                {
                    e.Row.Cells[e.Row.Cells.Count - 1].Controls.Add(add2CartBtn);
                    e.Row.Cells[e.Row.Cells.Count - 1].Controls.Add(add2WishlistBtn);
                }
                
                
                qtyDL.ID = pid + "_qty";

                // Add to quantity drop down list to column
                e.Row.Cells[e.Row.Cells.Count - 2].Controls.Add(qtyDL);

                // Insert product image
                String imgId = e.Row.Cells[1].Text;
                Literal img = new Literal();
                img.Text = "<img height='100px' width='100px' src='Image.aspx?ImageID=" + imgId + "'/>";
                e.Row.Cells[1].Controls.Add(img);

                // If product is on sale, stike out price and use sale price
                String priceStr = e.Row.Cells[4].Text;
                if (priceStr.Contains('/'))
                {
                    // Sale price/regular price
                    String[] prices = priceStr.Split('/');
                    Literal p = new Literal();
                    p.Text = "<strike>$" + prices[1] + "</strike><br/>$" + prices[0];
                    e.Row.Cells[4].Controls.Add(p);
                }

                // Pass product id & row to on-click event
                add2CartBtn.Click += new EventHandler(this.addProduct2CartBtn_Click);
                add2CartBtn.CommandArgument = e.Row.RowIndex + "-" + pid;

                add2WishlistBtn.Click += new EventHandler(this.addProduct2WishlistBtn_Click);
                add2WishlistBtn.CommandArgument = e.Row.RowIndex + "-" + pid;

                editProductInfoBtn.Click += new EventHandler(this.editProductInfoBtn_Click);
                editProductInfoBtn.CommandArgument = e.Row.RowIndex + "-" + pid;

                deleteProductBtn.Click += new EventHandler(thisdeleteProductBtn_Click);
                deleteProductBtn.CommandArgument = e.Row.RowIndex + "-" + pid;
            }
        }

        /**
         * Delete product
         */
        protected void thisdeleteProductBtn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            String[] args = btn.CommandArgument.ToString().Split('-');
            int row = int.Parse(args[0]);
            String pid = args[1];
        }

        /**
         * Edit product info
         */
        protected void editProductInfoBtn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            String[] args = btn.CommandArgument.ToString().Split('-');
            int row = int.Parse(args[0]);
            String pid = args[1];

            Response.Redirect("~/UpdateProductInformation.aspx?pid=" + pid);
        }

        /**
         * Add product to shopping cart
         */
        protected void addProduct2CartBtn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            String[] args = btn.CommandArgument.ToString().Split('-');
            int row = int.Parse(args[0]);
            String pid = args[1];

            // Find quantity drop down list
            DropDownList qtyDL = (DropDownList)GridView1.Rows[row].Cells[5].FindControl(pid + "_qty");
            int qty = int.Parse(qtyDL.SelectedValue);
            if (qty < 1)
                return;

            if (Session["userID"] == null)
            {
                // Redirect user to login before doing anything else
                Response.Redirect("~/Login.aspx?redirect=" + HttpContext.Current.Request.Url.AbsoluteUri);
            }
            else
            {
                // Write to orders
                String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                SqlConnection myConnection = new SqlConnection(myConnectionString);
                try
                {
                    myConnection.Open();

                    SqlCommand cmd1 = new SqlCommand("Select COUNT(*) from Carts where EmailID=@EmailID and ProductID=@ProductID", myConnection);
                    cmd1.Parameters.AddWithValue("@EmailID", Session["userID"]);
                    cmd1.Parameters.AddWithValue("@ProductID", pid);
                    int idRows = (int)cmd1.ExecuteScalar();
                    if (idRows == 0)
                    {
                        // Insert new row
                        SqlCommand cmd = new SqlCommand("INSERT INTO [GadgetFox].[dbo].[Carts] VALUES(@EmailID,@ProductID,@Quantity)", myConnection);
                        cmd.Parameters.AddWithValue("@EmailID", Session["userID"]);
                        cmd.Parameters.AddWithValue("@ProductID", pid);
                        cmd.Parameters.AddWithValue("@Quantity", qty);

                        int rc = cmd.ExecuteNonQuery();
                        if (rc == 1)
                        {
                            returnLabel.Text = "Your items were added to the cart";
                        }
                    }
                    else
                    {
                        // Update existing row
                        SqlCommand cmd2 = new SqlCommand("Update Carts set Quantity=@Quantity where " +
                            "EmailID=@EmailID and ProductID=@ProductID", myConnection);
                        cmd2.Parameters.AddWithValue("@Quantity", qty);
                        cmd2.Parameters.AddWithValue("@EmailID", Session["userID"]);
                        cmd2.Parameters.AddWithValue("@ProductID", pid);
                        int rc = cmd2.ExecuteNonQuery();
                        if (rc == 1)
                        {
                            returnLabel.Text = "Your items were updated in the cart";
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
        }

        /** 
         * Add product to wishlist 
         */
        protected void addProduct2WishlistBtn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            String[] args = btn.CommandArgument.ToString().Split('-');
            int row = int.Parse(args[0]);
            String pid = args[1];

            // Write to wishlist
            if (Session["userID"] == null)
            {
                // Redirect user to login before doing anything else
                Response.Redirect("~/Login.aspx?redirect=" + HttpContext.Current.Request.Url.AbsoluteUri);
            }
            else
            {

                String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                SqlConnection myConnection = new SqlConnection(myConnectionString);
                try
                {
                    myConnection.Open();

                    SqlCommand cmd1 = new SqlCommand("Select COUNT(*) from WishLists where EmailID=@EmailID and ProductID=@ProductID", myConnection);
                    cmd1.Parameters.AddWithValue("@EmailID", Session["userID"]);
                    cmd1.Parameters.AddWithValue("@ProductID", pid);
                    int idRows = (int)cmd1.ExecuteScalar();
                    if (idRows == 0)
                    {
                        // Insert new row
                        SqlCommand cmd = new SqlCommand("INSERT INTO [GadgetFox].[dbo].[WishLists] VALUES(@EmailID,@ProductID,@WishListName,@UpdatedDate)", myConnection);
                        cmd.Parameters.AddWithValue("@EmailID", Session["userID"]);
                        cmd.Parameters.AddWithValue("@ProductID", pid);
                        cmd.Parameters.AddWithValue("@WishListName", "");
                        cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Today.Date);

                        int rc = cmd.ExecuteNonQuery();
                        if (rc == 1)
                        {
                            returnLabel.Text = "Your items were added to the wishlist";
                        }
                    }
                    else
                    {
                        returnLabel.Text = "Your items are already in the wishlist";
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
        }
    }
}