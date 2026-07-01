using Microsoft.AspNetCore.Mvc;
using Safi.Dto.Prices;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;
using Microsoft.AspNetCore.Authorization;
namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PricesController : ControllerBase
    {
        private readonly IPrices _pricesRepo;

        public PricesController(IPrices pricesRepo)
        {
            _pricesRepo = pricesRepo;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var prices = await _pricesRepo.GetAllAsync();
            return Ok(prices);
        }
[Authorize(Roles = "Admin,subadmin,Staff")]
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentPrices()
        {
            var prices = await _pricesRepo.GetCurrentPricesAsync();
            return Ok(prices);
        }
[Authorize(Roles = "Admin,subadmin,Staff")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var price = await _pricesRepo.GetByIdAsync(id);
            if (price == null) return NotFound();
            return Ok(price);
        }
        [Authorize(Roles = "Admin,subadmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePriceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var price = await _pricesRepo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = price.Id }, price.ToGetPriceDto());
        }
        [HttpGet("active-prices")]
        public async Task<IActionResult> GetActivePrices([FromBody] string serviceName)
        {
            var prices = await _pricesRepo.GetActivePricesAsync(serviceName);
            return Ok(prices);
        }

        [HttpGet("active-now")]
        public async Task<IActionResult> GetTheActivePriceNow([FromBody] string serviceName)
        {
            var price = await _pricesRepo.GetTheActivePriceNowAsync(serviceName);
            if (price == null) return NotFound();
            return Ok(price.ToGetPriceDto());
        }

        [HttpGet("price-now")]
        public async Task<IActionResult> GetPriceNow([FromBody] string serviceName)
        {
            var price = await _pricesRepo.GetPriceNowAsync(serviceName);
            return Ok(price);
        }

        [HttpGet("price-now/id/{id:int}")]
        public async Task<IActionResult> GetPriceNowById([FromRoute] int id)
        {
            var price = await _pricesRepo.GetPriceByIdAsync(id);
            return Ok(price);
        }

        [HttpGet("GetPriceInSpecificDate")]
        public async Task<IActionResult> GetPriceInSpecificDate([FromBody] string serviceName, DateOnly date)
        {
            var price = await _pricesRepo.GetPriceInSpecificDateAsync(serviceName, date);
            return Ok(price);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var deleted = await _pricesRepo.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
        [HttpGet("all-prices-service")]
        public async Task<IActionResult> GetAllPricesService([FromBody] string serviceName)
        {
            var prices = await _pricesRepo.GetAllPricesServiceAsync(serviceName);
            return Ok(prices);
        }
    }
}
