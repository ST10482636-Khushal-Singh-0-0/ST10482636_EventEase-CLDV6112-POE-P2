using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10482636_EventEase.Data;
using ST10482636_EventEase.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ST10482636_EventEase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ST10482636_EventEaseContext _context;

        // 1. Inject the database context
        public HomeController(ST10482636_EventEaseContext context)
        {
            _context = context;
        }

        // 2. Fetch the top 3 venues and pass them to the View
        public async Task<IActionResult> Index()
        {
            var featuredVenues = await _context.Venue.Take(3).ToListAsync();
            return View(featuredVenues);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}