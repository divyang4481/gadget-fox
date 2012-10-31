using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using AjaxControlToolkit;

namespace GadgetFox
{
    public partial class Sample : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //getInventoryProducts();
        }


        private DataSet getInventoryProducts()
        {
            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);
            DataSet ds = new DataSet();
            try
            {
                myConnection.Open();
                SqlCommand cmd = new SqlCommand("Select * from [GadgetFox].[dbo].[Products]", myConnection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                gdvInventory.DataSource = ds;
                gdvInventory.DataBind();
            }
            catch (SqlException ex)
            {
                Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('" + ex.Message + "')</SCRIPT>");
            }
            finally
            {
                myConnection.Close();
            }
            return ds;
        }

        protected void gdvInventory_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataSet ds = gdvInventory.DataSource as DataSet;
            if (ViewState[e.SortExpression] == null)
                ViewState[e.SortExpression] = "DESC";

            String strSortDirection, prevDirect = ViewState[e.SortExpression].ToString();

            ViewState[e.SortExpression] = strSortDirection = (prevDirect == "ASC") ? "DESC" : "ASC";

            if (ds != null)
            {
                DataView dataView = new DataView(ds.Tables[0]);
                dataView.Sort = e.SortExpression + " " + strSortDirection;

                gdvInventory.DataSource = dataView;
                gdvInventory.DataBind();
            }
        }

        protected void gdvInventory_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            String strQuantity = ((TextBox)gdvInventory.Rows[e.RowIndex].FindControl("txtQuantity")).Text;
            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);
            DataSet ds = new DataSet();
            try
            {
                myConnection.Open();
                SqlCommand cmd = new SqlCommand("UPDATE [GadgetFox].[dbo].[Products] SET Quantity=@Quantity WHERE PRroductID=@PRroductID", myConnection);
                cmd.Parameters.AddWithValue("@Quantity", Convert.ToInt32(strQuantity));
                cmd.Parameters.AddWithValue("@ProductID",gdvInventory.Rows[e.RowIndex].Cells[0].Text);
                int rows = cmd.ExecuteNonQuery();
                if (rows == 1)
                    Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('Quantity updated successfully')</SCRIPT>");

            }
            catch (SqlException ex)
            {
                Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('" + ex.Message + "')</SCRIPT>");
            }
            finally
            {
                myConnection.Close();
                gdvInventory.EditIndex = -1;
                getInventoryProducts();
            }
        }

        protected void gdvInventory_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gdvInventory.EditIndex = e.NewEditIndex;
            getInventoryProducts();
        }

        protected void gdvInventory_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gdvInventory.EditIndex = -1;
            getInventoryProducts();
        }

        protected void gdvInventory_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Update")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                String strQuantity = ((TextBox)gdvInventory.Rows[rowIndex].FindControl("txtQuantity")).Text;
                String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                SqlConnection myConnection = new SqlConnection(myConnectionString);
                DataSet ds = new DataSet();
                try
                {
                    myConnection.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE [GadgetFox].[dbo].[Products] SET Quantity=@Quantity WHERE PRroductID=@PRroductID", myConnection);
                    cmd.Parameters.AddWithValue("@Quantity", Convert.ToInt32(strQuantity));
                    cmd.Parameters.AddWithValue("@ProductID", gdvInventory.Rows[rowIndex].Cells[0].Text);
                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 1)
                        Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('Quantity updated successfully')</SCRIPT>");

                }
                catch (SqlException ex)
                {
                    Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('" + ex.Message + "')</SCRIPT>");
                }
                finally
                {
                    myConnection.Close();
                    gdvInventory.EditIndex = -1;
                    getInventoryProducts();
                }
            }
        }

    }
}