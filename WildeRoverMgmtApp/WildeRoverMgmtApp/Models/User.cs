using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WildeRoverMgmtApp.Models
{
    public partial class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Privilege { get; set; }
    }
}
