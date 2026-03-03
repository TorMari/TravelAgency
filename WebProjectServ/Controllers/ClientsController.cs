using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebProjectServ.Models;

namespace WebProjectServ.Controllers
{
    public class ClientsController : Controller
    {
        private readonly MyDataContext _context;
        public ClientsController(MyDataContext context)
        {
            _context = context;
        }

        // GET: ClientsController
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clients.ToListAsync());
        }

        // GET: ClientsController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients
                .Include(c => c.Bookings)
                .ThenInclude(b => b.Tour)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (client == null) return NotFound();

            return View(client);
        }

        // GET: ClientsController/Create
        public IActionResult Create()
        {
            var sessionData = HttpContext.Session.GetString("CreateClient");

            if (sessionData != null)
            {
                var client = JsonSerializer.Deserialize<Client>(sessionData);
                return View(client);
            }
            return View();
        }

        // POST: ClientsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,FirstName,LastName,Email,Phone,DateOfBirth")] Client client)
        {
            if (!ModelState.IsValid)
                return View(client);

            HttpContext.Session.SetString("CreateClient",
                JsonSerializer.Serialize(client));

            ViewBag.Mode = "Create";
            return View("Confirm", client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(string mode)
        {
            string modename = mode + "Client";
            var data = HttpContext.Session.GetString(modename);
            if (data == null) return RedirectToAction(nameof(Index));
            var client = JsonSerializer.Deserialize<Client>(data);

            if (mode == "Create")
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
            }
            else if (mode == "Edit")
            {
                var model = await _context.Clients.FindAsync(client.Id);
                if (model == null) return NotFound();

                model.FirstName = client.FirstName;
                model.LastName = client.LastName;
                model.Email = client.Email;
                model.Phone = client.Phone;
                model.DateOfBirth = client.DateOfBirth;

                await _context.SaveChangesAsync();
            }
            HttpContext.Session.Remove(modename);
            return RedirectToAction(nameof(Index));
        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateConfirmed()
        //{
        //    var sessionData = HttpContext.Session.GetString("CreateClient");
        //    if (sessionData == null)
        //        return RedirectToAction(nameof(Index));

        //    var client = JsonSerializer.Deserialize<Client>(sessionData);

        //    _context.Add(client);
        //    await _context.SaveChangesAsync();

        //    HttpContext.Session.Remove("CreateClient");

        //    return RedirectToAction(nameof(Index));
        //}

        // GET: ClientsController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            HttpContext.Session.SetString("EditClient",
                JsonSerializer.Serialize(client));

            return View(client);
        }

        // POST: ClientsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,FirstName,LastName,Email,Phone,DateOfBirth")] Client client)
        {
            if (id != client.Id) return NotFound();

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(client);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!ClientExists(client.Id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction(nameof(Index));
            //}

            if (!ModelState.IsValid)
                return View(client);

            HttpContext.Session.SetString("EditClient",
                JsonSerializer.Serialize(client));

            ViewBag.Mode = "Edit";
            return View("Confirm", client);
        }


        public IActionResult CancelCreate()
        {
            HttpContext.Session.Remove("CreateClient");
            return RedirectToAction(nameof(Create));
        }


        // GET: ClientsController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients
                .Include(c => c.Bookings)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (client == null) return NotFound();

            return View(client);
        }

        // POST: ClientsController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Bookings)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client != null)
            {
                _context.Bookings.RemoveRange(client.Bookings);
                _context.Clients.Remove(client);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
