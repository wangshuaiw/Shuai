using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shuai.IdentityServer.V1._0.Models.Account
{
    public class Login
    {
        [Required]
        [Display(Name ="用户名/邮箱/手机号码")]
        public string NameOrEmailOrPhone { set; get; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="密码")]
        public string Password { set; get; }

        [Display(Name = "记住密码")]
        public bool RememberMe { set; get; }


    }
}
