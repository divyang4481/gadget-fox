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
                    //connection setup
                    String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                    SqlConnection myConnection = new SqlConnection(myConnectionString);

                    //read the url parameter
                    string subcategory = "";
                    String qsubcat = Request.QueryString["subcategory"];

                    string category = "";
                    String qcat = Request.QueryString["subcategory"];

                    string search_criteria = "";
                    //set subquery information
                    if (qsubcat != null || qcat !=null )
                    {

                        if (qsubcat != null)
                        {
                            subcategory = Request.QueryString["subcategory"].ToString(); //read the url parameter
                            search_string.Text = subcategory;
                            search_criteria = "subcategoryname = '" + subcategory + "'";
                        }

                        if (qsubcat != null)
                        {
                            subcategory = Request.QueryString["category"].ToString(); //read the url parameter
                            search_string.Text = category;
                            search_criteria = "categoryname = '" + category + "'";
                        }
                        //declare a table to store rows
                        DataTable Table1 = new DataTable("SearchResults");
                        //setup a row
                        DataRow Row1;

                        //try to connect
                        try
                        {
                            myConnection.Open(); //open connection
                            SqlCommand cmd = new SqlCommand("select * from vw_ProductDetails where " + search_criteria, myConnection); //define SQL query
                            SqlDataReader dr = cmd.ExecuteReader(); //perform SQL query and store it

                            //add colums for each field into the table
                            //DataColumn image = new DataColumn("ID");
                            DataColumn name = new DataColumn("Name");
                            DataColumn description = new DataColumn("Description");
                            DataColumn price = new DataColumn("Price");
                            DataColumn sale_price = new DataColumn("SalePrice");

                            //image.DataType = System.Type.GetType("System.Byte[]");
                            name.DataType = System.Type.GetType("System.String");
                            description.DataType = System.Type.GetType("System.String");
                            price.DataType = System.Type.GetType("System.Double");
                            sale_price.DataType = System.Type.GetType("System.Double");

                            //Table1.Columns.Add(image);
                            Table1.Columns.Add(name);
                            Table1.Columns.Add(description);
                            Table1.Columns.Add(price);
                            Table1.Columns.Add(sale_price);

                            while (dr.Read())
                            {
                                Row1 = Table1.NewRow();
                                //insert values into the row from the Query, examples

                                Row1["Name"] = dr["Name"];
                                Row1["Description"] = dr["Description"];
                                Row1["Price"] = dr["Price"];
                                Row1["SalePrice"] = dr["SalePrice"];

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
        }
    }
}