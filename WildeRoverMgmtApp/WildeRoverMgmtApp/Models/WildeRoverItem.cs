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
            front = 1,
            back = 2,
            both = 4
        };

        public WildeRoverItem()
        {
            VendorItems = new HashSet<VendorItem>();
        }

        #region Entity

        [Key]
        public int WildeRoverItemId { get; set; }
        public string Name { get; set; }
        public int Par { get; set; }
        public int Have { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public House ItemHouse { get; set; }

        public HashSet<VendorItem> VendorItems { get; set; }      
        
        [NotMapped]
        public int Need { get { return Par - Have; } }

        VendorItem _defaultVendorItem;
        [NotMapped]
        public VendorItem DefaultVendorItem
        {
            get
            {
                if (_defaultVendorItem == null)
                {
                    decimal minPPU = Decimal.MaxValue;
                    VendorItem returnVal = null;
                    foreach(var item in VendorItems)
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
                else
                {
                    return _defaultVendorItem;
                }

            }
            set { _defaultVendorItem = value; }
        }

        #endregion Entity

        #region IInventoriable

        //int _inventoryCount;

        //public void SetInventory(int count)
        //{
        //    if (count < 0)
        //        throw new ArgumentOutOfRangeException("Argument less than zero.");
        //    else
        //        _inventoryCount = count;
        //}

        public int InventoryCount { get; set; }

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
