using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Shuai.IdentityServer.V1._0.Authorization
{
    public class SuperAdminAuthorizationHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var requirements = context.Requirements;
            var requirements2 = context.PendingRequirements;
            foreach(var requirement in requirements)
            {
                if (context.User.IsInRole(AppConsts.SuperAdminRoleName))
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
