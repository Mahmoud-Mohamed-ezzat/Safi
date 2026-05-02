using Microsoft.EntityFrameworkCore;
using Safi.Dto.Analysis;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;
using Safi.Services;

namespace Safi.Repositories
{
    public class AnalysisRepo : IAnalysis
    {
        private readonly SafiContext _context;
        private readonly ImageService _imageService;

        public AnalysisRepo(SafiContext context, ImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<Analysis> CreateAsync(CreateAnalysisDto dto)
        {
            var imagePath = await _imageService.SaveImageAsync(dto.Image);

            var analysis = dto.ToAnalysis();
            analysis.Image = imagePath;
            await _context.Analysis.AddAsync(analysis);
            await _context.SaveChangesAsync();
            // Load patient to return full info if needed
            await _context.Entry(analysis).Reference(a => a.Patient).LoadAsync();

            return analysis;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var analysis = await _context.Analysis.FindAsync(id);
            if (analysis == null) return false;

            // Delete the image file
            await _imageService.DeleteImageAsync(analysis.Image);

            _context.Analysis.Remove(analysis);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Analysis>> GetAllAsync()
        {
            return await _context.Analysis
                .Include(a => a.Patient)
                .ThenInclude(p => p.Departments)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Analysis?> GetByIdAsync(int id)
        {
            return await _context.Analysis
                .Include(a => a.Patient)
                .ThenInclude(p => p.Departments)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Analysis>> GetByPatientCustomIdAsync(int customId)
        {
            return await _context.Analysis
                .Include(a => a.Patient)
                .ThenInclude(p => p.Departments)
                .AsNoTracking()
                .Where(a => a.Patient != null && a.Patient.Custome_Id == customId)
                .ToListAsync();

        }

        public async Task<List<Analysis>> GetByPatientIdAsync(string patientId)
        {
            return await _context.Analysis
                .Include(a => a.Patient)
                .ThenInclude(p => p.Departments)
                .Where(a => a.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<Analysis?> UpdateAsync(int id, UpdateAnalysisDto dto)
        {
            var analysis = await _context.Analysis.FindAsync(id);
            if (analysis == null) return null;

            analysis.Name = dto.Name;

            if (dto.Image != null)
            {
                analysis.Image = await _imageService.UpdateImageAsync(analysis.Image, dto.Image);
            }
            else
            {
                analysis.Image = analysis.Image;
            }
            await _context.SaveChangesAsync();

            // Reload navigation properties
            await _context.Entry(analysis).Reference(a => a.Patient).LoadAsync();
            if (analysis.Patient != null)
            {
                await _context.Entry(analysis.Patient).Collection(p => p.Departments).LoadAsync();
            }

            return analysis;
        }
    }
}
