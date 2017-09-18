using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WildeRoverMgmtApp.Models
{
    public class AccountResetPasswordViewModel
    {
        public string Id { get; set; }
        public string Token { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string RePassword { get; set; }
    }
}
