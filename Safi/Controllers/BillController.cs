using Microsoft.AspNetCore.Authorization;
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
        [HttpGet("GetBillsOfPatient/{patientId}")]
        public async Task<IActionResult> GetBillsOfPatient(string patientId)
        {
            var bills = await _BillRepo.GetBillsOfPatient(patientId);
            return Ok(bills);
        }
        [HttpGet("GetBillsinRangeOfDate")]
        public async Task<IActionResult> GetAllBillsInRangeOfDate(DateOnly startDate, DateOnly EndDate)
        {
            var bills = await _BillRepo.GetAllBillsInRangeOfDate(startDate, EndDate);
            return Ok(bills);
        }
        [HttpPut("CloseBill/{id:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> CloseBill(int id, [FromQuery] string patientId)
        {
            if (string.IsNullOrEmpty(patientId))
            {
                return BadRequest("Patient ID is required.");
            }

            var updatedBill = await _BillRepo.CloseBill(id, patientId);
            if (updatedBill != null)
            {
                return Ok(updatedBill);
            }
            return BadRequest("Failed to close the bill. Verify the Bill ID and Patient ID.");
        }
    }
}