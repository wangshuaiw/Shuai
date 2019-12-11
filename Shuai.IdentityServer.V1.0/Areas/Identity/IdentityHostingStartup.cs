using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuai.IdentityServer.V1._0.Areas.Identity.Data;

[assembly: HostingStartup(typeof(Shuai.IdentityServer.V1._0.Areas.Identity.IdentityHostingStartup))]
namespace Shuai.IdentityServer.V1._0.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                //services.AddDbContext<AppIdentityContext>(options =>
                //    options.UseSqlServer(
                //        context.Configuration.GetConnectionString("AppIdentityContextConnection")));

                //services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
                //    .AddEntityFrameworkStores<AppIdentityContext>();
            });
        }
    }
}