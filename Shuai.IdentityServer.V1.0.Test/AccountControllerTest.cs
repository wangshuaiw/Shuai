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
using Shuai.IdentityServer.V1._0.Models.Account;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shuai.IdentityServer.V1._0.Test
{
    [TestClass]
    public class AccountControllerTest
    {
        private Mock<UserManager<AppUser>> UserManager
        {
            get
            {
                return new Mock<UserManager<AppUser>>(MockBehavior.Default,
                       new Mock<IUserStore<AppUser>>().Object,
                       new Mock<IOptions<IdentityOptions>>().Object,
                       new Mock<IPasswordHasher<AppUser>>().Object,
                       new IUserValidator<AppUser>[0],
                       new IPasswordValidator<AppUser>[0],
                       new Mock<ILookupNormalizer>().Object,
                       new Mock<IdentityErrorDescriber>().Object,
                       new Mock<IServiceProvider>().Object,
                       new Mock<ILogger<UserManager<AppUser>>>().Object
                   );
            }
        }

        private Mock<SignInManager<AppUser>> SignInManager
        {
            get
            {
                return new Mock<SignInManager<AppUser>>(
                        UserManager.Object,
                        new Mock<IHttpContextAccessor>().Object,
                        new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
                        new Mock<IOptions<IdentityOptions>>().Object,
                        new Mock<ILogger<SignInManager<AppUser>>>().Object,
                        new Mock<IAuthenticationSchemeProvider>().Object,
                        new Mock<IUserConfirmation<AppUser>>().Object
                    );
            }
        }

        private Mock<AppIdentityContext> Context
        {
            get
            {
                DbContextOptions<AppIdentityContext> option = new DbContextOptions<AppIdentityContext>();
                return new Mock<AppIdentityContext>(option);
            }
        }

        //private AccountController GetTestAccountController()
        //{
        //    //var userManagerLogger = loggerFactory.CreateLogger<UserManager<ApplicationUser>>();
        //    //var userManager = new Mock<UserManager<AppUser>>();
        //    //var signManager = new Mock<SignInManager<AppUser>>();

        //    var userManager = new Mock<UserManager<AppUser>>(MockBehavior.Default,
        //         new Mock<IUserStore<AppUser>>().Object,
        //           new Mock<IOptions<IdentityOptions>>().Object,
        //           new Mock<IPasswordHasher<AppUser>>().Object,
        //           new IUserValidator<AppUser>[0],
        //           new IPasswordValidator<AppUser>[0],
        //           new Mock<ILookupNormalizer>().Object,
        //           new Mock<IdentityErrorDescriber>().Object,
        //           new Mock<IServiceProvider>().Object,
        //           new Mock<ILogger<UserManager<AppUser>>>().Object);

        //    var signManager = new Mock<SignInManager<AppUser>>(
        //        userManager.Object,
        //        new Mock<IHttpContextAccessor>().Object,
        //        new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
        //        new Mock<IOptions<IdentityOptions>>().Object,
        //        new Mock<ILogger<SignInManager<AppUser>>>().Object,
        //        new Mock<IAuthenticationSchemeProvider>().Object,
        //        new Mock<IUserConfirmation<AppUser>>().Object
        //        );

        //    DbContextOptions<AppIdentityContext> option = new DbContextOptions<AppIdentityContext>();
        //    var context = new Mock<AppIdentityContext>(option);
        //    return new AccountController(userManager.Object, context.Object, signManager.Object);
        //}

        /// <summary>
        /// 登录页面：检查ViewData是否正确传输值
        /// </summary>
        [TestMethod]
        public void Login_RetrunUrlNotNULL_ReturnAViewResultWithReturnUrlViewData()
        {
            //arrange
            AccountController controller = new AccountController(UserManager.Object,Context.Object,SignInManager.Object);
            string returnUrl = "ReturnUrl";

            //act
            var result = controller.Login(returnUrl);

            //assert
            Assert.IsInstanceOfType(result,typeof(ViewResult));
            Assert.AreEqual(returnUrl, ((ViewResult)result).ViewData["returnUrl"]);
        }


        [TestMethod]
        public async Task PostLogin_NotFundUser_ReturnAViewResultWithModeStateError()
        {

            UserManager.Setup(repo => repo.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null).Verifiable();
            UserManager.Setup(repo => repo.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null).Verifiable();

            var opts = new DbContextOptionsBuilder<AppIdentityContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
                  .Options;
            var ctx = new AppIdentityContext(opts);
            AccountController controller = new AccountController(UserManager.Object, ctx, SignInManager.Object);
            var result = await controller.Login(new Login());
            //assert
            var state = controller.ModelState[string.Empty];
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }


    }
}
