using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NakedObjects.App.Demo.Startup))]
namespace NakedObjects.App.Demo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
