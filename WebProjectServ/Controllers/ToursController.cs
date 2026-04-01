using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebProjectServ.Models;

namespace WebProjectServ.Controllers
{
    public class ToursController : Controller
    {
        private readonly MyDataContext _context;

        public ToursController(MyDataContext context)
        {
            _context = context;
        }

        // GET: ToursController
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tours.ToListAsync());
        }

        // GET: ToursController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tour = await _context.Tours
                .Include(c => c.Bookings)
                .ThenInclude(b => b.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tour == null) return NotFound();

            var messages = await _context.Messages
               .Where(m => m.TourId == id)
               .OrderBy(m => m.SentAt)
               .ThenBy(m => m.Id)
               .ToListAsync();
            ViewBag.Msg = messages;

            return View(tour);
        }

        // GET: ToursController/Create
        public IActionResult Create()
        {
            var sessionData = HttpContext.Session.GetString("CreateTour");

            if (sessionData != null)
            {
                var tour = JsonSerializer.Deserialize<Tour>(sessionData);
                return View(tour);
            }
            return View();
        }

        // POST: ToursController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,Country,Price,DurationDays,StartDate,Description")] Tour tour)
        {
            if (!ModelState.IsValid)
                return View(tour);

            HttpContext.Session.SetString("CreateTour",
                JsonSerializer.Serialize(tour));

            ViewBag.Mode = "Create";
            return View("Confirm", tour);
        }

        // GET: ToursController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tour = await _context.Tours.FindAsync(id);
            if (tour == null) return NotFound();

            HttpContext.Session.SetString("EditTour",
                JsonSerializer.Serialize(tour));

            return View(tour);
        }

        // POST: ToursController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Country,Price,DurationDays,StartDate,Description")] Tour tour)
        {
            if (id != tour.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(tour);

            HttpContext.Session.SetString("EditTour",
                JsonSerializer.Serialize(tour));

            ViewBag.Mode = "Edit";
            return View("Confirm", tour);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(string mode)
        {
            string modename = mode + "Tour";
            var data = HttpContext.Session.GetString(modename);
            if (data == null) return RedirectToAction(nameof(Index));
            var tour = JsonSerializer.Deserialize<Tour>(data);

            if (mode == "Create")
            {
                _context.Add(tour);
                await _context.SaveChangesAsync();
            }
            else if (mode == "Edit")
            {
                var model = await _context.Tours.FindAsync(tour.Id);
                if (model == null) return NotFound();

                model.Name = tour.Name;
                model.Country = tour.Country;
                model.Price = tour.Price;
                model.DurationDays = tour.DurationDays;
                model.StartDate = tour.StartDate;
                model.Description = tour.Description;
                var bookings = _context.Bookings
                    .Where(b => b.TourId == tour.Id);

                foreach (var booking in bookings)
                {
                    booking.TotalPrice = booking.NumberOfPeople * tour.Price;
                }

                await _context.SaveChangesAsync();
            }
            HttpContext.Session.Remove(modename);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult CancelCreate()
        {
            HttpContext.Session.Remove("CreateTour");
            return RedirectToAction(nameof(Create));
        }

        // GET: ToursController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tour = await _context.Tours
                .Include(c => c.Bookings)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tour == null) return NotFound();

            return View(tour);
        }

        // POST: ToursController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tour = await _context.Tours
                .Include(c => c.Bookings)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (tour != null)
            {
                _context.Bookings.RemoveRange(tour.Bookings);
                _context.Tours.Remove(tour);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool TourExists(int id)
        {
            return _context.Tours.Any(e => e.Id == id);
        }
    }
}
