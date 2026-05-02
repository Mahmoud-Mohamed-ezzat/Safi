using Safi.Dto.Analysis;
using Safi.Dto.Department;
using Safi.Models;

namespace Safi.Mapper
{
    public static class AnalysisMapper
    {
        public static Analysis ToAnalysis(this CreateAnalysisDto dto)
        {
            return new Analysis
            {
                Name = dto.Name,
                PatientId = dto.PatientId,
                Image = dto.Image.FileName,
            };
        }

        public static GetAnalysisDto ToGetAnalysisDto(this Analysis model)
        {
            return new GetAnalysisDto
            {
                Id = model.Id,
                Name = model.Name,
                Image = model.Image,
                PatientId = model.PatientId
            };
        }

        public static GetAnalysisOfPatientDto ToGetAnalysisOfPatientDto(this Analysis model)
        {
            return new GetAnalysisOfPatientDto
            {
                Id = model.Id,
                Name = model.Name,
                Image = model.Image,
                PatientId = model.PatientId,
                PatientName = model.Patient?.Name ?? string.Empty,
                PatientDepartmentsIDs = model.Patient?.Departments?.Select(d => d.ToDepartmentDto()).ToList() ??new List<DepartmentDto>()
            };
        }
    }
}
