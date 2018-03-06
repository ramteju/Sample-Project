using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProductTracking.Startup))]
namespace ProductTracking
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
