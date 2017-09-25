using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WildeRoverMgmtApp.Models
{
    public partial class Vendor : IComparable<Vendor>
    {
        [Key]
        public int VendorId { get; set; }
        [Required]
        public string Name { get; set; }        
        public string Phone { get; set; }
        public string Email { get; set; }

        [Display(Name = "Point of Contact")]
        public string PointOfContact { get; set; }

        public virtual HashSet<VendorItem> Products { get; set; }

        public int CompareTo(Vendor other)
        {
            return this.Name.CompareTo(other.Name);
        }
    }
}
