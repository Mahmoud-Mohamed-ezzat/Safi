using Microsoft.AspNetCore.Http;

namespace Safi.Dto.Analysis
{
    public class UpdateAnalysisDto
    {
        public string? Name { get; set; }
        public IFormFile? Image { get; set; }
    }
}
