using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WildeRoverMgmtApp.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetService<UserManager<User>>();

            using (var context = new WildeRoverMgmtAppContext(
                serviceProvider.GetRequiredService<DbContextOptions<WildeRoverMgmtAppContext>>()))
            {
                if (context.WildeRoverItem.Any()) return;  //No need to seed data

                SeedUsers(context, userManager);
                SeedVendors(context);
                SeedWildeRoverItems(context);
                SeedVendorItems(context);
                SeedInventoryArea(context);
                SeedSlots(context);

                //EagerLoadWildeRoverItems(context);

                //PopulateVendorProducts(context);
                //PopulateWildeRoverItemVendorItems(context);
                //PopulateVendorItems(context);
            }

            return;
        }

        private static void EagerLoadWildeRoverItems(WildeRoverMgmtAppContext context)
        {
            foreach (var wr in context.WildeRoverItem)
            {
                var vendorItems = from vi in context.VendorItem
                                  where vi.WildeRoverItemId == wr.WildeRoverItemId
                                  select vi;

                foreach(var vi in vendorItems)
                {
                    wr.VendorItems.Add(vi);
                }

                context.Update(wr);
            }

            context.SaveChanges();
        }

        public static void SeedUsers(WildeRoverMgmtAppContext context, UserManager<User> userManager)
        {
            var user0 = new User
            {
                UserName = "OscarWilde",
                LastName = "Wilde",
                FirstName = "Oscar",
                PhoneNumber = "425-822-8940",
                Email = "info@wilderover.com",
                Privilege = 3
            };

            var result = userManager.CreateAsync(user0, "P@ssw0rd!").Result;

            context.SaveChanges();
        }

        public static void SeedVendors(WildeRoverMgmtAppContext context)
        {
            //if (context.Vendor.Any()) return;

            context.Vendor.AddRange(
                new Vendor
                {
                    Name = "Safeway",
                    Phone = "(555)555-5555",
                    EMail = "email@safeway.com",
                    PointOfContact = "Fred"
                },

                new Vendor
                {
                    Name = "QFC",
                    Phone = "(111)111-11111",
                    EMail = "email@qfc.com",
                    PointOfContact = "Bob"
                },

                new Vendor
                {
                    Name = "Whole Foods",
                    Phone = "(222)222-2222",
                    EMail = "email@wholefoods.com",
                    PointOfContact = "Tom"
                }
            );

            context.SaveChanges();
        }

        public static void SeedWildeRoverItems(WildeRoverMgmtAppContext context)
        {
            //if (context.WildeRoverItem.Any()) return;

            context.WildeRoverItem.AddRange
            (
                new WildeRoverItem
                {
                    Name = "Bud Light",
                    Par = 60,
                    Have = 40,
                    Type = "Beer",
                    SubType = "Bottle",
                    ItemHouse = WildeRoverItem.House.front,
                },

                new WildeRoverItem
                {
                    Name = "Blue Moon",
                    Par = 50,
                    Have = 30,
                    Type = "Beer",
                    SubType = "Bottle",
                    ItemHouse = WildeRoverItem.House.front,
                },

                new WildeRoverItem
                {
                    Name = "Captain Morgan",
                    Par = 6,
                    Have = 5,
                    Type = "Liquor",
                    SubType = "Rum",
                    ItemHouse = WildeRoverItem.House.front,
                },

                new WildeRoverItem
                {
                    Name = "Sky",
                    Par = 10,
                    Have = 7,
                    Type = "Liquor",
                    SubType = "Vodka",
                    ItemHouse = WildeRoverItem.House.front,
                },

                new WildeRoverItem
                {
                    Name = "Riesling",
                    Par = 5,
                    Have = 3,
                    Type = "Wine",
                    SubType = "White",
                    ItemHouse = WildeRoverItem.House.front,
                },

                new WildeRoverItem
                {
                    Name = "Hennessy",
                    Par = 3,
                    Have = 3,
                    Type = "Liquor",
                    SubType = "Cognac",
                    ItemHouse = WildeRoverItem.House.front,
                }
            );

            context.SaveChanges();
        }

        public static void SeedVendorItems(WildeRoverMgmtAppContext context)
        {
            //if (context.VendorItem.Any()) return;

            context.VendorItem.AddRange(
                new VendorItem
                {
                    Name = "Bud Light",
                    PackSize = 6,
                    Price = 9.99m,
                    //VendorId = (from v in context.Vendor
                    //            where v.Name == "Safeway"
                    //            select v).SingleOrDefault().VendorId,

                    Vendor = (from v in context.Vendor
                              where v.Name == "Safeway"
                              select v).SingleOrDefault(),

                    WildeRoverItem = (from wr in context.WildeRoverItem
                                      where wr.Name.ToLower().Contains("Bud Light")
                                      select wr).SingleOrDefault()

                },

                new VendorItem
                {
                    Name = "Blue Moon",
                    PackSize = 6,
                    Price = 11.99m,
                    Vendor = (from v in context.Vendor
                              where v.Name == "Safeway"
                              select v).SingleOrDefault(),

                    WildeRoverItem = (from wr in context.WildeRoverItem
                                      where wr.Name.ToLower().Contains("Blue Moon")
                                      select wr).SingleOrDefault()
                },

                new VendorItem
                {
                    Name = "Bud Light",
                    PackSize = 6,
                    Price = 10.99m,
                    Vendor = (from v in context.Vendor
                              where v.Name == "QFC"
                              select v).SingleOrDefault(),

                    WildeRoverItem = (from wr in context.WildeRoverItem
                                      where wr.Name.ToLower().Contains("Bud Light")
                                      select wr).SingleOrDefault()
                },

                new VendorItem
                {
                    Name = "Captain Morgan",
                    PackSize = 1,
                    Price = 19.99m,
                    Vendor = (from v in context.Vendor
                              where v.Name == "QFC"
                              select v).SingleOrDefault(),

                    WildeRoverItem = (from wr in context.WildeRoverItem
                                      where wr.Name.ToLower().Contains("Captain Morgan")
                                      select wr).SingleOrDefault()
                },

                new VendorItem
                {
                    Name = "Sky",
                    PackSize = 1,
                    Price = 29.99m,
                    Vendor = (from v in context.Vendor
                              where v.Name == "QFC"
                              select v).SingleOrDefault(),

                    WildeRoverItem = (from wr in context.WildeRoverItem
                                      where wr.Name.ToLower().Contains("Sky")
                                      select wr).SingleOrDefault()
                },

                new VendorItem
                {
                    Name = "Chateau St.Michelle Riesling",
                    PackSize = 1,
                    Price = 21.99m,
                    Vendor = (from v in context.Vendor
                              where v.Name == "Whole Foods"
                              select v).SingleOrDefault(),

                    WildeRoverItem = (from wr in context.WildeRoverItem
                                      where wr.Name.ToLower().Contains("Riesling")
                                      select wr).SingleOrDefault()
                },

                new VendorItem
                {
                    Name = "Hennessy",
                    PackSize = 1,
                    Price = 29.99m,
                    Vendor = (from v in context.Vendor
                              where v.Name == "Whole Foods"
                              select v).SingleOrDefault(),

                    WildeRoverItem = (from wr in context.WildeRoverItem
                                      where wr.Name.ToLower().Contains("Hennessy")
                                      select wr).SingleOrDefault()
                }
            );

            context.SaveChanges();
        }

        private static void PopulateVendorItems(WildeRoverMgmtAppContext context)
        {
            var vendorItems = from vi in context.VendorItem
                              select vi;

            var vendors = from v in context.Vendor
                          select v;

            var wrItems = from w in context.WildeRoverItem
                          select w;

            foreach(var vi in vendorItems)
            {
                //Add WildeRoverItem
                var temp = from wr in wrItems
                           where vi.Name.ToLower().Contains(wr.Name.ToLower())
                           select wr;

                vi.WildeRoverItem = temp.SingleOrDefault();
                vi.WildeRoverItemId = vi.WildeRoverItem.WildeRoverItemId;

                context.Update(vi);
            }

            context.SaveChanges();
        }

        private static void SeedInventoryArea(WildeRoverMgmtAppContext context)
        {
            context.InventoryAreas.AddRange
                (
                    new InventoryArea
                    {
                        Name = "Bar1"
                    },

                    new InventoryArea
                    {
                        Name = "Bar2"
                    },

                    new InventoryArea
                    {
                        Name = "Bar3"
                    }

                );

            context.SaveChanges();
        }

        private static void SeedSlots(WildeRoverMgmtAppContext context)
        {
            var bar1 = (from ia in context.InventoryAreas
                        where ia.Name == "Bar1"
                        select ia).SingleOrDefault();

            var bar2 = (from ia in context.InventoryAreas
                        where ia.Name == "Bar2"
                        select ia).SingleOrDefault();

            var bar3 = (from ia in context.InventoryAreas
                        where ia.Name == "Bar3"
                        select ia).SingleOrDefault();

            var item1 = (from i in context.WildeRoverItem
                         where i.Name == "Sky"
                         select i).SingleOrDefault();

            var item2 = (from i in context.WildeRoverItem
                         where i.Name == "Captain Morgan"
                         select i).SingleOrDefault();

            var item3 = (from i in context.WildeRoverItem
                         where i.Name == "Hennessy"
                         select i).SingleOrDefault();

            var item4 = (from i in context.WildeRoverItem
                         where i.Name == "Riesling"
                         select i).SingleOrDefault();

            var item5 = (from i in context.WildeRoverItem
                         where i.Name == "Bud Light"
                         select i).SingleOrDefault();

            InventorySlot slot1 = new InventorySlot();
            slot1.InventoryArea = bar1;
            slot1.InventoryAreaId = bar1.InventoryAreaId;
            slot1.Slot = 1;
            slot1.WildeRoverItem = item1;
            slot1.WildeRoverItemId = item1.WildeRoverItemId;

            InventorySlot slot2 = new InventorySlot();
            slot2.InventoryArea = bar1;
            slot2.InventoryAreaId = bar1.InventoryAreaId;
            slot2.Slot = 2;
            slot2.WildeRoverItem = item2;
            slot2.WildeRoverItemId = item2.WildeRoverItemId;

            InventorySlot slot3 = new InventorySlot();
            slot3.InventoryArea = bar1;
            slot3.InventoryAreaId = bar1.InventoryAreaId;
            slot3.Slot = 3;
            slot3.WildeRoverItem = item3;
            slot3.WildeRoverItemId = item3.WildeRoverItemId;

            InventorySlot slot4 = new InventorySlot();
            slot4.InventoryArea = bar2;
            slot4.InventoryAreaId = bar2.InventoryAreaId;
            slot4.Slot = 1;
            slot4.WildeRoverItem = item4;
            slot4.WildeRoverItemId = item4.WildeRoverItemId;

            InventorySlot slot5 = new InventorySlot();
            slot5.InventoryArea = bar2;
            slot4.InventoryAreaId = bar2.InventoryAreaId;
            slot5.Slot = 2;
            slot5.WildeRoverItem = item5;
            slot5.WildeRoverItemId = item5.WildeRoverItemId;

            InventorySlot slot6 = new InventorySlot();
            slot6.InventoryArea = bar3;
            slot6.InventoryAreaId = bar3.InventoryAreaId;
            slot6.Slot = 1;
            slot6.WildeRoverItem = item5;
            slot6.WildeRoverItemId = item5.WildeRoverItemId;

            context.Slots.AddRange(slot1, slot2, slot3, slot4, slot5, slot5, slot6);
            context.SaveChanges();

        }

    }
}
