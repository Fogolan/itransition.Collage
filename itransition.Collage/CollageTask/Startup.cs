using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CollageTask.Startup))]
namespace CollageTask
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
