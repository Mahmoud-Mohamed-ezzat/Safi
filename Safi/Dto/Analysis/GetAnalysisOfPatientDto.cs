using Safi.Dto.Department;

namespace Safi.Dto.Analysis
{
    public class GetAnalysisOfPatientDto
    {
         public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public List<DepartmentDto>? PatientDepartmentsIDs { get; set; } 
    }
}
