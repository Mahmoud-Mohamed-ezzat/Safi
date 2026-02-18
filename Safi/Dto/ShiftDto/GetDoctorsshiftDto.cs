using Safi.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Dto.ShiftDto
{
    public class GetDoctorsshiftDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string University { get; set; } // University graduated from
        public string Degree { get; set; } // Medical degree
        public float Rank { get; set; } // Ranking or rating
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int room_id { get; set; }
        public int room_number { get; set; }
    }
}
