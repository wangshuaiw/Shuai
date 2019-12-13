using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using Shuai.IdentityServer.V1._0.Areas.Identity.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Shuai.IdentityServer.V1._0
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<AppIdentityContext>(options =>
            //        options.UseSqlServer(
            //            Configuration.GetConnectionString("AppIdentityContextConnection")));

            //services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<AppIdentityContext>();

            services.AddDbContext<AppIdentityContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("AppIdentityContextConnection"));
            });

            services.AddIdentity<AppUser, IdentityRole>(options =>
             {
                 options.SignIn.RequireConfirmedAccount = false;
                 options.Password.RequireNonAlphanumeric = false;
                 options.Password.RequireUppercase = false;
             })
            .AddDefaultUI()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<AppIdentityContext>();

            services.AddRazorPages();
            services.AddControllersWithViews(config=> 
            {
                //���Ĭ�������֤���򣺵�¼����ܷ���
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            AppIdentitySeedData.InitializeAsync(app).Wait();
        }

        
    }
}
