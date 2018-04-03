[assembly: Microsoft.Owin.OwinStartup(typeof(Api.Startup))]

namespace Api
{
    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseWebApi();
        }
    }
}