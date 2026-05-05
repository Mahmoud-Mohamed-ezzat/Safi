using Safi.Dto.Prices;
using Safi.Models;

namespace Safi.Mapper
{
    public static class PricesMapper
    {
        public static Prices ToPrices(this CreatePriceDto dto)
        {
            return new Prices
            {
                ServiceName = dto.ServiceName,
                Price = dto.Price,
                St_Date = dto.St_Date??DateOnly.FromDateTime(DateTime.Now),
                End_Date = dto.End_Date,
                Is_Deleted = false
            };
        }

        public static GetPriceDto ToGetPriceDto(this Prices model)
        {
            return new GetPriceDto
            {
                Id = model.Id,
                ServiceName = model.ServiceName,
                Price = model.Price,
                St_Date = model.St_Date,
                End_Date = model.End_Date,
                Is_Deleted = model.Is_Deleted
            };
        }
    }
}
