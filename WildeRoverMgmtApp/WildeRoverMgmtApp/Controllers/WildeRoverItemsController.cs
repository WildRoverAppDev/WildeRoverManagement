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
    public class WildeRoverItemsController : Controller
    {
        private readonly WildeRoverMgmtAppContext _context;

        public WildeRoverItemsController(WildeRoverMgmtAppContext context)
        {
            _context = context;
        }

        // GET: WildeRoverItems
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.WildeRoverItem.ToListAsync());
        }


        // GET: WildeRoverItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wildeRoverItem = await _context.WildeRoverItem.Include("VendorItems.Vendor")
                .SingleOrDefaultAsync(m => m.WildeRoverItemId == id);
            if (wildeRoverItem == null)
            {
                return NotFound();
            }


            return View(wildeRoverItem);
        }

        // GET: WildeRoverItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WildeRoverItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WildeRoverItemId,Name,Par,Have,Type, SubType")] WildeRoverItem wildeRoverItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wildeRoverItem);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }        

            return View(wildeRoverItem);            
        }

        // GET: WildeRoverItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wildeRoverItem = await _context.WildeRoverItem.SingleOrDefaultAsync(m => m.WildeRoverItemId == id);
            if (wildeRoverItem == null)
            {
                return NotFound();
            }

            return View(wildeRoverItem);
        }

        // POST: WildeRoverItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WildeRoverItemId,Name,Par,Have,Type,SubType")] WildeRoverItem wildeRoverItem)
        {
            if (id != wildeRoverItem.WildeRoverItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wildeRoverItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WildeRoverItemExists(wildeRoverItem.WildeRoverItemId))
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
            return View(wildeRoverItem);
        }

        [HttpGet]
        public Task<IActionResult> Delete(int id)
        {
            return DeleteConfirmed(id);
        }

        // POST: WildeRoverItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wildeRoverItem = await _context.WildeRoverItem.SingleOrDefaultAsync(m => m.WildeRoverItemId == id);
            _context.WildeRoverItem.Remove(wildeRoverItem);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool WildeRoverItemExists(int id)
        {
            return _context.WildeRoverItem.Any(e => e.WildeRoverItemId == id);
        }
    }
}
