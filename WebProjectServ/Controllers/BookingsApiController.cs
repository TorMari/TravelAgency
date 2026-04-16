using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProjectServ.Models;

namespace WebProjectServ.Controllers
{
    [Route("api/Bookings")]
    [ApiController]
    public class BookingsApiController : ControllerBase
    {
        private readonly IRepository<Booking> _repository;
        private readonly IRepository<Tour> _tourRepository;

        public BookingsApiController(IRepository<Booking> repository, IRepository<Tour> tourRepository)
        {
            _repository = repository;
            _tourRepository = tourRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _repository.GetAllAsync();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var booking = await _repository.GetByIdWithIncludesAsync(id, b => b.Client, b => b.Tour);
            if (booking == null) return NotFound();

            var result = new BookingDto
            {
                Id = booking.Id,
                Status = booking.Status,
                NumberOfPeople = booking.NumberOfPeople,
                TotalPrice = booking.TotalPrice,
                BookingDate = booking.BookingDate,
                ClientId = booking.ClientId,
                Client = new Client
                {
                    Id = booking.Client.Id,
                    FirstName = booking.Client.FirstName,
                    LastName = booking.Client.LastName,
                    Email = booking.Client.Email,
                    Phone = booking.Client.Phone,
                    DateOfBirth = booking.Client.DateOfBirth
                },
                TourId = booking.TourId,
                Tour = new Tour
                {
                    Id = booking.Tour.Id,
                    Name = booking.Tour.Name,
                    Country = booking.Tour.Country,
                    Price = booking.Tour.Price,
                    StartDate = booking.Tour.StartDate,
                    DurationDays = booking.Tour.DurationDays,
                    Description = booking.Tour.Description
                }
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookingDto dto)
        {
            var tour = await _tourRepository.GetByIdAsync(dto.TourId);
            if (tour == null)
                return BadRequest("Tour not found");

            var booking = new Booking
            {
                ClientId = dto.ClientId,
                TourId = dto.TourId,
                Status = dto.Status,
                NumberOfPeople = dto.NumberOfPeople,

                BookingDate = DateTime.Now,
                TotalPrice = tour.Price * dto.NumberOfPeople
            };

            await _repository.AddAsync(booking);

            var result = new BookingDto
            {
                Id = booking.Id,
                ClientId = booking.ClientId,
                TourId = booking.TourId,
                Status = booking.Status,
                NumberOfPeople = booking.NumberOfPeople,
                TotalPrice = booking.TotalPrice,
                BookingDate = booking.BookingDate
            };

            return CreatedAtAction(nameof(Get), new { id = booking.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BookingDto dto)
        {
            var booking = await _repository.GetByIdWithIncludesAsync(id, b => b.Client, b => b.Tour);

            if (booking == null)
                return NotFound();

            booking.Status = dto.Status;
            booking.NumberOfPeople = dto.NumberOfPeople;
            booking.TotalPrice = booking.Tour.Price * dto.NumberOfPeople;

            await _repository.UpdateAsync(booking);

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
