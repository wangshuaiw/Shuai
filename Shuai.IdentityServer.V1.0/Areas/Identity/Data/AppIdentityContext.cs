using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shuai.IdentityServer.V1._0.Areas.Identity.Data;

namespace Shuai.IdentityServer.V1._0.Areas.Identity.Data
{
    public class AppIdentityContext : IdentityDbContext<AppUser,IdentityRole,string>
    {
        public AppIdentityContext(DbContextOptions<AppIdentityContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //builder.Entity<AppUser>().ToTable("User", "Identity");
            //builder.Entity<AppUser>().Property(t => t.UserName)
            //    .HasColumnName("Name")
            //    .IsRequired()
            //    .HasMaxLength(20);


            //https://docs.microsoft.com/zh-cn/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-3.1
            builder.Entity<AppUser>(b =>
            {
                b.ToTable("Users", "Identity");
            });
            builder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.ToTable("UserClaims", "Identity");
            });
            builder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.ToTable("UserLogins", "Identity");
            });
            builder.Entity<IdentityUserToken<string>>(b =>
            {
                b.ToTable("UserTokens", "Identity");
            });
            builder.Entity<IdentityRole>(b =>
            {
                b.ToTable("Roles", "Identity");
            });
            builder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.ToTable("RoleClaims", "Identity");
            });
            builder.Entity<IdentityUserRole<string>>(b =>
            {
                b.ToTable("UserRoles", "Identity");
            });
            
        }
    }
}
