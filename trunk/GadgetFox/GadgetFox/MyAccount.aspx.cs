using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GadgetFox
{
    public partial class MyAccount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Session["userRole"] != null && Page.Session["userRole"].Equals("2"))
                pnlIM.Visible = true;
            else
                pnlIM.Visible = false;
        }
    }
}