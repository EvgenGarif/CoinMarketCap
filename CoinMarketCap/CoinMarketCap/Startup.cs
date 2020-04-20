using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CoinMarketCap.Startup))]
namespace CoinMarketCap
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
