using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Safi.Dto.AIModelsDto;
using Safi.Services.AIService;

namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIModelsController : ControllerBase
    {
        readonly LiverModel _liverModel;
        readonly HeartDiseasModel _heartDiseasModel;
        readonly MedicineModelService _medicineModelService;

        public AIModelsController(LiverModel liverModel, HeartDiseasModel heartDiseasModel, MedicineModelService medicineModelService)
        {
            _liverModel = liverModel;
            _heartDiseasModel = heartDiseasModel;
            _medicineModelService = medicineModelService;
        }

        [Authorize(Roles = "Doctor,Patient")]
        [HttpPost("PredictLiverDisease")]
        public async Task<IActionResult> PredictLiverDisease([FromBody] LiverModelRequestDto Input)
        {
            var predict_result = await _liverModel.PredictLiverModel(Input);
            return Ok(predict_result);
        }

        // [Authorize(Roles = "Doctor,Patient")]
        [HttpPost("PredictHeartDisease")]
        public async Task<IActionResult> PredictHeartDisease([FromBody] HeartDiseaseRequestDto Input)
        {
            var predict_result = await _heartDiseasModel.PredictHeartDiseasModel(Input);
            return Ok(predict_result);
        }

        [HttpPost("LookupMedicine")]
        public async Task<IActionResult> LookupMedicine([FromBody] MedicineLookupRequestDto Input)
        {
            var result = await _medicineModelService.LookupMedicineAsync(Input);
            return Ok(result);
        }

        [HttpPost("AnalyzePrescription")]
        public async Task<IActionResult> AnalyzePrescription(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded.");

            using var stream = image.OpenReadStream();
            var result = await _medicineModelService.AnalyzePrescriptionAsync(stream, image.FileName);
            return Ok(result);
        }
    }
}
