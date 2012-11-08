using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.SessionState;

namespace GadgetFox
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Session["userRole"] != null && Page.Session["userRole"].Equals("2"))
            {
                pnlInventoryManager.Visible = true;
                pnlCustomerHome.Visible = false;
            }
            else
                pnlInventoryManager.Visible = false;
        }
    }
}