using Microsoft.EntityFrameworkCore;
using Safi.Dto.AppointmentToRoom;
using Safi.Models;

namespace Safi.Services
{
    public class PriceSegmentCalculator
    {
        private readonly SafiContext _context;
        public PriceSegmentCalculator(SafiContext context)
        {
            _context = context;
        }
        public async Task<decimal> Calculate(AppointmentToRoomDto appointment, string type)
        {
            var appointmentStart = DateOnly.FromDateTime((DateTime)appointment.StartTime);

            var prices = await _context.Prices
                .Where(p => (p.End_Date == null || p.End_Date >= appointmentStart)
                         && p.ServiceName.ToLower() == type.ToLower())
                .OrderBy(p => p.St_Date)       
                .ToListAsync();
            var startTime = (DateTime)appointment.StartTime;
            var endTime = (DateTime)appointment.EndTime;
            decimal total = 0;
            foreach (var price in prices)
            {
                var segStart = price.St_Date.ToDateTime(TimeOnly.MinValue) < startTime
                    ? startTime
                    : price.St_Date.ToDateTime(TimeOnly.MinValue);
                var segEnd = price.End_Date == null || price.End_Date.Value.ToDateTime(TimeOnly.MinValue) > endTime
                    ? endTime
                    : price.End_Date.Value.ToDateTime(TimeOnly.MinValue);
                var hours = (decimal)(segEnd - segStart).TotalHours;
                if (hours <= 0) continue;  
                total += (decimal)price.Price * hours;
            }
            return total;
        }
    }
}
