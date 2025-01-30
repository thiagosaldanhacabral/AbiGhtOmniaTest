using AbiGhtOmniaTest.Domain.Entities;

namespace AbiGhtOmniaTest.Domain.Interfaces;

public interface ISaleRepository
{
    Task<Guid> AddSaleAsync(Sale sale);
    Task UpdateSaleAsync(Sale sale);
    Task DeleteSaleAsync(Guid saleId);
}
