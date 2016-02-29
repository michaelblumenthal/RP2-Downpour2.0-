using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SocialUproarWebVote.Startup))]
namespace SocialUproarWebVote
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
