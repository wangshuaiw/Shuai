﻿using System;
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
    }
}