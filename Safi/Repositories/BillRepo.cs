using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Safi.Dto.Bill;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;

namespace Safi.Repositories
{
    public class BillRepo : IBill
    {
        public readonly SafiContext _context;
        public BillRepo(SafiContext context)
        {
            _context = context;
        }

        public async Task<bool> AddPriceTobill(UpdateBillDto dto)
        {
            try
            {
                var bill = await _context.Bills.FirstOrDefaultAsync(b => b.Id == dto.Id && b.PatientId == dto.PatientId);
                if (bill == null) return false;
                bill.TotalAmount += dto.TotalAmount;
                if (!string.IsNullOrWhiteSpace(dto.Details))
                    bill.Details = string.IsNullOrWhiteSpace(bill.Details)
                        ? dto.Details
                        : bill.Details + dto.Details;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
                return false;

            }
        }

        public async Task<int> CheckThisBillActive(string patientId)
        {
            var bill = await _context.Bills.FirstOrDefaultAsync(b=>b.PatientId==patientId && b.Status=="Open" );
            if (bill == null) return 0;
            return bill.Id;
        }

        public async Task<bool> CheckThisBillExist(int id, string patientId)
        {
            var bill=await _context.Bills.Include(b=>b.Patient).FirstOrDefaultAsync(b => b.PatientId == patientId && b.Id == id);
            return bill != null;
        }

        public async Task<BillDto> CloseBill(int id, string patientId)
        {
            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.PatientId == patientId && b.Id == id);
                        if (bill == null) return null;
            var Appointments = await _context
                .AppointmentToRooms
                .Where(a => a.PatientId == patientId 
                && a.StartTime >= bill.st_Date 
                && (bill.end_Date == null || a.EndTime <= bill.end_Date))
                .ToListAsync();
            bill.TotalAmount += Appointments.Sum(a => a.TotalPrice) ?? 0;
            bill.Status = "closed";
            bill.end_Date = DateTime.Now;
            await _context.SaveChangesAsync();
            return bill.toBillDto();
        }
        public async Task<Decimal> GetPriceOfBill(int id, string patientId)
        {
            var bill = await _context.Bills
                .FirstOrDefaultAsync(b => b.PatientId == patientId && b.Id == id);
            if (bill == null) return 0;
            var Appointments = await _context
                .AppointmentToRooms
                .Where(a => a.PatientId == patientId
                && a.StartTime >= bill.st_Date
                && (bill.end_Date == null || a.EndTime <= bill.end_Date))
                .ToListAsync();
            var TotalAmount = Appointments.Sum(a => a.TotalPrice)+bill.TotalAmount ?? 0;
              return TotalAmount;
        }

        public async Task<BillDto> CreateBill(createbillDto dto)
        {
            var db = await _context.Database.BeginTransactionAsync();
            try
            {
                var billId = await CheckThisBillActive(dto.PatientId);
                if (billId != 0)
                {
                    await db.RollbackAsync();
                    return null;
                }
                var bill = dto.ToCreateBill();
                await _context.Bills.AddAsync(bill);
                await _context.SaveChangesAsync();
                await db.CommitAsync();
                
                // Reload the bill with Patient included
                var billWithPatient = await _context.Bills
                    .Include(b => b.Patient)
                    .FirstOrDefaultAsync(b => b.Id == bill.Id);
                
                return billWithPatient?.toBillDto();
            }
            catch (Exception ex)
            {
                await db.RollbackAsync();
                return null;
            }
        }
        public async  Task<List<BillDto>> GetAllBills()
        {
            var bills = await _context.Bills.Include(b => b.Patient).ToListAsync();
            return bills.Select(b => b.toBillDto()).ToList();
        }
        public async  Task<decimal> GetAllBillsInRangeOfDate(DateOnly st_date, DateOnly end_date)
        {
            var billsAmount =  _context.Bills.Include(b => b.Patient)
                .AsNoTracking()
                .Where(b => DateOnly.FromDateTime(b.st_Date) >= st_date && (b.end_Date == null || DateOnly.FromDateTime(b.end_Date.Value) <= end_date))
                .Sum(b=>b.TotalAmount);
            return billsAmount; 
        }

        public async Task<BillDto> GetBill(int id)
        {
            var bill = await _context.Bills.Include(b => b.Patient).AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
            return bill?.toBillDto();
        }
    }
}
