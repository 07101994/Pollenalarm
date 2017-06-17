using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Pollenalarm.Backend.AspNet.Startup))]

namespace Pollenalarm.Backend.AspNet
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}