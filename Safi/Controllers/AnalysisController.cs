using Microsoft.AspNetCore.Mvc;
using Safi.Dto.Analysis;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;

namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private readonly IAnalysis _analysisRepo;

        public AnalysisController(IAnalysis analysisRepo)
        {
            _analysisRepo = analysisRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var analyses = await _analysisRepo.GetAllAsync();
            var dtos = analyses.Select(a => a.ToGetAnalysisDto());
            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var analysis = await _analysisRepo.GetByIdAsync(id);
            if (analysis == null) return NotFound();
            return Ok(analysis.ToGetAnalysisDto());
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatientId([FromRoute] string patientId)
        {
            var analyses = await _analysisRepo.GetByPatientIdAsync(patientId);
            var dtos = analyses.Select(a => a.ToGetAnalysisOfPatientDto());
            return Ok(dtos);
        }

        [HttpGet("patient/custom/{customId:int}")]
        public async Task<IActionResult> GetByPatientCustomId([FromRoute] int customId)
        {
            var analyses = await _analysisRepo.GetByPatientCustomIdAsync(customId);
            var dtos = analyses.Select(a => a.ToGetAnalysisOfPatientDto());
            return Ok(dtos);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateAnalysisDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var analysis = await _analysisRepo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = analysis.Id }, analysis.ToGetAnalysisDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdateAnalysisDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var analysis = await _analysisRepo.UpdateAsync(id, dto);
            if (analysis == null) return NotFound();
            return Ok(analysis.ToGetAnalysisDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var deleted = await _analysisRepo.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
