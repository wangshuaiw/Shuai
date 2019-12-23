using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Shuai.IdentityServer.V1._0.Models.Account;
using Shuai.IdentityServer.V1._0.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Shuai.IdentityServer.V1._0.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUser> UserManager { get; }

        private RoleManager<IdentityRole> RoleManager { get; }

        private AppIdentityContext IdentityContext { get; }

        private SignInManager<AppUser> SignManager { get; }


        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppIdentityContext context,SignInManager<AppUser> signManager) 
        {
            UserManager = userManager;
            RoleManager = roleManager;
            IdentityContext = context;
            SignManager = signManager;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            if(!ModelState.IsValid)
            {
                return View(login);
            }

            string returnUrl = ViewData["returnUrl"] == null ? null : ViewData["returnUrl"].ToString();

            var user = await UserManager.FindByNameAsync(login.NameOrEmailOrPhone);
            if(user==null)
            {
                user = await UserManager.FindByEmailAsync(login.NameOrEmailOrPhone);
            }
            if(user==null)
            {
                user = await IdentityContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == login.NameOrEmailOrPhone);
            }
            if(user==null)
            {
                ModelState.AddModelError(string.Empty, "用户不存在！");
                return View(login);
            }

            var result =await SignManager.PasswordSignInAsync(user, login.Password, login.RememberMe,false);
            if(result.Succeeded)
            {
                if (string.IsNullOrWhiteSpace(returnUrl))
                {
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "密码错误！");
                return View(login);
            }
        }

        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(Register register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }
            if(register.RegisterType=="email"&&string.IsNullOrWhiteSpace(register.Email))
            {
                ModelState.AddModelError(string.Empty, "邮箱不可为空！");
                return View(register);
            }
            if(register.RegisterType=="phone"&&string.IsNullOrWhiteSpace(register.Phone))
            {
                ModelState.AddModelError(string.Empty, "手机号码不可为空！");
                return View(register);
            }

            AppUser user = null;

            if(register.RegisterType == "email")
            {
                user = await UserManager.FindByEmailAsync(register.Email);
                if (user != null)
                {
                    ModelState.AddModelError(string.Empty, "此邮箱已注册用户！");
                    return View(register);
                }
                else
                {
                    user = new AppUser()
                    {
                        UserName = register.Email,
                        Email = register.Email
                    };
                }
            }
            else if(register.RegisterType =="phone")
            {
                user = await IdentityContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == register.Phone);
                if(user!=null)
                {
                    ModelState.AddModelError(string.Empty, "此手机号码已注册用户！");
                    return View(register);
                }
                else
                {
                    user = new AppUser()
                    {
                        UserName = register.Phone,
                        PhoneNumber = register.Phone
                    };
                }
            }
            
            if(user==null)
            {
                throw new Exception("注册用户未指定注册类型");
            }

            string returnUrl = ViewData["returnUrl"] == null ? null : ViewData["returnUrl"].ToString();

            var result = await UserManager.CreateAsync(user, register.Password);
            if(result.Succeeded)
            {
                await SignManager.SignInAsync(user, false);

                if (string.IsNullOrWhiteSpace(returnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }
            else
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty,error.Description);
                }
            }
            return View(register);
        }

        public async Task<IActionResult> Manage()
        {
            var user = await UserManager.GetUserAsync(User);

            Profile profile = new Profile()
            {
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber
            };

            Manage manage = new Manage()
            {
                SelectTab = ManageSelectTab.Profile,
                Profile = profile
            };
            return View(manage);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(Manage manage)
        {
            manage.SelectTab = ManageSelectTab.Profile;
            if(!ModelState.IsValid)
            {
                return View(manage);
            }
            if(manage.Profile==null||string.IsNullOrWhiteSpace(manage.Profile.UserName))
            {
                ModelState.AddModelError(string.Empty, "请输入用户名");
                return View(manage);
            }
            var user = await UserManager.GetUserAsync(User);
            if(user.UserName!=manage.Profile.UserName)
            {
                manage.Profile.UserName = manage.Profile.UserName.Trim();
                var existUserSameName = await UserManager.FindByNameAsync(manage.Profile.UserName);
                if (existUserSameName != null)
                {
                    ModelState.AddModelError(string.Empty, "用户名已被占用");
                    return View("Manage", manage);
                }
                var changeNameResult = await UserManager.SetUserNameAsync(user, manage.Profile.UserName);
                if (!changeNameResult.Succeeded)
                    throw new Exception("修改用户名失败！");
            }
            if(user.Email!=manage.Profile.Email)
            {
                manage.Profile.Email = manage.Profile.Email.Trim();
                if (!string.IsNullOrWhiteSpace(manage.Profile.Email))
                {
                    var existUserSameEmail = await UserManager.FindByEmailAsync(manage.Profile.Email);
                    if(existUserSameEmail!=null)
                    {
                        ModelState.AddModelError(string.Empty, "邮箱已被占用");
                        return View("Manage", manage);
                    }
                }
                var changeEmailResult = await UserManager.SetEmailAsync(user, manage.Profile.Email);
                if (!changeEmailResult.Succeeded)
                    throw new Exception("修改邮箱失败！");
            }
            if(user.PhoneNumber!=manage.Profile.Phone)
            {
                manage.Profile.Phone = manage.Profile.Phone.Trim();
                if (!string.IsNullOrWhiteSpace(manage.Profile.Phone))
                {
                    var existUserSamePhone = await IdentityContext.Users.AnyAsync(u => u.PhoneNumber == manage.Profile.Phone);
                    if(existUserSamePhone)
                    {
                        ModelState.AddModelError(string.Empty, "手机号码已被占用");
                        return View("Manage", manage);
                    }
                }
                var changePhoneResult = await UserManager.SetPhoneNumberAsync(user, manage.Profile.Phone);
                if (!changePhoneResult.Succeeded)
                    throw new Exception("修改手机号码失败");
            }
            return View("Manage",manage);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(Manage manage)
        {
            manage.SelectTab = ManageSelectTab.ChangePassword;
            return View(manage);
        }

        [HttpPost]
        public async Task<IActionResult> CloseAccount(Manage manage)
        {
            manage.SelectTab = ManageSelectTab.CloseAccount;
            return View();
        }
    }
}