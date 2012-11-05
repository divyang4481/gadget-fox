using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace GadgetFox
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
        void Profile_OnMigrateAnonymous(object sender, ProfileMigrateEventArgs e)
          {
               ProfileCommon anonymousProfile = Profile.GetProfile(e.AnonymousID);
                if (anonymousProfile.SCart != null)
            {
                  if (Profile.SCart == null)
                  Profile.SCart = new GadgetFox.Cart();
                  Profile.SCart.Items.AddRange(anonymousProfile.SCart.Items);
                  anonymousProfile.SCart = null;
            }
     ProfileManager.DeleteProfile(e.AnonymousID);
      AnonymousIdentificationModule.ClearAnonymousIdentifier();
         }
    }
}