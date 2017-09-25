using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace WildeRoverMgmtApp.Models
{
    public partial class WildeRoverItem : IInventoriable, IOrderable, IComparable<WildeRoverItem>
    {
        public enum House
        {
            [Display(Name = "Front of House")]
            front = 1,
            [Display(Name = "Back of House")]
            back = 2,
            [Display(Name = "Front and Back")]
            both = 3
        };

        public WildeRoverItem()
        {
            VendorItems = new HashSet<VendorItem>();
        }

        #region Entity

        [Key]
        public int WildeRoverItemId { get; set; }

        [Display(Name = "House Item")]
        public string Name { get; set; }

        public int Par { get; set; }

        [Range(0,int.MaxValue)]
        public int Have { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string SubType { get; set; }

        [Required]
        public House ItemHouse { get; set; }

        public HashSet<VendorItem> VendorItems { get; set; }      
        
        [NotMapped]
        public int Need { get { return Par - Have; } }

        //public int? DefaultVendorItemId { get; set; }
        //private VendorItem _defaultVendorItem;
        [NotMapped]
        public VendorItem DefaultVendorItem
        {
            get
            {
                decimal minPPU = Decimal.MaxValue;
                VendorItem returnVal = null;
                foreach (var item in VendorItems)
                {
                    decimal pricePerUnit = item.Price / item.PackSize;
                    if (pricePerUnit < minPPU)
                    {
                        minPPU = pricePerUnit;
                        returnVal = item;
                    }
                }

                return returnVal;
            }         
        }

        //public VendorItem GetDefaultVendorItem ()
        //{
        //    if (DefaultVendorItemId == null)
        //    {
        //        decimal minPPU = Decimal.MaxValue;
        //        VendorItem returnVal = null;
        //        foreach (var item in VendorItems)
        //        {
        //            decimal pricePerUnit = item.Price / item.PackSize;
        //            if (pricePerUnit < minPPU)
        //            {
        //                minPPU = pricePerUnit;
        //                returnVal = item;
        //            }
        //        }

        //        return returnVal;
        //    }
        //    else
        //    {
        //        return DefaultVendorItem;
        //    }
        //}

        ////[NotMapped]
        //[Display(Name = "Default Vendor Item")]
        //public VendorItem DefaultVendorItem
        //{
        //    get
        //    {
        //        if (DefaultVendorItemId == null)
        //        {
        //            decimal minPPU = Decimal.MaxValue;
        //            VendorItem returnVal = null;
        //            foreach (var item in VendorItems)
        //            {
        //                decimal pricePerUnit = item.Price / item.PackSize;
        //                if (pricePerUnit < minPPU)
        //                {
        //                    minPPU = pricePerUnit;
        //                    returnVal = item;
        //                }
        //            }

        //            return returnVal;
        //        }
        //        else
        //        {
        //            return _defaultVendorItem;
        //        }

        //    }
        //    set { _defaultVendorItem = value; }
        //}

        #endregion Entity

        #region IInventoriable

        //TODO: Implement
        public string ToStringInventory()
        {
            throw new NotImplementedException();
        }

        #endregion IInventoriable

        #region IOrderable

        public int OrderCount { get; set; }

        #endregion IOrderable

        
        //TODO: Implement
        public string ToStringOrder()
        {
            throw new NotImplementedException();
        }

        #region IComparable

        public int CompareTo(WildeRoverItem other)
        {
            if (this.Type == other.Type)
            {
                return this.Name.CompareTo(other.Name);
            }
            else
            {
                return this.Type.CompareTo(other.Type);
            }
        }

        #endregion IComparable



    }
}
