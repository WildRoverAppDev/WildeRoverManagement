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
            return View(await _context.VendorItem.Include("Vendor").ToListAsync());
        }

        // GET: VendorItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendorItem = await _context.VendorItem
                .SingleOrDefaultAsync(m => m.VendorItemId == id);
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

        // POST: VendorItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VendorItemId,Name,PackSize,Price")] VendorItem vendorItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vendorItem);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(vendorItem);
        }

        // GET: VendorItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendorItem = await _context.VendorItem.SingleOrDefaultAsync(m => m.VendorItemId == id);
            if (vendorItem == null)
            {
                return NotFound();
            }
            return View(vendorItem);
        }

        // POST: VendorItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VendorItemId,Name,PackSize,Price")] VendorItem vendorItem)
        {
            if (id != vendorItem.VendorItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vendorItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendorItemExists(vendorItem.VendorItemId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(vendorItem);
        }

        // GET: VendorItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendorItem = await _context.VendorItem
                .SingleOrDefaultAsync(m => m.VendorItemId == id);
            if (vendorItem == null)
            {
                return NotFound();
            }

            return View(vendorItem);
        }

        // POST: VendorItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vendorItem = await _context.VendorItem.SingleOrDefaultAsync(m => m.VendorItemId == id);
            _context.VendorItem.Remove(vendorItem);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool VendorItemExists(int id)
        {
            return _context.VendorItem.Any(e => e.VendorItemId == id);
        }
    }
}
