using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shuai.IdentityServer.V1._0.Areas.Identity.Data;
using Shuai.IdentityServer.V1._0.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shuai.IdentityServer.V1._0.Test
{
    [TestClass]
    public class AccountControllerTest
    {
        [TestMethod]
        public void Login_RetrunUrlNotNULL_ReturnAViewResultWithReturnUrlViewData()
        {
            //arrange

            //var userManagerLogger = loggerFactory.CreateLogger<UserManager<ApplicationUser>>();
            var userManager = new Mock<UserManager<AppUser>>(MockBehavior.Default,
                 new Mock<IUserStore<AppUser>>().Object,
                   new Mock<IOptions<IdentityOptions>>().Object,
                   new Mock<IPasswordHasher<AppUser>>().Object,
                   new IUserValidator<AppUser>[0],
                   new IPasswordValidator<AppUser>[0],
                   new Mock<ILookupNormalizer>().Object,
                   new Mock<IdentityErrorDescriber>().Object,
                   new Mock<IServiceProvider>().Object,
                   new Mock<ILogger<UserManager<AppUser>>>().Object);

            //var userManager = new Mock<UserManager<AppUser>>();
            //var signManager = new Mock<SignInManager<AppUser>>();
            var signManager = new Mock<SignInManager<AppUser>>(
                userManager,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<AppUser>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<AppUser>>().Object
                );
            DbContextOptions<AppIdentityContext> option = new DbContextOptions<AppIdentityContext>();
            var context = new Mock<AppIdentityContext>(option);
            AccountController controller = new AccountController(userManager.Object, context.Object, signManager.Object);

            string returnUrl = "ReturnUrl";

            //act
            var result = controller.Login(returnUrl);

            //assert
            Assert.IsInstanceOfType(result,typeof(ViewResult));
            Assert.AreEqual(returnUrl, ((ViewResult)result).ViewData["returnUrl"]);
        }
    }
}
