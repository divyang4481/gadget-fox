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
    public partial class Wishlist : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userID"] == null)
            {
                // Redirect user to login before doing anything else
                Response.Redirect("~/Login.aspx?redirect=Wishlist.aspx");
                return;
            }

            //connection setup
            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);

            //declare a table to store rows
            DataTable Table1 = new DataTable("Wishlist");
            //setup a row
            DataRow Row1;

            //try to connect
            try
            {
                myConnection.Open(); //open connection
                SqlCommand cmd = new SqlCommand("select * from vw_Wishlist where EmailID=@EmailID", myConnection); //define SQL query
                cmd.Parameters.AddWithValue("@EmailID", Session["userID"]);
                
                SqlDataReader dr = cmd.ExecuteReader(); 

                //add colums for each field into the table
                //EmailID, ProductID, Name, Price, SalePrice, InSale, ImageID, ImageData 
                DataColumn pid = new DataColumn("Product ID");
                DataColumn image = new DataColumn("Image");                    
                DataColumn name = new DataColumn("Name");
                DataColumn price = new DataColumn("Price");
                DataColumn sale_price = new DataColumn("Sale Price");
                DataColumn actions = new DataColumn("#");

                pid.DataType = System.Type.GetType("System.String");
                image.DataType = System.Type.GetType("System.String");
                name.DataType = System.Type.GetType("System.String");
                price.DataType = System.Type.GetType("System.Double");
                sale_price.DataType = System.Type.GetType("System.Double");

                actions.DataType = System.Type.GetType("System.String");

                Table1.Columns.Add(pid);
                Table1.Columns.Add(image);
                Table1.Columns.Add(name);
                Table1.Columns.Add(price);
                Table1.Columns.Add(sale_price);

                Table1.Columns.Add(actions);

                while (dr.Read())
                {
                    Row1 = Table1.NewRow();

                    //insert values into the row from the query
                    Row1["Product ID"] = dr["ProductID"];
                    Row1["Image"] = dr["ImageID"];
                    Row1["Name"] = dr["Name"];
                    Row1["Price"] = dr["Price"];
                    Row1["Sale Price"] = dr["SalePrice"];

                    //columns to purchase item
                    Row1["#"] = "";  //for add to cart and wishlist buttons 

                    //add row
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

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //create delete from wishlist and add to cart button
            Button deleteBtn = new Button();
            deleteBtn.Style.Add("padding", "5px");
            deleteBtn.Style.Add("margin", "5px");
            deleteBtn.Text = "Delete";

            Button add2cartBtn = new Button();
            add2cartBtn.Style.Add("padding", "5px");
            add2cartBtn.Style.Add("margin", "5px");
            add2cartBtn.Text = "Add to cart";

            if (e.Row.RowIndex > -1)
            {
                //add buttons to column
                String pid = e.Row.Cells[0].Text;
                e.Row.Cells[e.Row.Cells.Count - 1].Controls.Add(deleteBtn);
                e.Row.Cells[e.Row.Cells.Count - 1].Controls.Add(add2cartBtn);

                //insert product image
                String imgId = e.Row.Cells[1].Text;
                Literal img = new Literal();
                img.Text = "<img height='80px' width='80px' src='Image.aspx?ImageID=" + imgId + "'/>";
                e.Row.Cells[1].Controls.Add(img);
                                
                //pass product id to on-click event
                deleteBtn.Click += new EventHandler(this.deleteBtn_Click);
                deleteBtn.CommandArgument = e.Row.RowIndex + "-" + pid;

                add2cartBtn.Click += new EventHandler(this.add2cartBtn_Click);
                add2cartBtn.CommandArgument = e.Row.RowIndex + "-" + pid;
            }
        }

        /**
         * Add product to shopping cart
         */
        protected void add2cartBtn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            String[] args = btn.CommandArgument.ToString().Split('-');
            int row = int.Parse(args[0]);
            String pid = args[1];

            //default quantity to 1
            int qty = 1;

            //write to orders
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
                    //insert new row
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
                    //update existing row
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

            Response.Redirect("~/Wishlist.aspx");
        }

        /**
         * Delete item from wishlist on click         
         */
        protected void deleteBtn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            String[] args = btn.CommandArgument.ToString().Split('-');
            int row = int.Parse(args[0]);
            String pid = args[1];
            
            //write to view
            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);
            try
            {
                myConnection.Open();

                SqlCommand cmd1 = new SqlCommand("Select COUNT(*) from Wishlists where EmailID=@EmailID and ProductID=@ProductID", myConnection);
                cmd1.Parameters.AddWithValue("@EmailID", Session["userID"]);
                cmd1.Parameters.AddWithValue("@ProductID", pid);
                int idRows = (int)cmd1.ExecuteScalar();
                if (idRows != 0)
                {
                    //delete product from wishlist
                    SqlCommand cmd2 = new SqlCommand("Delete from [GadgetFox].[dbo].[Wishlists] where " +
                       "EmailID=@EmailID and ProductID=@ProductID", myConnection);
                    cmd2.Parameters.AddWithValue("@EmailID", Session["userID"]);
                    cmd2.Parameters.AddWithValue("@ProductID", pid);
                    int rc = cmd2.ExecuteNonQuery();
                    if (rc == 1)
                    {
                        returnLabel.Text = "Your items were deleted from the cart";
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

            Response.Redirect("~/Wishlist.aspx");
        }

        /**
         * Continue shopping on click         
         */
        protected void shopBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("Home.aspx");
        }
    }
}