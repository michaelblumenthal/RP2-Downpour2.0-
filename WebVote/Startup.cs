using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebVote.Startup))]
namespace WebVote
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
