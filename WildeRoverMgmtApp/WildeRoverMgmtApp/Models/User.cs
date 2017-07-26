using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WildeRoverMgmtApp.Models
{
    public partial class User
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public Contact Contact { get; set; }
        public string Phone { get; set; }
        public string EMail { get; set; }

        public int Privilege { get; set; }
    }
}
