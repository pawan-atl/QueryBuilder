using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LinqQueryBuilder.Startup))]
namespace LinqQueryBuilder
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
