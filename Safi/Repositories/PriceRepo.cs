using Microsoft.EntityFrameworkCore;
using Safi.Dto.Prices;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;

namespace Safi.Repositories
{
    public class PriceRepo : IPrices
    {
        private readonly SafiContext _context;
        public PriceRepo(SafiContext context)
        {
            _context = context;
        }
        public async Task<Prices> CreateAsync(CreatePriceDto dto)
        {
          var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
          try{ 
                var today = DateOnly.FromDateTime(DateTime.Now);
                DateOnly effectiveStDate = dto.St_Date ?? today;
                
            var ExistPrice_Active = await _context.Prices
            .Where(p => p.ServiceName == dto.ServiceName
                     && !p.Is_Deleted
                     && p.St_Date <= (dto.End_Date ?? DateOnly.MaxValue)  // existing starts before new ends
                     && (p.End_Date == null || p.End_Date >= today))
           .FirstOrDefaultAsync();

           if(effectiveStDate < today)
           {
            throw new Exception("The start date must be greater than or equal to the current date");
           }

            if (ExistPrice_Active != null)
            {
                if (effectiveStDate == today)
              { 
                 ExistPrice_Active.Is_Deleted = true;
                ExistPrice_Active.End_Date = today.AddDays(-1);
              }
              else
              { 
                 ExistPrice_Active.Is_Deleted = true;
                ExistPrice_Active.End_Date = effectiveStDate.AddDays(-1);
              }
            }
            var newPrice = new Prices
            {
                ServiceName = dto.ServiceName,
                Price = dto.Price,
                St_Date = effectiveStDate,
                End_Date = dto.End_Date,
                Is_Deleted = false,
                CreatedAt=DateTime.UtcNow
                
            };
            await _context.Prices.AddAsync(newPrice);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return newPrice;
        }
        catch(Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception(ex.Message);
        }
    }

        public async Task<bool> DeleteAsync(int id)
        {
            var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try{
            var price = await _context.Prices.FindAsync(id);
            if (price == null) return false;
            price.Is_Deleted = true;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch(Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception(ex.Message);
        }
        }

        public async Task<List<GetPriceDto>> GetActivePricesAsync(string ServiceName)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var ActivedPrices= await _context.Prices
            .Where(p => p.ServiceName == ServiceName
                             && !p.Is_Deleted
                             && p.St_Date <= today
                             && (p.End_Date == null || p.End_Date > today))
            .ToListAsync();
            var pricesDto = ActivedPrices.Select(x=>x.ToGetPriceDto()).ToList();
            return pricesDto;
        }

        public async Task<List<GetPriceDto>> GetAllAsync()
        {
            var allPrices = await _context.Prices.ToListAsync();
            var pricesDto = allPrices.Select(x=>x.ToGetPriceDto()).ToList();
            return pricesDto;
        }

        public async Task<GetPriceDto?> GetByIdAsync(int id)
        {
            var price = await _context.Prices.FindAsync(id);
            if(price == null) return null;
            var priceDto = price.ToGetPriceDto();
            return priceDto;
        }

        public async Task<List<GetPriceDto>> GetCurrentPricesAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var currentPrices = await _context.Prices
            .Where(p => !p.Is_Deleted
            && p.St_Date <= today
            && (p.End_Date == null || p.End_Date > today))
            .OrderByDescending(p => p.St_Date)
            .ToListAsync();
            var pricesDto = currentPrices.Select(x=>x.ToGetPriceDto()).ToList();
            return pricesDto;
        }

        public async Task<decimal> GetPriceInSpecificDateAsync(string ServiceName, DateOnly date)
        {
            var price = await _context.Prices
            .Where(x => x.ServiceName == ServiceName 
            && x.St_Date <= date 
            && (x.End_Date >= date || x.End_Date == null))
            .FirstOrDefaultAsync();
            if(price == null) return 0;
            return price.Price;
        }

        public async Task<decimal> GetPriceNowAsync(string ServiceName)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var price = await _context.Prices
            .Where(p => p.ServiceName == ServiceName
            && !p.Is_Deleted
            && p.St_Date <= today
            && (p.End_Date == null || p.End_Date > today))
            .FirstOrDefaultAsync();
            if(price == null) return 0;
            return price.Price;
        }

        public async Task<decimal> GetPriceByIdAsync(int Id)
        {
            var price = await _context.Prices
            .Where(x => x.Id == Id )
            .FirstOrDefaultAsync();
            if(price == null) return 0;
            return price.Price;
        }

        public async Task<Prices> GetTheActivePriceNowAsync(string ServiceName)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var price = await _context.Prices
            .Where(p => p.ServiceName == ServiceName
            && !p.Is_Deleted
            && p.St_Date <= today
            && (p.End_Date == null || p.End_Date > today))
            .FirstOrDefaultAsync();
            if(price == null) return null;
            return price;
        }
        public async Task<List<GetPriceDto>> GetAllPricesServiceAsync(string ServiceName)
        {
            var allPrices = await _context.Prices
            .Where(x => x.ServiceName == ServiceName)
            .ToListAsync();
            var pricesDto = allPrices.Select(x=>x.ToGetPriceDto()).ToList();
            return pricesDto;
        }

        public async Task<List<GetPriceDto>> GetPricesByServiceAndDateRangeAsync(string serviceName, DateOnly startDate, DateOnly endDate)
        {
            var prices = await _context.Prices
                .Where(p => p.ServiceName == serviceName
                         && !p.Is_Deleted
                         && p.St_Date <= endDate
                         && (p.End_Date == null || p.End_Date >= startDate))
                .ToListAsync();
            return prices.Select(p => p.ToGetPriceDto()).ToList();
        }
    }
}
