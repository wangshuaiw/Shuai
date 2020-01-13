using IdentityModel;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shuai.IdentityServer.V1._0.Areas.Identity.Data
{
    public class AppIdentitySeedData
    {
        public static async Task InitializeAsync(IApplicationBuilder app)
        {
            await InitSuperAdminUserAsync(app);

        }

        private static async Task InitSuperAdminUserAsync(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                //获取需要的服务
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
                var userManage = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleManage = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var configration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                //string roleName = "SuperAdministrators";
                var roleExist = await roleManage.RoleExistsAsync(AppConsts.SuperAdminRoleName);
                if (!roleExist)
                {
                    var role = new IdentityRole()
                    {
                        Name = AppConsts.SuperAdminRoleName,
                        NormalizedName = AppConsts.SuperAdminRoleName
                    };
                    var roleResult = await roleManage.CreateAsync(role);
                    if (roleResult.Succeeded)
                    {
                        logger.LogInformation($"Create role {AppConsts.SuperAdminRoleName} success!");
                    }
                    else
                    {
                        logger.LogError($"Create role {AppConsts.SuperAdminRoleName} fail!");
                        return;
                    }
                }

                
                var user = await userManage.FindByNameAsync(AppConsts.SuperAdminUserName);
                if (user == null)
                {
                    user = new AppUser()
                    {
                        UserName = AppConsts.SuperAdminUserName,
                        EmailConfirmed = true,
                        NormalizedUserName = AppConsts.SuperAdminUserName,
                        Email=$"{AppConsts.SuperAdminUserName}@shuai.com",
                        NormalizedEmail = $"{AppConsts.SuperAdminUserName}@shuai.com",
                        PhoneNumberConfirmed = true
                    };
                    string password = configration.GetValue<string>("AppSetting:SuperAdminPwd");
                    var userResult = await userManage.CreateAsync(user, password);
                    if (userResult.Succeeded)
                    {
                        logger.LogInformation($"Create user {AppConsts.SuperAdminUserName} success!");
                    }
                    else
                    {
                        logger.LogError($"Create user {AppConsts.SuperAdminUserName} fail!");
                        return;
                    }
                }

                bool isInRole = await userManage.IsInRoleAsync(user, AppConsts.SuperAdminRoleName);
                if (!isInRole)
                {
                    var toRoleResult = await userManage.AddToRoleAsync(user, AppConsts.SuperAdminRoleName);
                    if (toRoleResult.Succeeded)
                    {
                        logger.LogInformation($"Add user {AppConsts.SuperAdminUserName} to Role {AppConsts.SuperAdminRoleName} success!");
                    }
                    else
                    {
                        logger.LogError($"Add user {AppConsts.SuperAdminUserName} to Role {AppConsts.SuperAdminRoleName} fail!");
                    }
                }


            }
        }

        private static async Task InitIdentityServerConfig(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                //if()
            }
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            var customProfile = new IdentityResource(name: "custom.profile", displayName: "Custom profile", claimTypes: new[] { "role" });

            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                customProfile
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1","My API",new List<string>(){ JwtClaimTypes.Role })
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client()
                {
                    ClientId = "client",
                    AllowedGrantTypes=GrantTypes.ClientCredentials,
                    ClientSecrets={new Secret("secret".Sha256())},
                    AllowedScopes={ "api1" }
                },
                new Client()
                {
                    ClientId="ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets ={new Secret("secret".Sha256())},
                    AllowedScopes={ "api1", IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, "custom.profile" },
                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Role,"admin")
                    }
                }
            };
        }
    }
}
