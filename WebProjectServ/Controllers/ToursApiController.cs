using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProjectServ.Models;

namespace WebProjectServ.Controllers
{
    [Route("api/Tours")]
    [ApiController]
    public class ToursApiController : ControllerBase
    {
        private readonly IRepository<Tour> _repository;

        public ToursApiController(IRepository<Tour> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tours = await _repository.GetAllAsync();
            return Ok(tours);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var tour = await _repository.GetByIdWithIncludesAsync(id, b => b.Bookings);

            if (tour == null)
                return NotFound();

            var result = new TourDto
            {
                Id = tour.Id,
                Name = tour.Name,
                Country = tour.Country,
                Price = tour.Price,
                StartDate = tour.StartDate,
                DurationDays = tour.DurationDays,
                Description = tour.Description,
                Bookings = tour.Bookings.Select(b => new BookingDto
                {
                    Id = b.Id,
                    Status = b.Status,
                    NumberOfPeople = b.NumberOfPeople,
                    TotalPrice = b.TotalPrice,
                    BookingDate = b.BookingDate
                }).ToList()
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Tour tour)
        {
            await _repository.AddAsync(tour);
            return CreatedAtAction(nameof(Get), new { id = tour.Id }, tour);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TourDto dto)
        {
            var tour = await _repository.GetByIdWithIncludesAsync(id, b => b.Bookings);

            if (tour == null)
                return NotFound();

            tour.Name = dto.Name;
            tour.Country = dto.Country;
            tour.StartDate = dto.StartDate;
            tour.DurationDays = dto.DurationDays;
            tour.Description = dto.Description;
            tour.Price = dto.Price;
            await _repository.UpdateAsync(tour);

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _repository.Exists(id))
                return NotFound();

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
