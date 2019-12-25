using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shuai.IdentityServer.V1._0.Models.Account
{
    public class Manage
    {
        public ManageSelectTab SelectTab { set; get; }

        //public bool Success { set; get; }

        public string StatusMessage { set; get; }

        public Profile Profile { set; get; }

        public ChangePassword ChangePassword { set; get; }

        public static string ActiveProfileTab(ManageSelectTab tab)
        {
            if (tab == ManageSelectTab.Profile)
                return "active";
            else
                return "";
        }

        public static string ActivePasswordTab(ManageSelectTab tab)
        {
            if (tab == ManageSelectTab.ChangePassword)
                return "active";
            else
                return "";
        }

        public static string ActiveCloseTab(ManageSelectTab tab)
        {
            return tab == ManageSelectTab.CloseAccount ? "active" : "";
        }

    }

    public enum ManageSelectTab
    {
        Profile,
        ChangePassword,
        CloseAccount
    }

    public class Profile
    {
        [Display(Name = "用户名")]
        public string UserName { set; get; }

        [Display(Name = "邮箱")]
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        public string Email { set; get; }

        [Display(Name = "手机号码")]
        [Phone(ErrorMessage = "手机号码格式不正确")]
        public string Phone { set; get; }
    }

    public class ChangePassword
    {
        [Display(Name = "原密码")]
        [DataType(DataType.Password)]
        public string OldPassword { set; get; }

        [Display(Name = "新密码")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密码最少6个字符，最多20个字符")]
        public string NewPassword { set; get; }

        [Display(Name = "确认密码")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "密码不一致")]
        public string ConfirmPassword { set; get; }
    }
}
