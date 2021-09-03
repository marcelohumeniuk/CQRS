using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(CQRS.UI.Web.Areas.Identity.IdentityHostingStartup))]
namespace CQRS.UI.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}