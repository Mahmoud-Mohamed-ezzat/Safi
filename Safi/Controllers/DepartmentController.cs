using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Dto.Department;
using Safi.Models;

namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        readonly SafiContext _context;
        readonly IDepartment _Repo;
        public DepartmentController(SafiContext context, IDepartment Repo)
        {
            _context = context;
            _Repo = Repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDeparment()
        {
            var Departments = await _Repo.GetAllDepartments();
            return Ok(Departments);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDepartmentById([FromRoute] int id)
        {
            var Departments = await _Repo.GetDepartmentById(id);
            return Ok(Departments);
        }
        [HttpGet("GetDoctorsOfDepartment")]
        public async Task<IActionResult> GetDoctorsOfDepartment(string name)
        {
            var Departments = await _Repo.GetDoctorsOfDepartment(name);
            return Ok(Departments);
        }
        [HttpGet("GetDoctorsOfDepartment/{id:int}")]
        public async Task<IActionResult> GetDoctorsOfDepartment([FromRoute] int id)
        {
            var Departments = await _Repo.GetDoctorsOfDepartment(id);
            return Ok(Departments);
        }
        [HttpGet("GetPatientsOfDepartment")]
        public async Task<IActionResult> GetPatientsOfDepartment(string name)
        {
            var Departments = await _Repo.GetPatientsOfDepartment(name);
            return Ok(Departments);
        }
        [HttpGet("GetPatientsOfDepartment/{id:int}")]
        public async Task<IActionResult> GetPatientsOfDepartment([FromRoute] int id)
        {
            var Departments = await _Repo.GetPatientsOfDepartment(id);
            return Ok(Departments);
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var Department = await _Repo.RemoveDepartment(id);
            if (Department != null)
            {
                return Ok($"{Department} Department is deleted successfully!");
            }
            return BadRequest();
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] DepartmentDto departmentDto)
        {
            var Department = await _Repo.UpdateDepartment(id, departmentDto);
            if (Department != null)
            {
                return Ok($"{Department} Department is updated successfully!");
            }
            return BadRequest();
        }
    }
}