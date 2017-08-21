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

            var inventory = await (from i in _context.InventoryLog.Include("Inventory.Item")
                                   where i.InventorySummaryId == id
                                   select i).SingleOrDefaultAsync();

            return View(inventory);
        }

        //GET
        [HttpGet]
        public async Task<IActionResult> Submit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await (from i in _context.InventoryLog.Include("Inventory.Item")
                                   where i.InventorySummaryId == id
                                   select i).SingleOrDefaultAsync();

            InventorySubmitViewModel isvm = new InventorySubmitViewModel();
            isvm.InventorySummaryId = inventory.InventorySummaryId;
            isvm.Summary = inventory;

            var types = (from ic in inventory.Inventory
                         select ic.Item.Type).Distinct().OrderBy(i => i);

            foreach(var type in types)
            {
                
                var items = (from ic in inventory.Inventory
                            where ic.Item.Type == type
                            orderby ic.Item.SubType, ic.Item.Name
                            select ic).ToList();

                isvm.SubItems.Add(type, items);
            }

            return View(isvm);

        }

        //POST
        [HttpPost]
        public async Task<IActionResult> Submit(int? id, InventorySubmitViewModel isvm)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var summary = await (from log in _context.InventoryLog
                                     where log.InventorySummaryId == isvm.InventorySummaryId
                                     select log).SingleOrDefaultAsync();

                    //====EMAIL PARTIES HERE====


                    summary.Submitted = true;

                    _context.Update(summary);
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