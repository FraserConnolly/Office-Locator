using Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(OffstageControls.OfficeLocator.Startup))]
namespace OffstageControls.OfficeLocator
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
}
