using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;

namespace GadgetFox
{
    public partial class Cart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session != null && Session["userID"] != null)
            {
                SqlDataSource1.SelectParameters.Add("@EmailID", Session["userID"].ToString());
            }
            else
            {
                SqlDataSource1.SelectParameters.Add("@EmailID", "");
            }
        }


        protected string getImage(string productID)
        {
            string strImgPath = "Image.aspx?ImageID=" + productID;
            return strImgPath;
        }
        protected string getPrice(string strPrice, string strSalePrice, bool isInSale)
        {
            string strFinalPrice = strPrice;
            if (isInSale)
                strFinalPrice = "<strike>" + strPrice + "</strike><br/>" + strSalePrice;
            return strFinalPrice;
        }
    }
}