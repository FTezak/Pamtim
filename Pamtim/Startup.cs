using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Pamtim.Startup))]
namespace Pamtim
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
