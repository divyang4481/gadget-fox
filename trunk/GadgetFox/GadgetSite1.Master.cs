using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GadgetFox
{
    public partial class GadgetSite1 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private void Page_Init(object sender, EventArgs e)
        {
            if (Page.ClientScript.IsStartupScriptRegistered(this.GetType(), "MaskedEditFix") == false)
                Page.ClientScript.RegisterStartupScript(this.GetType(), "MaskedEditFix", String.Format("<script type='text/javascript' src='{0}'></script>", Page.ResolveUrl("~/js/MaskedEditFix.js")));
        }


        protected void Button1_Click(object sender, EventArgs e)
        {

        }

    }
}