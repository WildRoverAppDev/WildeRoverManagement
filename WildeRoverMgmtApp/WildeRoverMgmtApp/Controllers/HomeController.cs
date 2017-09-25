using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WildeRoverMgmtApp.Models;

namespace WildeRoverMgmtApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly WildeRoverMgmtAppContext _context;
        private const int MAX_ENTRIES = 10; 

        public HomeController(WildeRoverMgmtAppContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            

            var invLogs = (from log in _context.InventoryLog
                           orderby log.Date descending
                           select log);

            var ordLogs = from log in _context.OrderLog
                          orderby log.Date descending
                          select log;

            HomeIndexViewModel model = new HomeIndexViewModel();

            int counter = 0;
            foreach(var log in invLogs)
            {
                if (counter < MAX_ENTRIES)
                {
                    model.InventoryList.Add(log);
                    counter++;
                }
                else
                {
                    break;
                }                
            }

            counter = 0;
            foreach(var log in ordLogs)
            {
                if (counter < MAX_ENTRIES)
                {
                    model.OrderList.Add(log);
                    counter++;
                }
                else
                {
                    break;
                }
            }

            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
