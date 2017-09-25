using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WildeRoverMgmtApp.Models;

namespace WildeRoverMgmtApp.Controllers
{
    [Authorize]
    public class VendorItemsController : Controller
    {
        private readonly WildeRoverMgmtAppContext _context;

        public VendorItemsController(WildeRoverMgmtAppContext context)
        {
            _context = context;    
        }

        // GET: VendorItems
        public async Task<IActionResult> Index()
        {
            var items = (from item in _context.VendorItem.Include("Vendor")
                         orderby item.Vendor.Name, item.Name
                         select item);

            return View(await items.ToListAsync());
        }

        // GET: VendorItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendorItem = await _context.VendorItem.Include(m => m.Vendor)
                .Include(m => m.WildeRoverItem).SingleOrDefaultAsync(m => m.VendorItemId == id);
            if (vendorItem == null)
            {
                return NotFound();
            }

            return View(vendorItem);
        }

        // GET: VendorItems/Create
        public IActionResult Create()
        {          
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(VendorItem model)
        {
            if (ModelState.IsValid)
            {
                if (model.WildeRoverItemId == 0)
                {
                    ModelState.AddModelError("WildeRoverItemId", "You must select a house item.");
                    return View(model);
                }

                if (model.VendorId == 0)
                {
                    ModelState.AddModelError("VendorId", "You must select a vendor.");
                    return View(model);
                }

                _context.VendorItem.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "VendorItems");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var vendorItem = await (from item in _context.VendorItem
                                    where item.VendorItemId == id
                                    select item).SingleOrDefaultAsync();
            if (vendorItem == null) return NotFound();

            return View(vendorItem);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, VendorItem model)
        {
            if (model == null) return NotFound();
            if (id != model.VendorItemId) return NotFound();

            if (ModelState.IsValid)
            {
                if (model.VendorId == 0)
                {
                    ModelState.AddModelError("VendorId", "You must select a vendor.");
                    return View(model);
                }

                if (model.WildeRoverItemId == 0)
                {
                    ModelState.AddModelError("WildeRoverItemid", "You must selecta house item.");
                    return View(model);
                }

                try
                {
                    _context.VendorItem.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException)
                {
                    if (!VendorItemExists(model.VendorItemId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Index", "VendorItems");
            }

            return View(model);

        }

        public IActionResult Delete(int id)
        {
            return DeleteConfirmed(id).Result;
        }

        // POST: VendorItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vendorItem = await _context.VendorItem.SingleOrDefaultAsync(m => m.VendorItemId == id);
            _context.VendorItem.Remove(vendorItem);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index" );
        }

        private bool VendorItemExists(int id)
        {
            return _context.VendorItem.Any(e => e.VendorItemId == id);
        }
    }
}
