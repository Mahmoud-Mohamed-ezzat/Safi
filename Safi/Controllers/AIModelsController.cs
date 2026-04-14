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
        public AIModelsController(LiverModel liverModel)
        {
            _liverModel = liverModel;
        }
        [HttpPost("PredictLiverDisease")]
        public async Task<IActionResult> PredictLiverDisease([FromBody] LiverModelRequestDto Input)
        {
            var predict_result = await _liverModel.PredictLiverModel(Input);
            return Ok(predict_result);
        }
    }
}
