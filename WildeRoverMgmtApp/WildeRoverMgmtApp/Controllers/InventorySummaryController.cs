using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WildeRoverMgmtApp.Models;
using Microsoft.EntityFrameworkCore;

namespace WildeRoverMgmtApp.Controllers
{
    [Authorize]
    public class InventorySummaryController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly WildeRoverMgmtAppContext _context;

        public InventorySummaryController(WildeRoverMgmtAppContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Main Page for Inventory Summary, shows a list of all InventorySummaries
        public async Task<IActionResult> Index()
        {
            var summaries = from s in _context.InventoryLog
                            orderby s.Date descending
                            select s;

            return View(await summaries.ToListAsync());
        }

        //Shows Details of an InventorySummary
        //id - Id of InventorySummary
        public async Task<IActionResult> Details(int ?id)
        {
            //Validate
            if (id == null)
            {
                return NotFound();
            }

            //get Inventory Summary
            var summary = await (from i in _context.InventoryLog.Include("InventoryAreaLogs.Inventory.Item")
                                   where i.InventorySummaryId == id
                                   select i).SingleOrDefaultAsync();

            //Create view model
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
        
        //Submit Page for InventorySummary
        //id - Id for InventorySummary
        [HttpGet]
        public async Task<IActionResult> Submit(int? id)
        {
            if (id == null) return NotFound();  //Validate

            //get inventory
            var inventory = await (from i in _context.InventoryLog.Include("InventoryAreaLogs.Inventory.Item")
                                   where i.InventorySummaryId == id
                                   select i).SingleOrDefaultAsync();

            //Get Items
            var items = from i in _context.WildeRoverItem
                        orderby i.Type, i.SubType, i.Name
                        select i;

            var itemList = items.ToList();

            //Create View Model
            InventorySummarySubmitViewModel model = new InventorySummarySubmitViewModel();
            model.Summary = inventory;
            model.InventorySummaryId = inventory.InventorySummaryId;

            //Track new ItemCounts in Dictionary in addition to ViewModel to make tallying
            //inventory not O(n^3)
            var itemCountDict = new Dictionary<int, ItemCount>();

            //Populate ViewModel Inventory
            foreach(var i in itemList)
            {
                //check Key for Type, if not add KeyValuePair
                if (!model.SubItems.ContainsKey(i.Type))
                {
                    model.SubItems.Add(i.Type, new List<ItemCount>());
                }

                //Add itemCount to Value

                //Create ItemCount, set initial inventory count to 0
                ItemCount temp = new ItemCount();
                temp.Item = i;
                temp.Count = 0;

                model.SubItems[i.Type].Add(temp);  //Add
                itemCountDict[i.WildeRoverItemId] = temp;
            }
            
            //Tally Inventory and update ItemCounts for View Model using Dictionary
            foreach(var areaLog in inventory.InventoryAreaLogs)
            {
                foreach(var ic in areaLog.Inventory)
                {
                    itemCountDict[ic.WildeRoverItemId].Count += ic.Count;
                }
            }
  
            return View(model);
        }

        //Submit Post Method
        [HttpPost]
        public async Task<IActionResult> Submit(int? id, InventorySummarySubmitViewModel isvm)
        {
            //Validate
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //save itemCounts

                    //Get InventorySummary
                    var summary = await (from log in _context.InventoryLog.Include("InventoryAreaLogs.Inventory.Item")
                                     where log.InventorySummaryId == isvm.InventorySummaryId
                                     select log).SingleOrDefaultAsync();

                    //Create Dictionary of Items for O(n) tallying
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

                            _context.WildeRoverItem.Update(temp);  //Update context
                        }
                    }

                    //Change LastEdited
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null) throw new InvalidOperationException();

                    summary.LastEdited = user.FullName;

                    _context.InventoryLog.Update(summary);

                    //====EMAIL PARTIES HERE===================================



                    //=========================================================

                    //Update InventorySummary status
                    summary.Submitted = true;
                    _context.InventoryLog.Update(summary);

                    await _context.SaveChangesAsync();  //Save context
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