using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shuai.IdentityServer.V1._0.Models.Account
{
    public class Register
    {
        public string RegisterType { set; get; }

        [Display(Name ="用户名")]
        public string UserName { set; get; }

        [Display(Name ="邮箱")]
        [EmailAddress(ErrorMessage ="邮箱格式不正确")]
        public string Email { set; get; }

        [Display(Name ="手机号码")]
        [Phone(ErrorMessage ="手机号码格式不正确")]
        public string Phone { set; get; }

        [Required(ErrorMessage ="密码不能为空")]
        [Display(Name ="密码")]
        [DataType(DataType.Password)]
        [StringLength(20,MinimumLength =6,ErrorMessage ="密码最少6个字符，最多20个字符")]
        public string Password { set; get; }

        [Display(Name ="确认密码")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="密码不一致")]
        public string ConfirmPassword { set; get; }
    }
}
