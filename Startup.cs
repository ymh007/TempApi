using Microsoft.Owin;
using Seagull2.Owin;
using Seagull2.YuanXin.AppApi;

[assembly: OwinStartup(typeof(Startup))]
namespace Seagull2.YuanXin.AppApi
{
    public class Startup: DefaultStartup
    {
        public Startup() { }
    }
}