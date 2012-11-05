using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GadgetFox
{
    public partial class ProductDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            double Price = double.Parse(((Label)
              DataList1.Controls[0].FindControl("PriceLabel")).Text);
            string ProductName = ((Label)
              DataList1.Controls[0].FindControl("NameLabel")).Text;
            string ProductImageUrl = ((Label)
             DataList1.Controls[0].FindControl("ImageUrlLabel")).Text;
            int ProductID = int.Parse(Request.QueryString["ProductID"]);
            if (Profile.SCart == null)
            {
                Profile.SCart = new GadgetFox.Cart();
            }
            Profile.SCart.Insert
                (ProductID, Price, 1, ProductName, ProductImageUrl);
            Server.Transfer("Products.aspx");
        }
    }
}