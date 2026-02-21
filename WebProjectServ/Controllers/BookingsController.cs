using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebProjectServ.Models;

namespace WebProjectServ.Controllers
{
    public class BookingsController : Controller
    {
        private readonly MyDataContext _context;

        public BookingsController(MyDataContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Tour);

            return View(await bookings.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Tour)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create(int? clientId, int? tourId)
        {
            ViewBag.FromClient = clientId;
            ViewBag.FromTour = tourId;

            ViewData["ClientId"] = new SelectList(
                _context.Clients
                    .Select(c => new
                    {
                        c.Id,
                        Display = c.Id + " " + c.FirstName + " " + c.LastName
                    }),
                "Id",
                "Display",
                clientId
            );
            ViewData["TourId"] = new SelectList(
                _context.Tours
                    .Select(t => new
                    {
                        t.Id,
                        Display = t.Id + " - " + t.Name
                    }),
                "Id",
                "Display",
                tourId
            );

            ViewBag.Order = clientId != null ? "clientFirst" :
                            tourId != null ? "tourFirst" :
                            "default";

            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking, int? fromClient, int? fromTour)
        {
            bool exists = await _context.Bookings
                .AnyAsync(b => b.ClientId == booking.ClientId
                            && b.TourId == booking.TourId);

            if (exists)
            {
                ModelState.AddModelError("",
                    "This client has already booked this tour.");
            }
            if (ModelState.IsValid)
            {
                booking.BookingDate = DateTime.Now;
                booking.TotalPrice = booking.NumberOfPeople *
                    _context.Tours
                        .Where(t => t.Id == booking.TourId)
                        .Select(t => t.Price)
                        .FirstOrDefault();
                _context.Add(booking);
                await _context.SaveChangesAsync();

                if (fromClient != null)
                    return RedirectToAction("Details", "Clients", new { id = fromClient });

                if (fromTour != null)
                    return RedirectToAction("Details", "Tours", new { id = fromTour });

                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(
                _context.Clients
                    .Select(c => new
                    {
                        c.Id,
                        Display = c.Id + " - " + c.FullName
                    })
                    .ToList(),
                "Id",
                "Display",
                booking.ClientId
            );

            ViewData["TourId"] = new SelectList(
                _context.Tours
                    .Select(c => new
                    {
                        c.Id,
                        Display = c.Id + " - " + c.Name
                    })
                    .ToList(),
                "Id",
                "Display",
                booking.TourId
            );
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            ViewData["ClientId"] = new SelectList(
                _context.Clients
                    .Select(c => new
                    {
                        c.Id,
                        Display = c.Id + " - " + c.FullName
                    })
                    .ToList(),
                "Id",
                "Display",
                booking.ClientId
            );

            ViewData["TourId"] = new SelectList(
                _context.Tours
                    .Select(c => new
                    {
                        c.Id,
                        Display = c.Id + " - " + c.Name
                    })
                    .ToList(),
                "Id",
                "Display",
                booking.TourId
            );
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,TourId,Status,NumberOfPeople")] Booking booking)
        {
            if (id != booking.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var model = await _context.Bookings.FindAsync(id);
                    if (model == null)
                        return NotFound();

                    model.ClientId = booking.ClientId;
                    model.TourId = booking.TourId;
                    model.Status = booking.Status;
                    model.NumberOfPeople = booking.NumberOfPeople;
                    var price = await _context.Tours
                        .Where(t => t.Id == booking.TourId)
                        .Select(t => t.Price)
                        .FirstOrDefaultAsync();

                    model.TotalPrice = booking.NumberOfPeople * price;


                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException?.Message.Contains("IX_Bookings_ClientId_TourId") == true)
                    {
                        ModelState.AddModelError("",
                            "This client is already booked for the selected tour.");
                    }
                    else
                    {
                        ModelState.AddModelError("",
                            "An error occurred while saving. Please try again.");
                    }
                }
            }

            ViewData["ClientId"] = new SelectList(
                _context.Clients
                    .Select(c => new
                    {
                        c.Id,
                        Display = c.Id + " - " + c.FullName
                    })
                    .ToList(),
                "Id",
                "Display",
                booking.ClientId
            );

            ViewData["TourId"] = new SelectList(
                _context.Tours
                    .Select(c => new
                    {
                        c.Id,
                        Display = c.Id + " - " + c.Name
                    })
                    .ToList(),
                "Id",
                "Display",
                booking.TourId
            );
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Tour)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}
