using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WildeRoverMgmtApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace WildeRoverMgmtApp.Controllers
{
    [Authorize]
    public class VendorsController : Controller
    {
        private readonly WildeRoverMgmtAppContext _context;

        public VendorsController(WildeRoverMgmtAppContext context)
        {
            _context = context;    
        }

        // GET: Vendors
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Vendor.ToListAsync());
        }

        // GET: Vendors/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendor = await _context.Vendor
                .SingleOrDefaultAsync(m => m.VendorId == id);
            if (vendor == null)
            {
                return NotFound();
            }

            return View(vendor);
        }

        [Authorize]
        // GET: Vendors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vendors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("VendorId,Name,Phone,Email,PointOfContact")] Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vendor);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(vendor);
        }

        // GET: Vendors/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendor = await _context.Vendor.SingleOrDefaultAsync(m => m.VendorId == id);
            if (vendor == null)
            {
                return NotFound();
            }
            return View(vendor);
        }

        // POST: Vendors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("VendorId,Name,Phone,EMail,PointOfContact")] Vendor vendor)
        {
            if (id != vendor.VendorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vendor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendorExists(vendor.VendorId))
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
            return View(vendor);
        }

        public Task<IActionResult> Delete(int id)
        {
            return DeleteConfirmed(id);
        }

        // POST: Vendors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vendor = await _context.Vendor.SingleOrDefaultAsync(m => m.VendorId == id);
            if (vendor == null) return NotFound();

            _context.Vendor.Remove(vendor);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool VendorExists(int id)
        {
            return _context.Vendor.Any(e => e.VendorId == id);
        }
    }
}
