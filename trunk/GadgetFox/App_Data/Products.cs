using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Collections.ObjectModel;


namespace GadgetFox
{

    [DataObject(true)]
    public class Products
    {


        SqlConnection con;


        /****************************************************************************************************
        * Constructor to set connection string for Products                                                 *
        * Input Parameters : None                                                                           *
        * Output : None                                                                                     *
        ****************************************************************************************************/

        public Products()
        {

            con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

        }



        /****************************************************************************************************
        * Calling the function to Get Categories                                                            *
        * Input Parameters : None                                                                           *
        * Output : Category Name                                                                            *
        ****************************************************************************************************/
        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable GetCategories()
        {
            SqlCommand cmCustomer = new SqlCommand(" SELECT Name FROM Categories", con);
            con.Open();

            //Note :DataReader implements IEnumerable interface
            return cmCustomer.ExecuteReader(CommandBehavior.CloseConnection);

        }

        /****************************************************************************************************
        * Calling the function to Get Categories                                                            *
        * Input Parameters : None                                                                           *
        * Output : Category Name                                                                            *
        ****************************************************************************************************/
        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable GetSubCategories(string categoryName)
        {
            SqlCommand cmCustomer = new SqlCommand(" SELECT a.Name FROM SubCategories a , Categories b where a.categoryID = b.categoryID and b.name ='" + categoryName + "'", con);
            con.Open();

            //Note :DataReader implements IEnumerable interface
            return cmCustomer.ExecuteReader(CommandBehavior.CloseConnection);

        }

        /**************************************************************
        * Method to get next product Id                               *
        * Input Parameters: None                                      *
        * Ouput Parameters: product Id                                *
        **************************************************************/

        public string fn_getNextProductId()
        {
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "p_NewProductId";
            cmd.Connection = con;


            SqlParameter ProdId = new SqlParameter("@ProductId", SqlDbType.VarChar, 4);
            ProdId.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(ProdId);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return Convert.ToString(ProdId.Value);
        }

        /****************************************************************************************************
        * Calling the function to insert records into Products table                                        *
        * Input Parameters : None                                                                           *
        * Output : Category Name                                                                            *
        ****************************************************************************************************/
        [DataObjectMethod(DataObjectMethodType.Insert)]
        public void fn_InsertProducts(string Name, string Description, decimal Price, decimal SalePrice, int InSale, int Quantity, string CategoryName, string SubCategoryName, string Color, string Weight, FileUpload imageFile, out int ProductStatus, out string ProductID)
        {
            
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_InsertProducts";
            cmd.Connection = con;

            cmd.Parameters.AddWithValue("@ProductName", Name);
            cmd.Parameters.AddWithValue("@ProductDescription", Description);
            cmd.Parameters.AddWithValue("@ProdPrice", Price);
            cmd.Parameters.AddWithValue("@ProdSalePrice", SalePrice);
            cmd.Parameters.AddWithValue("@ProdInSale", InSale);
            cmd.Parameters.AddWithValue("@CategoryName", CategoryName);
            cmd.Parameters.AddWithValue("@SubCategoryName", SubCategoryName);
            cmd.Parameters.AddWithValue("@ProdColor", Color);
            cmd.Parameters.AddWithValue("@ProdWeight", Weight);
            cmd.Parameters.AddWithValue("@ProdQuantity", Quantity);

            SqlParameter Status = new SqlParameter("@Status", SqlDbType.Int);
            Status.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(Status);

            SqlParameter ProdID = new SqlParameter("@ProductID", SqlDbType.VarChar, 4);
            ProdID.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(ProdID);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            ProductStatus = Convert.ToInt32(Status.Value);
            ProductID = ProdID.Value.ToString();

            //upload product image file
            uploadProductImage(ProductID, imageFile);
        }



        public void fnGetProductDetails(string ProductID, out string Name, out string Description,out decimal Price,out decimal SalePrice, out int InSale,
                                       out int Quantity, out string CategoryName,out string SubCategoryName, out string Color,out string Weight) 
                                       //out int ProductStatus)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "p_ProductDetails";
            cmd.Connection = con;

            cmd.Parameters.AddWithValue("@ProductID", ProductID);

            SqlParameter PName = new SqlParameter("@ProductName", SqlDbType.VarChar, 50);
            PName.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(PName);

            SqlParameter PDesc = new SqlParameter("@ProductDescription", SqlDbType.VarChar, 350);
            PDesc.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(PDesc);

            SqlParameter CName = new SqlParameter("@CategoryName", SqlDbType.VarChar, 16);
            CName.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(CName);

            SqlParameter SCName = new SqlParameter("@SubCategoryName", SqlDbType.VarChar, 16);
            SCName.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(SCName);

            SqlParameter PColor = new SqlParameter("@ProdColor", SqlDbType.VarChar, 16);
            PColor.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(PColor);

            SqlParameter PWeight = new SqlParameter("@ProdWeight", SqlDbType.VarChar, 16);
            PWeight.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(PWeight);

            SqlParameter PPrice = new SqlParameter("@ProdPrice", SqlDbType.Decimal);
            PPrice.Precision = 5;
            PPrice.Scale = 2;
            PPrice.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(PPrice);

            SqlParameter PSalePrice = new SqlParameter("@ProdSalePrice", SqlDbType.Decimal);
            PSalePrice.Precision = 5;
            PSalePrice.Scale = 2;
            PSalePrice.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(PSalePrice);

            SqlParameter PInSale = new SqlParameter("@ProdInSale", SqlDbType.Int);
            PInSale.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(PInSale);

            SqlParameter PQty = new SqlParameter("@ProdQuantity", SqlDbType.Int);
            PQty.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(PQty);

            //SqlParameter PStatus = new SqlParameter("@Status", SqlDbType.Int);
            //PStatus.Direction = ParameterDirection.Output;
            //cmd.Parameters.Add(PStatus);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
                        
            Name = PName.Value.ToString();
            Description = PDesc.Value.ToString();

            Price = 0;
            SalePrice = Convert.ToDecimal(0);
            if (PPrice.Value != System.DBNull.Value)
                Price = Convert.ToDecimal(PPrice.Value);
            if (PSalePrice.Value != System.DBNull.Value)
                SalePrice = Convert.ToDecimal(PSalePrice.Value);

            
            InSale= Convert.ToInt32(PInSale.Value);
            Quantity = Convert.ToInt32(PQty.Value);
            CategoryName = CName.Value.ToString();
            SubCategoryName = SCName.Value.ToString();
            Color = PColor.Value.ToString();
            Weight = PWeight.Value.ToString();
            //ProductStatus = Convert.ToInt32(PStatus.Value);

        }


        public void fn_UpdateProductDetails(string ProductID, string Name, string Description, decimal Price, decimal SalePrice, int InSale,
                               int Quantity, string CategoryName, string SubCategoryName, string Color, string Weight, FileUpload imageFile, out int ProductStatus, out string PID)
  
        {
            SqlCommand cmd = new SqlCommand();


            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_UpdateProducts";
            cmd.Connection = con;

            cmd.Parameters.AddWithValue("@ProdID", ProductID);
            cmd.Parameters.AddWithValue("@ProductName", Name);
            cmd.Parameters.AddWithValue("@ProductDescription", Description);
            cmd.Parameters.AddWithValue("@ProdPrice", Price);
            cmd.Parameters.AddWithValue("@ProdSalePrice", SalePrice);
            cmd.Parameters.AddWithValue("@ProdInSale", InSale);
            cmd.Parameters.AddWithValue("@CategoryName", CategoryName);
            cmd.Parameters.AddWithValue("@SubCategoryName", SubCategoryName);
            cmd.Parameters.AddWithValue("@ProdColor", Color);
            cmd.Parameters.AddWithValue("@ProdWeight", Weight);
            cmd.Parameters.AddWithValue("@ProdQuantity", Quantity);

            SqlParameter Status = new SqlParameter("@Status", SqlDbType.Int);
            Status.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(Status);

            SqlParameter ProdID = new SqlParameter("@ProductID", SqlDbType.VarChar, 4);
            ProdID.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(ProdID);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            ProductStatus = Convert.ToInt32(Status.Value);
            PID = ProdID.Value.ToString();

            //upload product image file
            uploadProductImage(PID, imageFile);
        }



        public void uploadProductImage(String productId, FileUpload fu)
        {
            // Base image ID on product ID
            int imgId = -1;

            SqlConnection myConnection = null;
            if (fu.HasFile)
            {
                try
                {
                    int fileLen = fu.PostedFile.ContentLength;
                    
                    // Create a byte array to hold the contents of the file.byte[] input = newbyte[fileLen - 1];
                    byte[] m_barrImg = fu.FileBytes;

                    String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                    myConnection = new SqlConnection(myConnectionString);

                    myConnection.Open();

                    System.Diagnostics.Trace.Write("Checking if image is in dbo.Images\n");
                    SqlCommand cmd1 = new SqlCommand("Select ImageID from Products where ProductID=@ProductID", myConnection);
                    cmd1.Parameters.AddWithValue("@ProductID", productId);
                    SqlDataReader dr = cmd1.ExecuteReader();
                    while (dr.Read())
                    {
                        System.Diagnostics.Trace.Write("ImageID found = {" + dr["ImageID"].ToString()  + "}\n");
                        if (dr["ImageID"].ToString().Length > 0)
                        {
                            System.Diagnostics.Trace.Write("ImageID being parsed\n");
                            imgId = int.Parse(dr["ImageID"].ToString());
                        }
                    }
                    dr.Close();

                    int rc = 0;
                    if (imgId == -1)
                    {
                        // Insert if image does not exist
                        System.Diagnostics.Trace.Write("Inserting image in dbo.Images\n");
                        SqlCommand cmd = new SqlCommand("INSERT INTO [GadgetFox].[dbo].[Images] VALUES(@ImageData,@UploadedDate)", myConnection);
                        cmd.Parameters.AddWithValue("@ImageData", m_barrImg);
                        cmd.Parameters.AddWithValue("@UploadedDate", DateTime.Today);
                        rc = cmd.ExecuteNonQuery();

                        cmd.CommandText = "SELECT @@Identity";
                        imgId = Convert.ToInt32(cmd.ExecuteScalar());

                        System.Diagnostics.Trace.Write("New imgId = " + imgId + "\n");
                    }
                    else
                    {
                        // Update if image it does exist
                        System.Diagnostics.Trace.Write("Updating image in dbo.Images\n");
                        SqlCommand cmd3 = new SqlCommand("UPDATE [GadgetFox].[dbo].[Images] SET ImageData=@ImageData, UploadedDate=@UploadedDate where ImageID=@ImageID", myConnection);
                        cmd3.Parameters.AddWithValue("@ImageData", m_barrImg);
                        cmd3.Parameters.AddWithValue("@UploadedDate", DateTime.Today);
                        cmd3.Parameters.AddWithValue("@ImageID", imgId);                        
                        rc = cmd3.ExecuteNonQuery();

                        cmd3.CommandText = "SELECT @@Identity";
                        imgId = (int)cmd3.ExecuteScalar();
                    }

                    // Update image ID for product
                    if (imgId > -1)
                    {
                        System.Diagnostics.Trace.Write("Updating image ID for product in dbo.Products\n");
                        SqlCommand cmd2 = new SqlCommand("UPDATE [GadgetFox].[dbo].[Products] SET ImageID=@ImageID where ProductID=@ProductID", myConnection);
                        cmd2.Parameters.AddWithValue("@ImageID", imgId);
                        cmd2.Parameters.AddWithValue("@ProductID", productId);

                        rc = cmd2.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    System.Diagnostics.Trace.Write("Exception: " + ex.Message + "\n");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.Write("Exception: " + ex.Message + "\n");
                }
                finally
                {
                    if (myConnection != null)
                        myConnection.Close();
                }
            }
        }
    }
}