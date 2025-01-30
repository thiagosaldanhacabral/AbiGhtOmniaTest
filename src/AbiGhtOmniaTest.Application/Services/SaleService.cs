using AbiGhtOmniaTest.Application.Interfaces;
using AbiGhtOmniaTest.Domain.Entities;
using AbiGhtOmniaTest.Domain.Interfaces;

namespace AbiGhtOmniaTest.Application.Services;

public class SaleService(ISaleRepository saleRepository) : ISaleService
{
    private readonly ISaleRepository _saleRepository = saleRepository;

    public async Task<Guid> ExecuteAsync(Sale sale)
    {
        foreach (var item in sale.Items)
        {
            if (item.Quantity > 20)
                throw new Exception("Cannot sell more than 20 items of a single product.");
            else if (item.Quantity >= 10)
                item.Discount = item.Quantity * item.UnitPrice * 0.20m;
            else if (item.Quantity >= 4)
                item.Discount = item.Quantity * item.UnitPrice * 0.10m;

            item.CalculateTotalAmount();
        }

        sale.CalculateTotalAmount();
        return await _saleRepository.AddSaleAsync(sale);
    }
}

