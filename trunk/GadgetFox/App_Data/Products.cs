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
        public void fn_InsertProducts(string Name, string Description, decimal Price, decimal SalePrice, int InSale, int Quantity, string CategoryName, string SubCategoryName, string Color, string Weight, out int ProductStatus, out string ProductID)
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

        }

    }
}