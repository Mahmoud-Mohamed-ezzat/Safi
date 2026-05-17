using Safi.Dto.Bill;
using Safi.Models;

namespace Safi.Mapper
{
    public static class BillMapper
    {
        public static Bill ToCreateBill(this createbillDto dto)
        {
            return new Bill
            {
                st_Date = dto.st_Date,
                end_Date = dto.end_Date,
                Status = dto.Status,
                Details = dto.Details,
                currency = dto.currency,
                TotalAmount = dto.TotalAmount,
                PatientId = dto.PatientId
            };
        }
        public static BillDto toBillDto(this Bill model)
        {
            return new BillDto
            {
                id = model.Id,
                st_Date = model.st_Date,
                end_Date = model.end_Date,
                Status = model.Status,    // "open" , "closed"
                Details = model.Details,
                currency = model.currency,
                TotalAmount = model.TotalAmount,
                PatientId = model.Patient.Id,
                PatientName = model.Patient?.Name
            };
        }
        public static void UpdateBill(this Bill bill, UpdateBillDto dto)
        {

            bill.TotalAmount = bill.TotalAmount+dto.TotalAmount;
            bill.PatientId = dto.PatientId ?? bill.PatientId;
        }
    }
}

