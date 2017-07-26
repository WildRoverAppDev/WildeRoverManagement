using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;

namespace WildeRoverMgmtApp.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new WildeRoverMgmtAppContext(
                serviceProvider.GetRequiredService<DbContextOptions<WildeRoverMgmtAppContext>>()))
            {
                if (context.WildeRoverItem.Any()) return;  //No need to seed data

                SeedVendors(context);
                SeedWildeRoverItems(context);
                SeedVendorItems(context);

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
                    ItemHouse = WildeRoverItem.House.front,
                },

                new WildeRoverItem
                {
                    Name = "Blue Moon",
                    Par = 50,
                    Have = 30,
                    Type = "Beer",
                    ItemHouse = WildeRoverItem.House.front,
                },

                new WildeRoverItem
                {
                    Name = "Captain Morgan",
                    Par = 6,
                    Have = 5,
                    Type = "Rum",
                    ItemHouse = WildeRoverItem.House.front,
                },

                new WildeRoverItem
                {
                    Name = "Sky",
                    Par = 10,
                    Have = 7,
                    Type = "Vodka",
                    ItemHouse = WildeRoverItem.House.front,
                },

                new WildeRoverItem
                {
                    Name = "Riesling",
                    Par = 5,
                    Have = 3,
                    Type = "Wine",
                    ItemHouse = WildeRoverItem.House.front,
                },

                new WildeRoverItem
                {
                    Name = "Hennessy",
                    Par = 3,
                    Have = 3,
                    Type = "Cognac",
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

        //private static void PopulateVendorProducts(WildeRoverMgmtAppContext context)
        //{
        //    foreach (var vendor in context.Vendor)
        //    {
        //        var items = from i in context.VendorItem
        //                    where i.Vendor == vendor
        //                    select i;

        //        foreach (var i in items)
        //        {
        //            vendor.Products.Add(i);
        //        }

        //        context.Update(vendor);
        //    }

        //    context.SaveChanges();
        //}

        //    private static void PopulateWildeRoverItemVendorItems(WildeRoverMgmtAppContext context)
        //    {
        //        foreach(var wr in context.WildeRoverItem)
        //        {
        //            var vItems = from vi in context.VendorItem
        //                         where vi.Name.ToLower().Contains(wr.Name.ToLower())
        //                         select vi;
        //            foreach(var vi in vItems)
        //            {
        //                wr.VendorItems.Add(vi);
        //            }

        //            context.Update(wr);
        //        }

        //        if (context.SaveChanges() == 0)
        //        {
        //            Console.WriteLine("Save Error");
        //        }
        //        else
        //        {
        //            Console.WriteLine("OK");
        //        }
        //    }
        //}

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

    }
}
