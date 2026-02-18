using Microsoft.EntityFrameworkCore;
using Safi.Interfaces;
using Safi.Models;

namespace Safi.Repositories
{
    public class DoctorRepo : IDoctor
    {
        private readonly SafiContext _context;

        public DoctorRepo(SafiContext context)
        {
            _context = context;
        }

        public async Task<Doctor?> GetByIdAsync(string doctorId)
        {
            return await _context.Doctors
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.Id == doctorId);
        }

        public async Task<bool> RateDoctorAsync(string doctorId, int rating)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);
            if (doctor == null)
            {
                return false;
            }

            // Formula: NewAverage = ((OldRank * OldCount) + NewRating) / (OldCount + 1)
            float totalPoints = (doctor.Rank * doctor.RankCount) + rating;
            doctor.RankCount++;
            doctor.Rank = totalPoints / doctor.RankCount;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
