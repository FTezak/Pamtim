using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI.WebControls;
using Pamtim.Models;

namespace Pamtim
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }


        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {

            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);


                if (authTicket.Expired) return;

                var serializer = new JavaScriptSerializer();
                var serializeModel = serializer.Deserialize<UserPrincipalSerializeModel>(authTicket.UserData);

                var newUser = new UserPrincipal(authTicket.Name) {UserName = serializeModel.UserName};


                newUser.Email = serializeModel.Email;
                newUser.ID_User = serializeModel.ID_User;

                HttpContext.Current.User = newUser;
            }
            //else
            //{
            //    HttpContext.Current.User = new TaskPrincipal("noname");

            //}


        }

    }
}
