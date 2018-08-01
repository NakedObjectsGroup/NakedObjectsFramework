using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(NakedObjects.Rest.Test.App.Startup))]

namespace NakedObjects.Rest.Test.App {
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
