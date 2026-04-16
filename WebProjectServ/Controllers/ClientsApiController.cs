using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProjectServ.Models;

namespace WebProjectServ.Controllers
{
    [Route("api/Clients")]
    [ApiController]
    public class ClientsApiController : ControllerBase
    {
        private readonly IRepository<Client> _repository;

        public ClientsApiController(IRepository<Client> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clients = await _repository.GetAllAsync();
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var client = await _repository.GetByIdWithIncludesAsync(id, b => b.Bookings);

            if (client == null)
                return NotFound();

            var result = new ClientDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = client.Email,
                Phone = client.Phone,
                DateOfBirth = client.DateOfBirth,
                Bookings = client.Bookings.Select(b => new BookingDto
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
        public async Task<IActionResult> Create(Client client)
        {
            await _repository.AddAsync(client);
            return CreatedAtAction(nameof(Get), new { id = client.Id }, client);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ClientDto dto)
        {
            var client = await _repository.GetByIdWithIncludesAsync(id, b => b.Bookings);

            if (client == null)
                return NotFound();

            client.FirstName = dto.FirstName;
            client.LastName = dto.LastName;
            client.Email = dto.Email;
            client.Phone = dto.Phone;
            client.DateOfBirth = dto.DateOfBirth;
            await _repository.UpdateAsync(client);

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
