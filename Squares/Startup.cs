using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Squares.Startup))]
namespace Squares
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
