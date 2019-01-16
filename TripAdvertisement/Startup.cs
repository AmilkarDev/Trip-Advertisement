using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TripAdvertisement.Startup))]
namespace TripAdvertisement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
