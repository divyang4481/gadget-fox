using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.SessionState;

namespace GadgetFox
{
    public partial class GadgetSite2 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user"] != null)
            {
                lblWelcome.Text = "Hi " + Session["user"].ToString();
                logoutLK.Visible = true;
            }
            else
            {
                logoutLK.Visible = false;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }

    }
}