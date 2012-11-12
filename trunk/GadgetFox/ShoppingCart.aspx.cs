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
    public partial class ShoppingCart : System.Web.UI.Page
    {
        Double purchaseTotal = 0.00;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userID"] == null)
                return;

            //connection setup
            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);

            //declare a table to store rows
            DataTable Table1 = new DataTable("ShoppingCart");
            //setup a row
            DataRow Row1;

            //try to connect
            try
            {
                myConnection.Open(); //open connection
                SqlCommand cmd = new SqlCommand("select * from viewCart where EmailID=@EmailID", myConnection); //define SQL query
                cmd.Parameters.AddWithValue("@EmailID", Session["userID"]);
                
                SqlDataReader dr = cmd.ExecuteReader(); 

                //add colums for each field into the table
                //EmailID, ProductID, Name, Price, SalePrice, InSale, ImageID, ImageData, Quantity 
                DataColumn pid = new DataColumn("Product ID");
                DataColumn image = new DataColumn("Image");                    
                DataColumn name = new DataColumn("Name");
                DataColumn price = new DataColumn("Price");
                DataColumn sale_price = new DataColumn("Sale Price");
                DataColumn qty = new DataColumn("Quantity");
                DataColumn actions = new DataColumn("#");

                pid.DataType = System.Type.GetType("System.String");
                image.DataType = System.Type.GetType("System.String");
                name.DataType = System.Type.GetType("System.String");
                price.DataType = System.Type.GetType("System.Double");
                sale_price.DataType = System.Type.GetType("System.Double");

                qty.DataType = System.Type.GetType("System.String");
                actions.DataType = System.Type.GetType("System.String");

                Table1.Columns.Add(pid);
                Table1.Columns.Add(image);
                Table1.Columns.Add(name);
                Table1.Columns.Add(price);
                Table1.Columns.Add(sale_price);

                Table1.Columns.Add(qty);
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
                    Row1["Quantity"] = dr["Quantity"];
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
            //create delete from cart button
            Button deleteBtn = new Button();
            deleteBtn.Style.Add("padding", "5px");
            deleteBtn.Style.Add("margin", "5px");
            deleteBtn.Text = "Delete";

            Button updateBtn = new Button();
            updateBtn.Style.Add("padding", "5px");
            updateBtn.Style.Add("margin", "5px");
            updateBtn.Text = "Update";

            TextBox qtyTB = new TextBox();

            if (e.Row.RowIndex > -1)
            {
                //add buttons to column
                String pid = e.Row.Cells[0].Text;
                e.Row.Cells[e.Row.Cells.Count - 1].Controls.Add(deleteBtn);
                e.Row.Cells[e.Row.Cells.Count - 1].Controls.Add(updateBtn);

                //put quantity in a textbox
                String qty = e.Row.Cells[e.Row.Cells.Count - 2].Text;
                qtyTB.Text = qty;
                qtyTB.MaxLength = 3;
                qtyTB.Width = 30;
                qtyTB.ID = pid + "_qty";
                e.Row.Cells[e.Row.Cells.Count - 2].Controls.Add(qtyTB);

                //pass product id to on-click event
                deleteBtn.Click += new EventHandler(this.deleteBtn_Click);
                deleteBtn.CommandArgument = e.Row.RowIndex + "-" + pid;

                updateBtn.Click += new EventHandler(this.updateBtn_Click);
                updateBtn.CommandArgument = e.Row.RowIndex + "-" + pid;

                String price = e.Row.Cells[3].Text;

                //insert product image
                String imgId = e.Row.Cells[1].Text;
                Literal img = new Literal();
                img.Text = "<img height='80px' width='80px' src='Image.aspx?ImageID=" + imgId + "'/>";
                e.Row.Cells[1].Controls.Add(img);

                //calculate purchase total
                if (e.Row.Cells[4].Text.Length > 0 && e.Row.Cells[4].Text != "&nbsp;")
                {
                    String salePrice = e.Row.Cells[4].Text;
                    purchaseTotal = purchaseTotal + Double.Parse(salePrice) * Double.Parse(qty);
                }
                else
                {
                    purchaseTotal = purchaseTotal + Double.Parse(price) * Double.Parse(qty);
                }
                //format total $#.##
                subTotalLB.Text = string.Format("{0:$#,###.##}", purchaseTotal);
                totalLB.Text = string.Format("{0:$#,###.##}", purchaseTotal);
            }
        }

        /**
         * Update item quantity on click
         */
        protected void updateBtn_Click(object sender, EventArgs e)
        {
            Button btn = (Button) sender;
            String[] args = btn.CommandArgument.ToString().Split('-');
            int row = int.Parse(args[0]);
            String pid = args[1];

            //find quantity
            TextBox qtyTB = (TextBox)GridView1.Rows[row].Cells[5].FindControl(pid + "_qty");
            int qty = int.Parse(qtyTB.Text);

            //write to view
            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);
            myConnection.Open();

            if (qty < 1)
            {
                //delete item quantity
                try
                {
                    SqlCommand cmd3 = new SqlCommand("Delete from [GadgetFox].[dbo].[Carts] where " +
                        "EmailID=@EmailID and ProductID=@ProductID", myConnection);
                    cmd3.Parameters.AddWithValue("@EmailID", Session["userID"]);
                    cmd3.Parameters.AddWithValue("@ProductID", pid);
                    int rc = cmd3.ExecuteNonQuery();
                    if (rc == 1)
                    {
                        returnLabel.Text = "Your items were deleted from the cart";
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
            else
            {
                //update item quantity
                try
                {
                    SqlCommand cmd1 = new SqlCommand("Select COUNT(*) from viewCart where EmailID=@EmailID and ProductID=@ProductID", myConnection);
                    cmd1.Parameters.AddWithValue("@EmailID", Session["userID"]);
                    cmd1.Parameters.AddWithValue("@ProductID", pid);
                    int idRows = (int)cmd1.ExecuteScalar();
                    if (idRows != 0)
                    {
                        //update existing row
                        SqlCommand cmd2 = new SqlCommand("Update viewCart set Quantity=@Quantity where " +
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
         * Delete item on click         
         */
        protected void deleteBtn_Click(object sender, EventArgs e)
        {
            Button btn = (Button) sender;
            String[] args = btn.CommandArgument.ToString().Split('-');
            int row = int.Parse(args[0]);
            String pid = args[1];

            //find quantity drop down list
            TextBox qtyTB = (TextBox)GridView1.Rows[row].Cells[5].FindControl(pid + "_qty");
            int qty = int.Parse(qtyTB.Text);
            if (qty < 1)
                return;

            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Alert", "<script>alert('pid = " + pid + "')</script>");

            //write to view
            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);
            try
            {
                myConnection.Open();

                SqlCommand cmd1 = new SqlCommand("Select COUNT(*) from viewCart where EmailID=@EmailID and ProductID=@ProductID", myConnection);
                cmd1.Parameters.AddWithValue("@EmailID", Session["userID"]);
                cmd1.Parameters.AddWithValue("@ProductID", pid);
                int idRows = (int)cmd1.ExecuteScalar();
                if (idRows != 0)
                {
                    //update existing row
                    SqlCommand cmd2 = new SqlCommand("Delete from [GadgetFox].[dbo].[Carts] where " +
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
        }

        /**
         * Checkout order on click
         */
        protected void checkoutBtn_Clicked(object sender, EventArgs e)
        {
            Response.Redirect("~/Checkout.aspx");
        }

    }
}