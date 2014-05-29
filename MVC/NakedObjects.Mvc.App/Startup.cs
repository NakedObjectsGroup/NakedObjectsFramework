using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NakedObjects.Mvc.App.Startup))]
namespace NakedObjects.Mvc.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
