using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.Analysis
{
    public class CreateAnalysisDto
    {
        public string Name { get; set; }
        public IFormFile Image { get; set; }
        public string PatientId { get; set; }
    }
}
