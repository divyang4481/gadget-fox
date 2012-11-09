using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.IO;

namespace GadgetFox
{
    public partial class ProductsPurchase : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            string strItemId = Request.QueryString["ItemID"];
            if (!String.IsNullOrEmpty(strItemId))
            {
                String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                SqlConnection myConnection = new SqlConnection(myConnectionString);
                try
                {
                    myConnection.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO [GadgetFox].[dbo].[Carts] VALUES(@EmailID,@ProductID,@Quantity)", myConnection);
                    cmd.Parameters.AddWithValue("@EmailID", Session["userID"]);
                    cmd.Parameters.AddWithValue("@ProductID", strItemId);
                    cmd.Parameters.AddWithValue("@Quantity", ddlQuantity.SelectedValue);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 1)
                    {
                        Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('Saved to cart')</SCRIPT>");
                        //Response.Redirect("~/Login.aspx");
                    }
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


      /*  protected void Button1_Click(object sender, EventArgs e)
        {
            SqlConnection myConnection = null;
            if (FileUpload1.HasFile)
            {
                try
                {
                    int fileLen = FileUpload1.PostedFile.ContentLength;


        
                    // Create a byte array to hold the contents of the file.byte[] input = newbyte[fileLen - 1];
                    byte[] m_barrImg = FileUpload1.FileBytes;



                    Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('Upload status: File uploaded!')</SCRIPT>");
                    String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                    myConnection = new SqlConnection(myConnectionString);

                    myConnection.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO [GadgetFox].[dbo].[Images] VALUES(@ImageData,@UploadedDate)", myConnection);
                    cmd.Parameters.AddWithValue("@ImageData", m_barrImg);
                    cmd.Parameters.AddWithValue("@UploadedDate", DateTime.Today);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 1)
                    {
                        cmd = new SqlCommand("UPDATE [GadgetFox].[dbo].[Products] SET ImageID=@ImageID where ProductID=@ProductID", myConnection);
                        cmd.Parameters.AddWithValue("@ImageID", 001);
                        cmd.Parameters.AddWithValue("@ProductID", "P006");

                        rows = cmd.ExecuteNonQuery();
                    }



                }
                catch (SqlException ex)
                {
                    Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('" + ex.Message + "')</SCRIPT>");
                }
                catch (Exception ex)
                {
                    Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('Upload status: The file could not be uploaded. The following error occured: '" + ex.Message + "')</SCRIPT>");
                }
                finally
                {
                    if (myConnection != null)
                        myConnection.Close();
                }
            }
        }*/
    }
}
