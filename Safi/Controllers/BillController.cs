using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Safi.Interfaces;

namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IBill _BillRepo;
        public BillController(IBill BillRepo)
        {
            _BillRepo = BillRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBill()
        {
            var bills = await _BillRepo.GetAllBills();
            return Ok(bills);
        }
        [HttpGet("GetBillByid/{id:int}")]
        public async Task<IActionResult> GetBillById(int id)
        {
            var bill = await _BillRepo.GetBill(id);
            return Ok(bill);
        }
        [HttpGet("GetBillsinRangeOfDate")]
        public async Task<IActionResult> GetAllBillsInRangeOfDate(DateOnly startDate, DateOnly EndDate)
        {
            var bills = await _BillRepo.GetAllBillsInRangeOfDate(startDate, EndDate);
            return Ok(bills);
        }
    }
}