using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WildeRoverMgmtApp.Models;

namespace WildeRoverMgmtApp.Controllers
{
    public class OrderSummaryController : Controller
    {

        private readonly WildeRoverMgmtAppContext _context;

        public OrderSummaryController(WildeRoverMgmtAppContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var order = from o in _context.OrderLog
                        orderby o.Date descending
                        select o;

            return View(await order.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await (from o in _context.OrderLog.Include("OrderList.Item")
                               where o.OrderSummaryId == id
                               select o).SingleOrDefaultAsync();

            if (order == null) return NotFound();

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> Submit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await (from log in _context.OrderLog.Include("OrderList.Item.VendorItems.Vendor")
                               where log.OrderSummaryId == id
                               select log).SingleOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            OrderSubmitViewModel osvm = new OrderSubmitViewModel();
            osvm.Order = order;
            osvm.OrderSummaryId = order.OrderSummaryId;
            osvm.Total = order.CalculateTotal();

            List<Vendor> vendorList = (from ic in order.OrderList
                                       select ic.Item.DefaultVendorItem.Vendor).Distinct().ToList();

            foreach (var vendor in vendorList)
            {
                var items = (from ic in order.OrderList
                             where ic.Item.DefaultVendorItem.Vendor == vendor
                             orderby ic.Item.SubType, ic.Item.Name
                             select ic).ToList();

                osvm.OrderItems.Add(vendor.Name, items);
            }

            return View(osvm);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(int? id, OrderSubmitViewModel osvm)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var order = await (from log in _context.OrderLog.Include("OrderList.Item.VendorItems.Vendor")
                                       where log.OrderSummaryId == id
                                       select log).SingleOrDefaultAsync();

                    //====EMAIL VENDORS HERE====

                    //==========================

                    order.Completed = true;
                    order.Total = osvm.Total;

                    _context.OrderLog.Update(order);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");

                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}