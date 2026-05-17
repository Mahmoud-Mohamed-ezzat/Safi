using Safi.Dto.Bill;

namespace Safi.Interfaces
{
    public interface IBill
    {
        public Task<BillDto> CreateBill(createbillDto dto);
       
        public Task<List<BillDto>> GetAllBills();
        public Task<BillDto> GetBill(int id);
        public Task<bool> CheckThisBillExist(int id, string patientId);
        public Task<int> CheckThisBillActive(string patientId);
        public Task<decimal> GetAllBillsInRangeOfDate(DateOnly st_date, DateOnly end_date);

        public Task<BillDto> CloseBill(int id, string  patientId);
        public Task<bool> AddPriceTobill(UpdateBillDto dto);

    }
}
