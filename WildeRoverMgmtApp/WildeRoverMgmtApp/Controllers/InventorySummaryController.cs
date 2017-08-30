using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WildeRoverMgmtApp.Models;
using Microsoft.EntityFrameworkCore;

namespace WildeRoverMgmtApp.Controllers
{
    public class InventorySummaryController : Controller
    {
        private readonly WildeRoverMgmtAppContext _context;

        public InventorySummaryController(WildeRoverMgmtAppContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var summaries = from s in _context.InventoryLog
                            orderby s.Date descending
                            select s;

            return View(summaries.ToList());
        }

        public async Task<IActionResult> Details(int ?id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //get Inventory Summary
            var summary = await (from i in _context.InventoryLog.Include("InventoryAreaLogs.Inventory.Item")
                                   where i.InventorySummaryId == id
                                   select i).SingleOrDefaultAsync();

            //get Inventory Area
            InventorySummaryDetailsViewModel model = new InventorySummaryDetailsViewModel();
            model.Summary = summary;


            //Generate model entries
            var items = await (from i in _context.WildeRoverItem
                               orderby i.Type, i.SubType, i.Name
                               select i).ToListAsync();

            foreach (var item in items)
            {
                model.Inventory.Add(item, 0);
            }


            //foreach (var ic in summary.Inventory)
            //{
            //    model.Inventory.Add(ic.Item, 0);
            //}

            //Calculate inventory
            foreach (var areaLog in summary.InventoryAreaLogs)
            {
                foreach (var ic in areaLog.Inventory)
                {
                    model.Inventory[ic.Item] += ic.Count;
                }
            }
            

            return View(model);
        }

        ////GET
        //[HttpGet]
        //public async Task<IActionResult> Submit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var inventory = await (from i in _context.InventoryLog.Include("Inventory.Item").Include("InventoryAreaLogs.Inventory.Item")
        //                           where i.InventorySummaryId == id
        //                           select i).SingleOrDefaultAsync();

        //    InventorySubmitViewModel isvm = new InventorySubmitViewModel();
        //    isvm.InventorySummaryId = inventory.InventorySummaryId;
        //    isvm.Summary = inventory;

        //    var types = (from ic in inventory.Inventory
        //                 select ic.Item.Type).Distinct().OrderBy(i => i);

        //    foreach(var type in types)
        //    {
                
        //        var items = (from ic in inventory.Inventory
        //                    where ic.Item.Type == type
        //                    orderby ic.Item.SubType, ic.Item.Name
        //                    select ic).ToList();

        //        isvm.SubItems.Add(type, items);
        //    }

        //    return View(isvm);

        //}
        [HttpGet]
        public async Task<IActionResult> Submit(int? id)
        {
            if (id == null) return NotFound();

            //Tally Area Inventory Logs

            //get inventory
            var inventory = await (from i in _context.InventoryLog.Include("InventoryAreaLogs.Inventory.Item")
                                   where i.InventorySummaryId == id
                                   select i).SingleOrDefaultAsync();

            //Get Items
            var items = from i in _context.WildeRoverItem
                        orderby i.Type, i.SubType, i.Name
                        select i;

            //var items = from ic in inventory.Inventory
            //            orderby ic.Item.Type, ic.Item.SubType, ic.Item.Name
            //            select ic;

            //var itemList = items.ToList();
            //var itemDict = itemList.ToDictionary(t => t.WildeRoverItemId);

            var itemList = items.ToList();

            //Get Item Categories
            //var categories = (from i in inventory.Inventory
            //                  orderby i.Item.Type ascending
            //                  select i.Item.Type).Distinct();

            InventorySummarySubmitViewModel model = new InventorySummarySubmitViewModel();
            model.Summary = inventory;
            model.InventorySummaryId = inventory.InventorySummaryId;

            var itemCountDict = new Dictionary<int, ItemCount>();

            foreach(var i in itemList)
            {
                //check Key for Type
                if (!model.SubItems.ContainsKey(i.Type))
                {
                    model.SubItems.Add(i.Type, new List<ItemCount>());
                }

                ItemCount temp = new ItemCount();
                temp.Item = i;
                temp.Count = 0;

                model.SubItems[i.Type].Add(temp);
                itemCountDict[i.WildeRoverItemId] = temp;
            }
            

            foreach(var areaLog in inventory.InventoryAreaLogs)
            {
                foreach(var ic in areaLog.Inventory)
                {
                    itemCountDict[ic.WildeRoverItemId].Count += ic.Count;
                }
            }
  
            return View(model);

        }

        //POST
        [HttpPost]
        public async Task<IActionResult> Submit(int? id, InventorySummarySubmitViewModel isvm)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //save itemCounts
                    var summary = await (from log in _context.InventoryLog.Include("InventoryAreaLogs.Inventory.Item")
                                     where log.InventorySummaryId == isvm.InventorySummaryId
                                     select log).SingleOrDefaultAsync();

                    var items = await (from i in _context.WildeRoverItem
                                       select i).ToDictionaryAsync(t => t.WildeRoverItemId);

                    //Reset item Have values
                    foreach(var item in items)
                    {
                        item.Value.Have = 0;
                    }

                    //Save Inventory counts to Item Have
                    foreach(var log in summary.InventoryAreaLogs)
                    {
                        foreach(var item in log.Inventory)
                        {
                            WildeRoverItem temp = items[item.WildeRoverItemId];
                            temp.Have += item.Count;

                            _context.WildeRoverItem.Update(temp);
                        }
                    }


                    //====EMAIL PARTIES HERE====


                    summary.Submitted = true;

                    _context.InventoryLog.Update(summary);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}