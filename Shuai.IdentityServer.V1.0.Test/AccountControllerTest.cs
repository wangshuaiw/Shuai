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
using System.Linq;
using System.Threading.Tasks;

namespace Shuai.IdentityServer.V1._0.Test
{
    [TestClass]
    public class AccountControllerTest
    {
        private Mock<UserManager<AppUser>> GetUserManagerMock()
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

        private Mock<SignInManager<AppUser>>  GetSignInManagerMock(UserManager<AppUser> userManger)
        {
            return new Mock<SignInManager<AppUser>>(
                    userManger,
                    new Mock<IHttpContextAccessor>().Object,
                    new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new Mock<ILogger<SignInManager<AppUser>>>().Object,
                    new Mock<IAuthenticationSchemeProvider>().Object,
                    new Mock<IUserConfirmation<AppUser>>().Object
                );
        }

        private Mock<AppIdentityContext> GetContextMock()
        {
            DbContextOptions<AppIdentityContext> option = new DbContextOptions<AppIdentityContext>();
            return new Mock<AppIdentityContext>(option);
        }

        private AppIdentityContext GetContextInMemoryDatabase()
        {
            var opts = new DbContextOptionsBuilder<AppIdentityContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            return new AppIdentityContext(opts);
        }

        /// <summary>
        /// 登录页面：检查ViewData是否正确传输值
        /// </summary>
        [TestMethod]
        public void Login_RetrunUrlNotNULL_ReturnAViewResultWithReturnUrlViewData()
        {
            //arrange
            var mockUserMgr = GetUserManagerMock();
            var mockSignMgr = GetSignInManagerMock(mockUserMgr.Object);
            var mockCtx = GetContextMock();
            AccountController controller = new AccountController(mockUserMgr.Object, mockCtx.Object, mockSignMgr.Object);
            string returnUrl = "ReturnUrl";

            //act
            var result = controller.Login(returnUrl);

            //assert
            Assert.IsInstanceOfType(result,typeof(ViewResult));
            Assert.AreEqual(returnUrl, ((ViewResult)result).ViewData["returnUrl"]);
        }

        /// <summary>
        /// 登录：用户不存在
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PostLogin_NotFundUser_ReturnAViewResultWithModeStateErrorOfNoUser()
        {
            //arrange
            var mockMgr = GetUserManagerMock();
            mockMgr.Setup(repo => repo.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null).Verifiable();
            mockMgr.Setup(repo => repo.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null).Verifiable();

            var mockSignMgr = GetSignInManagerMock(mockMgr.Object);

            var opts = new DbContextOptionsBuilder<AppIdentityContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
                  .Options;
            using var context = GetContextInMemoryDatabase();
            AccountController controller = new AccountController(mockMgr.Object, context, mockSignMgr.Object);

            //act
            var result = await controller.Login(new Login());

            //assert
            var isNoUser = controller.ModelState[string.Empty].Errors.Any(e => e.ErrorMessage == "用户不存在！");
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual(isNoUser, true);
            mockMgr.Verify();
        }

        /// <summary>
        /// 登录：密码错误
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PostLogin_PasswordError_ReturnAViewResultWithModeStateErrorOfPasswordError()
        {
            //arrange
            var mockMgr = GetUserManagerMock();
            mockMgr.Setup(repo => repo.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new AppUser());
            var mockSignMgr = GetSignInManagerMock(mockMgr.Object);
            mockSignMgr.Setup(repo => repo.PasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed)
                .Verifiable();
            var mockCtx = GetContextMock();
            AccountController controller = new AccountController(mockMgr.Object, mockCtx.Object, mockSignMgr.Object);

            //act
            var result = await controller.Login(new Login());

            //assert
            var pwdError = controller.ModelState[string.Empty].Errors.Any(e => e.ErrorMessage == "密码错误！");
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual(pwdError, true);
            mockSignMgr.Verify();
        }

        /// <summary>
        /// 登陆：用户名登陆，重定向Home/Index
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PostLogin_UserNameLogin_ReturnARedirectViewResultToHomeIndex()
        {
            //arrange
            var mockUserMgr = GetUserManagerMock();
            var mockSignMgr = GetSignInManagerMock(mockUserMgr.Object);
            var mockCtx = GetContextMock();
            mockUserMgr.Setup(repo => repo.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new AppUser()).Verifiable();
            mockSignMgr.Setup(repo => repo.PasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success)
                .Verifiable();
            AccountController controller = new AccountController(mockUserMgr.Object, mockCtx.Object, mockSignMgr.Object);

            //act
            var result = await controller.Login(new Login());

            //assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.AreEqual(((RedirectToActionResult)result).ControllerName, "Home");
            Assert.AreEqual(((RedirectToActionResult)result).ActionName, "Index");
            mockUserMgr.Verify();
            mockSignMgr.Verify();
            
        }

        /// <summary>
        /// 登陆：Email登陆，重定向到指定的Url
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PostLogin_EmailLogin_ReurnARedirectViewResultToDesignatedUrl()
        {
            //arrange
            var mockUserMgr = GetUserManagerMock();
            var mockSignMgr = GetSignInManagerMock(mockUserMgr.Object);
            var mockCtx = GetContextMock();
            mockUserMgr.Setup(repo => repo.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null).Verifiable();
            mockUserMgr.Setup(repo => repo.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new AppUser()).Verifiable();
            mockSignMgr.Setup(repo => repo.PasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success)
                .Verifiable();
            AccountController controller = new AccountController(mockUserMgr.Object, mockCtx.Object, mockSignMgr.Object);
            string designatedUrl = "Account/Test";

            //act
            var result = await controller.Login(new Login() { ReturnUrl = designatedUrl });

            //assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual(((RedirectResult)result).Url, designatedUrl);
            mockUserMgr.Verify();
            mockSignMgr.Verify();
        }

        /// <summary>
        /// 登陆：手机号码登陆
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PostLogin_PhoneLogin_ReturnARedirectViewResultToHomeIndex()
        {
            //arrange
            var mockUserMgr = GetUserManagerMock();
            var mockSignMgr = GetSignInManagerMock(mockUserMgr.Object);
            
            mockUserMgr.Setup(repo => repo.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null).Verifiable();
            mockUserMgr.Setup(repo => repo.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null).Verifiable();
            
            mockSignMgr.Setup(repo => repo.PasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success)
                .Verifiable();
            using (var context = GetContextInMemoryDatabase())
            {
                string phone = "11111111111";
                await context.Users.AddAsync(new AppUser() { PhoneNumber = phone });
                await context.SaveChangesAsync();
                AccountController controller = new AccountController(mockUserMgr.Object, context, mockSignMgr.Object);

                //act
                var result = await controller.Login(new Login() { NameOrEmailOrPhone = phone });

                //assert
                Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
                Assert.AreEqual(((RedirectToActionResult)result).ControllerName, "Home");
                Assert.AreEqual(((RedirectToActionResult)result).ActionName, "Index");
                mockUserMgr.Verify();
                mockSignMgr.Verify();
            }
        }

        /// <summary>
        /// 注册页面：检查ViewData是否正确传输值
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void Register_RetrunUrl_ReturnAViewResultWithReturnUrlViewData()
        {
            //arrange
            var mockUserMgr = GetUserManagerMock();
            var mockSignMgr = GetSignInManagerMock(mockUserMgr.Object);
            var mockCtx = GetContextMock();
            AccountController controller = new AccountController(mockUserMgr.Object, mockCtx.Object, mockSignMgr.Object);
            string returnUrl = "ReturnUrl";

            //act
            var result = controller.Register(returnUrl);

            //assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual(returnUrl, ((ViewResult)result).ViewData["returnUrl"]);
        }

        //注册：邮箱为空
        //注册：手机号为空
        //注册：邮箱已被占用
        //注册：手机号也被占用
        //注册：注册类型未指定
        //注册：邮箱注册，未指定returnUrl
        //注册：手机注册，指定returnUrl
    }
}
