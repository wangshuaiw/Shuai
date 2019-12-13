using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

                string roleName = "SuperAdministrators";
                var roleExist = await roleManage.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var role = new IdentityRole()
                    {
                        Name = roleName,
                        NormalizedName = roleName
                    };
                    var roleResult = await roleManage.CreateAsync(role);
                    if (roleResult.Succeeded)
                    {
                        logger.LogInformation($"Create role {roleName} success!");
                    }
                    else
                    {
                        logger.LogError($"Create role {roleName} fail!");
                        return;
                    }
                }

                string userName = "SuperAdmin";
                var user = await userManage.FindByNameAsync(userName);
                if (user == null)
                {
                    user = new AppUser()
                    {
                        UserName = userName,
                        EmailConfirmed = true,
                        NormalizedUserName = userName,
                        PhoneNumberConfirmed = true
                    };
                    string password = configration.GetValue<string>("AppSetting:SuperAdminPwd");
                    var userResult = await userManage.CreateAsync(user, password);
                    if (userResult.Succeeded)
                    {
                        logger.LogInformation($"Create user {userName} success!");
                    }
                    else
                    {
                        logger.LogError($"Create user {userName} fail!");
                        return;
                    }
                }

                bool isInRole = await userManage.IsInRoleAsync(user, roleName);
                if (!isInRole)
                {
                    var toRoleResult = await userManage.AddToRoleAsync(user, roleName);
                    if (toRoleResult.Succeeded)
                    {
                        logger.LogInformation($"Add user {userName} to Role {roleName} success!");
                    }
                    else
                    {
                        logger.LogError($"Add user {userName} to Role {roleName} fail!");
                    }
                }


            }
        }
    }
}
