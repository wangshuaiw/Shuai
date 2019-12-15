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
                        //EmailConfirmed = true,
                        //NormalizedUserName = AppConsts.SuperAdminUserName,
                        Email=$"{AppConsts.SuperAdminUserName}@shuai.com",
                        //NormalizedEmail = $"{AppConsts.SuperAdminUserName}@shuai.com",
                        //PhoneNumberConfirmed = true
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
    }
}
