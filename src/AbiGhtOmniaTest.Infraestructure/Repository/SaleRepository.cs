using AbiGhtOmniaTest.Domain.Entities;
using AbiGhtOmniaTest.Domain.Interfaces;
using AbiGhtOmniaTest.Infraestructure.Data;
using Serilog;

namespace AbiGhtOmniaTest.Infraestructure.Repository;

public class SaleRepository(DeveloperStoreDbContext context, ILogger logger) : ISaleRepository
{
    private readonly DeveloperStoreDbContext _context = context;
    private readonly ILogger _logger = logger;

    public async Task<Guid> AddSaleAsync(Sale sale)
    {
        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();
        _logger.Information("Sale added with ID: {SaleId}", sale.SaleId);
        return sale.SaleId;
    }

    public async Task UpdateSaleAsync(Sale sale)
    {
        var existingSale = await _context.Sales.FindAsync(sale.SaleId);
        if (existingSale != null)
        {
            _context.Entry(existingSale).CurrentValues.SetValues(sale);
            await _context.SaveChangesAsync();
            _logger.Information("Sale updated with ID: {SaleId}", sale.SaleId);
        }
        else
        {
            _logger.Warning("Sale with ID: {SaleId} not found for update", sale.SaleId);
        }
    }

    public async Task DeleteSaleAsync(Guid saleId)
    {
        var sale = await _context.Sales.FindAsync(saleId);
        if (sale != null)
        {
            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
            _logger.Information("Sale deleted with ID: {SaleId}", saleId);
        }
        else
        {
            _logger.Warning("Sale with ID: {SaleId} not found for deletion", saleId);
        }
    }
}
