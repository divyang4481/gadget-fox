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
    public partial class ViewInventory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userID"] == null)
            {
                // Redirect user to login before doing anything else
                Response.Redirect("~/Login.aspx?redirect=ViewInventory.aspx");
            }
            else if (Session["userID"] != null && Session["userRole"].Equals("1"))
            {
                // Redirect user to login before doing anything else
                Response.Redirect("~/Home.aspx");
            }
        }
    }
}