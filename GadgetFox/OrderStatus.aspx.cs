using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GadgetFox
{
    public partial class OrderStatus : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userID"] == null)
            {
                // Redirect user to login before doing anything else
                Response.Redirect("~/Login.aspx?redirect=ViewInventory.aspx");
            }
            else if (Session["userID"] != null)
            {
                // Redirect user to login before doing anything else
                Response.Redirect("~/Home.aspx");
            }
            else if (Session["userRole"].Equals("1"))
            {
                Response.Redirect("~/Forbidden.aspx");
            }
        }
    }
}