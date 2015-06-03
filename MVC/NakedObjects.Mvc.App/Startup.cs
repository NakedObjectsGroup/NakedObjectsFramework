using Microsoft.Owin;
using NakedObjects.Mvc.App;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
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
