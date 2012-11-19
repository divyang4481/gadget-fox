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
    public partial class MonitorInventory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userID"] == null)
            {
                // Redirect user to login before doing anything else
                Response.Redirect("~/Login.aspx?redirect=MonitorInventory.aspx");
            }
            else if (Session["userID"] != null && Session["userRole"].Equals("1"))
            {
                Response.Redirect("~/Forbidden.aspx");
            }
        }

        protected string getPrice(Decimal price, Decimal salePrice, bool onSale)
        {
            return String.Format("{0:c}", (onSale ? salePrice : price));
        }

        protected string getImage(string imageId)
        {
            return "Image.aspx?ImageID=" + imageId;
        }
    }
}